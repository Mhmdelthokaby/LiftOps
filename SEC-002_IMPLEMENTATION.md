# SEC-002 — Implementation record

**Branch:** `feature/SEC-002-emergency-authorization`  
**Related:** `SAAS_TASKS.md` → Phase 0 → SEC-002  

## Summary

Replaced the blanket `[Authorize]` on `EmergencyController` with **named authorization policies** so create, read, dispatch (assign), resolve, and admin-style update/delete are each limited to appropriate roles. Added **integration tests** that assert **403 Forbidden** for disallowed roles using the real JWT pipeline.

---

## Policies (registered in `Program.cs`)

| Policy | Roles | Endpoints |
|--------|--------|-----------|
| `EmergencyReport` | Manager, MaintenanceAdmin, InstallationAdmin, FaultsAdmin, Technician | `POST /api/Emergency` |
| `EmergencyRead` | Same as report | `GET /api/Emergency`, `GET /api/Emergency/{id}`, `GET /api/Emergency/open` |
| `EmergencyDispatch` | Manager, MaintenanceAdmin, FaultsAdmin | `PUT .../assign-technician` |
| `EmergencyResolve` | Manager, MaintenanceAdmin, FaultsAdmin, Technician | `POST .../resolve` |
| `EmergencyManage` | Manager, MaintenanceAdmin | `PUT /api/Emergency/{id}`, `DELETE /api/Emergency/{id}` |

**Rationale:** Aligns with operational split (field report/resolve vs dispatch vs data correction), and with the Faults module pattern (Manager / MaintenanceAdmin / FaultsAdmin for dispatch-style actions). InstallationAdmin can report and read but cannot assign technicians or change records created by admins without Manager/MaintenanceAdmin.

---

## Integration tests

- **Project:** `LiftOps/LiftOps-BackEnd.API.Tests`
- **Factory:** `EmergencyWebApplicationFactory` injects `UseInMemoryDatabase=true` and JWT settings via `ConfigureAppConfiguration` so the API host matches production auth.
- **Tokens:** `IntegrationTestJwt.CreateAccessToken(role)` signs JWTs with the same issuer/audience/key as configured for tests (must match `Jwt:*` in host configuration).
- **Cases:** Inventory/Finance denied on read; Technician denied on delete/update/assign; InstallationAdmin denied on assign; Finance denied on resolve; MaintenanceAdmin/Technician allowed where policy permits.

**Run:** `dotnet test LiftOps/LiftOps-BackEnd.API.Tests/LiftOps-BackEnd.API.Tests.csproj`

---

## Infrastructure / host changes

- **`UseInMemoryDatabase`** (config): When `true`, `DependencyInjection` registers EF InMemory; **does not** register `FreeMaintenanceProcessorService` (avoids background work against test DB).
- **`Program.cs`:** Database **migrate + seed** runs only when `UseInMemoryDatabase` is **false** (normal SQL Server runs).
- **`appsettings.Testing.json`:** Documents `UseInMemoryDatabase` + JWT for optional `dotnet run --environment Testing`.

---

## Follow-ups

- Optional: add tests for **Manager** and **FaultsAdmin** on each policy.
- If product needs InstallationAdmin to assign emergencies, extend `EmergencyDispatch` and adjust tests.
