using LiftOps_BackEnd.Domain.Entities;
using LiftOps_BackEnd.Domain.Interfaces;
using LiftOps_BackEnd.Domain.Interfaces.Installation;
using LiftOps_BackEnd.Domain.Interfaces.Maintenance;
using LiftOps_BackEnd.Domain.Interfaces.Faults;
using LiftOps_BackEnd.Domain.Interfaces.Emergency;
using LiftOps_BackEnd.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LiftOps_BackEnd.Application.Interfaces;
using LiftOps_BackEnd.Infrastructure.Services;
using LiftOps_BackEnd.Infrastructure.Repositories;
using LiftOps_BackEnd.Infrastructure.Repositories.Installation;
using LiftOps_BackEnd.Infrastructure.Repositories.Maintenance;
using LiftOps_BackEnd.Infrastructure.Repositories.Faults;
using LiftOps_BackEnd.Infrastructure.Repositories.Emergency;
using LiftOps_BackEnd.Application.Interfaces.Installation;
using LiftOps_BackEnd.Application.Interfaces.Maintenance;
using LiftOps_BackEnd.Application.Interfaces.Faults;
using LiftOps_BackEnd.Application.Interfaces.Emergency;
using LiftOps_BackEnd.Infrastructure.Services.Installation;
using LiftOps_BackEnd.Infrastructure.Services.Maintenance;
using LiftOps_BackEnd.Infrastructure.Services.Emergency;

namespace LiftOps_BackEnd.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var useInMemory = configuration.GetValue("UseInMemoryDatabase", false);

        if (useInMemory)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("LiftOpsDb"));
        }
        else
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString,
                    b =>
                    {
                        b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                        b.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(10),
                            errorNumbersToAdd: null);
                    }));
        }

        services.AddIdentity<AppUser, IdentityRole<Guid>>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredLength = 8;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<ICurrentTenantService, CurrentTenantService>();
        services.AddScoped<ICompanyProvisioningService, CompanyProvisioningService>();
        services.AddScoped<ISubscriptionLifecycleService, SubscriptionLifecycleService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<LiftOps_BackEnd.Application.Interfaces.IApplicationDbContext>(sp =>
            sp.GetRequiredService<ApplicationDbContext>());

        // Installation Module Repositories
        services.AddScoped<IInstallationProjectRepository, InstallationProjectRepository>();
        services.AddScoped<IElevatorRepository, ElevatorRepository>();
        services.AddScoped<IInstallationStageRepository, InstallationStageRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IStageRequiredPartRepository, StageRequiredPartRepository>();
        services.AddScoped<IStageTechnicianRepository, StageTechnicianRepository>();
        services.AddScoped<ITechnicianRepository, TechnicianRepository>();
        services.AddScoped<ITechnicianAssignmentRepository, TechnicianAssignmentRepository>();
        services.AddScoped<IInspectionRequestRepository, InspectionRequestRepository>();
        services.AddScoped<IOfferRepository, OfferRepository>();
        services.AddScoped<IQuotationRepository, QuotationRepository>();
        services.AddScoped<IMaintenanceRepository, MaintenanceRepository>();
        services.AddScoped<IFaultRepository, FaultRepository>();
        services.AddScoped<IEmergencyRepository, EmergencyRepository>();

        // Installation Module Services
        services.AddScoped<IInstallationProjectService, InstallationProjectService>();
        services.AddScoped<IElevatorService, ElevatorService>();
        services.AddScoped<IStageService, StageService>();
        services.AddScoped<IPartSelectionService, PartSelectionService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IPdfGenerator, PdfGeneratorService>();
        services.AddScoped<ITechnicianService, TechnicianService>();
        services.AddScoped<IMaintenanceService, LiftOps_BackEnd.Infrastructure.Services.Maintenance.MaintenanceService>();
        services.AddScoped<IFaultService, LiftOps_BackEnd.Infrastructure.Services.Faults.FaultService>();
        services.AddScoped<IEmergencyService, EmergencyService>();
        services.AddScoped<LiftOps_BackEnd.Application.Interfaces.Installation.ICustomerStatusService, CustomerStatusService>();

        if (!useInMemory)
            services.AddHostedService<FreeMaintenanceProcessorService>();

        return services;
    }
}
