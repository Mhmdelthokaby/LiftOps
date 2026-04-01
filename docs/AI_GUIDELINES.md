# AI Guidelines for LiftOps

These guidelines define how AI assistants should work in this repository.

## 1) Read Order (Always)

Before making major decisions or edits, review in this order:

1. `docs/PROJECT_OVERVIEW.md`
2. `docs/ARCHITECTURE.md`
3. `docs/AI_GUIDELINES.md`
4. Relevant feature docs (`docs/BACKEND_GUIDE.md`, `docs/FRONTEND_GUIDE.md`, `docs/SAAS_TASKS.md`)

## 2) Core Principles

- Keep multi-tenant safety first.
- Preserve role-based authorization expectations.
- Prefer clear, maintainable code over clever code.
- Avoid changes that break current module boundaries.
- Keep documentation in sync with implementation changes.

## 3) Backend Rules

- Follow Clean Architecture boundaries.
- Keep controllers thin; place logic in handlers/services.
- Ensure tenant-scoped routes require valid tenant context.
- Apply authorization for every new endpoint.
- Keep validation explicit for write operations.
- Do not bypass subscription/tenant middleware behavior unintentionally.

## 4) Frontend Rules

- Use centralized API helpers in `lib/api.ts`.
- Keep access control consistent with role helpers and `AuthGuard`.
- Handle loading/error states for async UI actions.
- Use strong TypeScript typing; avoid `any`.
- Keep feature components focused and reusable.

## 5) Documentation Rules

- If architecture or behavior changes, update the related docs in `docs/`.
- Add new high-level docs under `docs/` rather than scattering files.
- Avoid duplicate docs that describe the same thing differently.

## 6) Change Safety Checklist

Before finalizing a change, verify:

- Tenant isolation is preserved.
- Role access behavior remains correct.
- API contracts remain consistent.
- New/updated docs are included when needed.
- No unrelated files are modified.

## 7) Preferred Output Style for AI Updates

- Be concise and implementation-focused.
- Mention what changed, where, and why.
- Provide clear next steps when relevant (test/build/commit).
