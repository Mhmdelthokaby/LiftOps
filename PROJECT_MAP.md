# LiftOps — Project Map

Comprehensive file map. All paths are relative to the project root.

## Configuration

| File | Purpose |
|------|---------|
| `package.json` | Dependencies, scripts |
| `tsconfig.json` | TypeScript config, path alias `@/*` |
| `next.config.ts` | Next.js config (images, headers, server deps) |
| `postcss.config.mjs` | PostCSS + Tailwind config |
| `components.json` | ShadCN components config |
| `eslint.config.mjs` | ESLint config |
| `.env.example` | Environment variable template |

## Prisma (Database)

| File | Purpose |
|------|---------|
| `prisma/schema.prisma` | All models: Company, AppUser, Subscription, Project, Elevator, Stage, Customer, Technician, Maintenance*, Ticket, Inventory*, etc. |
| `prisma/seed.ts` | Seed script: platform company, admin user, subscription plan |

## Public Assets

| File | Purpose |
|------|---------|
| `public/icon.svg` | App favicon |
| `public/*.jpg` | Landing page illustrations (hero, about, features, etc.) |
| `public/*.png` | Logos, icons, user placeholders |
| `public/*.svg` | Logos, placeholders |

## Source (`src/`)

### Root Level

| File | Purpose |
|------|---------|
| `src/middleware.ts` | Edge middleware: API token refresh + page auth guard for `/admin/*` |

### Pages (`src/app/`)

#### Route Groups

| Path | Purpose |
|------|---------|
| `(marketing)/` | Landing pages (/, /home, /about, /pricing, /contact) |
| `(auth)/` | Auth pages (/login, /admin/login) |
| `(admin)/admin/` | Platform admin pages (dashboard, companies, plans, subscriptions, users) |

#### Direct Routes

| Path | Purpose |
|------|---------|
| `dashboard/page.tsx` | Main app dashboard |
| `clients/page.tsx` | Client list |
| `clients/[id]/page.tsx` | Client detail |
| `clients/[id]/edit/page.tsx` | Edit client |
| `projects/page.tsx` | Project list |
| `projects/new/page.tsx` | New project |
| `projects/[id]/page.tsx` | Project detail |
| `technicians/page.tsx` | Technician list |
| `technicians/new/page.tsx` | New technician |
| `technicians/[id]/edit/page.tsx` | Edit technician |
| `maintenance/page.tsx` | Maintenance overview |
| `maintenance/projects/page.tsx` | Maintenance projects |
| `maintenance/projects/new/page.tsx` | New maintenance project |
| `maintenance/projects/[id]/page.tsx` | Maintenance project detail |
| `maintenance/elevators/page.tsx` | Elevator list |
| `maintenance/assign-visits/page.tsx` | Assign maintenance visits |
| `installation/page.tsx` | Installation overview |
| `emergency/page.tsx` | Emergency tickets |
| `inventory/page.tsx` | Inventory management |
| `inspection/new/page.tsx` | New inspection request |
| `finance/page.tsx` | Finance dashboard |
| `settings/page.tsx` | Company settings |
| `technician/visits/page.tsx` | Technician visit list |
| `pdf-test/page.tsx` | PDF generation test |
| `globals.css` | Tailwind v4 + theme variables |
| `layout.tsx` | Root layout (ThemeProvider, AuthGuard, Toaster) |

### API Routes (`src/app/api/`)

#### Auth

| Route | Method | Handler |
|-------|--------|---------|
| `auth/login/route.ts` | POST | Login with email/password |
| `auth/register/route.ts` | POST | Register new user |
| `auth/refresh/route.ts` | POST | Refresh access token |
| `auth/logout/route.ts` | POST | Clear cookies |
| `auth/me/route.ts` | GET | Current user profile |
| `Admin/refresh-token/route.ts` | POST | Alias for frontend .NET-era refresh |

#### Companies

| Route | Methods |
|-------|---------|
| `companies/route.ts` | GET, POST |
| `companies/[id]/route.ts` | GET, PUT |
| `companies/profile/route.ts` | GET |

#### Dashboard

| Route | Methods | Purpose |
|-------|---------|---------|
| `dashboard/route.ts` | GET | KPI metrics |

#### Installation

| Route | Methods |
|-------|---------|
| `installation/customers/route.ts` | GET, POST |
| `installation/customers/[id]/route.ts` | GET, PUT, DELETE |
| `installation/elevators/route.ts` | GET, POST |
| `installation/elevators/[id]/route.ts` | GET, PUT, DELETE |
| `installation/stages/route.ts` | GET, POST |
| `installation/stages/[id]/route.ts` | GET, PUT, DELETE |
| `installation/projects/route.ts` | GET, POST |
| `installation/projects/[id]/route.ts` | GET, PUT |
| `installation/projects/[id]/approve-inspection/route.ts` | POST |
| `installation/projects/[id]/reject/route.ts` | POST |
| `installation/technicians/route.ts` | GET, POST |
| `installation/technicians/[id]/route.ts` | GET, PUT, DELETE |
| `installation/inspections/route.ts` | GET, POST |
| `installation/offers/route.ts` | GET, POST |
| `installation/offers/[id]/approve/route.ts` | POST |
| `installation/offers/[id]/convert-to-project/route.ts` | POST |
| `installation/quotations/route.ts` | GET, POST |
| `installation/quotations/[id]/approve/route.ts` | POST |
| `installation/quotations/[id]/reject/route.ts` | POST |

#### Maintenance

| Route | Methods |
|-------|---------|
| `maintenance/contracts/route.ts` | GET, POST |
| `maintenance/contracts/[id]/route.ts` | GET, PUT |
| `maintenance/visits/route.ts` | GET, POST |
| `maintenance/visits/[id]/route.ts` | GET, PUT |
| `maintenance/checklists/route.ts` | GET, POST |
| `maintenance/statistics/route.ts` | GET |

#### Tickets / Emergency

| Route | Methods |
|-------|---------|
| `tickets/route.ts` | GET, POST |
| `tickets/[id]/route.ts` | GET, PUT |
| `tickets/[id]/assign/route.ts` | POST |
| `tickets/[id]/resolve/route.ts` | POST |
| `emergency/route.ts` | GET, POST |
| `emergency/open/route.ts` | GET |
| `emergency/[id]/route.ts` | GET, PUT |
| `emergency/[id]/assign/route.ts` | POST |
| `emergency/[id]/resolve/route.ts` | POST |

#### Inventory

| Route | Methods |
|-------|---------|
| `inventory/items/route.ts` | GET, POST |
| `inventory/items/[id]/route.ts` | GET, PUT, DELETE |
| `inventory/categories/route.ts` | GET, POST |
| `inventory/value/route.ts` | GET |

#### Platform (Super Admin)

| Route | Methods | Purpose |
|-------|---------|---------|
| `platform/dashboard/route.ts` | GET | Platform-wide stats |
| `platform/users/route.ts` | GET, POST | |
| `platform/users/[id]/route.ts` | GET, PUT, DELETE | |

#### Subscription

| Route | Methods |
|-------|---------|
| `subscription/route.ts` | GET, PUT, POST |
| `subscription/plans/route.ts` | GET, POST |
| `subscription/plans/[id]/route.ts` | GET, PUT, DELETE |
| `subscription/change-plan/route.ts` | POST |
| `subscription/webhook/route.ts` | POST |

#### Other

| Route | Methods | Purpose |
|-------|---------|---------|
| `categories/route.ts` | GET, POST | General categories |
| `categories/[id]/route.ts` | GET, PUT, DELETE | Category CRUD |
| `health/route.ts` | GET | Health check |

### Components (`src/components/`)

| Path | Purpose |
|------|---------|
| `auth-guard.tsx` | Client-side route protection |
| `auth/login-form.tsx` | Login form with react-hook-form + zod |
| `app-sidebar.tsx` | Main sidebar navigation |
| `app-header.tsx` | Top navigation bar |
| `theme-provider.tsx` | next-themes provider for 4 themes |
| `impersonation-banner-host.tsx` | Super admin impersonation indicator |

#### Feature Components

| Path | Purpose |
|------|---------|
| `dashboard/*` | KPI cards, revenue chart, activity feed, project progress |
| `projects/*` | Project table, project detail |
| `installation/*` | Pipeline, offer list, inspection list, stage/dialog forms |
| `maintenance/*` | Calendar, detail, list, contract/checklist/elevator dialogs |
| `emergency/*` | Ticket list, detail, form, stats |
| `inventory/*` | Overview, table, item/category/request forms |
| `settings/*` | Company profile, admin management, category management |
| `admin/*` | Admin error state, badges, charts, data table, page header, dialogs |
| `admin/companies/*` | Create company form, edit dialog, subscription plan select |
| `marketing/*` | Navbar, logo |

#### UI Components (ShadCN)

| Path | Components |
|------|------------|
| `ui/*` | Accordion, Alert, Avatar, Badge, Button, Calendar, Card, Chart, Checkbox, Command, Dialog, Dropdown, Form, Input, Label, Modal, Pagination, Popover, Select, Sheet, Sidebar, Skeleton, Switch, Table, Tabs, Toast, Toggle, Tooltip, etc. |

### Library (`src/lib/`)

#### Auth

| File | Purpose |
|------|---------|
| `auth-client.ts` | Client auth (login, logout, token mgmt) |
| `auth/index.ts` | Auth barrel export |
| `auth/jwt.ts` | Server-side JWT sign/verify (jose) |
| `auth/middleware.ts` | API token refresh middleware helper |
| `auth/password.ts` | bcryptjs hash/compare |
| `auth/schemas.ts` | Auth zod validation schemas |
| `auth/session.ts` | Session utilities |

#### Services (Business Logic)

| File | Purpose |
|------|---------|
| `services/auth.service.ts` | Auth business logic |
| `services/company.service.ts` | Company CRUD |
| `services/subscription.service.ts` | Subscription/plan management |
| `services/dashboard/dashboard.service.ts` | Company-level dashboard KPI queries |
| `services/dashboard/platform.service.ts` | Platform-level dashboard (total companies, revenue, etc.) |
| `services/installation/` | Customer, elevator, stage, project, offer, inspection, technician |
| `services/maintenance/` | Contract, visit, checklist |
| `services/inventory/` | Item, category |
| `services/tickets/` | Fault ticket workflows |
| `services/emergency/` | Emergency ticket workflows |

#### API / Client

| File | Purpose |
|------|---------|
| `api-client.ts` | HTTP client with auth + URL mapping |
| `api-config.ts` | API base URL config |
| `api.ts` | All API endpoint function calls |
| `api-platform.ts` | Platform admin API functions |
| `user.ts` | Role helpers (isPlatformAdmin, isTechnician, etc.) — `isPlatformAdmin` also matches `SUPER_ADMIN` |
| `navigation.ts` | Route guards, redirect logic |
| `jwt-edge.ts` | JWT decode for edge middleware |
| `impersonation.ts` | Super admin impersonation helpers |
| `company-tier.ts` | Company tier/plan helpers |

#### Validation (Zod)

| File | Purpose |
|------|---------|
| `validators/index.ts` | Barrel export |
| `validators/company.ts` | Company validation |
| `validators/subscription.ts` | Subscription validation |
| `validators/installation/*` | Customer, elevator, stage, project, technician |
| `validators/maintenance/*` | Contract, visit, checklist |
| `validators/inventory/*` | Item, category |
| `validators/tickets/*` | Ticket validation |
| `validators/emergency/*` | Emergency ticket validation |

#### Utilities

| File | Purpose |
|------|---------|
| `utils/index.ts` | cn(), formatDate(), format helpers |
| `utils/logger.ts` | Server-side logging |
| `utils/pagination.ts` | Pagination helpers |
| `response/index.ts` | Response envelope (success, succeeded, data, error) |
| `errors/index.ts` | Error classes |
| `db/client.ts` | Prisma client singleton |
| `power-pass.ts` | Power pass utility |
| `pdf-utils.ts` | PDF generation helpers |

### Types

| File | Purpose |
|------|---------|
| `types/index.ts` | Shared type definitions |
| `types/admin.ts` | Admin portal types |

### Hooks

| File | Purpose |
|------|---------|
| `hooks/use-admin-data.ts` | Admin data fetching |
| `hooks/use-mobile.ts` | Mobile detection |
| `hooks/use-toast.ts` | Toast notifications |

### Config

| File | Purpose |
|------|---------|
| `config/app.ts` | App config (cookie names, JWT expiry, etc.) |
| `config/env.ts` | Environment variable access |
