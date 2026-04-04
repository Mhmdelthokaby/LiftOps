using LiftOps_BackEnd.API.Filters;
using LiftOps_BackEnd.API.Middleware;
using LiftOps_BackEnd.API.Options;
using LiftOps_BackEnd.API.Security;
using LiftOps_BackEnd.Application;
using LiftOps_BackEnd.Domain.Common;
using LiftOps_BackEnd.Domain.Entities;
using LiftOps_BackEnd.Infrastructure;
using LiftOps_BackEnd.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.RateLimiting;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var corsAllowedOrigins = (builder.Configuration["Cors:AllowedOriginsCsv"] ?? string.Empty)
    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

if (corsAllowedOrigins.Length == 0)
{
    corsAllowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            if (builder.Environment.IsDevelopment())
            {
                // Local development: keep it permissive for easier iteration.
                policy.SetIsOriginAllowed(_ => true);
            }
            else if (corsAllowedOrigins.Length > 0)
            {
                // Staging/production: explicitly allow configured origins.
                policy.WithOrigins(corsAllowedOrigins);
            }
            else
            {
                // No configured origins => browser calls will fail CORS preflight (safe default).
                policy.SetIsOriginAllowed(_ => false);
            }

            policy.AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

const string JwtKeyPlaceholder = "REPLACE_WITH_ENV_JWT_KEY";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtKey = builder.Configuration["Jwt:Key"];
    if (string.IsNullOrWhiteSpace(jwtKey) || jwtKey == JwtKeyPlaceholder)
    {
        throw new InvalidOperationException(
            "JWT signing key is not configured. Set it via environment variable `Jwt__Key` (recommended: Key Vault).");
    }

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };

    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = context =>
        {
            var endpoint = context.HttpContext.GetEndpoint();
            if (endpoint == null)
            {
                return Task.CompletedTask;
            }

            var isAnonymous = endpoint.Metadata.GetMetadata<IAllowAnonymous>() != null;
            var isAuthorizedEndpoint = endpoint.Metadata.GetOrderedMetadata<IAuthorizeData>().Count != 0;
            var isPlatformRoute = context.HttpContext.Request.Path.StartsWithSegments("/api/platform", StringComparison.OrdinalIgnoreCase);

            if (!isAnonymous && isAuthorizedEndpoint && !isPlatformRoute)
            {
                var hasCompanyClaim = context.Principal?.HasClaim(c =>
                    c.Type == "company_id" && !string.IsNullOrWhiteSpace(c.Value)) == true;

                if (!hasCompanyClaim)
                {
                    context.Fail("Missing required company_id claim.");
                }
            }

            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization(options =>
{
    static bool HasTenantClaim(AuthorizationHandlerContext context) =>
        context.User.HasClaim(c => c.Type == "company_id" && !string.IsNullOrWhiteSpace(c.Value));

    options.AddPolicy("RequireManager", policy => policy
        .RequireRole(Roles.Manager)
        .RequireAssertion(HasTenantClaim));
    options.AddPolicy("RequireInstallation", policy => policy
        .RequireRole(Roles.Manager, Roles.InstallationAdmin)
        .RequireAssertion(HasTenantClaim));
    options.AddPolicy("RequireMaintenance", policy => policy
        .RequireRole(Roles.Manager, Roles.MaintenanceAdmin)
        .RequireAssertion(HasTenantClaim));
    options.AddPolicy("RequireInventory", policy => policy
        .RequireRole(Roles.Manager, Roles.InventoryAdmin)
        .RequireAssertion(HasTenantClaim));
    options.AddPolicy("RequireFaults", policy => policy
        .RequireRole(Roles.Manager, Roles.FaultsAdmin, Roles.MaintenanceAdmin)
        .RequireAssertion(HasTenantClaim));
    options.AddPolicy("EmergencyReport", policy => policy
        .RequireRole(Roles.Manager, Roles.MaintenanceAdmin, Roles.InstallationAdmin, Roles.FaultsAdmin, Roles.Technician)
        .RequireAssertion(HasTenantClaim));
    options.AddPolicy("EmergencyRead", policy => policy
        .RequireRole(Roles.Manager, Roles.MaintenanceAdmin, Roles.InstallationAdmin, Roles.FaultsAdmin, Roles.Technician)
        .RequireAssertion(HasTenantClaim));
    options.AddPolicy("EmergencyDispatch", policy => policy
        .RequireRole(Roles.Manager, Roles.MaintenanceAdmin, Roles.FaultsAdmin)
        .RequireAssertion(HasTenantClaim));
    options.AddPolicy("EmergencyResolve", policy => policy
        .RequireRole(Roles.Manager, Roles.MaintenanceAdmin, Roles.FaultsAdmin, Roles.Technician)
        .RequireAssertion(HasTenantClaim));
    options.AddPolicy("EmergencyManage", policy => policy
        .RequireRole(Roles.Manager, Roles.MaintenanceAdmin)
        .RequireAssertion(HasTenantClaim));
    options.AddPolicy("RequirePlatformAdmin", policy => policy
        .RequireRole(Roles.PlatformAdmin));
});

builder.Services.AddDataProtection();
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = async (context, _) =>
    {
        context.HttpContext.Response.ContentType = "application/json";
        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            code = "rate_limited",
            message = "Too many requests for this tenant. Please retry shortly."
        });
    };

    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
    {
        var path = httpContext.Request.Path.Value ?? string.Empty;
        var isAuthEndpoint = path.StartsWith("/api/Admin/login", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/api/Admin/refresh-token", StringComparison.OrdinalIgnoreCase);
        var isExpensiveEndpoint = path.Contains("/pdf", StringComparison.OrdinalIgnoreCase)
            || path.EndsWith("/statistics", StringComparison.OrdinalIgnoreCase)
            || path.Contains("/dashboard", StringComparison.OrdinalIgnoreCase);

        var scope = isAuthEndpoint ? "auth" : isExpensiveEndpoint ? "expensive" : "general";
        var tenantOrIp = httpContext.User.FindFirst("company_id")?.Value
            ?? httpContext.Connection.RemoteIpAddress?.ToString()
            ?? "unknown";
        var partitionKey = $"{scope}:{tenantOrIp}";

        var permitLimit = isAuthEndpoint ? 15 : isExpensiveEndpoint ? 30 : 120;
        var queueLimit = isAuthEndpoint ? 2 : 5;

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey,
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = permitLimit,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = queueLimit
            });
    });
});
builder.Services.Configure<MaintenancePdfOptions>(builder.Configuration.GetSection(MaintenancePdfOptions.SectionName));
builder.Services.AddScoped<MaintenanceVisitPdfAccessService>();
builder.Services.AddScoped<DevelopmentOnlyFilter>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    // Use full type name to avoid schema name conflicts
    c.CustomSchemaIds(type => type.FullName);
});

var app = builder.Build();

// Apply database migrations (skipped when UseInMemoryDatabase=true, e.g. integration tests)
var useInMemoryDb = app.Configuration.GetValue<bool>("UseInMemoryDatabase");
if (!useInMemoryDb)
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<Program>>();
        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            logger.LogInformation("Applying database migrations...");
            await SqlServerDatabaseEnsurer.EnsureExistsAsync(
                app.Configuration.GetConnectionString("DefaultConnection"),
                logger);
            var autoBaseline = app.Configuration.GetValue<bool>("Database:AutoBaselineMigrationHistory");
            await EfMigrationHistoryBaseline.TryBaselineAsync(context, autoBaseline, logger);
            await context.Database.MigrateAsync();
            logger.LogInformation("Database migrations applied successfully.");
        }
        catch (Exception ex)
        {
            if (ex is SqlException sql && sql.Number == 2714
                && sql.Message.Contains("AspNetRoles", StringComparison.OrdinalIgnoreCase))
            {
                logger.LogError(
                    "Migration failed: tables like AspNetRoles already exist, but __EFMigrationsHistory is empty or missing early migrations. " +
                    "Entity Framework is trying to run the first migration again. " +
                    "Fix (keep data): open SQL Server and run LiftOps-BackEnd.Infrastructure/Scripts/BaselineEfMigrationHistory.sql against this database, " +
                    "then restart the API. " +
                    "If your schema is not yet on PLAT001, comment out the PLAT001 block in that script before running it, then run: " +
                    "dotnet ef database update --project LiftOps-BackEnd.Infrastructure --startup-project LiftOps-BackEnd.API. " +
                    "Fix (dev only): drop and recreate the database, then restart. " +
                    "See: https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/");
            }

            logger.LogError(ex, "An error occurred during database migration.");
            throw; // Re-throw to prevent app from starting with a broken database
        }

        try
        {
            var userManager = services.GetRequiredService<UserManager<AppUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            await PlatformAdminSeeder.SeedAsync(userManager, roleManager, app.Configuration, logger);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Platform admin seeding failed.");
            throw;
        }
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

var forwardedHeadersEnabled = builder.Configuration.GetValue<bool?>("ForwardedHeaders:Enabled")
    ?? !app.Environment.IsDevelopment();

if (forwardedHeadersEnabled)
{
    var forwardedHeadersOptions = new ForwardedHeadersOptions
    {
        ForwardedHeaders =
            ForwardedHeaders.XForwardedFor |
            ForwardedHeaders.XForwardedProto |
            ForwardedHeaders.XForwardedHost
    };

    // Optional hardening: trust forwarded headers only from known proxies/networks (configure in staging/prod).
    var knownProxies = builder.Configuration.GetSection("ForwardedHeaders:KnownProxies").Get<string[]>();
    if (knownProxies != null)
    {
        foreach (var proxy in knownProxies)
        {
            if (IPAddress.TryParse(proxy, out var ip))
                forwardedHeadersOptions.KnownProxies.Add(ip);
        }
    }

    var knownNetworks = builder.Configuration.GetSection("ForwardedHeaders:KnownIPNetworks").Get<string[]>();
    if (knownNetworks == null || knownNetworks.Length == 0)
        knownNetworks = builder.Configuration.GetSection("ForwardedHeaders:KnownNetworks").Get<string[]>();

    if (knownNetworks != null)
    {
        foreach (var network in knownNetworks)
        {
            try
            {
                forwardedHeadersOptions.KnownIPNetworks.Add(System.Net.IPNetwork.Parse(network));
            }
            catch
            {
                // Ignore invalid entries; keep startup resilient.
            }
        }
    }

    app.UseForwardedHeaders(forwardedHeadersOptions);
}

app.UseCors("AllowFrontend");
app.UseRateLimiter();

app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (FluentValidation.ValidationException ex)
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new
        {
            code = "validation_failed",
            message = "Validation failed",
            details = ex.Errors.Select(e => e.ErrorMessage).ToArray()
        });
    }
    catch (Exception ex)
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new
        {
            code = "internal_server_error",
            message = "An unexpected error occurred.",
            details = app.Environment.IsDevelopment() ? ex.Message : null
        });
    }
});

app.UseAuthentication();
app.UseMiddleware<SubscriptionStatusMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
