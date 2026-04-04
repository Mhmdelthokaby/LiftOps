# Frontend Guide

This guide reflects **`liftops-frontend/`** (Next.js App Router). There is no Pages Router in use for the main app.

## Stack

- **Next.js** 16.0.x (App Router)
- **React** 19.x, **TypeScript** 5.x
- **Tailwind CSS** 4.x, **ShadCN-style** components (Radix primitives under `components/ui/`)
- **react-hook-form**, **zod**, **@hookform/resolvers**
- **TanStack Table** for data tables (admin console)
- **lucide-react** icons

## Directory structure (high level)

| Path | Role |
|------|------|
| `app/` | Routes: marketing, dashboard modules, **`(admin)/admin/*`**, **`(auth)/admin/login`**, **`api/auth/*`** |
| `components/` | Feature UI, **`auth-guard.tsx`**, **`auth/login-form.tsx`**, sidebar, ShadCN building blocks |
| `lib/` | **`auth.ts`**, **`api.ts`**, **`api-client.ts`**, **`api-platform.ts`**, **`api-config.ts`**, **`user.ts`**, **`navigation.ts`**, **`jwt-edge.ts`** |
| `hooks/` | Data hooks (e.g. admin platform data) |
| `types/` | Shared TS types (e.g. `types/admin.ts`) |
| `middleware.ts` | Edge middleware for **`/admin/*`** protection |

Route groups **`(admin)`** and **`(auth)`** organize layouts without affecting the URL path.

## Authentication — client and BFF

### `lib/auth.ts`

- **`login(data)`** — **`POST`** to same-origin **`/api/auth/login`** (Next Route Handler). On success, use **`saveAuthDocs`** to persist **accessToken**, **refreshToken**, and **user** JSON in **`localStorage`** (existing API client behavior).
- **`loginAdmin(data)`** — direct **`POST`** to **`${NEXT_PUBLIC_API_URL}/api/auth/login`** for tools or non-browser use.
- **`saveAuthDocs`**, **`logout`**, **`getCurrentUser`**, **`getValidToken`**, **`refreshToken`**, token expiry helpers.
- **`logout`** calls **`POST /api/auth/logout`** to clear **httpOnly** cookies, then clears **localStorage** and redirects to **`/login`**.

### Route Handlers

- **`app/api/auth/login/route.ts`**: server-side fetch to backend **`/api/auth/login`**, sets cookies **`liftops_access`** and **`liftops_refresh`** (**httpOnly**, **SameSite=Lax**, secure in production).
- **`app/api/auth/logout/route.ts`**: deletes those cookies via **`cookies()`**.

Environment: **`NEXT_PUBLIC_API_URL`** (see `lib/api-config.ts`) must point at the .NET API (e.g. `http://localhost:5295`).

### Login pages

- **`app/login/page.tsx`** — tenant/general sign-in; uses **`LoginForm`** → **`login()`** (BFF + cookies).
- **`app/(auth)/admin/login/page.tsx`** — platform admin entry; same **`login()`**; on success redirects **`PlatformAdmin`** to **`/admin/dashboard`**, others to role-based home via **`getPostLoginRedirectPath`**.

Marketing link from **`/login`** to **`/admin/login`** is present for operators.

## Middleware (`middleware.ts`)

- **Public** paths include **`/login`**, **`/admin/login`**, marketing routes, etc.
- For paths under **`/admin`** (except login): if **`NEXT_PUBLIC_DEV_BYPASS_ADMIN_AUTH !== "true"`**:
  - Requires **`liftops_access`** cookie.
  - Decodes JWT payload (no verification signature in middleware — **first gate only**; API still validates tokens).
  - Rejects expired tokens ( **`exp`** ).
  - Requires **`PlatformAdmin`** in **`role`** claim(s) or long-form role claim URI used by .NET.
- Non–platform users hitting **`/admin`** are redirected to **`/login`**.
- **`NEXT_PUBLIC_DEV_BYPASS_ADMIN_AUTH=true`** skips middleware enforcement for **`/admin/*`** (client **`AuthGuard`** may still apply).

Helpers live in **`lib/jwt-edge.ts`**.

## Client-side guard (`components/auth-guard.tsx`)

- Wraps authenticated app in root layout.
- Uses **`localStorage`** token + **`lib/user.ts`** role helpers.
- **`skipLoginForAdminRoutes()`** in **`lib/navigation.ts`** is **`true`** only when **`NEXT_PUBLIC_DEV_BYPASS_ADMIN_AUTH === "true"`** (default: strict).
- Role-based route map for modules (dashboard, maintenance, etc.); **`/admin`** requires **`isPlatformAdmin()`** unless bypass is on.

## Navigation helpers (`lib/navigation.ts`)

- **`getPostLoginRedirectPath(roles)`** — includes **`PlatformAdmin` → `/admin/dashboard`**.
- **`PUBLIC_PATHS`** / **`isPublicPath`** — includes **`/admin/login`**.

## Platform admin UI

- Under **`app/(admin)/admin/`**: dashboard, companies, plans, subscriptions, users, etc.
- API calls via **`lib/api-platform.ts`** and shared **`apiClient`** (Bearer from **`localStorage`**).

## Styling and UI rules

- Use **ShadCN** primitives from **`components/ui/`** and **Tailwind** utilities.
- Prefer **`cn()`** from **`lib/utils`** for conditional classes.

## Related docs

- `docs/ARCHITECTURE.md` — tenant vs platform JWT behavior.
- `docs/BACKEND_GUIDE.md` — login API contract.
- `docs/AI_CONTEXT.md` — naming and prompting.
