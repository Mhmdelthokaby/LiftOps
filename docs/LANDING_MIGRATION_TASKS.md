# Landing Site Migration — COMPLETED

This migration was completed when the three projects (backend `lifops-next`, frontend `liftops-frontend`, admin `liftops-admin`) were merged into a single Next.js project.

## What Was Done

- Marketing pages from `home/` → `src/app/(marketing)/`
- Dashboard moved from `/` → `/dashboard`
- Landing theme scoped via `.marketing` CSS class
- All public assets (JPEGs, icons) copied to `src/public/`
- AuthGuard extended for public marketing routes
- Route group `(marketing)` with navbar/footer layout

## Current State

- Marketing routes: `/`, `/home`, `/about`, `/pricing`, `/contact`
- All landing pages work unauthenticated
- Dashboard at `/dashboard` requires auth
- The old `home/` directory has been deleted

This file is kept for historical reference.
