# LiftOps SaaS Transformation Tasks

Production-oriented backlog to evolve the single-tenant LiftOps stack (ASP.NET Core Clean Architecture + MediatR, EF Core + SQL Server, JWT + Identity, Next.js) into a multi-tenant SaaS.

---

## Phase 0: System Hardening

### SEC-001 — Audit and close unauthenticated or overly permissive endpoints
- [x] Inventory: add `[Authorize]` + appropriate policy to `GET /api/Inventory/value` (or equivalent route exposing total inventory value).
- [x] Test/health: restrict `TestController` in non-development environments (remove public access or gate behind API key / internal network).
- [x] Maintenance PDF: replace `[AllowAnonymous]` on visit PDF download with signed time-limited URLs or token query param validated per tenant (design choice documented in ADR).
- [x] Document every `[AllowAnonymous]` and justify retention.

### SEC-002 — Tighten Emergency module authorization
- [x] Replace class-level `[Authorize]` only on `EmergencyController` with explicit policies aligned to product rules (e.g. roles allowed to create vs resolve).
- [x] Add integration tests for forbidden cross-role access.

### SEC-003 — Remove unsafe identity fallbacks
- [x] In installation project creation (and similar), remove `Guid.NewGuid()` fallback when installation admin user id is missing from JWT; return 401/403 instead.
- [x] Add unit tests for handler behavior when claims are absent.

### SEC-004 — CORS and forwarded headers
- [x] Replace `SetIsOriginAllowed(_ => true)` with configurable allowed origins from `appsettings` / env for staging and production.
- [x] Configure `ForwardedHeaders` if behind reverse proxy for correct scheme/host.

### SEC-005 — Input validation pass
- [x] Ensure all write endpoints use FluentValidation or equivalent; align DTOs with max lengths matching DB columns.
- [x] Add missing validation on MediatR commands that currently trust raw DTOs.

### SEC-006 — Secrets and configuration
- [x] Verify JWT signing keys, connection strings, and third-party keys are only from environment / Key Vault — no secrets in repo.
- [x] Add checklist item to deployment runbook.

### OBS-001 — Dashboard metrics accuracy
- [x] Fix `OpenEmergencies` (or rename) so KPI reflects intended entity (`EmergencyTicket` vs `FaultTicket`) per product spec.
- [x] Remove or feature-flag hardcoded revenue/expenses in dashboard handler; return real aggregates or explicit “mock” flag in API for UI.

---

## Phase 1: Multi-Tenancy Implementation

### MT-001 — Domain model: `Company` (tenant)
- [ ] Add `Company` entity: `Id`, `Name`, `Slug` (optional), `IsActive`, `CreatedAt`, audit fields.
- [ ] Add optional fields for billing contact email, timezone (for later subscription UI).

### MT-002 — Wire `CompanyId` on business tables
- [ ] Add nullable-then-backfill `CompanyId` (`uniqueidentifier`) to: `Customers`, `InstallationProjects`, `Elevators`, `InstallationStages`, `StageRequiredParts`, `StageTechnicians`, `TechnicianAssignments`, `InspectionRequests`, `Offers`, `Quotations`, `QuotationAttachments`, `Notifications`, `Categories`, `InventoryItems`, `Technicians`, `MaintenanceContracts`, `MaintenanceElevators`, `MaintenanceVisits`, `MaintenanceSparePartUsages`, `MaintenanceChecklistItems`, `MaintenanceVisitChecklistItems`, `FaultTickets`, `FaultSparePartUsages`, `EmergencyTickets`.
- [ ] Confirm list against current `ApplicationDbContext` — add any new tables introduced since doc freeze.

### MT-003 — User–tenant relationship
- [ ] Add `CompanyId` to `AppUser` **or** introduce `UserCompany` join table if multi-company users are in scope for MVP (pick one; default MVP: single `CompanyId` on user).

### MT-004 — EF Core configuration
- [ ] Configure relationships and required `CompanyId` where appropriate after backfill.
- [ ] Add shadow property or explicit property consistency — no orphan rows.

### MT-005 — Migration strategy for existing data
- [ ] Script: insert default `Company` row (“Legacy” / “Default”).
- [ ] Backfill all existing rows with `CompanyId = default company`.
- [ ] Alter columns to `NOT NULL` after backfill.
- [ ] Dry-run on copy of production DB; measure downtime window.

### MT-006 — `ICurrentTenantService`
- [ ] Interface: `Guid? CompanyId { get; }`, `bool IsResolved { get; }`, `string? Slug { get; }` (if slug routing).
- [ ] Implementation reads from `HttpContext.User` claims after authentication.

### MT-007 — JWT claims
- [ ] On login/refresh, emit `company_id` (and `company_slug` if used) as short-lived claims.
- [ ] Update token validation to require `company_id` for all tenant-scoped endpoints.

### MT-008 — Global query filters
- [ ] In `OnModelCreating`, apply `HasQueryFilter` for each tenant entity: `e => e.CompanyId == _currentTenant.CompanyId`.
- [ ] Provide bypass mechanism for system jobs only (e.g. `IDbContextFactory` with explicit tenant id for worker).

### MT-009 — Repository and handler refactor
- [ ] Audit all `DbSet<T>.Where(...)` / raw SQL — ensure no unfiltered `Set<T>()` in handlers.
- [ ] Replace any `ListAllAsync()` without tenant predicate.
- [ ] Add code analyzer rule or PR checklist: new entities must register filter + `CompanyId`.

### MT-010 — SaveChanges tenant stamp
- [ ] Override `SaveChanges` / `SaveChangesAsync` to assign `CompanyId` on insert from `ICurrentTenantService` when null (defense in depth).

### MT-011 — Files, PDFs, reports
- [ ] Change storage paths from `{guid}/...` to `{companyId}/{...}/...` under `wwwroot` or blob container prefix.
- [ ] Migration script: move existing files to default company prefix or lazy-migrate on first access.

### MT-012 — Background jobs (if any)
- [ ] Pass explicit `CompanyId` into queued work items; no reliance on ambient HTTP context in workers.

---

## Phase 2: Authentication & Authorization Update

### AUTH-001 — Identity model
- [ ] Extend registration (admin-only or public signup — product decision) to create or join a `Company`.
- [ ] Enforce: first user of org = Owner/Manager; document role matrix per tenant.

### AUTH-002 — Login / refresh
- [ ] Include `company_id` in access token; refresh token rotation re-issues same claims.
- [ ] Handle user with no company (invite pending) — return 403 with structured error code.

### AUTH-003 — Policies
- [ ] Update authorization policies to combine role + tenant context (e.g. `RequireManager` AND same company as resource).
- [ ] Add `RequireFinance` usage or remove dead policy from `Program.cs` if unused.

### AUTH-004 — Resource-based checks
- [ ] For `GET/PUT` by id, verify entity’s `CompanyId` matches JWT `company_id` before returning 200 (prevent ID enumeration across tenants).

---

## Phase 3: Database Refactoring

### DB-001 — Unique constraints
- [ ] Drop global unique on `InstallationProjects.ProjectNumber`; add unique index on `(CompanyId, ProjectNumber)`.
- [ ] Repeat for other natural keys (ticket numbers, inventory item numbers if globally unique today).

### DB-002 — Indexes
- [ ] Add composite indexes: `(CompanyId, CreatedAt)`, `(CompanyId, Status)` on high-traffic tables per query plan review.

### DB-003 — Foreign keys across tenant boundary
- [ ] Verify no cross-tenant FK possibility (e.g. technician assigned to elevator in another company) — enforce in application layer + DB check constraints if needed.

### DB-004 — Connection resiliency
- [ ] Enable retry on transient failures for SQL Server in EF Core for SaaS traffic patterns.

---

## Phase 4: Subscription System (MVP)

### SUB-001 — Entities
- [ ] `Subscription`: `Id`, `CompanyId`, `PlanId`, `Status` (Trial, Active, PastDue, Cancelled), `CurrentPeriodEnd`, `ExternalCustomerId` (nullable), audit.
- [ ] `Plan`: `Id`, `Code` (basic, pro), `Name`, `MonthlyPrice`, feature flags JSON or columns.

### SUB-002 — Business rules
- [ ] On create company: create `Subscription` with `Trial` and `CurrentPeriodEnd = UtcNow + trial days`.
- [ ] Job or synchronous check: `PastDue` after failed payment webhook (stub webhook for MVP).

### SUB-003 — Enforcement middleware / filter
- [ ] `SubscriptionStatusFilter` or middleware: block mutating verbs when `Cancelled` or `PastDue` (configurable allowlist: GET profile, POST billing portal).
- [ ] Return 402 or 403 with machine-readable code `subscription_inactive`.

### SUB-004 — Admin override
- [ ] Platform super-admin role (not tenant Manager) can extend trial / set status — separate API under `/api/platform/...` with distinct auth.

---

## Phase 5: API Refactoring

### API-001 — Tenant audit sweep
- [ ] Per controller, verify MediatR pipeline receives tenant context; add integration tests that two tenants cannot read each other’s IDs.

### API-002 — Response consistency
- [ ] Standardize error envelope: `{ code, message, details? }` for 4xx/5xx from API layer.
- [ ] Ensure no stack traces in production responses.

### API-003 — Pagination and sorting
- [ ] All list endpoints: require explicit `page`, `pageSize`, sort; default sort includes `CompanyId` in index-friendly order.

### API-004 — Rate limiting
- [ ] Per-tenant rate limits on auth and expensive endpoints (optional MVP: per-IP if easier).

---

## Phase 6: Frontend Updates

### FE-001 — Company context
- [ ] Parse `company_id` from JWT payload (or session) after login; store in secure memory / context provider — avoid duplicating sensitive data in `localStorage` beyond token if possible.

### FE-002 — API client
- [ ] Ensure `Authorization` header on all calls; centralize 401/403 handling — redirect to login or “subscription expired” screen based on error code.

### FE-003 — Subscription UI (MVP)
- [ ] Banner or settings row: plan name, renewal date, status; warning when trial < 7 days.
- [ ] Link placeholder to billing portal (even if external URL stub).

### FE-004 — Tenant settings page
- [ ] Form: company display name, timezone; `PUT /api/company` (new endpoint).

### FE-005 — Faults module (if product requires)
- [ ] Add Faults screens + `lib/api.ts` helpers to match backend (currently missing in frontend per architecture review).

---

## Phase 7: Deployment & DevOps

### OPS-001 — Environments
- [ ] Define `Development`, `Staging`, `Production` with separate DBs and JWT keys.

### OPS-002 — Configuration
- [ ] Document all env vars: `ConnectionStrings__DefaultConnection`, `Jwt__*`, `Cors__Origins`, blob storage keys, payment provider keys.

### OPS-003 — CI/CD
- [ ] Pipeline: build + test backend; build Next.js; run EF migrations on deploy (or manual gated step for prod).
- [ ] Artifact: Docker images or Azure Web App zip — team choice documented.

### OPS-004 — Database migrations
- [ ] Migration order documented: Phase 1 migrations before enabling tenant middleware in production (feature flag).

### OPS-005 — Observability
- [ ] Structured logging with `CompanyId` on each log line (where resolved).
- [ ] Health checks: DB + disk; readiness vs liveness for orchestrator.

---

## Phase 8: Go-To-Market Preparation

### GTM-001 — Demo tenant
- [ ] Seed script: demo company, sample customers/projects, read-only flag optional.

### GTM-002 — Trial flow
- [ ] Public signup page → creates Company + Owner user + Trial subscription — or invite-only MVP.

### GTM-003 — Pricing
- [ ] Internal doc: plan tiers, elevator/project limits, support SLAs (even if not enforced in code in MVP).

### GTM-004 — Legal
- [ ] Terms of service and privacy policy links in signup; cookie banner if EU traffic.

---

## Execution Timeline (2–4 weeks plan)

### Week 1 — Critical path: security + tenant foundation
| Priority | Work |
|----------|------|
| P0 | Phase 0: SEC-001–SEC-004, OBS-001 (stop data leaks and misleading metrics). |
| P0 | Phase 1: MT-001–MT-005, MT-006–MT-008, MT-010 (Company entity, migrations, backfill, filters, SaveChanges stamp). |
| P1 | Phase 2: AUTH-001–AUTH-002 (JWT `company_id`, user linkage). |

**Exit criteria:** Default company in DB; all existing rows scoped; new requests cannot insert without tenant; filters active in dev/staging.

---

### Week 2 — Enforcement + API + DB constraints
| Priority | Work |
|----------|------|
| P0 | Phase 1: MT-009, MT-011 (handlers + file paths for new uploads). |
| P0 | Phase 2: AUTH-003–AUTH-004; Phase 3: DB-001–DB-003. |
| P1 | Phase 5: API-001–API-002. |

**Exit criteria:** Composite unique indexes; resource-by-id checks; integration tests for two-tenant isolation.

---

### Week 3 — Subscription MVP + frontend
| Priority | Work |
|----------|------|
| P0 | Phase 4: SUB-001–SUB-003; platform admin override stub if needed. |
| P0 | Phase 6: FE-001–FE-003. |
| P1 | Phase 4: SUB-004; Phase 6: FE-004. |

**Exit criteria:** Trial expiry blocks writes; UI shows status; happy path signup → trial → blocked after expiry (test env).

---

### Week 4 — Hardening, DevOps, GTM
| Priority | Work |
|----------|------|
| P0 | Phase 7: OPS-001–OPS-004; production config review. |
| P1 | Phase 5: API-003–API-004; Phase 0 remaining validation (SEC-005–SEC-006). |
| P1 | Phase 8: GTM-001–GTM-002; demo seed. |

**Exit criteria:** Staging deploy with migrations; smoke tests; demo tenant ready for sales.

---

### Priorities summary
- **P0 (blockers):** Tenant isolation correct; no cross-tenant reads; auth gaps closed; subscription gate for write API.
- **P1 (near-term):** File migration, rate limits, full FE settings, GTM assets.
- **Defer if schedule slips:** Public multi-company per user (AUTH-001 join table), advanced billing webhooks, Faults UI (FE-005).

### Critical path
`Company` + migrations + global filters → JWT claims → handler/resource checks → unique indexes → subscription middleware → frontend token handling → deploy with feature flag.

---

*Tasks are intentionally implementation-shaped; adjust IDs and scope to your issue tracker (Jira/Azure DevOps).*
