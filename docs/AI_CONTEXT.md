# AI Context — LiftOps (Master Context)

Paste or attach this file at the start of a new chat when you want the assistant to behave like a **senior architect** on this repo without re-discovering layout and conventions.

---

## Project identity

- **Name:** LiftOps  
- **Core value:** Multi-tenant operations software for elevator businesses—installation pipeline, maintenance contracts and visits, inventory, emergency/fault workflows, dashboards—plus a **platform operator console** to manage **companies**, **plans**, and **subscriptions**.

---

## Repository layout (non-negotiable facts)

- **Backend solution:** `LiftOps/LiftOps-BackEnd.API` (startup), `LiftOps-BackEnd.Application`, `LiftOps-BackEnd.Domain`, `LiftOps-BackEnd.Infrastructure`. There is **no** `Src/` prefix in this repo.
- **Frontend:** `liftops-frontend/` — Next.js **App Router** only (`app/`).
- **Docs:** `docs/` — architecture, backend/frontend guides, SaaS task backlog, **this file**.

---

## Tech stack and versions (as implemented)

**Backend**

- .NET **10** (`net10.0`)
- ASP.NET Core Web API, **JWT Bearer** (symmetric key, **HMAC-SHA512** in `TokenService`)
- **EF Core 10** + SQL Server
- **ASP.NET Core Identity** — password hashing is Identity’s default (**PBKDF2**), not BCrypt unless you add it
- **MediatR** 14 — commands/queries and handlers in Application
- **FluentValidation** 12
- **AutoMapper** 12 (where wired)

**Frontend**

- **Next.js** 16.x, **React** 19.x, **TypeScript** 5.x
- **Tailwind CSS** 4.x
- **ShadCN-style** UI (`components/ui/`, Radix primitives)
- **react-hook-form** + **zod**
- **TanStack Table** (admin lists)

---

## Architectural rules

1. **Clean Architecture:** API → Application (MediatR) → Domain; Infrastructure implements interfaces and hosts EF + external services. Controllers stay thin.
2. **MediatR** is the default handoff from HTTP to business logic; new features add **Command/Query + Handler + Validator** in Application.
3. **Multi-tenancy** is enforced by:
   - **`company_id` JWT claim** for tenant users (not for `PlatformAdmin`).
   - **Global EF query filters** on `BaseAuditableEntity` scoped by current tenant / bypass flag.
   - **`JwtBearerEvents.OnTokenValidated`**: authorized requests **must** carry **`company_id`** except routes under **`/api/platform`**.
4. **Platform APIs** live under **`/api/platform/*`**, policy **`RequirePlatformAdmin`** (role only).
5. **Super admin** = user in role **`PlatformAdmin`** with **`AppUser.CompanyId == null`**; JWT **does not** emit **`company_id`** for that role.

---

## Naming conventions

**C#**

- Types, methods, properties: **PascalCase**
- Private fields: **_camelCase**
- Interfaces: **I** prefix
- DTOs: **Dto** suffix where used (`AuthResponseDto`, `LoginDto`)
- JSON over the wire: **camelCase** (configured on controllers)

**TypeScript / React**

- Components and files: **PascalCase** for components; route files follow App Router conventions (`page.tsx`, `layout.tsx`)
- Functions, variables, hooks: **camelCase**
- Constants: **UPPER_SNAKE_CASE** for env-driven or fixed constants

---

## Critical context (bugs avoided if you remember this)

1. **`AppUser.CompanyId`** is **`Guid?`**. Platform admins use **`null`**. Tenant users without a company get **`company_membership_required`** on login/refresh.
2. **Two login URLs** on the API behave the same: **`POST /api/auth/login`** (`AuthController`) and **`POST /api/Admin/login`** (`AdminController`); both dispatch **`LoginAdminCommand`** with **`LoginDto` { email, password }.
3. **`appsettings.Development.json`** is **gitignored** — real **`Jwt:Key`** and **`PlatformAdmin:*`** seed values are local-only; production **`appsettings.json`** uses placeholders and **`SeedOnStartup: false`**.
4. **Frontend auth:** **`lib/auth.ts`** `login()` hits **Next** **`POST /api/auth/login`** (BFF), which sets **httpOnly** cookies **`liftops_access`** / **`liftops_refresh`**; client still stores tokens in **`localStorage`** for **`apiClient`**.
5. **`middleware.ts`** protects **`/admin/*`** (not login): decodes JWT from cookie, requires **`PlatformAdmin`**. **`NEXT_PUBLIC_DEV_BYPASS_ADMIN_AUTH=true`** relaxes middleware; **`AuthGuard`** may still apply.
6. **Rate limiting:** stricter **`auth`** partition applies to **`/api/Admin/login`** and **`/api/Admin/refresh-token`** only; **`/api/auth/login`** is currently under the general bucket unless code is updated.
7. **EF migrations:** run from Infrastructure with API as startup, e.g.  
   `dotnet ef migrations add Name --project LiftOps-BackEnd.Infrastructure --startup-project LiftOps-BackEnd.API`

---

## Key files (quick navigation)

| Area | Path |
|------|------|
| JWT + policies + tenant validation | `LiftOps/LiftOps-BackEnd.API/Program.cs` |
| Tenant login + refresh | `Controllers/AuthController.cs`, `Controllers/AdminController.cs` |
| Login command | `LiftOps-BackEnd.Application/Features/Admins/Commands/LoginAdmin/` |
| DTOs | `Features/Admins/DTOs/AdminDtos.cs` |
| Tokens | `LiftOps-BackEnd.Infrastructure/Services/TokenService.cs` |
| Tenant from claims | `CurrentTenantService` (Infrastructure) |
| DbContext + filters | `ApplicationDbContext` (Infrastructure) |
| Platform admin seed | `PlatformAdminSeeder.cs` |
| Next auth BFF | `liftops-frontend/app/api/auth/login/route.ts`, `logout/route.ts` |
| Edge guard | `liftops-frontend/middleware.ts`, `lib/jwt-edge.ts` |
| Client auth | `liftops-frontend/lib/auth.ts`, `components/auth-guard.tsx` |
| Platform API client | `liftops-frontend/lib/api-platform.ts` |

---

## Prompting guide (how to get good answers)

1. **Name the layer:** “change the **Application** handler” vs “add an **API** endpoint” avoids wrong-file edits.
2. **State tenant vs platform:** “tenant user” vs “**PlatformAdmin** on **`/api/platform`**” — rules differ (`company_id`, filters, policies).
3. **Paste errors and paths:** build output, HTTP status, and file path shorten debug loops.
4. **Ask for doc updates** when behavior changes: **`docs/ARCHITECTURE.md`**, **`BACKEND_GUIDE.md`**, **`FRONTEND_GUIDE.md`**, **`SAAS_TASKS.md`**, and this **`AI_CONTEXT.md`** should stay aligned with code.
5. **Do not assume BCrypt** for passwords unless you see it in code; default is **ASP.NET Identity** hashing.

---

## Related human docs

- `docs/ARCHITECTURE.md` — diagrams and multi-tenant strategy in depth  
- `docs/BACKEND_GUIDE.md` — auth flow, DTOs, seeding  
- `docs/FRONTEND_GUIDE.md` — App Router, middleware, BFF  
- `docs/SAAS_TASKS.md` — backlog and **audit status** block  

---

*Last intended use: attach at session start; refresh this file when you ship major auth or tenancy changes.*
