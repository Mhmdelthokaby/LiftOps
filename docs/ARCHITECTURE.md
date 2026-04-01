# LiftOps Architecture

## System Overview

LiftOps uses a full-stack architecture:

- Backend: ASP.NET Core Web API
- Frontend: Next.js (App Router) with React + TypeScript
- Database: SQL Server via EF Core
- Auth: JWT with role and tenant claims

## Backend Architecture (Clean Architecture)

Codebase root: `LiftOps/`

- `LiftOps-BackEnd.API`
  - Controllers, middleware, startup, auth/policies
- `LiftOps-BackEnd.Application`
  - Commands/queries (MediatR), DTOs, validators, interfaces
- `LiftOps-BackEnd.Domain`
  - Core entities, enums, role constants
- `LiftOps-BackEnd.Infrastructure`
  - EF Core persistence, repositories/services, migrations

Key characteristics:

- MediatR for application flow
- Policy + role-based authorization
- Tenant claim requirement (`company_id`) for tenant-scoped routes
- Subscription middleware to control write access by plan status
- Global error envelope and request rate limiting

## Frontend Architecture (Next.js App Router)

Codebase root: `liftops-frontend/`

- `app/` route pages
- `components/` feature and shared components
- `lib/` auth, API client, user helpers
- `hooks/` reusable hooks
- `middleware.ts` route matcher

Key characteristics:

- Client-side `AuthGuard` enforces route access rules
- Sidebar menus are filtered by user role
- API interactions centralized in `lib/api.ts`
- Role-based redirect behavior for unauthorized route access

## Multi-Tenancy Model

- Tenant unit: `Company`
- Company context is carried in JWT claim: `company_id`
- Data isolation is enforced in API and persistence layers
- Platform-level operations are separated under platform-only routes

## Core Domains

- Installation
- Maintenance
- Emergency/Faults
- Inventory
- Admin and role management
- Subscription/platform controls

## Request Flow (High Level)

1. User authenticates and receives JWT.
2. Frontend stores token/user context and applies role checks.
3. API validates token, role, and tenant claim.
4. Controller dispatches command/query through MediatR.
5. Application + infrastructure handle business logic and persistence.
6. API returns standardized response to frontend.
