# LiftOps Documentation

This folder contains the core project documentation and implementation guides.

## Core Docs (Keep and Maintain)

- `docs/PROJECT_IDEA.md` - product vision, scope, and module map.
- `docs/ARCHITECTURE.md` - Clean Architecture flow, multi-tenant vs platform admin, system overview.
- `docs/BACKEND_GUIDE.md` - backend stack, auth flow, DTOs/commands, seeding, policies.
- `docs/FRONTEND_GUIDE.md` - App Router layout, auth BFF, middleware, guards.
- `docs/SAAS_TASKS.md` - SaaS transformation backlog, audit status, next critical tasks.
- `docs/AI_CONTEXT.md` - **context anchor for AI assistants** (stack, rules, critical facts, prompting).

## Existing Supporting Docs in Repository

These are still useful references and should be kept unless intentionally retired:

- Root-level status/summary docs:
  - `BACKEND_STATUS.md`
  - `FRONTEND_STATUS.md`
  - `IMPLEMENTATION_SUMMARY.md`
  - `COMPLETE_IMPLEMENTATION_SUMMARY.md`
  - `FRONTEND_IMPLEMENTATION_SUMMARY.md`
  - `PROJECT_STATUS_EXPLANATION.md`
- Security and transformation docs:
  - `SEC-001_IMPLEMENTATION.md`
  - `SEC-002_IMPLEMENTATION.md`
  - `SAAS_TRANSFORMATION_PLAN.md`
- Feature workflow doc:
  - `INSPECTION_OFFER_WORKFLOW.md`
- Backend and frontend module-specific docs:
  - `LiftOps/API_ENDPOINTS.md`
  - `LiftOps/PROJECT_REFERENCE.md`
  - `LiftOps/API_TENANT_AUDIT.md`
  - `LiftOps/AUTH_ROLE_MATRIX.md`
  - `LiftOps/MT005_DRY_RUN.md`
  - `liftops-frontend/API_ENDPOINTS.md`
  - `liftops-frontend/ENV_SETUP.md`
  - `liftops-frontend/VERCEL_DEPLOYMENT.md`
  - `liftops-frontend/RECOMMENDED_DEPENDENCIES.md`

## Rule for New Docs

- Put high-level product and architecture docs under `docs/`.
- Keep technical module docs near code when strongly tied to one subproject.
- Avoid duplicate docs that describe the same thing with different names.
