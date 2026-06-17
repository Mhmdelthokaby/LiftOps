# LiftOps Architecture

The project is a single Next.js 15 monolith (App Router) that serves both API routes and frontend pages from one codebase.

## Tech Stack

| Layer | Technology |
|-------|------------|
| Framework | Next.js 15 (App Router) |
| Language | TypeScript, React 19 |
| Styling | Tailwind CSS 4, ShadCN/Radix UI |
| Authentication | JWT (jose), httpOnly cookies, bcryptjs |
| Database | PostgreSQL |
| ORM | Prisma 6 |
| API | Next.js Route Handlers |

## High-Level Structure

```
src/
├── app/           # Pages + API routes (Next.js App Router)
│   ├── api/       # All backend endpoints
│   ├── (admin)/   # Platform admin UI
│   ├── (auth)/    # Login pages
│   └── (marketing)/ # Landing pages
├── components/    # Reusable React components
├── lib/           # Business logic, services, auth, validators
├── config/        # App configuration
├── types/         # Shared TypeScript types
└── middleware.ts  # Edge middleware (auth guard)
```

## Data Flow

```
Browser → Next.js Edge Middleware (auth check)
  → Route Handler (API) or Page Component (SSR/CSR)
    → Service Layer (src/lib/services/)
      → Prisma Client → PostgreSQL
```

## API Layer

All backend logic lives in `src/app/api/`. Route Handlers:

1. Parse + validate request input (zod schemas).
2. Authenticate via JWT (middleware or per-route auth helpers).
3. Dispatch to service layer.
4. Return typed JSON responses.

## Service Layer

Business logic is organized by domain in `src/lib/services/`:

- `auth/` — login, register, refresh, logout
- `company/` — company CRUD
- `dashboard/` — KPI metrics
- `installation/` — customers, elevators, stages, projects, offers, inspections, technicians
- `maintenance/` — contracts, visits, checklists
- `inventory/` — items, categories
- `tickets/` — fault/emergency tickets
- `emergency/` — emergency ticket workflows
- `subscription/` — plans, subscriptions, billing

## Auth Flow

```
Login → POST /api/auth/login
  → Validate credentials (bcryptjs)
  → Generate JWT (jose) with claims: sub, email, name, role, company_id
  → Set httpOnly cookies: liftops_access, liftops_refresh
  → Return { token, refreshToken, name, email, roles }

Middleware (src/middleware.ts):
  - API paths: auto-refresh expired tokens via refresh cookie
  - Page paths: verify JWT from cookie for /admin/* routes
```

## Multi-Tenant Strategy

- **Tenant = Company** (`companies` table).
- All business tables inherit `companyId`.
- JWT includes `company_id` claim for tenant users (omitted for `SUPER_ADMIN`).
- Service layer filters queries by `companyId` from the authenticated user's token.
- Platform admin routes under `/api/platform/*` bypass tenant scoping.

## Key Directories

| Path | Purpose |
|------|---------|
| `prisma/schema.prisma` | Database schema |
| `src/app/api/` | All API endpoints |
| `src/lib/services/` | Business logic |
| `src/lib/auth/` | JWT, password hashing, session |
| `src/lib/validators/` | Zod schemas |
| `src/lib/response/` | Response helpers |
| `src/middleware.ts` | Auth guard + token refresh |
