# LiftOps Project Overview

## What LiftOps Is

LiftOps is a multi-tenant SaaS platform for elevator companies to manage operations end-to-end:

- Installation projects
- Maintenance contracts and visits
- Emergency/fault tickets
- Inventory and spare parts
- Technician assignments
- Role-based admin operations

## Problem It Solves

Many elevator businesses run critical operations through spreadsheets and manual processes, which causes:

- Missed schedules
- Weak visibility
- Slow coordination across teams
- Higher operational risk

LiftOps centralizes workflows and makes operations trackable and scalable.

## Product Goals

- Deliver a secure multi-tenant SaaS foundation.
- Enforce role-based access and data isolation by company.
- Provide reliable day-to-day operational workflows.
- Improve decision-making with dashboards and reporting.

## Core Roles

- `Manager`
- `InstallationAdmin`
- `MaintenanceAdmin`
- `InventoryAdmin`
- `FinanceAdmin`
- `FaultsAdmin`
- `Technician`
- `PlatformAdmin` (platform-level operations)

## Current Documentation Map

- High-level idea: `docs/PROJECT_IDEA.md`
- System architecture: `docs/ARCHITECTURE.md`
- AI working rules: `docs/AI_GUIDELINES.md`
- Backend guide: `docs/BACKEND_GUIDE.md`
- Frontend guide: `docs/FRONTEND_GUIDE.md`
- SaaS backlog: `docs/SAAS_TASKS.md`

## Scope Direction

LiftOps is actively evolving from a single-tenant product to a production-grade SaaS with:

- Tenant-aware security and authorization
- Subscription lifecycle support
- Better frontend handling for auth and tenant context
- Deployment and observability hardening
