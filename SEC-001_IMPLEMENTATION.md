# SEC-001 — Implementation record

**Branch:** `feature/SEC-001-audit-unauthenticated-endpoints`  
**Related:** `SAAS_TASKS.md` → Phase 0 → SEC-001  

## Summary

Closed unauthenticated or overly permissive API surface called out in the SaaS hardening backlog: inventory total value, diagnostic `Test` endpoints, maintenance visit PDF download, and documentation of remaining `[AllowAnonymous]` usage.

---

## 1. Inventory — `GET /api/Inventory/value`

- **Change:** Added `[Authorize(Policy = "RequireInventory")]` so only **Manager** or **InventoryAdmin** can read aggregate stock value (matches `Program.cs` policy `RequireInventory`).
- **File:** `LiftOps/LiftOps-BackEnd.API/Controllers/InventoryController.cs`
- **Frontend:** Existing `apiClient` usage in `liftops-frontend/lib/api.ts` already sends JWT for `/api/inventory/value` — no UI change required.

---

## 2. Test / health — `TestController`

- **Change:** Applied `[ServiceFilter(typeof(DevelopmentOnlyFilter))]` at controller level. Outside **Development**, actions return **404 Not Found** (no body leak).
- **Files:**  
  - `LiftOps/LiftOps-BackEnd.API/Filters/DevelopmentOnlyFilter.cs`  
  - `LiftOps/LiftOps-BackEnd.API/Controllers/TestController.cs`  
  - Registered in `Program.cs`: `AddScoped<DevelopmentOnlyFilter>()`

---

## 3. Maintenance visit PDF — `GET /api/maintenance/visit/{visitId}/pdf`

- **Removed:** Unconditional anonymous access.
- **Behavior:**
  - **JWT:** User must be **Manager**, **MaintenanceAdmin**, or **Technician** (same operational set as completing a visit). `Authorization: Bearer` is validated by the pipeline; action uses `[AllowAnonymous]` only so authorization is implemented in code for the hybrid rules below.
  - **Share link:** Optional query `?pdfToken=...` — time-limited token bound to `visitId`, produced by Data Protection API (see ADR below).
- **New endpoint:** `POST /api/maintenance/visit/{visitId}/pdf-share-token`  
  - Auth: **Manager**, **MaintenanceAdmin**, or **Technician**  
  - Response: `pdfToken`, `expiresAtUtc`, `downloadPath`, `lifetimeMinutes`
- **Configuration:** `Maintenance:PdfAccessTokenLifetimeMinutes` in `appsettings.json` (default **15**).
- **Files:**  
  - `LiftOps/LiftOps-BackEnd.API/Security/MaintenanceVisitPdfAccessService.cs`  
  - `LiftOps/LiftOps-BackEnd.API/Options/MaintenancePdfOptions.cs`  
  - `LiftOps/LiftOps-BackEnd.API/Controllers/MaintenanceController.cs`  
  - `Program.cs`: `AddDataProtection()`, options + service registration

### ADR: Maintenance PDF access (SEC-001)

| Decision | Choice |
|----------|--------|
| Primary access | JWT Bearer (existing SPA / technician flows). |
| Secondary access | Short-lived `pdfToken` in query string for shareable URLs (email/SMS). Tokens are **not** raw JWTs in the URL; they are **Data Protection–wrapped** payloads `{visitId}|expUnix`. |
| Why not only JWT | Browsers opening a bare `GET` link cannot always attach `Authorization`; share tokens avoid embedding long-lived secrets in URLs. |
| Production note | Persist Data Protection keys (volume/Azure Blob) so tokens survive app restarts; otherwise issued tokens become invalid after recycle. |

---

## 4. Inventory of `[AllowAnonymous]` (post-change)

| Location | Endpoint / usage | Retention reason |
|----------|------------------|------------------|
| `AdminController` | `POST /api/Admin/login` | Public login. |
| `AdminController` | `POST /api/Admin/refresh-token` | Token refresh without prior authenticated session. |
| `MaintenanceController` | `GET .../visit/{visitId}/pdf` | Anonymous **attribute** only; action requires JWT **or** valid `pdfToken` (no unauthenticated download). |

---

## 5. Verification

- `dotnet build LiftOps/LiftOps-BackEnd.sln -c Release` — **succeeded**
- `npm run build` (liftops-frontend) — **succeeded**

---

## 6. Follow-ups (out of SEC-001 scope)

- Persist Data Protection key ring in staging/production.
- Optional: add integration tests for 401/404 behaviors.
- `SEC-002` and later items in `SAAS_TASKS.md` remain open.
