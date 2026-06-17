# API Guide

All API endpoints live in `src/app/api/` as Next.js Route Handlers.

## Auth Endpoints

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/api/auth/login` | POST | Login with email/password, returns tokens + user |
| `/api/auth/register` | POST | Register new user |
| `/api/auth/refresh` | POST | Refresh access token |
| `/api/auth/logout` | POST | Clear auth cookies |
| `/api/auth/me` | GET | Get current user profile |

### Login Response

```json
{
  "token": "jwt_access_token",
  "refreshToken": "jwt_refresh_token",
  "name": "User Name",
  "email": "user@example.com",
  "roles": ["ADMIN"]
}
```

### Cookie Names

- `liftops_access` — JWT access token (httpOnly)
- `liftops_refresh` — JWT refresh token (httpOnly)

## Business Endpoints

### Installation

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/api/installation/customers` | GET/POST | List/create customers |
| `/api/installation/customers/[id]` | GET/PUT/DELETE | Customer CRUD |
| `/api/installation/elevators` | GET/POST | List/create elevators |
| `/api/installation/elevators/[id]` | GET/PUT/DELETE | Elevator CRUD |
| `/api/installation/stages` | GET/POST | List/create stages |
| `/api/installation/stages/[id]` | GET/PUT/DELETE | Stage CRUD |
| `/api/installation/projects` | GET/POST | List/create projects |
| `/api/installation/projects/[id]` | GET/PUT | Project CRUD |
| `/api/installation/technicians` | GET/POST | List/create technicians |
| `/api/installation/technicians/[id]` | GET/PUT/DELETE | Technician CRUD |
| `/api/installation/inspections` | GET/POST | Inspection requests |
| `/api/installation/offers` | GET/POST | Pricing offers |
| `/api/installation/quotations` | GET/POST | Quotations |

### Maintenance

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/api/maintenance/contracts` | GET/POST | Contracts |
| `/api/maintenance/contracts/[id]` | GET/PUT | Contract CRUD |
| `/api/maintenance/visits` | GET/POST | Maintenance visits |
| `/api/maintenance/visits/[id]` | GET/PUT | Visit CRUD |
| `/api/maintenance/checklists` | GET/POST | Checklist templates |
| `/api/maintenance/statistics` | GET | Maintenance stats |

### Tickets / Emergency

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/api/tickets` | GET/POST | Fault tickets |
| `/api/tickets/[id]` | GET/PUT | Ticket CRUD |
| `/api/tickets/[id]/assign` | POST | Assign technician |
| `/api/tickets/[id]/resolve` | POST | Resolve ticket |
| `/api/emergency` | GET/POST | Emergency tickets |
| `/api/emergency/open` | GET | Open emergencies |
| `/api/emergency/[id]` | GET/PUT | Emergency CRUD |
| `/api/emergency/[id]/assign` | POST | Assign technician |
| `/api/emergency/[id]/resolve` | POST | Resolve emergency |

### Inventory

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/api/inventory/items` | GET/POST | Inventory items |
| `/api/inventory/items/[id]` | GET/PUT/DELETE | Item CRUD |
| `/api/inventory/categories` | GET/POST | Item categories |
| `/api/inventory/value` | GET | Total inventory value |

### Dashboard

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/api/dashboard` | GET | KPI summary |
| `/api/companies/profile` | GET | Company profile |

### Platform (Super Admin)

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/api/platform/users` | GET/POST | Platform users |
| `/api/platform/users/[id]` | GET/PUT/DELETE | User CRUD |
| `/api/companies` | GET/POST | Companies |
| `/api/companies/[id]` | GET/PUT | Company CRUD |
| `/api/subscription/plans` | GET/POST | Subscription plans |
| `/api/subscription/plans/[id]` | GET/PUT/DELETE | Plan CRUD |
| `/api/subscription` | GET/PUT/POST | Subscription management |
| `/api/subscription/change-plan` | POST | Change plan |

## Auth Headers

All business endpoints require `Authorization: Bearer <token>` header (or httpOnly cookie). Platform admin endpoints require `SUPER_ADMIN` role.

## Response Format

```json
{
  "success": true,
  "succeeded": true,
  "data": { ... },
  "message": "optional message"
}
```

Error responses include `error`/`errors` field with machine-readable codes.
