# Backend Guide

This guide matches the **current** `LiftOps/` solution layout (not `Src/`).

## Stack and versions

- **.NET** 10 (`net10.0`)
- **ASP.NET Core** Web API, JWT Bearer authentication
- **EF Core** 10 + SQL Server
- **MediatR** 14
- **FluentValidation** 12
- **AutoMapper** 12 (Infrastructure/Application as configured)
- **ASP.NET Core Identity** with `UserManager` / `RoleManager` — passwords are hashed with **Identity’s default hasher** (PBKDF2), not BCrypt

## Solution layout

| Project | Responsibility |
|---------|----------------|
| `LiftOps-BackEnd.API` | Controllers, `Program.cs`, middleware, authorization policies |
| `LiftOps-BackEnd.Application` | Features (e.g. `Features/Admins`, `Features/PlatformAdmin`), MediatR handlers, DTOs, validators, `IApplicationDbContext` |
| `LiftOps-BackEnd.Domain` | Entities (`AppUser`, `Company`, modules), `Roles`, enums |
| `LiftOps-BackEnd.Infrastructure` | `ApplicationDbContext`, migrations, `TokenService`, `CurrentTenantService`, repositories, `PlatformAdminSeeder`, `SqlServerDatabaseEnsurer`, `EfMigrationHistoryBaseline` |

## User model

- **`AppUser`** (`Domain/Entities/AppUser.cs`): extends `IdentityUser<Guid>`.
- Fields include **`CompanyId`** as **`Guid?`**. **Platform administrators** use **`null`**. Tenant users have a real company id.
- Other profile fields: `FullName`, `IsDisabled`, `LastLogin`, refresh token fields, audit timestamps.

## Authentication flow (implemented)

### Login (same behavior on both routes)

1. **HTTP** `POST /api/auth/login` — `AuthController` (`Controllers/AuthController.cs`).
2. **HTTP** `POST /api/Admin/login` — `AdminController` (legacy-friendly path).

Both send **`LoginAdminCommand`** via MediatR.

### Handler logic (`Features/Admins/Commands/LoginAdmin/LoginAdminCommand.cs`)

1. `UserManager.FindByEmailAsync`.
2. Reject if user missing, disabled, or password fails `CheckPasswordAsync`.
3. Load roles with `GetRolesAsync`.
4. If the user is **not** `PlatformAdmin` and `CompanyId` is null or `Guid.Empty`, return error code **`company_membership_required`** (403 from controller).
5. Otherwise `ITokenService.CreateToken(user, roles)`, issue refresh token, update user, return **`AuthResponseDto`**.

### Refresh token

- **`POST /api/Admin/refresh-token`** — `RefreshTokenCommand`.
- Same **company** rule as login: non–platform users must have a resolved company id.

### JWT generation (`Infrastructure/Services/TokenService.cs`)

- Signing: **HMAC-SHA512** with symmetric key from configuration **`Jwt:Key`**.
- Claims include:
  - **`sub`** — user id (JWT registered name)
  - **`ClaimTypes.NameIdentifier`** — user id
  - **Email**, **name**
  - **`ClaimTypes.Role`** — one claim per role
  - **`company_id`** — only if user is **not** `PlatformAdmin` and `CompanyId` is present and not empty
- Issuer / audience / lifetime from **`Jwt:Issuer`**, **`Jwt:Audience`**, **`Jwt:DurationInMinutes`**.

### Configuration

- **`appsettings.json`**: JWT key placeholder `REPLACE_WITH_ENV_JWT_KEY` is **rejected** at startup unless overridden.
- **`appsettings.Development.json`** is **gitignored**; local dev should set a real **`Jwt:Key`** there or via **`Jwt__Key`** environment variable.
- **CORS**: `Cors:AllowedOrigins` (or related config in `Program.cs`) for frontend origins.

## DTOs and commands (login)

| Name | Location | Purpose |
|------|-----------|---------|
| `LoginDto` | `Application/Features/Admins/DTOs/AdminDtos.cs` | Request body: **Email**, **Password** (JSON camelCase: `email`, `password`) |
| `LoginAdminCommand` | `Features/Admins/Commands/LoginAdmin/` | MediatR command wrapping `LoginDto` |
| `AuthResponseDto` | `AdminDtos.cs` | **Token**, **RefreshToken**, **RefreshTokenExpiry**, name, email, **Roles** |
| `AuthCommandResult` | `AdminDtos.cs` | Success with `Auth` or failure with **ErrorCode** / **ErrorMessage** |
| `RefreshTokenCommand` | `Features/Admins/Commands/RefreshToken/` | Refresh flow |

Validation: **`LoginDtoValidator`** (FluentValidation) for login DTO.

## Authorization policies (`Program.cs`)

- Tenant-scoped policies (**Manager**, **Installation**, **Maintenance**, etc.) require both **role** and **`company_id`** claim.
- **`RequirePlatformAdmin`**: role **PlatformAdmin** only (no tenant claim required).
- JWT **`OnTokenValidated`**: non-platform authorized endpoints require **`company_id`** (see architecture doc).

## Platform API surface

Controllers under **`Controllers/Platform/`**, route prefix **`/api/platform`** (ASP.NET conventional routing). All use **`RequirePlatformAdmin`**. Includes companies, plans, subscriptions, users, dashboard, impersonation.

## Database seeding — platform super admin

- **`PlatformAdminSeeder`** (`Infrastructure/Persistence/PlatformAdminSeeder.cs`) runs **after migrations** in **`Program.cs`** (when not using in-memory database).
- Controlled by configuration:
  - **`PlatformAdmin:SeedOnStartup`** (boolean)
  - **`PlatformAdmin:Email`**, **`PlatformAdmin:Password`**, optional **`PlatformAdmin:FullName`**
- Behavior: ensures **`PlatformAdmin`** role exists; if user by email exists, adds role if missing; otherwise creates user with **`CompanyId = null`** and **`PlatformAdmin`** role.
- **Production** `appsettings.json` ships **`SeedOnStartup: false`** and empty password; **Development** values live in the **ignored** `appsettings.Development.json` — do not commit secrets.

There is **no** broad business data seeder in startup (inventory/demo data) in the current flow; only optional platform admin seeding.

## Rate limiting

- Global fixed-window limiter in **`Program.cs`** partitions by scope (`auth`, `expensive`, `general`) and by **`company_id`** claim or client IP.
- **`auth` scope** (stricter limits) is applied only to paths starting **`/api/Admin/login`** and **`/api/Admin/refresh-token`**. **`POST /api/auth/login`** currently uses the **general** partition unless you extend the `isAuthEndpoint` check.

## Operational extras

- **`SqlServerDatabaseEnsurer`**: creates the SQL database on the server if missing (connects to `master`) before migrate/baseline.
- **`EfMigrationHistoryBaseline`**: optional dev aid when `__EFMigrationsHistory` is out of sync (see `Database:AutoBaselineMigrationHistory`).

## Development checklist (unchanged intent)

- New tenant-scoped entities: inherit **`BaseAuditableEntity`**, get **`CompanyId`** + query filter.
- Platform-wide queries: **`IgnoreQueryFilters()`** and explicit safety checks.
- Prefer **MediatR** over controller logic; validate commands.

## Related docs

- `docs/ARCHITECTURE.md` — multi-tenant and JWT claim rules.
- `docs/SAAS_TASKS.md` — backlog status.
- `docs/AI_CONTEXT.md` — quick reference for tools and naming.
