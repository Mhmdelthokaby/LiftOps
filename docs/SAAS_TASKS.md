# LiftOps SaaS Transformation Tasks

Production-oriented backlog for the merged Next.js project (`lifops-next/`).

## Audit Status

### ✅ Completed — Platform Authentication
- Super Admin login with JWT, httpOnly cookies, role-based auth.
- Platform admin UI under `src/app/(admin)/admin/`.
- Middleware auth guard for `/admin/*` routes.
- Token refresh via httpOnly cookie rotation.
- Auth bypass dev mode (`NEXT_PUBLIC_DEV_BYPASS_ADMIN_AUTH`).

### ✅ Completed — Multi-Tenant Foundation
- `Company` entity with `companyId` on all business tables.
- JWT `company_id` claim for tenant users (omitted for `SUPER_ADMIN`).
- Service-layer tenant filtering on all queries.
- Platform admin routes bypass tenant scoping.

### ✅ Completed — Core Modules
- Installation pipeline (customers, elevators, stages, projects, offers, inspections, technicians).
- Maintenance contracts, visits, checklists.
- Emergency and fault ticket workflows.
- Inventory management with categories.
- Dashboard with KPIs and charts.
- Subscription plans and management.

### ✅ Completed — Prisma Migration
- Schema converted from EF Core to Prisma.
- All relations validated for Prisma 6.
- Seed script for development data.

## Next Critical Tasks

| Priority | Task |
|----------|------|
| P0 | End-to-end testing of all API routes |
| P0 | Build verification (`npm run build`) |
| P1 | Error boundary and loading states for all pages |
| P1 | PDF generation for reports and invoices |
| P2 | Email service integration |
| P2 | Real-time notifications (SSE/WebSocket) |

## Known Gaps

- Finance module: partial implementation
- Email service: not wired
- Faults UI (FE-005): matching frontend screens for backend tickets
- Public signup flow (tenant onboarding)
- Subscription enforcement middleware for write operations
