# API Tenant Audit Sweep

This checklist records tenant-context review for API controllers and their MediatR flows.

- Tenant claim enforcement is centralized in JWT validation for authorized endpoints (`company_id` required except platform routes).
- Tenant filtering is enforced by EF global query filters in `ApplicationDbContext` for tenant entities.
- SaveChanges tenant stamping assigns `CompanyId` for inserts when not explicitly set.
- Cross-tenant FK guards block unsafe relationship links at persistence layer.

Controller notes:
- `InstallationController`: tenant-scoped policy + query filters cover project/elevator/stage resources.
- `MaintenanceController`: role-gated with tenant filters and signed PDF access checks.
- `InventoryController`: audited with integration tests proving cross-tenant id update/read isolation.
- `EmergencyController` / `FaultsController`: policy-based authorization; tenant filters enforce isolation.
- `AdminController`: company-aware user operations (list/update/disable/roles) scoped by tenant checks.
- `PlatformSubscriptionsController`: intentionally platform-scoped and excluded from tenant claim requirement.
