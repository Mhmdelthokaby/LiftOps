# AI Guidelines for LiftOps

## 1) Read Order (Always)

Before making major decisions or edits, review:
1. `docs/PROJECT_OVERVIEW.md`
2. `docs/ARCHITECTURE.md`
3. `docs/AI_GUIDELINES.md`
4. Relevant feature docs (`docs/API_GUIDE.md`, `docs/FRONTEND_GUIDE.md`, `docs/SAAS_TASKS.md`)
5. `lifops-next/PROJECT_MAP.md`

## 2) Core Principles

- Keep multi-tenant safety first.
- Preserve role-based authorization expectations.
- Prefer clear, maintainable code over clever code.
- Avoid changes that break current module boundaries.
- Keep documentation in sync with implementation changes.

## 3) Backend (API) Rules

- Route Handlers stay thin; place logic in service layer.
- Ensure tenant-scoped routes require valid tenant context (JWT `company_id`).
- Apply authorization for every new endpoint (check role + tenant).
- Validate input with zod schemas for write operations.
- Do not bypass tenant filtering unintentionally.

## 4) Frontend Rules

- Use centralized API client in `src/lib/api-client.ts`.
- Keep access control consistent with `AuthGuard` and role helpers.
- Handle loading/error states for async UI actions.
- Use strong TypeScript typing; avoid `any`.
- Keep feature components focused and reusable.

## 5) Documentation Rules

- If architecture or behavior changes, update related docs in `docs/`.
- Add new high-level docs under `docs/` rather than scattering files.
- Avoid duplicate docs that describe the same thing differently.
- Update `PROJECT_MAP.md` when adding/removing major files.

## 6) Change Safety Checklist

Before finalizing a change, verify:
- Tenant isolation is preserved.
- Role access behavior remains correct.
- API contracts remain consistent.
- New/updated docs are included when needed.
- No unrelated files are modified.

## 7) Preferred Output Style

- Be concise and implementation-focused.
- Mention what changed, where, and why.
- Provide clear next steps when relevant (build/test/commit).
