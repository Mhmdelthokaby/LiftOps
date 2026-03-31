# Tenant Role Matrix

This matrix documents role intent per company (tenant) for the current MVP.

| Role | Scope | Key permissions |
|---|---|---|
| Manager | Tenant-wide | Manage users and roles in the same company, full module access |
| InstallationAdmin | Tenant-wide | Installation workflows, projects, stages, technicians (installation side) |
| MaintenanceAdmin | Tenant-wide | Maintenance contracts, visits, scheduling, operational maintenance actions |
| InventoryAdmin | Tenant-wide | Inventory items, categories, stock operations |
| FinanceAdmin | Tenant-wide | Finance-related endpoints and reporting access |
| FaultsAdmin | Tenant-wide | Fault ticket management and emergency dispatch/resolution workflows |
| Technician | Tenant-scoped operational | Assigned field operations and limited maintenance/emergency actions |

Rules:
- Every user belongs to exactly one company in MVP (`AppUser.CompanyId`).
- The first user created in a company is automatically assigned `Manager`.
- Cross-tenant role assignment is not allowed.
