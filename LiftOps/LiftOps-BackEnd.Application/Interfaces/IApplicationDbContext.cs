using LiftOps_BackEnd.Domain.Entities;
using LiftOps_BackEnd.Domain.Entities.Installation;
using LiftOps_BackEnd.Domain.Entities.Maintenance;
using LiftOps_BackEnd.Domain.Entities.Subscription;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace LiftOps_BackEnd.Application.Interfaces;

public interface IApplicationDbContext
{
    DatabaseFacade Database { get; }
    DbSet<Company> Companies { get; }
    DbSet<Subscription> Subscriptions { get; }
    DbSet<SubscriptionPlan> SubscriptionPlans { get; }
    DbSet<AppUser> Users { get; }
    DbSet<IdentityUserRole<Guid>> UserRoles { get; }
    DbSet<IdentityRole<Guid>> Roles { get; }
    DbSet<Elevator> Elevators { get; }
    DbSet<MaintenanceElevator> MaintenanceElevators { get; }
    DbSet<MaintenanceContract> MaintenanceContracts { get; }
    DbSet<InstallationProject> InstallationProjects { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
