using LiftOps_BackEnd.API.Filters;
using LiftOps_BackEnd.API.Options;
using LiftOps_BackEnd.API.Security;
using LiftOps_BackEnd.Application;
using LiftOps_BackEnd.Domain.Common;
using LiftOps_BackEnd.Domain.Entities;
using LiftOps_BackEnd.Infrastructure;
using LiftOps_BackEnd.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.SetIsOriginAllowed(origin => true) // Allow any origin for development
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireManager", policy => policy.RequireRole(Roles.Manager));
    options.AddPolicy("RequireInstallation", policy => policy.RequireRole(Roles.Manager, Roles.InstallationAdmin));
    options.AddPolicy("RequireMaintenance", policy => policy.RequireRole(Roles.Manager, Roles.MaintenanceAdmin));
    options.AddPolicy("RequireInventory", policy => policy.RequireRole(Roles.Manager, Roles.InventoryAdmin));
    options.AddPolicy("RequireFinance", policy => policy.RequireRole(Roles.Manager, Roles.FinanceAdmin));
    options.AddPolicy("RequireFaults", policy => policy.RequireRole(Roles.Manager, Roles.FaultsAdmin, Roles.MaintenanceAdmin));
});

builder.Services.AddDataProtection();
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

// Migrate and Seed Database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        logger.LogInformation("Applying database migrations...");
        await context.Database.MigrateAsync();
        logger.LogInformation("Database migrations applied successfully.");

        var userManager = services.GetRequiredService<UserManager<AppUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        await AdminSeeder.SeedAsync(userManager, roleManager);
        // DashboardDataSeeder.SeedAsync(context); // Commented out - only manager/admin seeding is active
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred during migration or seeding.");
        throw; // Re-throw to prevent app from starting with a broken database
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
