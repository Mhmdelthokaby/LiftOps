using Collins_BackEnd.Domain.Entities;
using Collins_BackEnd.Domain.Interfaces;
using Collins_BackEnd.Domain.Interfaces.Installation;
using Collins_BackEnd.Domain.Interfaces.Maintenance;
using Collins_BackEnd.Domain.Interfaces.Faults;
using Collins_BackEnd.Domain.Interfaces.Emergency;
using Collins_BackEnd.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Collins_BackEnd.Application.Interfaces;
using Collins_BackEnd.Infrastructure.Services;
using Collins_BackEnd.Infrastructure.Repositories;
using Collins_BackEnd.Infrastructure.Repositories.Installation;
using Collins_BackEnd.Infrastructure.Repositories.Maintenance;
using Collins_BackEnd.Infrastructure.Repositories.Faults;
using Collins_BackEnd.Infrastructure.Repositories.Emergency;
using Collins_BackEnd.Application.Interfaces.Installation;
using Collins_BackEnd.Application.Interfaces.Maintenance;
using Collins_BackEnd.Application.Interfaces.Faults;
using Collins_BackEnd.Application.Interfaces.Emergency;
using Collins_BackEnd.Infrastructure.Services.Installation;
using Collins_BackEnd.Infrastructure.Services.Maintenance;
using Collins_BackEnd.Infrastructure.Services.Emergency;

namespace Collins_BackEnd.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString,
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

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
        services.AddScoped<ITokenService, TokenService>();

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
        services.AddScoped<IMaintenanceService, Collins_BackEnd.Infrastructure.Services.Maintenance.MaintenanceService>();
        services.AddScoped<IFaultService, Collins_BackEnd.Infrastructure.Services.Faults.FaultService>();
        services.AddScoped<IEmergencyService, EmergencyService>();
        services.AddScoped<Collins_BackEnd.Application.Interfaces.Installation.ICustomerStatusService, CustomerStatusService>();

        // Background Services
        services.AddHostedService<FreeMaintenanceProcessorService>();

        return services;
    }
}
