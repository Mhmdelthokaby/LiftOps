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

- `MANAGER`, `ADMIN`, `TECHNICIAN`, `CLIENT`
- `SUPER_ADMIN` (platform-level SaaS operations)

## Technical Stack

| Layer | Technology |
|-------|------------|
| Framework | Next.js 15 (App Router) |
| Language | TypeScript, React 19 |
| Styling | Tailwind CSS 4 + ShadCN/Radix UI |
| Database | PostgreSQL |
| ORM | Prisma 6 |
| Auth | JWT (jose) + httpOnly cookies + bcryptjs |
| API | Next.js Route Handlers (monolith) |

## Documentation Map

- Architecture guide: `docs/ARCHITECTURE.md`
- API guide: `docs/API_GUIDE.md`
- Frontend guide: `docs/FRONTEND_GUIDE.md`
- SaaS task backlog: `docs/SAAS_TASKS.md`
- Project map: `lifops-next/PROJECT_MAP.md`
