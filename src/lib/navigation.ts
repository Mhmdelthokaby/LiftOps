/**
 * Default route after successful login (and when redirecting authenticated users away from /login).
 */
export function getPostLoginRedirectPath(roles: string[]): string {
  if (roles.includes("PlatformAdmin") || roles.includes("SUPER_ADMIN")) return "/admin/dashboard"
  if (roles.includes("Technician")) return "/technician/visits"
  if (roles.includes("Manager")) return "/dashboard"
  if (roles.includes("InstallationAdmin")) return "/installation"
  if (roles.includes("MaintenanceAdmin")) return "/maintenance?view=projects"
  if (roles.includes("InventoryAdmin")) return "/inventory"
  if (roles.includes("FinanceAdmin")) return "/finance"
  if (roles.includes("FaultsAdmin")) return "/emergency"
  return "/dashboard"
}

/** Paths that do not require authentication (public marketing + login). */
export const PUBLIC_PATHS = new Set([
  "/",
  "/home",
  "/about",
  "/pricing",
  "/contact",
  "/login",
  "/admin/login",
])

export function isPublicPath(pathname: string): boolean {
  return PUBLIC_PATHS.has(pathname)
}

/**
 * When `true`, `/admin/*` skips client AuthGuard (middleware still enforces PlatformAdmin via cookie when possible).
 * Set `NEXT_PUBLIC_DEV_BYPASS_ADMIN_AUTH=true` for UI-only admin exploration without signing in.
 */
export function skipLoginForAdminRoutes(): boolean {
  return typeof process !== "undefined" && process.env.NEXT_PUBLIC_DEV_BYPASS_ADMIN_AUTH === "true"
}
