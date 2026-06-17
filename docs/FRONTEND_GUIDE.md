# Frontend Guide

## Stack

- **Next.js** 15.x (App Router)
- **React** 19.x, **TypeScript** 5.x
- **Tailwind CSS** 4.x, **ShadCN/Radix** UI components
- **react-hook-form** + **zod** for forms
- **lucide-react** icons
- **recharts** for charts

## Directory Structure

| Path | Role |
|------|------|
| `src/app/(marketing)/` | Landing pages (/, /about, /pricing, /contact) |
| `src/app/(auth)/` | Login pages (/login, /admin/login) |
| `src/app/(admin)/admin/` | Platform admin dashboard |
| `src/app/dashboard/` | Main app dashboard |
| `src/app/clients/` | Client management pages |
| `src/app/projects/` | Project pages |
| `src/app/technicians/` | Technician management |
| `src/app/maintenance/` | Maintenance pages |
| `src/app/installation/` | Installation pages |
| `src/app/emergency/` | Emergency pages |
| `src/app/inventory/` | Inventory pages |
| `src/app/inspection/` | Inspection pages |
| `src/app/finance/` | Finance pages |
| `src/app/settings/` | Settings pages |
| `src/components/` | Reusable UI components |
| `src/components/ui/` | ShadCN primitives |
| `src/hooks/` | Custom React hooks |
| `src/lib/` | API client, auth helpers, utils |
| `src/types/` | Shared TypeScript types |

## Authentication

### Client-Side Auth (`src/lib/auth-client.ts`)

- **`login(data)`** — POST to `/api/auth/login`, stores tokens in localStorage.
- **`logout()`** — calls `/api/auth/logout`, clears localStorage, redirects to `/login`.
- **`getCurrentUser()`** — reads user from localStorage.
- **`getValidToken()`** — returns valid token or refreshes if expired.
- **`refreshToken()`** — POST to `/api/Admin/refresh-token`.

### API Client (`src/lib/api-client.ts`)

- Centralized fetch wrapper with automatic auth headers.
- Handles token refresh on 401.
- URL mapping layer rewrites .NET-era paths to current routes.

### Auth Guard (`src/components/auth-guard.tsx`)

- Wraps authenticated routes in root layout.
- Uses localStorage token + role helpers.
- Public paths (/, /about, /login, etc.) bypass auth check.

### Edge Middleware (`src/middleware.ts`)

- Protects `/admin/*` routes (except login).
- Decodes JWT from httpOnly cookie.
- Requires `SUPER_ADMIN` role for admin pages.
- Automatically refreshes expired API tokens via refresh cookie.

## Routing

Route groups organize layouts without affecting URL:
- `(marketing)` — landing pages with navbar/footer
- `(auth)` — login pages centered layout
- `(admin)` — admin sidebar layout

## Styling

- ShadCN components from `src/components/ui/`.
- Tailwind CSS v4 for all styling.
- `cn()` utility from `src/lib/utils/` for conditional classes.
- Four themes: light, dark, sunset, frost (via `next-themes`).

## Key Files

| File | Purpose |
|------|---------|
| `src/lib/auth-client.ts` | Auth helpers (login, logout, token management) |
| `src/lib/api-client.ts` | HTTP client with auth + URL mapping |
| `src/lib/api-config.ts` | API base URL config |
| `src/lib/api.ts` | All API function calls |
| `src/lib/user.ts` | Role helpers (isAdmin, isTechnician, etc.) |
| `src/lib/navigation.ts` | Route guards, redirect helpers |
| `src/lib/jwt-edge.ts` | JWT decode for edge middleware |
| `src/middleware.ts` | Page + API auth guard |
| `src/components/auth-guard.tsx` | Client-side route protection |
