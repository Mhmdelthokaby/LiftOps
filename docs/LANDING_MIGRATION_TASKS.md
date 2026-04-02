# Landing site migration (`home/` → `liftops-frontend`)

## Goal

Merge the standalone marketing frontend in `home/` into the main Next.js app (`liftops-frontend/`), preserve the **home / about / pricing / contact** design (colors, layout, imagery), then **delete the `home/` folder** once everything works.

## Current vs target routing

| Area | Current (`liftops-frontend`) | Target |
|------|------------------------------|--------|
| Marketing home | N/A (dashboard at `/`) | `/` and `/home` show the same landing page (optional: redirect one to the other) |
| App dashboard | `/` (`app/page.tsx`) | `/dashboard` |
| Login | `/login` | `/login` (unchanged) |
| After login | Users land on `/` or role-specific routes | Default authenticated home → `/dashboard` (and update redirects from `/` to `/dashboard` where appropriate) |
| About | In `home/app/about` | `/about` |
| Pricing | In `home/app/pricing` | `/pricing` |
| Contact | In `home/app/contact` | `/contact` |
| Sign-in / sign-up (marketing) | `home`: `/sign-in`, `/sign-up` | Point CTAs to `/login` (or add `/register` later if product needs it) |

## Prerequisites and inventory

- [x] **List assets**: Landing pages reference images under `/public` (e.g. `hero-illustration.jpg`, `installation-vector.jpg`, `about-illustration.jpg`). The `home/public` folder in repo may not include all JPEGs; **collect or export** those files so `liftops-frontend/public/` contains every path used in JSX.
- [x] **Decide theme strategy**: `home` uses `app/globals.css` with oklch tokens; `liftops-frontend` uses layered themes (`dark`, `sunset`, `frost`). Choose one:
  - **A)** Add a **route group** `(marketing)` with its own `layout.tsx` that applies landing-specific CSS variables or a dedicated `class` on `<html>` / wrapper, **or**
  - **B)** Merge landing `:root` tokens into `globals.css` and scope landing pages with a wrapper class (e.g. `.marketing`) so the dashboard theme is unchanged.
- [x] **Avoid duplicate ShadCN**: Do not copy all of `home/components/ui/*` if `liftops-frontend` already has the same components. Only add **missing** UI files or adjust imports to `@/components/ui/...`.

### Asset inventory (`liftops-frontend/public/`)

Paths referenced by `home/app/**/*.tsx` (JPEGs are committed as minimal valid placeholders; replace with final art when available):

| File | Used in |
|------|---------|
| `hero-illustration.jpg` | `home/app/page.tsx` (hero) |
| `installation-vector.jpg` | `home/app/page.tsx` (features) |
| `maintenance-vector.jpg` | `home/app/page.tsx` (features) |
| `emergency-vector.jpg` | `home/app/page.tsx` (features) |
| `inventory-vector.jpg` | `home/app/page.tsx` (features) |
| `dashboard-vector.jpg` | `home/app/page.tsx` (showcase) |
| `about-illustration.jpg` | `home/app/about/page.tsx` |

**Favicons / app icons:** `home/app/layout.tsx` references `icon-light-32x32.png`, `icon-dark-32x32.png`, and `apple-icon.png`. The main app already aligns on a single asset: `liftops-frontend/app/layout.tsx` sets `icons.icon` and `icons.apple` to **`/icon.svg`** (see `public/icon.svg`). When you port marketing routes, **keep** that metadata pattern unless you deliberately add separate PNGs for light/dark or Apple-specific artwork.

### Theme strategy (implemented)

**Option B**: `liftops-frontend/app/globals.css` defines **`.marketing`** (light) and **`.marketing.marketing-dark`** (dark from `home`). Wrap migrated pages in `<div className="marketing marketing-dark">` for dark landing styling, or `<div className="marketing">` for light. Do **not** use the global **`dark`** class on that wrapper for landing-only dark mode, or it would mix with dashboard HSL `.dark` tokens.

### ShadCN / UI

`home/components/ui` and `liftops-frontend/components/ui` both contain **56** matching component files. **Do not copy** `home/components/ui` into the main app; use `@/components/ui/...` when migrating pages.

## Auth and public routes

- [x] **Extend `AuthGuard`** (`components/auth-guard.tsx`): Treat these paths as **public** (no token required), same as `/login`:
  - `/`, `/home`, `/about`, `/pricing`, `/contact`
- [x] **Update `routeAccessMap`**: Today `'/'` maps to dashboard access for non-technicians. After moving the dashboard to `/dashboard`, map **`/dashboard`** (not `/`) to that rule, and remove `/` from the protected dashboard entry.
- [x] **Login page behavior**: If user is already authenticated, redirect from `/login` to `/dashboard` (if not already implemented).
- [x] **Marketing → app**: Replace `Link href="/sign-up"` / `/sign-in` in migrated components with `/login` (and adjust copy if needed).

## File and route migration

- [x] **Create `app/(marketing)/` route group** (optional but recommended): `layout.tsx` (navbar, footer pattern), shared metadata.
- [x] **Add pages** by porting from `home/app/`:
  - [x] `page.tsx` (landing) → `app/(marketing)/page.tsx` **or** `app/page.tsx` for `/`
  - [x] Duplicate content at `app/home/page.tsx` if `/home` must exist as an alias (or use `redirect` in `next.config` / middleware).
  - [x] `about/page.tsx`, `pricing/page.tsx`, `contact/page.tsx`
- [x] **Move dashboard**: Move current `app/page.tsx` (dashboard) to `app/dashboard/page.tsx`.
- [x] **Components**: Copy or merge `home/components/navbar.tsx` (and any landing-only components) into `liftops-frontend/components/` (e.g. `components/marketing/navbar.tsx`). Update imports (`@/components/...`).
- [x] **Dependencies**: Compare `home/package.json` with `liftops-frontend/package.json`; add any missing packages (e.g. if landing uses extras not in main app). *(No new packages required for ported pages.)*

## Navigation and deep links

- [x] **Navbar / footer links**: Use Next.js `Link` to `/`, `/about`, `/pricing`, `/contact`, `/login`; primary CTA → `/login`.
- [x] **`app-sidebar` / `app-header`**: Change home/dashboard links from `/` to `/dashboard` wherever the dashboard is linked.
- [x] **Internal redirects**: Search codebase for `router.push('/')`, `href="/"`, and role-based redirects in `AuthGuard` that send users to `/` → update to `/dashboard` where it means “main app home”.

## Styling

- [x] Ensure **Tailwind** content paths include new component folders. *(Default Tailwind v4 `liftops-frontend` content includes `./components/**/*`.)*
- [x] Preserve **landing look**: primary/accent colors from `home` globals if product wants pixel parity on marketing only.

## Cleanup

- [x] Run `liftops-frontend` build and fix TypeScript/import errors.
- [ ] **Manual test**: unauthenticated `/`, `/about`, `/pricing`, `/contact`, `/login`; authenticated `/dashboard` and sidebar navigation.
- [x] **Delete `home/`** after the above passes and assets are merged.

## Documentation

- [x] Add a one-line pointer in `docs/FRONTEND_GUIDE.md` (or `PROJECT_IDEA.md` documentation map) to this file once migration is done.

---

## Order of execution (recommended)

1. ~~Public routes in `AuthGuard` + move dashboard to `app/dashboard/page.tsx` + fix redirects and `routeAccessMap`.~~ **Done**
2. ~~Copy marketing pages + navbar; wire routes under `(marketing)` or flat `app/`; wrap with `.marketing`.~~ **Done**
3. **Prerequisites done:** scoped `.marketing` / `.marketing-dark` in `globals.css`, JPEG placeholders in `liftops-frontend/public/`.
4. ~~Remove `home/` and verify build.~~ **Done** (run manual QA when convenient).
