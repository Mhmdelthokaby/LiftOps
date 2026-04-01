# Backend Guide

This guide describes the LiftOps backend architecture, roles, and API route map.

## Stack

- ASP.NET Core Web API
- Clean Architecture (API, Application, Domain, Infrastructure)
- EF Core + SQL Server
- MediatR for command/query handlers
- JWT authentication + role/policy authorization

## High-Level Architecture

- `LiftOps/LiftOps-BackEnd.API`: controllers, middleware, auth/policies, app startup.
- `LiftOps/LiftOps-BackEnd.Application`: DTOs, commands/queries, validators, interfaces.
- `LiftOps/LiftOps-BackEnd.Domain`: entities, enums, shared constants like roles.
- `LiftOps/LiftOps-BackEnd.Infrastructure`: EF persistence, services, migrations.

## Authentication and Tenant Security

- JWT is configured in `Program.cs`.
- Authorized tenant endpoints require `company_id` claim.
- Platform routes (`/api/platform/...`) are separated and use platform policy.
- Subscription middleware can block write operations for inactive subscriptions.

## Roles

Defined in `LiftOps-BackEnd.Domain/Common/Roles.cs`:

- `Manager`
- `InstallationAdmin`
- `MaintenanceAdmin`
- `InventoryAdmin`
- `FinanceAdmin`
- `FaultsAdmin`
- `Technician`
- `PlatformAdmin`

## Authorization Policies

Registered in `LiftOps-BackEnd.API/Program.cs`:

- `RequireManager`
- `RequireInstallation`
- `RequireMaintenance`
- `RequireInventory`
- `RequireFaults`
- `EmergencyReport`
- `EmergencyRead`
- `EmergencyDispatch`
- `EmergencyResolve`
- `EmergencyManage`
- `RequirePlatformAdmin`

## Main API Controllers and Routes

Base paths and examples:

- `api/Admin`
  - `POST login`
  - `POST refresh-token`
  - `POST register`
  - `GET list`
  - `PUT update/{id}`
  - `PUT disable/{id}`
  - `PUT roles/{id}`

- `api/Installation`
  - `POST project/add`
  - `GET projects`
  - `GET project/{id}`
  - `POST stage/start`
  - `POST stage/complete`
  - `POST inspection/create`
  - `POST offer/create`
  - `POST quotation/create`

- `api/Maintenance`
  - `POST add-contract`
  - `POST visit/schedule`
  - `POST visit/{visitId}/complete`
  - `GET projects`
  - `GET schedule/monthly`
  - `GET statistics`

- `api/Inventory`
  - `POST add`
  - `PUT update/{id}`
  - `GET all`
  - `GET active`
  - `GET value`

- `api/Category`
  - `POST add`
  - `GET list`

- `api/Technician`
  - `GET visits/today`
  - `GET visits/{visitId}`
  - `PUT visits/{visitId}/status`
  - `POST visits/{visitId}/complete`
  - `GET all`
  - `POST add`

- `api/Customers`
  - `GET`
  - `PUT {id}`
  - `PUT {id}/status`

- `api/Emergency`
  - `POST`
  - `GET`
  - `GET {id}`
  - `PUT {id}`
  - `DELETE {id}`
  - `PUT {id}/assign-technician`
  - `POST {id}/resolve`
  - `GET open`

- `api/Faults`
  - `POST create-ticket`
  - `PUT {ticketId}/assign-technician`
  - `POST {ticketId}/resolve`
  - `GET open`

- `api/Dashboard`
  - `GET summary`

- `api/platform/subscriptions` (platform admin)
  - `POST {companyId}/extend-trial`
  - `PUT {companyId}/status`

- `api/subscription/webhook`
  - `POST payment-failed`

## Request Pipeline Notes

- CORS policy: permissive in dev, configured origins in non-dev.
- Forwarded headers support for reverse-proxy environments.
- Global rate limiter includes auth and expensive endpoint partitions.
- Unified JSON error envelope shape (`code`, `message`, `details`).

## Development Checklist

- Add authorization for every new route.
- Ensure tenant-safe access for every by-id read/update.
- Prefer MediatR handlers over controller business logic.
- Add/maintain validation on write commands.
- Keep migrations additive and safe for shared multi-tenant data.
