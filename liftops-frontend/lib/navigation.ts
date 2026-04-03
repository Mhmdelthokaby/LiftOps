/**
 * Default route after successful login (and when redirecting authenticated users away from /login).
 */
export function getPostLoginRedirectPath(roles: string[]): string {
  if (roles.includes("PlatformAdmin")) return "/admin/dashboard"
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
])

export function isPublicPath(pathname: string): boolean {
  return PUBLIC_PATHS.has(pathname)
}

/**
 * Temporary: `/admin/*` works without signing in (UI + navigation only; API calls still need a token).
 * Set to `false` or use `NEXT_PUBLIC_DEV_BYPASS_ADMIN_AUTH=false` to require login + PlatformAdmin again.
 */
export function skipLoginForAdminRoutes(): boolean {
  if (typeof process !== "undefined" && process.env.NEXT_PUBLIC_DEV_BYPASS_ADMIN_AUTH === "false") {
    return false
  }
  if (typeof process !== "undefined" && process.env.NEXT_PUBLIC_DEV_BYPASS_ADMIN_AUTH === "true") {
    return true
  }
  return true
}
