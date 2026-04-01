# LiftOps Project Idea

LiftOps is a multi-tenant SaaS platform for elevator companies to run installation, maintenance, emergency, inventory, and admin workflows in one system.

## Vision

- Replace spreadsheet/manual operations with a structured workflow system.
- Keep each company isolated using tenant-based security.
- Give role-based access so each team sees only what they need.
- Provide measurable operational visibility via dashboards and reports.

## Core Business Modules

- Installation projects and stage progress tracking.
- Maintenance contracts, scheduling, and visit execution.
- Emergency and fault ticket handling.
- Inventory and spare parts management.
- Technician assignment and daily operations.
- Financial tracking and reporting.

## Product Roles

- `Manager`
- `InstallationAdmin`
- `MaintenanceAdmin`
- `InventoryAdmin`
- `FinanceAdmin`
- `FaultsAdmin`
- `Technician`
- `PlatformAdmin` (platform-level SaaS operations)

## Technical Direction

- Backend: ASP.NET Core + Clean Architecture + MediatR + EF Core + SQL Server.
- Frontend: Next.js App Router + TypeScript + ShadCN UI.
- Security: JWT with role and tenant claims (`company_id`).
- SaaS foundation: company-scoped data isolation + subscription enforcement.

## Documentation Map

- Backend implementation guide: `docs/BACKEND_GUIDE.md`
- Frontend implementation guide: `docs/FRONTEND_GUIDE.md`
- SaaS task backlog: `docs/SAAS_TASKS.md`

