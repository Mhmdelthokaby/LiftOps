# LiftOps

Multi-tenant SaaS platform for elevator companies to manage installation projects, maintenance contracts, emergency tickets, inventory, and technician assignments — all in one Next.js monolith.

## Tech Stack

| Layer | Technology |
|-------|------------|
| Framework | Next.js 15 (App Router) |
| Language | TypeScript, React 19 |
| Styling | Tailwind CSS 4 + ShadCN/Radix UI |
| Database | PostgreSQL |
| ORM | Prisma 6 |
| Auth | JWT (jose) + httpOnly cookies + bcryptjs |
| API | Next.js Route Handlers (no separate backend) |
| Charts | Recharts |
| Forms | react-hook-form + zod |

## Features

- **Installation Pipeline** — inspection requests, offers, quotations, project stages, elevators
- **Maintenance** — contracts, visit scheduling, checklists, spare parts usage
- **Emergency Tickets** — fault reporting, technician assignment, resolution tracking
- **Inventory** — item catalog, categories, stock tracking, low-stock alerts
- **Dashboard** — KPI cards, revenue charts, activity feed, project progress
- **Platform Admin** — multi-tenant company management, subscription plans, user administration

## Quick Start

```bash
cp .env.example .env
npm install
npx prisma db push
npx tsx prisma/seed.ts
npm run dev
```

- **App:** http://localhost:3000
- **Admin Console:** http://localhost:3000/admin/login
- **Credentials (after seed):** `admin@lifops.com` / `Admin@123`

## Project Structure

```
├── prisma/                 # Schema + seed
├── public/                 # Static assets (images, icons)
├── src/
│   ├── app/                # Pages + API routes
│   │   ├── api/            # All backend endpoints
│   │   ├── (admin)/admin/  # Platform admin UI
│   │   ├── (auth)/         # Login pages
│   │   ├── (marketing)/    # Landing pages
│   │   ├── dashboard/      # Main app dashboard
│   │   ├── projects/       # Project pages
│   │   ├── clients/        # Client management
│   │   ├── technicians/    # Technician management
│   │   ├── maintenance/    # Maintenance pages
│   │   ├── installation/   # Installation pages
│   │   ├── emergency/      # Emergency pages
│   │   ├── inventory/      # Inventory pages
│   │   └── settings/       # Settings pages
│   ├── components/         # React components (ui/, features, admin, marketing)
│   ├── lib/                # Services, auth, validators, utils
│   ├── config/             # App configuration
│   ├── types/              # TypeScript types
│   └── middleware.ts       # Edge auth guard + API token refresh
├── PROJECT_MAP.md          # Full file-by-file reference
├── docs/                   # Architecture, API, frontend, AI guides
```

## Scripts

| Command | Description |
|---------|-------------|
| `npm run dev` | Start dev server (Turbopack) |
| `npm run build` | Generate Prisma client + build |
| `npm run start` | Start production server |
| `npm run lint` | Run ESLint |
| `npm run type-check` | TypeScript type checking |
| `npm run db:push` | Push Prisma schema to DB |
| `npm run db:seed` | Seed database |
| `npm run db:studio` | Open Prisma Studio |

## Documentation

| Doc | Description |
|-----|-------------|
| `PROJECT_MAP.md` | Complete file-by-file project map |
| `docs/ARCHITECTURE.md` | System architecture & multi-tenant strategy |
| `docs/API_GUIDE.md` | All API endpoints reference |
| `docs/FRONTEND_GUIDE.md` | Pages, components, auth flow |
| `docs/AI_CONTEXT.md` | Quick context for AI assistants |
| `docs/SAAS_TASKS.md` | Transformation backlog |
