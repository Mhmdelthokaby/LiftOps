# LiftOps Project Overview

## What LiftOps Is

LiftOps is a multi-tenant SaaS platform for elevator companies to manage operations end-to-end from a single Next.js application:

- Installation projects
- Maintenance contracts and visits
- Emergency/fault tickets
- Inventory and spare parts
- Technician assignments
- Role-based admin operations (platform console)

## Problem It Solves

Many elevator businesses run critical operations through spreadsheets and manual processes, which causes missed schedules, weak visibility, slow coordination, and higher operational risk. LiftOps centralizes workflows and makes operations trackable and scalable.

## Product Goals

- Deliver a secure multi-tenant SaaS foundation.
- Enforce role-based access and data isolation by company.
- Provide reliable day-to-day operational workflows.
- Improve decision-making with dashboards and reporting.

## Core Roles

- `SUPER_ADMIN` — platform-wide operations
- `ADMIN` — company-level management
- `MANAGER` — day-to-day operational management
- `TECHNICIAN` — field work and visit execution
- `CLIENT` — customer portal access

## Architecture

Single Next.js 15 project with:

- **API layer:** Route Handlers under `src/app/api/`
- **Service layer:** Business logic in `src/lib/services/`
- **Auth layer:** JWT (jose) + httpOnly cookies + bcryptjs
- **Data layer:** Prisma ORM + PostgreSQL
- **Frontend:** React 19 Server/Client Components, Tailwind 4, ShadCN UI

## Documentation Map

- Architecture: `docs/ARCHITECTURE.md`
- API guide: `docs/API_GUIDE.md`
- Frontend guide: `docs/FRONTEND_GUIDE.md`
- SaaS backlog: `docs/SAAS_TASKS.md`
- AI context: `docs/AI_CONTEXT.md`
- Project map: `lifops-next/PROJECT_MAP.md`
