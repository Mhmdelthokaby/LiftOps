using LiftOps_BackEnd.Domain.Entities;
using LiftOps_BackEnd.Domain.Entities.Installation;
using LiftOps_BackEnd.Domain.Entities.Maintenance;
using LiftOps_BackEnd.Domain.Entities.Faults;
using LiftOps_BackEnd.Domain.Entities.Emergency;
using LiftOps_BackEnd.Domain.Entities.Subscription;
using LiftOps_BackEnd.Application.Interfaces; // Restored
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LiftOps_BackEnd.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<AppUser, Microsoft.AspNetCore.Identity.IdentityRole<Guid>, Guid>, LiftOps_BackEnd.Application.Interfaces.IApplicationDbContext
{
    private readonly ICurrentUserService _currentUserService;
    private readonly ICurrentTenantService _currentTenantService;
    private bool _bypassTenantFilter;
    private Guid? _overrideTenantId;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ICurrentUserService currentUserService,
        ICurrentTenantService currentTenantService)
        : base(options)
    {
        _currentUserService = currentUserService;
        _currentTenantService = currentTenantService;
    }

    private Guid? EffectiveTenantId => _overrideTenantId ?? _currentTenantService.CompanyId;
    private bool BypassTenantFilter => _bypassTenantFilter;

    public IDisposable UseSystemTenantBypass(Guid? explicitTenantId = null)
    {
        var previousBypass = _bypassTenantFilter;
        var previousOverrideTenantId = _overrideTenantId;

        _bypassTenantFilter = true;
        _overrideTenantId = explicitTenantId;

        return new TenantBypassScope(() =>
        {
            _bypassTenantFilter = previousBypass;
            _overrideTenantId = previousOverrideTenantId;
        });
    }

    private sealed class TenantBypassScope : IDisposable
    {
        private readonly Action _disposeAction;
        private bool _disposed;

        public TenantBypassScope(Action disposeAction)
        {
            _disposeAction = disposeAction;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposeAction();
            _disposed = true;
        }
    }

    private void ApplyTenantQueryFilter<TEntity>(ModelBuilder modelBuilder)
        where TEntity : LiftOps_BackEnd.Domain.Common.BaseAuditableEntity
    {
        // Use Guid? == Guid (never .Value) so evaluation does not throw when tenant is null but bypass is true.
        modelBuilder.Entity<TEntity>()
            .HasQueryFilter(e => BypassTenantFilter || EffectiveTenantId == e.CompanyId);
    }

    public DbSet<InventoryItem> InventoryItems { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Company> Companies { get; set; } = null!;
    public DbSet<Subscription> Subscriptions { get; set; } = null!;
    public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; } = null!;

    // Installation Module
    public DbSet<Customer> Customers { get; set; } = null!;
    public DbSet<InstallationProject> InstallationProjects { get; set; } = null!;
    public DbSet<Elevator> Elevators { get; set; } = null!;
    public DbSet<InstallationStage> InstallationStages { get; set; } = null!;
    public DbSet<StageRequiredPart> StageRequiredParts { get; set; } = null!;
    public DbSet<StageTechnician> StageTechnicians { get; set; } = null!;
    public DbSet<Notification> Notifications { get; set; } = null!;
    public DbSet<Technician> Technicians { get; set; } = null!;
    public DbSet<TechnicianAssignment> TechnicianAssignments { get; set; } = null!;
    public DbSet<InspectionRequest> InspectionRequests { get; set; } = null!;
    public DbSet<Offer> Offers { get; set; } = null!;
    public DbSet<Quotation> Quotations { get; set; } = null!;
    public DbSet<QuotationAttachment> QuotationAttachments { get; set; } = null!;

    // Maintenance Module
    public DbSet<MaintenanceContract> MaintenanceContracts { get; set; } = null!;
    public DbSet<MaintenanceElevator> MaintenanceElevators { get; set; } = null!;
    public DbSet<MaintenanceVisit> MaintenanceVisits { get; set; } = null!;
    public DbSet<MaintenanceSparePartUsage> MaintenanceSparePartUsages { get; set; } = null!;
    public DbSet<MaintenanceChecklistItem> MaintenanceChecklistItems { get; set; } = null!;
    public DbSet<MaintenanceVisitChecklistItem> MaintenanceVisitChecklistItems { get; set; } = null!;
    
    // Faults Module
    public DbSet<FaultTicket> FaultTickets { get; set; } = null!;
    public DbSet<FaultSparePartUsage> FaultSparePartUsages { get; set; } = null!;
    
    // Emergency Module
    public DbSet<EmergencyTicket> EmergencyTickets { get; set; } = null!;

    public override int SaveChanges()
    {
        ApplyAuditAndTenantStamp();
        ValidateCrossTenantReferences();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditAndTenantStamp();
        ValidateCrossTenantReferences();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyAuditAndTenantStamp()
    {
        var tenantId = _currentTenantService.CompanyId;

        foreach (var entry in ChangeTracker.Entries<AppUser>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    if ((!entry.Entity.CompanyId.HasValue || entry.Entity.CompanyId == Guid.Empty) && tenantId.HasValue)
                    {
                        entry.Entity.CompanyId = tenantId.Value;
                    }
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.CreatedBy = _currentUserService.UserEmail;
                    break;
                case EntityState.Modified:
                    entry.Entity.LastModifiedAt = DateTime.UtcNow;
                    entry.Entity.LastModifiedBy = _currentUserService.UserEmail;
                    break;
            }
        }

        foreach (var entry in ChangeTracker.Entries<LiftOps_BackEnd.Domain.Common.BaseAuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    if (entry.Entity.CompanyId == Guid.Empty && tenantId.HasValue)
                    {
                        entry.Entity.CompanyId = tenantId.Value;
                    }
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.CreatedBy = _currentUserService.UserEmail;
                    break;
                case EntityState.Modified:
                    entry.Entity.LastModifiedAt = DateTime.UtcNow;
                    entry.Entity.LastModifiedBy = _currentUserService.UserEmail;
                    break;
            }
        }
    }

    private void ValidateCrossTenantReferences()
    {
        ValidateTechnicianAssignments();
        ValidateStageTechnicians();
        ValidateMaintenanceElevators();
    }

    private void ValidateTechnicianAssignments()
    {
        var entries = ChangeTracker.Entries<TechnicianAssignment>()
            .Where(e => e.State is EntityState.Added or EntityState.Modified)
            .ToList();

        foreach (var entry in entries)
        {
            var assignmentCompanyId = entry.Entity.CompanyId;
            var technicianCompanyId = ResolveCompanyId<Technician>(entry.Entity.TechnicianId);
            var elevatorCompanyId = ResolveCompanyId<Elevator>(entry.Entity.ElevatorId);

            if (assignmentCompanyId != technicianCompanyId || assignmentCompanyId != elevatorCompanyId)
            {
                throw new InvalidOperationException("Cross-tenant technician assignment is not allowed.");
            }
        }
    }

    private void ValidateStageTechnicians()
    {
        var entries = ChangeTracker.Entries<StageTechnician>()
            .Where(e => e.State is EntityState.Added or EntityState.Modified)
            .ToList();

        foreach (var entry in entries)
        {
            var assignmentCompanyId = entry.Entity.CompanyId;
            var stageCompanyId = ResolveCompanyId<InstallationStage>(entry.Entity.StageId);
            var technicianCompanyId = ResolveCompanyId<Technician>(entry.Entity.TechnicianId);

            if (assignmentCompanyId != stageCompanyId || assignmentCompanyId != technicianCompanyId)
            {
                throw new InvalidOperationException("Cross-tenant stage technician assignment is not allowed.");
            }
        }
    }

    private void ValidateMaintenanceElevators()
    {
        var entries = ChangeTracker.Entries<MaintenanceElevator>()
            .Where(e => e.State is EntityState.Added or EntityState.Modified)
            .ToList();

        foreach (var entry in entries)
        {
            var elevatorCompanyId = entry.Entity.CompanyId;
            var contractCompanyId = ResolveCompanyId<MaintenanceContract>(entry.Entity.ContractId);

            if (elevatorCompanyId != contractCompanyId)
            {
                throw new InvalidOperationException("Cross-tenant maintenance elevator link is not allowed.");
            }
        }
    }

    private Guid ResolveCompanyId<TEntity>(Guid id)
        where TEntity : LiftOps_BackEnd.Domain.Common.BaseAuditableEntity
    {
        var tracked = ChangeTracker.Entries<TEntity>()
            .FirstOrDefault(e => e.Entity.Id == id && e.State != EntityState.Deleted);
        if (tracked != null)
        {
            return tracked.Entity.CompanyId;
        }

        return Set<TEntity>()
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => x.CompanyId)
            .FirstOrDefault();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Apply configurations if needed
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        modelBuilder.Entity<InventoryItem>()
            .Property(i => i.UnitPrice)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Company>()
            .Property(c => c.Name)
            .HasMaxLength(200);

        modelBuilder.Entity<Company>()
            .Property(c => c.Slug)
            .HasMaxLength(120);

        modelBuilder.Entity<Company>()
            .Property(c => c.BillingContactEmail)
            .HasMaxLength(256);

        modelBuilder.Entity<Company>()
            .Property(c => c.Timezone)
            .HasMaxLength(100);

        modelBuilder.Entity<Company>()
            .Property(c => c.ContactPhone)
            .HasMaxLength(50);

        modelBuilder.Entity<Company>()
            .Property(c => c.SuspensionReason)
            .HasMaxLength(2000);

        modelBuilder.Entity<Company>()
            .HasIndex(c => c.IsDeleted);

        modelBuilder.Entity<Company>()
            .HasIndex(c => c.Slug)
            .IsUnique()
            .HasFilter("[Slug] IS NOT NULL AND [Slug] <> ''");

        modelBuilder.Entity<Company>()
            .HasOne(c => c.Plan)
            .WithMany()
            .HasForeignKey(c => c.PlanId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<SubscriptionPlan>()
            .Property(p => p.Code)
            .HasMaxLength(64);

        modelBuilder.Entity<SubscriptionPlan>()
            .Property(p => p.Name)
            .HasMaxLength(200);

        modelBuilder.Entity<SubscriptionPlan>()
            .Property(p => p.MonthlyPrice)
            .HasPrecision(18, 2);

        modelBuilder.Entity<SubscriptionPlan>()
            .Property(p => p.YearlyPrice)
            .HasPrecision(18, 2);

        modelBuilder.Entity<SubscriptionPlan>()
            .Property(p => p.FeatureFlagsJson)
            .HasMaxLength(4000);

        modelBuilder.Entity<SubscriptionPlan>()
            .HasIndex(p => p.Code)
            .IsUnique();

        modelBuilder.Entity<Subscription>()
            .Property(s => s.ExternalCustomerId)
            .HasMaxLength(256);

        // Company is the tenant root and should not reference another tenant.
        modelBuilder.Entity<Company>()
            .Ignore(c => c.CompanyId);

        modelBuilder.Entity<AppUser>()
            .HasIndex(u => u.CompanyId);

        modelBuilder.Entity<AppUser>()
            .HasOne<Company>()
            .WithMany()
            .HasForeignKey(u => u.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        // Apply tenant relationship consistently across all business entities.
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clrType = entityType.ClrType;
            if (!typeof(LiftOps_BackEnd.Domain.Common.BaseAuditableEntity).IsAssignableFrom(clrType) || clrType == typeof(Company))
            {
                continue;
            }

            modelBuilder.Entity(clrType)
                .HasOne(typeof(Company))
                .WithMany()
                .HasForeignKey("CompanyId")
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity(clrType)
                .HasIndex("CompanyId");

            modelBuilder.Entity(clrType)
                .HasIndex("CompanyId", "CreatedAt");

            if (entityType.FindProperty("Status") != null)
            {
                modelBuilder.Entity(clrType)
                    .HasIndex("CompanyId", "Status");
            }

            typeof(ApplicationDbContext)
                .GetMethod(nameof(ApplyTenantQueryFilter), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!
                .MakeGenericMethod(clrType)
                .Invoke(this, new object[] { modelBuilder });
        }

        modelBuilder.Entity<InstallationProject>()
           .Property(p => p.InstallationPricePerUnit)
           .HasPrecision(18, 2);

        modelBuilder.Entity<Elevator>()
            .Property(e => e.PitWidth)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Elevator>()
            .Property(e => e.PitDepth)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Elevator>()
            .Property(e => e.LastFloorHeight)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Elevator>()
            .Property(e => e.HoleDepth)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Elevator>()
            .Property(e => e.TravelLength)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Elevator>()
            .Property(e => e.Price)
            .HasPrecision(18, 2);

        modelBuilder.Entity<InstallationProject>()
            .Property(p => p.TotalPrice)
            .HasPrecision(18, 2);

        modelBuilder.Entity<MaintenanceVisitChecklistItem>()
            .Property(v => v.Percentage)
            .HasPrecision(5, 2); // Percentage: 0.00 to 100.00

        // Tenant-scoped unique natural keys.
        modelBuilder.Entity<InstallationProject>()
            .HasIndex(p => new { p.CompanyId, p.ProjectNumber })
            .IsUnique()
            .HasFilter("[ProjectNumber] IS NOT NULL AND [ProjectNumber] <> ''");

        modelBuilder.Entity<MaintenanceContract>()
            .HasIndex(c => new { c.CompanyId, c.ProjectNumber })
            .IsUnique()
            .HasFilter("[ProjectNumber] IS NOT NULL AND [ProjectNumber] <> ''");

        modelBuilder.Entity<FaultTicket>()
            .HasIndex(f => new { f.CompanyId, f.TicketNumber })
            .IsUnique()
            .HasFilter("[TicketNumber] IS NOT NULL AND [TicketNumber] <> ''");

        modelBuilder.Entity<EmergencyTicket>()
            .HasIndex(e => new { e.CompanyId, e.TicketNumber })
            .IsUnique();

        modelBuilder.Entity<InventoryItem>()
            .HasIndex(i => new { i.CompanyId, i.ItemNumber })
            .IsUnique()
            .HasFilter("[ItemNumber] IS NOT NULL AND [ItemNumber] <> ''");

        modelBuilder.Entity<InstallationStage>()
            .Property(s => s.SupplyCost)
            .HasPrecision(18, 2);

        modelBuilder.Entity<InstallationStage>()
            .Property(s => s.StagePrice)
            .HasPrecision(18, 2);
        
        // Technician self-referencing relationship (Leader)
        modelBuilder.Entity<Technician>()
            .HasOne(t => t.Leader)
            .WithMany(t => t.Subordinates)
            .HasForeignKey(t => t.LeaderId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

        // Technician Assignment M:N
        modelBuilder.Entity<TechnicianAssignment>()
            .HasKey(ta => new { ta.TechnicianId, ta.ElevatorId });

        modelBuilder.Entity<TechnicianAssignment>()
            .HasOne(ta => ta.Technician)
            .WithMany(t => t.Assignments)
            .HasForeignKey(ta => ta.TechnicianId)
            .OnDelete(DeleteBehavior.Restrict); // Don't delete tech if assignment deleted

        modelBuilder.Entity<TechnicianAssignment>()
            .HasOne(ta => ta.Elevator)
            .WithMany(e => e.TechnicianAssignments)
            .HasForeignKey(ta => ta.ElevatorId)
            .OnDelete(DeleteBehavior.Cascade); // Delete assignments if elevator deleted

        // Stage Technician M:N
        modelBuilder.Entity<StageTechnician>()
            .HasOne(st => st.Stage)
            .WithMany(s => s.Technicians)
            .HasForeignKey(st => st.StageId)
            .OnDelete(DeleteBehavior.Cascade); // Delete technician assignments if stage deleted

        modelBuilder.Entity<StageTechnician>()
            .HasOne(st => st.Technician)
            .WithMany()
            .HasForeignKey(st => st.TechnicianId)
            .OnDelete(DeleteBehavior.Restrict); // Don't delete tech if stage assignment deleted

        // Maintenance Configurations
        modelBuilder.Entity<MaintenanceContract>()
            .Property(c => c.PricePerMonth)
            .HasPrecision(18, 2);

        modelBuilder.Entity<MaintenanceSparePartUsage>()
            .Property(s => s.PriceAtTimeOfUsage)
            .HasPrecision(18, 2);

        modelBuilder.Entity<FaultSparePartUsage>()
            .Property(s => s.PriceAtTimeOfUsage)
            .HasPrecision(18, 2);
        
        // Inspection & Offer Configurations
        modelBuilder.Entity<Offer>()
            .Property(o => o.InstallationPricePerUnit)
            .HasPrecision(18, 2);
        
        modelBuilder.Entity<Offer>()
            .Property(o => o.TotalInstallationPrice)
            .HasPrecision(18, 2);
        
        modelBuilder.Entity<InspectionRequest>()
            .Property(i => i.ShaftWidth)
            .HasPrecision(18, 2);
        
        modelBuilder.Entity<InspectionRequest>()
            .Property(i => i.ShaftDepth)
            .HasPrecision(18, 2);
        
        modelBuilder.Entity<InspectionRequest>()
            .Property(i => i.LastFloorHeight)
            .HasPrecision(18, 2);
        
        modelBuilder.Entity<InspectionRequest>()
            .Property(i => i.PitDepth)
            .HasPrecision(18, 2);
        
        modelBuilder.Entity<InspectionRequest>()
            .Property(i => i.TravelHeight)
            .HasPrecision(18, 2);
        
        // InstallationProject technical fields precision
        modelBuilder.Entity<InstallationProject>()
            .Property(p => p.ShaftWidth)
            .HasPrecision(18, 2);
        
        modelBuilder.Entity<InstallationProject>()
            .Property(p => p.ShaftDepth)
            .HasPrecision(18, 2);
        
        modelBuilder.Entity<InstallationProject>()
            .Property(p => p.LastFloorHeight)
            .HasPrecision(18, 2);
        
        modelBuilder.Entity<InstallationProject>()
            .Property(p => p.PitDepth)
            .HasPrecision(18, 2);
        
        modelBuilder.Entity<InstallationProject>()
            .Property(p => p.TravelHeight)
            .HasPrecision(18, 2);
        
        modelBuilder.Entity<InstallationProject>()
            .Property(p => p.HoleDepth)
            .HasPrecision(18, 2);
        
        // Quotation configurations
        modelBuilder.Entity<Quotation>()
            .Property(q => q.Price)
            .HasPrecision(18, 2);
        
        // Relationships
        modelBuilder.Entity<InspectionRequest>()
            .HasOne(i => i.Offer)
            .WithOne(o => o.InspectionRequest)
            .HasForeignKey<Offer>(o => o.InspectionRequestId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<InspectionRequest>()
            .HasOne(i => i.ConvertedToProject)
            .WithOne(p => p.ConvertedFromInspection)
            .HasForeignKey<InspectionRequest>(i => i.ConvertedToProjectId)
            .OnDelete(DeleteBehavior.SetNull);
        
        // InspectionRequest to Customer relationship (optional)
        // Use NoAction to avoid cascade path conflicts
        modelBuilder.Entity<InspectionRequest>()
            .HasOne(i => i.Client)
            .WithMany()
            .HasForeignKey(i => i.ClientId)
            .OnDelete(DeleteBehavior.NoAction);
        
        // Quotation to InstallationProject relationship
        modelBuilder.Entity<Quotation>()
            .HasOne(q => q.Project)
            .WithOne(p => p.Quotation)
            .HasForeignKey<Quotation>(q => q.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // QuotationAttachment to Quotation relationship
        modelBuilder.Entity<QuotationAttachment>()
            .HasOne(a => a.Quotation)
            .WithMany(q => q.Attachments)
            .HasForeignKey(a => a.QuotationId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Maintenance Checklist Item relationships
        modelBuilder.Entity<MaintenanceVisitChecklistItem>()
            .HasOne(vci => vci.Visit)
            .WithMany(v => v.ChecklistItems)
            .HasForeignKey(vci => vci.VisitId)
            .OnDelete(DeleteBehavior.Cascade); // Delete checklist items when visit is deleted

        modelBuilder.Entity<MaintenanceVisitChecklistItem>()
            .HasOne(vci => vci.ChecklistItem)
            .WithMany(ci => ci.VisitChecklistItems)
            .HasForeignKey(vci => vci.ChecklistItemId)
            .OnDelete(DeleteBehavior.Restrict); // Don't delete checklist item template if used in visits

        // Seed Subscription Plans
        modelBuilder.Entity<SubscriptionPlan>().HasData(
            new SubscriptionPlan
            {
                Id = new Guid("11111111-1111-1111-1111-111111111111"),
                Name = "Basic",
                Code = "basic",
                MonthlyPrice = 49.00m,
                YearlyPrice = 490.00m,
                MaxUsers = 5,
                MaxElevators = 50,
                IsActive = true
            },
            new SubscriptionPlan
            {
                Id = new Guid("22222222-2222-2222-2222-222222222222"),
                Name = "Pro",
                Code = "pro",
                MonthlyPrice = 99.00m,
                YearlyPrice = 990.00m,
                MaxUsers = 20,
                MaxElevators = 200,
                IsActive = true
            },
            new SubscriptionPlan
            {
                Id = new Guid("33333333-3333-3333-3333-333333333333"),
                Name = "Enterprise",
                Code = "enterprise",
                MonthlyPrice = 249.00m,
                YearlyPrice = 2490.00m,
                MaxUsers = 100,
                MaxElevators = 1000,
                IsActive = true
            }
        );
    }
}
