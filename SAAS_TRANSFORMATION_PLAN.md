# SaaS Transformation Plan — LiftOps Elevator Platform

**Document purpose:** Technical reference extracted from the repository (`LiftOps` ASP.NET Core backend, `liftops-frontend` Next.js app) to support evolving the product into multi-tenant SaaS.  
**Stack (from code):** ASP.NET Core, Entity Framework Core, SQL Server, JWT + ASP.NET Identity, MediatR, Next.js (App Router), REST.

---

## 1. Project Overview

### 1.1 What the system does

The application supports **elevator installation project management**, **post-installation maintenance contracts and visits**, **fault (breakdown) tickets**, **emergency tickets**, **inventory / spare parts**, **technicians**, and **admin user management** with role-based access. Data is persisted in SQL Server via EF Core; the SPA calls JSON REST endpoints with JWT authentication.

### 1.2 Main modules (from `ApplicationDbContext` and controllers)

| Module | Scope (high level) |
|--------|---------------------|
| **Identity / Admin** | `AppUser`, roles, login, refresh token, register/list/update/disable admins, assign roles |
| **Customers (Clients)** | Customer records linked to installation; status updates |
| **Installation** | Projects, elevators, stages, parts, technician assignment to elevators, notifications, inspection requests, offers, quotations, conversion to projects |
| **Maintenance** | Contracts, maintenance elevators, scheduled visits, checklist templates, spare usage on visits, PDF reports, contract/elevator freeze/stop/activate, statistics |
| **Faults** | Fault tickets: create, assign technician, resolve (separate from `EmergencyTicket`) |
| **Emergency** | Emergency tickets: CRUD, assign, resolve, list open |
| **Inventory** | Categories, inventory items, stock, total value endpoint |
| **Dashboard** | Aggregated KPIs and activity feed |
| **Technician portal API** | Today’s visits, visit detail/complete/status, checklist items, assigned emergency tickets |

**Core entities (business):** `Customer`, `InstallationProject`, `Elevator`, `InstallationStage`, `MaintenanceContract`, `MaintenanceElevator`, `MaintenanceVisit`, `FaultTicket`, `EmergencyTicket`, `Technician`, `InventoryItem`, `Category`.

---

## 2. Backend Structure

**Base route pattern:** `[Route("api/[controller]")]` — controller names resolve to paths such as `api/Admin`, `api/Installation`, `api/maintenance` (ASP.NET Core routing is case-insensitive by default).

**Authorization summary:**

- JWT Bearer (`Program.cs`).
- Policies: `RequireManager`, `RequireInstallation`, `RequireMaintenance`, `RequireInventory`, `RequireFinance`, `RequireFaults`.
- Several endpoints use `[AllowAnonymous]` or have **no** `[Authorize]` (called out in §6).

**Request/response:** Unless noted, bodies are JSON DTOs from `LiftOps-BackEnd.Application` (e.g. `CreateProjectDto`, `CreateFaultTicketDto`). Exact field lists are **not** duplicated here; see the corresponding `*Dto` types and command/query handlers in the Application project.

---

### 2.1 Admin (`AdminController` → `api/Admin`)

| Method | Endpoint | Auth | Description | Response shape (from code) |
|--------|----------|------|-------------|----------------------------|
| POST | `/api/Admin/login` | Anonymous | Admin login | `{ token, refreshToken, refreshTokenExpiry, name, email, roles }` or 401 |
| POST | `/api/Admin/refresh-token` | Anonymous | Refresh JWT | Handler result or 401 |
| POST | `/api/Admin/register` | Manager | Register admin | `{ message }` or errors |
| GET | `/api/Admin/list` | Manager | List admins | List from `ListAdminsQuery` |
| PUT | `/api/Admin/update/{id}` | Authenticated (manager or self) | Update admin | `{ message }` |
| PUT | `/api/Admin/disable/{id}` | Manager | Enable/disable admin | `{ message }` |
| PUT | `/api/Admin/roles/{id}` | Manager | Assign roles | `{ message }` |

---

### 2.2 Customers (`CustomersController` → `api/Customers`)

| Method | Endpoint | Auth | Description |
|--------|----------|------|---------------|
| GET | `/api/Customers` | Manager, InstallationAdmin | List all customers |
| PUT | `/api/Customers/{id}` | Manager, InstallationAdmin | Update customer |
| PUT | `/api/Customers/{id}/status` | Manager, InstallationAdmin | Update `CustomerStatus` (body: enum value) |

---

### 2.3 Installation (`InstallationController` → `api/Installation`)

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/api/Installation/project/add` | RequireInstallation | Create installation project |
| POST | `/api/Installation/project/{projectId}/elevator/add` | RequireInstallation | Add elevator to project |
| PUT | `/api/Installation/elevator/{elevatorId}` | RequireInstallation | Update elevator |
| POST | `/api/Installation/stage/start` | RequireInstallation | Start stage |
| POST | `/api/Installation/stage/complete` | RequireInstallation | Complete stage |
| POST | `/api/Installation/stage/parts` | RequireInstallation | Add required parts to stage |
| PUT | `/api/Installation/stage/update` | RequireInstallation | Update stage |
| GET | `/api/Installation/projects` | RequireInstallation | List projects (`status` query optional) |
| GET | `/api/Installation/project/{id}` | RequireInstallation | Project details |
| POST | `/api/Installation/project/{id}/approve-inspection` | RequireInstallation | Approve inspection |
| POST | `/api/Installation/project/{id}/reject` | RequireInstallation | Reject project |
| PUT | `/api/Installation/project/{id}` | RequireInstallation | Update project |
| GET | `/api/Installation/stage/{id}` | RequireInstallation | Stage details |
| POST | `/api/Installation/projects/fix-numbers` | RequireInstallation | Fix project numbers (maintenance command) |
| GET | `/api/Installation/check-project-number` | RequireInstallation | Check if project number exists |
| GET | `/api/Installation/notifications` | RequireInstallation | Notifications for current user |
| POST | `/api/Installation/elevator/{elevatorId}/assign-technicians` | Manager, InstallationAdmin | Assign technicians to elevator |
| DELETE | `/api/Installation/elevator/{elevatorId}/unassign-technician/{techId}` | Manager, InstallationAdmin | Unassign technician |
| POST | `/api/Installation/inspection/create` | RequireInstallation | Create inspection request |
| PUT | `/api/Installation/inspection/{inspectionId}/technical-data` | RequireInstallation | Update inspection technical data |
| GET | `/api/Installation/inspections` | RequireInstallation | List inspections (`status` optional) |
| GET | `/api/Installation/inspection/{id}` | RequireInstallation | Inspection details |
| POST | `/api/Installation/offer/create` | RequireInstallation | Create offer |
| PUT | `/api/Installation/offer/{offerId}` | RequireInstallation | Update offer |
| PUT | `/api/Installation/offer/{offerId}/pdf` | RequireInstallation | Update offer PDF path |
| PUT | `/api/Installation/offer/{offerId}/approve` | RequireInstallation | Approve offer (manager override considered in command) |
| GET | `/api/Installation/offers` | RequireInstallation | List offers (`status` optional) |
| POST | `/api/Installation/offer/{offerId}/convert-to-project` | RequireInstallation | Convert approved offer to project |
| POST | `/api/Installation/inspection-project/create` | RequireInstallation | Create inspection-project flow |
| GET | `/api/Installation/inspection-projects` | RequireInstallation | List inspection projects (`status` optional) |
| POST | `/api/Installation/quotation/create` | RequireInstallation | Create quotation |
| POST | `/api/Installation/quotation/{quotationId}/approve` | RequireInstallation | Approve quotation |
| POST | `/api/Installation/quotation/{quotationId}/reject` | RequireInstallation | Reject quotation |

---

### 2.4 Maintenance (`MaintenanceController` → `api/Maintenance`)

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/api/Maintenance/add-contract` | Manager, MaintenanceAdmin | Add maintenance contract |
| POST | `/api/Maintenance/visit/schedule` | Manager, MaintenanceAdmin | Schedule visit |
| POST | `/api/Maintenance/visit/{visitId}/complete` | Manager, MaintenanceAdmin, Technician | Complete visit |
| GET | `/api/Maintenance/visit/{visitId}` | Manager, MaintenanceAdmin | Visit details (anonymous-shaped DTO in controller) |
| POST | `/api/Maintenance/checklist-item/add` | Manager, MaintenanceAdmin | Add checklist template |
| PUT | `/api/Maintenance/checklist-item/{id}` | Manager, MaintenanceAdmin | Update checklist template |
| DELETE | `/api/Maintenance/checklist-item/{id}` | Manager, MaintenanceAdmin | Delete checklist template |
| GET | `/api/Maintenance/checklist-item/list` | Manager, MaintenanceAdmin, Technician | List checklist items |
| GET | `/api/Maintenance/projects` | Manager, MaintenanceAdmin | All maintenance contracts |
| GET | `/api/Maintenance/projects/{contractId}` | Manager, MaintenanceAdmin | Contract details |
| GET | `/api/Maintenance/check-project-number` | Manager, MaintenanceAdmin | Check project number |
| POST | `/api/Maintenance/projects/create` | Manager, MaintenanceAdmin | Create maintenance project |
| PUT | `/api/Maintenance/projects/{contractId}` | Manager, MaintenanceAdmin | Update contract |
| PUT | `/api/Maintenance/elevators/{elevatorId}` | Manager, MaintenanceAdmin | Update maintenance elevator |
| POST | `/api/Maintenance/visit/{visitId}/mark-paid` | Manager, MaintenanceAdmin | Mark visit paid |
| GET | `/api/Maintenance/schedule/monthly` | Manager, MaintenanceAdmin | Monthly schedule (`month`, `year`) |
| GET | `/api/Maintenance/elevators` | Manager, MaintenanceAdmin | All maintenance elevators |
| POST | `/api/Maintenance/elevator/{elevatorId}/freeze` | Manager, MaintenanceAdmin | Freeze elevator |
| POST | `/api/Maintenance/elevator/{elevatorId}/stop` | Manager, MaintenanceAdmin | Stop elevator |
| POST | `/api/Maintenance/elevator/{elevatorId}/activate` | Manager, MaintenanceAdmin | Activate elevator |
| POST | `/api/Maintenance/contract/{contractId}/freeze` | Manager, MaintenanceAdmin | Freeze contract |
| POST | `/api/Maintenance/contract/{contractId}/stop` | Manager, MaintenanceAdmin | Stop contract |
| POST | `/api/Maintenance/contract/{contractId}/activate` | Manager, MaintenanceAdmin | Activate contract |
| GET | `/api/Maintenance/contract/{contractId}/visits` | Manager, MaintenanceAdmin | Visits by contract + month/year |
| GET | `/api/Maintenance/elevator/{elevatorId}/visits` | Manager, MaintenanceAdmin | Visits by elevator (optional month/year) |
| GET | `/api/Maintenance/visit/{visitId}/pdf` | **AllowAnonymous** | Download maintenance visit PDF |
| POST | `/api/Maintenance/visit/assign-technician` | Manager, MaintenanceAdmin | Assign technician to visit |
| PUT | `/api/Maintenance/visit/{visitId}/status` | Manager, MaintenanceAdmin | Update visit status |
| POST | `/api/Maintenance/visits/cancel-incomplete-by-date` | Manager, MaintenanceAdmin | Cancel incomplete visits by date |
| POST | `/api/Maintenance/visits/update-order` | Manager, MaintenanceAdmin | Update visit order |
| POST | `/api/Maintenance/contract/assign-technicians` | Manager, MaintenanceAdmin | Bulk assign technicians for contract/date |
| GET | `/api/Maintenance/statistics` | Manager, MaintenanceAdmin | Maintenance statistics (`month`, `year`) |

---

### 2.5 Faults (`FaultsController` → `api/Faults`)

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/api/Faults/create-ticket` | Manager, MaintenanceAdmin, FaultsAdmin | Create fault ticket |
| PUT | `/api/Faults/{ticketId}/assign-technician` | Same | Assign technician |
| POST | `/api/Faults/{ticketId}/resolve` | Same | Resolve ticket |
| GET | `/api/Faults/open` | Same | Open fault tickets |

---

### 2.6 Emergency (`EmergencyController` → `api/Emergency`)

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/api/Emergency` | Authenticated | Create emergency ticket |
| GET | `/api/Emergency` | Authenticated | List all |
| GET | `/api/Emergency/{id}` | Authenticated | Get by id |
| PUT | `/api/Emergency/{id}` | Authenticated | Update |
| DELETE | `/api/Emergency/{id}` | Authenticated | Delete |
| PUT | `/api/Emergency/{id}/assign-technician` | Authenticated | Assign technician |
| POST | `/api/Emergency/{id}/resolve` | Authenticated | Resolve (`Notes` in body) |
| GET | `/api/Emergency/open` | Authenticated | Open tickets |

**Note:** Controller is `[Authorize]` without role restriction — any authenticated user can call these endpoints (see §6).

---

### 2.7 Inventory (`InventoryController` → `api/Inventory`)

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/api/Inventory/add` | Manager, InventoryAdmin | Add item |
| PUT | `/api/Inventory/update/{id}` | Manager, InventoryAdmin | Update item |
| PUT | `/api/Inventory/disable/{id}` | Manager, InventoryAdmin | Enable/disable item |
| GET | `/api/Inventory/all` | Manager, InventoryAdmin | All items |
| GET | `/api/Inventory/active` | Manager, InventoryAdmin, InstallationAdmin, MaintenanceAdmin | Active items |
| GET | `/api/Inventory/value` | **No `[Authorize]` in code** | Total inventory value |

---

### 2.8 Category (`CategoryController` → `api/Category`)

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/api/Category/add` | Manager, InventoryAdmin | Add category |
| GET | `/api/Category/list` | Manager, InventoryAdmin, InstallationAdmin, MaintenanceAdmin | List categories |

---

### 2.9 Technician (`TechnicianController` → `api/Technician`)

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/Technician/visits/today` | Authenticated | Technician’s visits for date |
| GET | `/api/Technician/visits/{visitId}` | Authenticated | Visit details (assigned tech only) |
| PUT | `/api/Technician/visits/{visitId}/status` | Authenticated | Update status (assigned tech only) |
| POST | `/api/Technician/visits/{visitId}/complete` | Authenticated | Complete visit (assigned tech only) |
| GET | `/api/Technician/all` | Manager | All technicians |
| GET | `/api/Technician/available` | Manager, InstallationAdmin | Available technicians |
| POST | `/api/Technician/add` | Manager | Create technician |
| PUT | `/api/Technician/update/{id}` | Manager | Update technician |
| PUT | `/api/Technician/disable/{id}` | Manager | Disable/enable |
| DELETE | `/api/Technician/delete/{id}` | Manager | Delete technician |
| GET | `/api/Technician/checklist-items` | Authenticated (linked technician) | Checklist items |
| GET | `/api/Technician/emergency-tickets` | Authenticated (linked technician) | Emergency tickets for tech |

---

### 2.10 Dashboard (`DashboardController` → `api/Dashboard`)

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/Dashboard/summary` | Authenticated | Dashboard summary (`DashboardSummaryDto`) |

---

### 2.11 Test (`TestController` → `api/Test`)

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/Test` | **None** | Health message + UTC time |
| GET | `/api/Test/ping` | **None** | `pong` |

---

### 2.12 Policies defined but not referenced on controllers (from `Program.cs`)

- `RequireFinance` — **Not Found** applied on any controller in the scanned API project (FinanceAdmin role exists; no finance CRUD API discovered in controllers).

---

## 3. Database Schema

Source: EF Core model snapshot `ApplicationDbContextModelSnapshot.cs` (SQL Server).

### 3.1 Identity tables

| Table | Purpose |
|-------|---------|
| `AspNetUsers` | Extended user (`AppUser`: `FullName`, `IsDisabled`, `RefreshToken`, audit fields, etc.) |
| `AspNetRoles` | Roles (`Guid` PK) |
| `AspNetUserRoles` | User–role M:N |
| `AspNetUserClaims`, `AspNetRoleClaims`, `AspNetUserLogins`, `AspNetUserTokens` | Identity plumbing |

### 3.2 Domain tables — columns and relationships

The following lists **table name**, **key columns** (not every audit field repeated), and **foreign keys** where configured in the snapshot.

| Table | Key columns (types) | Foreign keys / notes |
|-------|------------------------|----------------------|
| **Categories** | `Id` (uniqueidentifier), `Name`, `Description`, audit | — |
| **InventoryItems** | `CategoryId`, `ItemNumber`, `Name`, `StockQuantity`, `UnitPrice` (decimal 18,2), `SupplierName`, `IsDisabled`, `AddedByAdminName` | FK → `Categories` |
| **Customers** | `Name`, `Email`, `Phone`, `Address`, `City`, `ProjectNumber`, `Status` (int), `GoogleMapsLink` | — |
| **InstallationProjects** | `CustomerId`, `InstallationAdminId`, `ProjectNumber` (unique filtered index), `ProjectStatus`, prices, shaft/travel fields, `QuotationId` (shadow/link), `ConvertedFromInspectionId` | FK → `Customers`; optional 1:1 with inspection via `InspectionRequests.ConvertedToProjectId` |
| **Elevators** | `ProjectId`, `ElevatorType`, counts, pit/travel/price decimals, `Notes` | FK → `InstallationProjects` (cascade) |
| **InstallationStages** | `ElevatorId`, `StageNumber`, `Status`, dates, `StagePrice`, `SupplyCost`, `PdfPath`, `StageAdminId` | FK → `Elevators` |
| **StageRequiredParts** | `StageId`, `InventoryItemId`, `Quantity`, `IsOutOfStock` | FK → `InstallationStages`, `InventoryItems` |
| **StageTechnicians** | `StageId`, `TechnicianId`, `Rating` | FK → `InstallationStages`, `Technicians` |
| **Technicians** | `Name`, `Phone`, `LeaderId` (self-FK), `UserId`, ratings, counts, `IsDisabled` | FK `LeaderId` → `Technicians` |
| **TechnicianAssignments** | Composite PK `TechnicianId`, `ElevatorId`; `AssignedBy`, `AssignedAt`, `ExpectedFinishDate`, `Id` | FK → `Technicians`, `Elevators` |
| **Notifications** | `TargetUserId`, `Title`, `Message`, `Type`, `IsRead` | No FK to user in snapshot |
| **InspectionRequests** | Client fields, technical measures, `Status`, `CreatedByAdminId`, `ClientId` (optional), `ConvertedToProjectId` | FK → `Customers` (optional); FK → `InstallationProjects` for conversion |
| **Offers** | `InspectionRequestId` (unique), pricing, dates, `Status`, `OfferPdfPath` | FK → `InspectionRequests` |
| **Quotations** | `ProjectId` (unique), `Price`, `Status`, `DurationDays`, `AttachmentPath` | FK → `InstallationProjects` |
| **QuotationAttachments** | `QuotationId`, `FileName`, `FilePath`, `ContentType`, `FileSize` | FK → `Quotations` |
| **MaintenanceContracts** | `CustomerId`, `TechnicianId` (optional), `ProjectNumber`, `City`, `ProjectAddress`, dates, `PricePerMonth`, `Status`, freeze fields, `IsFromInstallation` | FK → `Customers`, `Technicians` |
| **MaintenanceElevators** | `ContractId`, `InstallationElevatorId` (optional guid), `Type`, floors/stops, `Status`, `NextMaintenanceDate` | FK → `MaintenanceContracts`; **`InstallationElevatorId` — no FK relationship defined in snapshot** |
| **MaintenanceVisits** | `MaintenanceElevatorId`, `TechnicianId`, `VisitDate`, `Status`, `DisplayOrder`, `IsPaid`, notes | FK → `MaintenanceElevators`, `Technicians` |
| **MaintenanceSparePartUsages** | `MaintenanceVisitId`, `InventoryItemId`, `Quantity`, `PriceAtTimeOfUsage`, `IsPaid` | FK → `MaintenanceVisits`, `InventoryItems` |
| **MaintenanceChecklistItems** | `Title`, `Description`, `Order`, `IsActive` | — |
| **MaintenanceVisitChecklistItems** | `VisitId`, `ChecklistItemId`, `IsCompleted`, `Percentage`, `Count`, `Notes`, `Status` | FK → `MaintenanceVisits`, `MaintenanceChecklistItems` |
| **FaultTickets** | `TicketNumber`, customer/phone/address fields, `FaultDescription`, `Severity`, `Status`, `AssignedTechnicianId`, dates | FK → `Technicians` (optional) |
| **FaultSparePartUsages** | `FaultTicketId`, `InventoryItemId`, `Quantity`, `PriceAtTimeOfUsage`, `IsPaid` | FK → `FaultTickets`, `InventoryItems` |
| **EmergencyTickets** | `TicketNumber` (int), `Project`, `UnitId`, `Location`, `Description`, `Status`, `Priority`, `AssignedTechnicianId`, etc. | FK → `Technicians` (optional) |

### 3.3 Core entities (highlight)

- **Tenant boundary today:** **None** — no `CompanyId` / `TenantId` on business tables.
- **Hub entities for SaaS scoping later:** `Customer`, `InstallationProject`, `MaintenanceContract`, `InventoryItem` / `Category`, `Technician`, `FaultTicket`, `EmergencyTicket`, `AppUser`.

---

## 4. Frontend (`liftops-frontend`)

Stack: **Next.js App Router**, client-side role checks from `localStorage` (`lib/user.ts`), API wrapper `lib/api-client.ts` + `lib/api.ts`.

### 4.1 Pages / views

| Route | Purpose | Related API(s) (from `lib/api.ts` and usage) |
|-------|---------|-----------------------------------------------|
| `/` | Dashboard KPIs, charts, activity | `GET /api/dashboard/summary` |
| `/login` | Admin login | `POST /api/Admin/login` (via `lib/auth.ts`) |
| `/clients` | Client list | `GET /api/customers` |
| `/clients/[id]` | Client detail | `GET /api/customers` (client-side) / customer data from list |
| `/clients/[id]/edit` | Edit client | `PUT /api/customers/{id}` |
| `/projects` | Installation projects | `GET /api/installation/projects` |
| `/projects/new` | New project | `POST /api/installation/project/add`, check project number, etc. |
| `/projects/[id]` | Project detail, stages, elevators | `GET /api/installation/project/{id}`, stage/elevator update endpoints |
| `/installation` | Installation pipeline (inspections, offers, quotations) | `GET/POST /api/Installation/inspections`, `offers`, `quotation/*`, `inspection-projects`, etc. |
| `/inspection/new` | New inspection project | `POST /api/Installation/inspection-project/create` |
| `/inventory` | Inventory management | `GET/POST/PUT /api/inventory/*`, `GET /api/category/list` |
| `/technicians` | Technician list / CRUD | `GET/POST/PUT/DELETE /api/Technician/*` |
| `/technicians/new`, `/technicians/[id]/edit` | Create / edit technician | Same as above |
| `/maintenance` | Maintenance hub (tabs: projects, calendar, …) | `GET /api/maintenance/projects`, `schedule/monthly`, statistics, etc. |
| `/maintenance/projects` | Contract list | `GET /api/maintenance/projects` |
| `/maintenance/projects/new` | New maintenance project | `POST /api/maintenance/projects/create`, `check-project-number` |
| `/maintenance/projects/[id]` | Contract / elevators / visits | `GET/PUT /api/maintenance/projects/{id}`, visits, elevators |
| `/maintenance/assign-visits` | Assign technicians to visits | `POST /api/maintenance/contract/assign-technicians`, related maintenance GETs |
| `/maintenance/elevators` | Maintenance elevators view | `GET /api/maintenance/elevators` |
| `/emergency` | Emergency tickets | `GET/POST/PUT/DELETE /api/Emergency`, assign, resolve |
| `/finance` | Finance UI | **Not Found** — page uses **static mock data** only; no finance API calls in this page |
| `/settings` | Settings (categories, admin users) | `GET/POST /api/category/*`, `GET/POST/PUT /api/Admin/*` |
| `/technician/visits` | Technician daily work | `GET /api/technician/visits/today`, `GET/POST/PUT` technician visit endpoints, `GET /api/technician/emergency-tickets`, maintenance complete/status |

### 4.2 Backend endpoints without matching helpers in `lib/api.ts` (scan result)

- **Faults module (`/api/Faults/*`):** **Not Found** in `liftops-frontend/lib/api.ts` — no dedicated fault ticket UI discovered in `app/` routes.
- **Installation elevator assign/unassign:** **Not Found** in `lib/api.ts` (endpoints exist on backend).
- **Some paths** use mixed casing (`/api/Installation/...`) — works on default ASP.NET routing but is inconsistent with lowercase `installation` elsewhere.

### 4.3 مرتبط بأنهي API (ملخص سريع)

- **لوحة التحكم:** `GET /api/dashboard/summary`
- **العملاء:** `GET/PUT /api/customers`, `PUT /api/customers/{id}/status`
- **التركيب والمشاريع:** عائلة ` /api/Installation/*` (مشاريع، مصاعد، مراحل، كشوف، عروض، عروض أسعار)
- **الصيانة:** عائلة `/api/Maintenance/*`
- **الطوارئ:** `/api/Emergency/*`
- **المخزون والفئات:** `/api/Inventory/*`, `/api/Category/*`
- **الفنّيون (إدارة +تطبيق الفني):** `/api/Technician/*`
- **المصادقة والمدراء:** `/api/Admin/*`

---

## 5. Business Logic (workflows from application code)

### 5.1 Fault reporting (`FaultTicket`)

1. Authorized role calls `POST /api/Faults/create-ticket` with customer/contact/location/fault fields.
2. `FaultService.CreateTicketAsync` sets `TicketNumber` (`FLT-{UtcTicks}`), `Status = Pending`, `FaultDate = UtcNow`, persists.
3. `PUT /api/Faults/{id}/assign-technician` sets technician and `Status = InProgress`.
4. `POST /api/Faults/{id}/resolve` completes resolution (notes via `ResolveFaultDto` in handler).
5. Spare parts can be recorded via service layer (`AddSparePartsAsync`) with inventory decrement and optional low-stock notification — **Not Found** exposed as dedicated public API route in `FaultsController` (only create/assign/resolve/open are mapped there).

### 5.2 Maintenance scheduling

1. Contracts created via `POST /api/Maintenance/add-contract` or `POST /api/Maintenance/projects/create` (project-style DTO).
2. Visits scheduled with `POST /api/Maintenance/visit/schedule`.
3. Technicians assigned per visit (`POST .../visit/assign-technician`) or in bulk (`POST .../contract/assign-technicians`).
4. Completion: admin path `POST /api/Maintenance/visit/{id}/complete` or technician path `POST /api/Technician/visits/{id}/complete` with assignment checks.
5. Checklist templates: CRUD on `MaintenanceChecklistItems`; per-visit completion stored in `MaintenanceVisitChecklistItems`.
6. Monthly calendar: `GET /api/Maintenance/schedule/monthly`.
7. PDF: `GET /api/Maintenance/visit/{visitId}/pdf` (**anonymous**).

### 5.3 Elevator lifecycle (installation)

1. **Customer** exists; **installation project** created and linked (`InstallationProject`).
2. **Elevators** added per project; each elevator has ordered **stages** (`InstallationStage`).
3. Stages move through start/complete flows; **parts** linked to inventory; **technicians** can be assigned to stages and/or elevators (`TechnicianAssignment`).
4. Parallel **sales/engineering** paths: **inspection** → **offer** → convert to project; or **inspection-project** → **quotation** → approve/reject.
5. **Notifications** can be raised for installation users (e.g. stock warnings in fault service).

---

## 6. Current Limitations

| Area | Finding (evidence in code) |
|------|----------------------------|
| **Multi-tenancy** | **Single-tenant data model** — one shared database; no tenant/company discriminator on entities. |
| **Dashboard “open emergencies”** | `GetDashboardSummaryQueryHandler` sets `OpenEmergencies` from **`FaultTicket`** counts — **not** `EmergencyTicket`. Misleading naming. |
| **Dashboard revenue** | **Hardcoded** monthly `RevenueData` / `Expenses` in `GetDashboardSummaryQueryHandler` (comment: mocked). |
| **CORS** | `SetIsOriginAllowed(origin => true)` — permissive (dev-style). |
| **Installation admin fallback** | `InstallationController.CreateProject` uses `Guid.NewGuid()` if user id claim missing — risky outside dev. |
| **Authentication gaps** | `TestController`: no auth. `InventoryController.GetTotalValue`: no `[Authorize]`. `MaintenanceController` PDF: `[AllowAnonymous]`. |
| **Emergency API** | Class-level `[Authorize]` only — **no role policy**; broader than other modules. |
| **Finance** | `RequireFinance` policy exists; **no controller** using it found. Frontend finance page is **mock-only**. |
| **Faults UI** | Backend API exists; **no** matching Next.js page/API client in scanned frontend files. |
| **Subscription / billing** | **Not Found** in codebase. |
| **Public customer portal** | **Not Found** — fault creation requires staff roles today. |

---

## 7. SaaS Readiness Analysis

### 7.1 Multi-tenancy — what must change

1. **Tenant model:** Introduce `Company` / `Tenant` entity with its own settings, subscription reference, and status.
2. **Row-level isolation:** Add `CompanyId` (or `TenantId`) to **every** tenant-owned table and enforce it in:
   - EF Core global query filters + disciplined `SaveChanges` assignment, **or**
   - separate database per tenant (heavier ops), **or**
   - schema per tenant (middle ground).
3. **Identity:** Either:
   - Users belong to one company (`AppUser.CompanyId`), or
   - User–tenant M:N with active tenant in JWT claim (B2B agencies).
4. **Cross-tenant uniqueness:** Today `InstallationProjects.ProjectNumber` is globally unique — must become **unique per tenant** (composite unique index).
5. **Background jobs / PDFs / files:** Storage paths must be partitioned by tenant (container prefix or path segment).
6. **Seeding:** `AdminSeeder` and migrations must create per-tenant defaults (roles can stay global or be cloned per tenant per product decision).

### 7.2 Where to add `CompanyId` (recommended first pass)

| Layer | Entities / tables |
|-------|-------------------|
| **Master data** | `Categories`, `InventoryItems`, `Technicians` |
| **CRM / installation** | `Customers`, `InstallationProjects`, `Elevators`, `InstallationStages`, `StageRequiredParts`, `StageTechnicians`, `TechnicianAssignments`, `InspectionRequests`, `Offers`, `Quotations`, `QuotationAttachments`, `Notifications` |
| **Maintenance** | `MaintenanceContracts`, `MaintenanceElevators`, `MaintenanceVisits`, `MaintenanceSparePartUsages`, `MaintenanceChecklistItems`, `MaintenanceVisitChecklistItems` |
| **Operations** | `FaultTickets`, `FaultSparePartUsages`, `EmergencyTickets` |
| **Users** | `AspNetUsers` (or join table UserTenants) |

**Identity tables:** Keep `AspNetRoles` global **or** namespace role names per tenant (`TenantId:Role`); simplest MVP is single role catalog + `CompanyId` on user.

### 7.3 Required SaaS modules (not present today)

| Module | Status in repo |
|--------|----------------|
| **Authentication** | Present (JWT + Identity + refresh). Needs tenant claims and optional SSO later. |
| **Role management** | Present (Manager assigns roles). Needs tenant-scoped roles / permissions matrix for SaaS. |
| **Subscription** | **Not Found** — need plans, entitlements, billing provider integration, trial/expiry. |
| **Tenant onboarding** | **Not Found** — signup, company profile, DNS/subdomain mapping. |
| **Audit / compliance** | Partial (`BaseAuditableEntity`); may need immutable audit log per tenant. |
| **Usage metering** | **Not Found** — optional for billing (users, elevators, projects caps). |

---

## 8. Suggested Refactoring Plan (MVP-first)

### Phase 0 — Harden single-tenant (1–2 weeks)

1. Remove or gate `Guid.NewGuid()` fallback for installation admin id; require valid JWT.
2. Add `[Authorize]` to `Inventory/value`; restrict `Test` in non-dev; protect maintenance PDF or use signed URLs.
3. Tighten `EmergencyController` with the same role policies as other modules (define explicitly).
4. Fix dashboard: rename metric or include real `EmergencyTicket` counts; remove or flag mocked revenue.

### Phase 1 — Tenant foundation (MVP SaaS)

1. Add `Companies` table + `CompanyId` on `AppUser` and all business tables (migration).
2. Composite unique indexes where needed (`ProjectNumber` + `CompanyId`).
3. Implement `ICurrentTenantService` (from JWT + validation) and EF **query filters** on tenant entities.
4. On register/login, issue claims: `company_id`, optional `subscription_tier`.
5. Seed first company + migrate existing rows to `CompanyId = default`.

### Phase 2 — API and UI tenancy

1. Add company admin UI (settings) — replace hardcoded “LiftOps” placeholders on `settings` page with API-backed company profile.
2. Ensure all MediatR handlers use tenant-scoped repositories (no `ListAllAsync()` without filter).
3. Add frontend: tenant switcher only if supporting multi-company users.

### Phase 3 — Subscription (minimal)

1. `Subscriptions` table: `CompanyId`, plan, status, period end, external customer id.
2. Middleware or filter: block write operations when `PastDue` / `Cancelled` (configurable).
3. Integrate one billing provider (Stripe, Paddle, etc.) — **product choice not in codebase**.

### Phase 4 — Product gaps

1. Expose **Faults** in the SPA or a customer portal; align dashboard KPIs with real modules.
2. Implement **Finance** API or remove mock page until backend exists.
3. Installation **assign-technicians** UI if still required by operations.

---

## Appendix A — Solution layout (reference)

| Path | Role |
|------|------|
| `LiftOps/LiftOps-BackEnd.API` | HTTP API, controllers, `Program.cs` |
| `LiftOps/LiftOps-BackEnd.Application` | MediatR features, DTOs |
| `LiftOps/LiftOps-BackEnd.Domain` | Entities, enums, role names |
| `LiftOps/LiftOps-BackEnd.Infrastructure` | EF Core, migrations, services |
| `liftops-frontend` | Next.js UI |

---

*This document reflects the repository state at extraction time. If a behavior is not listed here, treat it as **Not Found** unless verified in code.*
