import { NextResponse } from "next/server"
import type { NextRequest } from "next/server"
import { isJwtExpired, parseJwtPayloadJson, rolesFromJwtPayload } from "@/lib/jwt-edge"

const publicRoutes = new Set([
  "/",
  "/home",
  "/about",
  "/pricing",
  "/contact",
  "/login",
  "/admin/login",
])

function isPublicPath(pathname: string): boolean {
  if (publicRoutes.has(pathname)) return true
  if (pathname.startsWith("/admin/login")) return true
  return false
}

export function middleware(request: NextRequest) {
  const { pathname } = request.nextUrl

  if (isPublicPath(pathname)) {
    return NextResponse.next()
  }

  const bypassAdminGate = process.env.NEXT_PUBLIC_DEV_BYPASS_ADMIN_AUTH === "true"

  if (pathname.startsWith("/admin") && !bypassAdminGate) {
    const token = request.cookies.get("liftops_access")?.value
    if (!token) {
      const url = request.nextUrl.clone()
      url.pathname = "/admin/login"
      url.search = ""
      return NextResponse.redirect(url)
    }

    const payload = parseJwtPayloadJson(token)
    if (!payload || isJwtExpired(payload)) {
      const url = request.nextUrl.clone()
      url.pathname = "/admin/login"
      url.search = ""
      return NextResponse.redirect(url)
    }

    const roles = rolesFromJwtPayload(payload)
    if (!roles.includes("PlatformAdmin")) {
      const url = request.nextUrl.clone()
      url.pathname = "/login"
      url.search = ""
      return NextResponse.redirect(url)
    }
  }

  return NextResponse.next()
}

export const config = {
  matcher: [
    "/((?!api|_next/static|_next/image|favicon.ico|.*\\.(?:svg|png|jpg|jpeg|gif|webp)$).*)",
  ],
}
