# Frontend Guide

This guide describes the LiftOps frontend structure, route layout, roles, and implementation direction.

## Stack

- Next.js (App Router)
- React + TypeScript
- Tailwind CSS + ShadCN UI
- Centralized API layer in `liftops-frontend/lib/api.ts`

## High-Level Architecture

- `liftops-frontend/app`: route pages (App Router).
- `liftops-frontend/components`: feature and shared UI components.
- `liftops-frontend/lib`: auth, API client, user role helpers, utilities.
- `liftops-frontend/hooks`: shared hooks.
- `liftops-frontend/middleware.ts`: route matcher with client-side auth enforcement.

## Authentication and Access Control

- Global guard is applied in `app/layout.tsx` via `AuthGuard`.
- `components/auth-guard.tsx` controls route access by role.
- Role checks use helpers from `lib/user.ts`.
- Auth token and user info are handled in `lib/auth.ts`.

## Frontend Routes

Public marketing site (no login): `/`, `/home` (redirects to `/`), `/about`, `/pricing`, `/contact`. Authenticated app home: `/dashboard`. See `docs/LANDING_MIGRATION_TASKS.md` for the landing migration checklist.

Primary pages currently under `app/`:

- `/` marketing home
- `/dashboard` dashboard
- `/login`
- `/settings`
- `/clients`
- `/clients/[id]`
- `/clients/[id]/edit`
- `/projects`
- `/projects/new`
- `/projects/[id]`
- `/installation`
- `/maintenance`
- `/maintenance/projects`
- `/maintenance/projects/new`
- `/maintenance/projects/[id]`
- `/maintenance/elevators`
- `/maintenance/assign-visits`
- `/inventory`
- `/emergency`
- `/finance`
- `/technicians`
- `/technicians/new`
- `/technicians/[id]/edit`
- `/technician/visits`
- `/inspection/new`

## Sidebar Navigation and Role-Based Views

Sidebar items are managed in `components/app-sidebar.tsx` and filtered by role:

- Dashboard
- My Visits (technician)
- Clients
- Projects
- Installation Pipeline
- Inventory
- Technicians
- Maintenance
- Emergency Tickets
- Settings

## Role-Based Redirect Behavior

When access is denied, `AuthGuard` redirects users based on role priority:

- `Technician` -> `/technician/visits`
- `Manager` -> `/dashboard`
- `InstallationAdmin` -> `/installation`
- `MaintenanceAdmin` -> `/maintenance?view=projects`
- `InventoryAdmin` -> `/inventory`
- `FinanceAdmin` -> `/finance`
- `FaultsAdmin` -> `/emergency`

## Frontend Implementation Guidelines

- Keep API calls centralized in `lib/api.ts`.
- Use strict TypeScript types for DTOs and responses.
- Handle loading and error states in pages/components.
- Show user feedback with toasts for async actions.
- Keep feature components focused and reusable.

## Next Improvements

- Add explicit company context in frontend state based on JWT `company_id`.
- Standardize 401/403/subscription-expired handling in one place.
- Add route-level docs for each module as features expand.
