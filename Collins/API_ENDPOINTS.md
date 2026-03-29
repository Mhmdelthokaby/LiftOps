# Exaustive API Endpoints Reference

This document provides a complete reference for every endpoint available in the Collins-BackEnd API, organized by controller.

---

## **Base Configuration**
- **Base URL**: `http://localhost:5295/api`
- **Auth Scheme**: Bearer Token (JWT)
- **Standard Response Wrapper**: 
  ```json
  {
    "succeeded": true,
    "data": { ... },
    "errors": [],
    "message": "Optional message"
  }
  ```

---

## **1. Admin (AdminController)**
**Route**: `api/Admin`

### **Login**
- **Method**: `POST` | `/login`
- **Auth**: `[AllowAnonymous]`
- **Request Body (`LoginDto`)**:
  - `email` (string)
  - `password` (string)
- **Response (`AuthResponseDto`)**:
  - `token`, `refreshToken`, `refreshTokenExpiry`, `name`, `email`, `roles` (List)

### **Refresh Token**
- **Method**: `POST` | `/refresh-token`
- **Auth**: `[AllowAnonymous]`
- **Request Body**: `{ "token": "...", "refreshToken": "..." }`
- **Response**: Same as Login.

### **Register Admin**
- **Method**: `POST` | `/register`
- **Auth**: `Manager` only
- **Request Body (`RegisterAdminDto`)**:
  - `name`, `email`, `phone`, `password`, `roles` (List<string>)

### **List Admins**
- **Method**: `GET` | `/list`
- **Auth**: `Manager` only
- **Response**: List of `AdminListItemDto`.

### **Update Admin**
- **Method**: `PUT` | `/update/{id}`
- **Auth**: `Authorized` (Self or Manager)
- **Request Body (`UpdateAdminDto`)**:
  - `name`, `email`, `phone`, `password` (optional)

### **Disable/Enable Admin**
- **Method**: `PUT` | `/disable/{id}`
- **Auth**: `Manager` only
- **Request Body**: `bool` (true to disable, false to enable)

### **Assign Roles**
- **Method**: `PUT` | `/roles/{id}`
- **Auth**: `Manager` only
- **Request Body**: `List<string>` (e.g., `["Manager", "InstallationAdmin"]`)

---

## **2. Installation (InstallationController)**
**Route**: `api/Installation`

### **Create Full Project**
- **Method**: `POST` | `/project/add`
- **Request Body (`CreateProjectDto`)**:
  - `customer`: `CustomerDto` (Name, Phone, Email, Address, ProjectNumber, GoogleMapsLink)
  - `contract`: `ContractDto` (InstallationPricePerUnit, TotalPrice, ContractDate, etc.)
  - `elevators`: `List<CreateElevatorDto>` (Type, Stops, Floors, NumberOfElevators)

### **Add Elevator to Existing Project**
- **Method**: `POST` | `/project/{projectId}/elevator/add`
- **Request Body (`CreateElevatorDto`)**: `ElevatorType`, `NumberOfStops`, `NumberOfFloors`, `NumberOfElevators`

### **Start Stage**
- **Method**: `POST` | `/stage/start`
- **Request Body (`StartStageDto`)**: `{ "stageId": "...", "startDate": "..." }`

### **Complete Stage**
- **Method**: `POST` | `/stage/complete`
- **Request Body (`CompleteStageDto`)**: `{ "stageId": "...", "supplyCost": 0, "notes": "..." }`

### **Add Stage Parts**
- **Method**: `POST` | `/stage/parts`
- **Request Body (`AddStagePartsDto`)**: `{ "stageId": "...", "parts": [ { "inventoryItemId": "...", "quantity": 1 } ] }`

### **List Projects**
- **Method**: `GET` | `/projects`
- **Response**: List of `InstallationProjectDto`.

### **Get Project Details**
- **Method**: `GET` | `/project/{id}`
- **Response**: `InstallationProjectDto` (includes Elevators and Stages).

### **Get Stage Details**
- **Method**: `GET` | `/stage/{id}`
- **Response**: `InstallationStageDto`.

### **Get Notifications**
- **Method**: `GET` | `/notifications`
- **Response**: List of `NotificationDto`.

### **Assign Technicians to Elevator**
- **Method**: `POST` | `/elevator/{elevatorId}/assign-technicians`
- **Auth**: `Manager` or `InstallationAdmin`
- **Request Body (`AssignTechnicianDto`)**: `{ "technicianIds": ["GUID1", "GUID2"] }`

### **Unassign Technician**
- **Method**: `DELETE` | `/elevator/{elevatorId}/unassign-technician/{techId}`
- **Auth**: `Manager` or `InstallationAdmin`

---

## **3. Inventory (InventoryController)**
**Route**: `api/Inventory`
**Roles**: `Manager`, `InventoryAdmin`

### **Add Inventory Item**
- **Method**: `POST` | `/add`
- **Request Body (`CreateInventoryItemDto`)**: `Name`, `CategoryId`, `UnitPrice`, `StockQuantity`, `SupplierName`

### **Update Item**
- **Method**: `PUT` | `/update/{id}`
- **Request Body (`UpdateInventoryItemDto`)**: `Name`, `CategoryId`, `UnitPrice`, `StockQuantity`, `SupplierName`

### **Disable/Enable Item**
- **Method**: `PUT` | `/disable/{id}`
- **Request Body**: `bool` (disable flag)

### **List All Items**
- **Method**: `GET` | `/all`

### **List Active Items**
- **Method**: `GET` | `/active`

### **Get Total Value**
- **Method**: `GET` | `/value`
- **Response**: `{ "totalValue": 1234.56 }`

---

## **4. Categories (CategoryController)**
**Route**: `api/Category`
**Roles**: `Manager`, `InventoryAdmin`

### **Add Category**
- **Method**: `POST` | `/add`
- **Request Body (`CreateCategoryDto`)**: `Name`, `Description`

### **List Categories**
- **Method**: `GET` | `/list`
- **Response**: List of `CategoryDto`.

---

## **5. Technicians (TechnicianController)**
**Route**: `api/Technician`
**Roles**: `Manager` only

### **Add Technician**
- **Method**: `POST` | `/add`
- **Request Body (`CreateTechnicianDto`)**: `Name`, `Phone`, `Specialization`

### **Update Technician**
- **Method**: `PUT` | `/update/{id}`
- **Request Body (`UpdateTechnicianDto`)**: `Name`, `Phone`, `Specialization`

### **Disable/Enable Technician**
- **Method**: `PUT` | `/disable/{id}`
- **Request Body**: `bool` (disable flag)

### **List All**
- **Method**: `GET` | `/all`

### **List Available (Low Workload)**
- **Method**: `GET` | `/available` (Workload < 5 active elevators)

---

## **6. Maintenance (MaintenanceController)**
**Route**: `api/Maintenance`
**Roles**: `Manager`, `MaintenanceAdmin`

### **Add Contract**
- **Method**: `POST` | `/add-contract`
- **Request Body (`CreateMaintenanceContractDto`)**: `CustomerId`, `StartDate`, `EndDate`, `PricePerMonth`, `FreeMonths`

### **Schedule Visit**
- **Method**: `POST` | `/visit/schedule`
- **Request Body (`ScheduleVisitDto`)**: `ElevatorId`, `VisitDate`

### **Complete Visit**
- **Method**: `POST` | `/visit/{visitId}/complete`
- **Request Body (`CompleteVisitDto`)**: `Notes`, `PartsUsed` (List of ItemId/Quantity)

---

## **7. Faults (FaultsController)**
**Route**: `api/Faults`
**Roles**: `Manager`, `MaintenanceAdmin`, `FaultsAdmin`

### **Create Ticket**
- **Method**: `POST` | `/create-ticket`
- **Request Body (`CreateFaultTicketDto`)**: `CustomerName`, `Phone`, `ProjectAddress`, `ElevatorType`, `FaultDescription`, `Severity` (Enum)

### **Assign Technician**
- **Method**: `PUT` | `/{ticketId}/assign-technician`
- **Request Body (`AssignFaultTechnicianDto`)**: `TechnicianId`

### **Resolve Ticket**
- **Method**: `POST` | `/{ticketId}/resolve`
- **Request Body (`ResolveFaultDto`)**: `Notes`

### **List Open Tickets**
- **Method**: `GET` | `/open`

---

## **8. Dashboard (DashboardController)**
**Route**: `api/Dashboard`
**Auth**: `Authorized`

### **Get Summary**
- **Method**: `GET` | `/summary`
- **Response (`DashboardSummaryDto`)**: KPIs, `revenueData`, `projectStatusData`, `recentActivities`.

---

## **9. Customers (CustomersController)**
**Route**: `api/Customers`
**Auth**: `Authorized`

### **List All Customers**
- **Method**: `GET` | `/`
- **Response**: List of `CustomerDto`.

---

## **10. Utility/Test (TestController)**
**Route**: `api/Test`
**Auth**: `AllowAnonymous`

### **Status Check**
- **Method**: `GET` | `/`
- **Response**: `{ "message": "...", "time": "..." }`

### **Ping**
- **Method**: `GET` | `/ping`
- **Response**: `"pong"`
