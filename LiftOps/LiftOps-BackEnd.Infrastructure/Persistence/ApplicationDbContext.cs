using LiftOps_BackEnd.Domain.Entities;
using LiftOps_BackEnd.Domain.Entities.Installation;
using LiftOps_BackEnd.Domain.Entities.Maintenance;
using LiftOps_BackEnd.Domain.Entities.Faults;
using LiftOps_BackEnd.Domain.Entities.Emergency;
using LiftOps_BackEnd.Application.Interfaces; // Restored
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LiftOps_BackEnd.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<AppUser, Microsoft.AspNetCore.Identity.IdentityRole<Guid>, Guid>
{
    private readonly ICurrentUserService _currentUserService;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ICurrentUserService currentUserService)
        : base(options)
    {
        _currentUserService = currentUserService;
    }

    public DbSet<InventoryItem> InventoryItems { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Company> Companies { get; set; } = null!;

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

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<AppUser>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
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
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.CreatedBy = _currentUserService.UserEmail;
                    break;
                case EntityState.Modified:
                    entry.Entity.LastModifiedAt = DateTime.UtcNow;
                    entry.Entity.LastModifiedBy = _currentUserService.UserEmail;
                    break;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
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
            .HasIndex(c => c.Slug)
            .IsUnique()
            .HasFilter("[Slug] IS NOT NULL AND [Slug] <> ''");

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

        // Ensure ProjectNumber is unique
        modelBuilder.Entity<InstallationProject>()
            .HasIndex(p => p.ProjectNumber)
            .IsUnique()
            .HasFilter("[ProjectNumber] IS NOT NULL AND [ProjectNumber] <> ''");

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
    }
}
