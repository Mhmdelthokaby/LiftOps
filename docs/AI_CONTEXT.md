# AI Context — LiftOps (Master Context)

Paste this file at the start of a new chat for the assistant to behave as a senior architect on this repo.

## Project Identity

- **Name:** LiftOps
- **Stack:** Next.js 15 (App Router), React 19, TypeScript, Tailwind 4, ShadCN, PostgreSQL + Prisma 6
- **Core value:** Multi-tenant operations software for elevator businesses — installation, maintenance, inventory, emergency/fault workflows, plus a platform operator console to manage companies, plans, and subscriptions.

## Repository Layout

```
LiftOps/
├── docs/               # Architecture, API, frontend, AI guides
├── prisma/             # Schema + seed
├── public/             # Static assets
├── src/
│   ├── app/
│   │   ├── api/        # All backend routes
│   │   └── *           # Pages
│   ├── components/     # React components
│   ├── lib/            # Services, auth, validators, utils
│   └── middleware.ts   # Edge auth guard
├── PROJECT_MAP.md      # File-by-file reference
```

## Tech Stack (as implemented)

- **Next.js** 15.1.x (App Router, Turbopack)
- **React** 19.x, **TypeScript** 5.x
- **Tailwind CSS** 4.x + **ShadCN** components
- **Prisma** 6.x ORM + **PostgreSQL**
- **JWT** auth: **jose** (sign/verify), **bcryptjs** (password hash)
- **zod** for validation, **react-hook-form** for forms
- **httpOnly cookies** (`liftops_access`, `liftops_refresh`) for auth transport
- **next-themes** with 4 themes (light, dark, sunset, frost)

## Architectural Rules

1. **Monolith:** API routes and frontend pages share the same Next.js project. No separate backend.
2. **API org:** All endpoints under `src/app/api/`. Route Handlers call service layer (`src/lib/services/`), which calls Prisma.
3. **Auth:** JWT via jose library. Access tokens in httpOnly cookie + localStorage. Refresh token rotation.
4. **Multi-tenancy:** `companyId` in JWT claim filters all business queries. Platform admins (`SUPER_ADMIN`) have `companyId` omitted.
5. **Middlewares:** Single `src/middleware.ts` handles both API token refresh and page auth guard.
6. **Response shape:** `{ success, succeeded, data?, message?, error?, errors? }` — `succeeded` duplicates `success` for .NET-API-client compat.

## Naming Conventions

**TypeScript / React:**
- Components: PascalCase (`EmergencyForm.tsx`)
- Route files: Next.js App Router conventions (`page.tsx`, `layout.tsx`, `route.ts`)
- Functions, variables, hooks: camelCase
- Constants: UPPER_SNAKE_CASE

**Prisma / Database:**
- Models: PascalCase
- Tables: snake_case (`@map("companies")`)
- Enums: PascalCase (mapped to UPPER_SNAKE in DB)

## Critical Context

1. **Cookie names:** `liftops_access` and `liftops_refresh` (httpOnly, SameSite=Lax).
2. **Alias route:** `POST /api/Admin/refresh-token` exists for frontend .NET-era compat.
3. **Auth response:** Returns flat `{ token, refreshToken, name, email, roles }`, not wrapped in `data`.
4. **Response format:** Both `success` and `succeeded` boolean fields (succeeded exists for old API client).
5. **URL mapping:** `src/lib/api-client.ts` has `mapEndpoint()` to rewrite .NET-era paths (e.g. `/api/dashboard/summary` → `/api/dashboard`).
6. **Seed credentials:** `admin@lifops.com` / `Admin@123` (platform SUPER_ADMIN).
7. **Prisma** is the single source of truth for DB schema. No raw SQL migrations — use `prisma db push` or `prisma migrate dev`.
8. **Company entity:** `id = "00000000-0000-0000-0000-000000000000"` is the platform company.
9. **Technician** model links to `AppUser` via `userId`. `MaintenanceVisit.technician` references `AppUser`, not `Technician`.

## Key Files

| Area | Path |
|------|------|
| Schema | `prisma/schema.prisma` |
| Seed | `prisma/seed.ts` |
| Middleware | `src/middleware.ts` |
| Auth service | `src/lib/services/auth.service.ts` |
| Auth helpers (client) | `src/lib/auth-client.ts` |
| API client | `src/lib/api-client.ts` |
| JWT edge decode | `src/lib/jwt-edge.ts` |
| Auth server (JWT) | `src/lib/auth/jwt.ts` |
| Config | `src/config/app.ts` |
| Response helper | `src/lib/response/index.ts` |

## Related Docs

- `docs/ARCHITECTURE.md` — system overview and multi-tenant strategy
- `docs/API_GUIDE.md` — all API endpoints
- `docs/FRONTEND_GUIDE.md` — pages, components, middleware
- `docs/SAAS_TASKS.md` — transformation backlog
- `PROJECT_MAP.md` — detailed file map
