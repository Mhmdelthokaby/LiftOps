import { NextResponse } from "next/server";
import type { NextRequest } from "next/server";
import { tryRefreshAccessToken } from "@/lib/auth/middleware";

const apiPublicPaths = [
  "/api/auth/login",
  "/api/auth/admin/login",
  "/api/auth/admin/refresh",
  "/api/auth/register",
  "/api/auth/refresh",
  "/api/health",
];

const publicPagePaths = new Set([
  "/",
  "/home",
  "/about",
  "/pricing",
  "/contact",
  "/login",
  "/admin/login",
]);

function isPublicPagePath(pathname: string): boolean {
  if (publicPagePaths.has(pathname)) return true;
  if (pathname.startsWith("/admin/login")) return true;
  return false;
}

export async function middleware(request: NextRequest) {
  const { pathname } = request.nextUrl;

  if (pathname.startsWith("/api")) {
    if (apiPublicPaths.some((p) => pathname.startsWith(p))) {
      return NextResponse.next();
    }
    const refreshResult = await tryRefreshAccessToken(request);
    if (refreshResult) {
      return refreshResult.response;
    }
    return NextResponse.next();
  }

  if (isPublicPagePath(pathname)) {
    return NextResponse.next();
  }

  const bypassAdminGate = process.env.NEXT_PUBLIC_DEV_BYPASS_ADMIN_AUTH === "true";

  if (pathname.startsWith("/admin") && !bypassAdminGate) {
    const token = request.cookies.get("liftops_access")?.value;
    if (!token) {
      const url = request.nextUrl.clone();
      url.pathname = "/admin/login";
      url.search = "";
      return NextResponse.redirect(url);
    }

    const parts = token.split(".");
    if (parts.length !== 3) {
      const url = request.nextUrl.clone();
      url.pathname = "/admin/login";
      url.search = "";
      return NextResponse.redirect(url);
    }

    try {
      const padded = parts[1].padEnd(parts[1].length + ((4 - (parts[1].length % 4)) % 4), "=");
      const b64 = padded.replace(/-/g, "+").replace(/_/g, "/");
      const payload = JSON.parse(atob(b64));

      if (payload.exp && Date.now() >= payload.exp * 1000) {
        const url = request.nextUrl.clone();
        url.pathname = "/admin/login";
        url.search = "";
        return NextResponse.redirect(url);
      }

      const type = payload.type;
      const role = payload.role;
      if (type !== "admin" || !role || !["SUPER_ADMIN", "ADMIN"].includes(role)) {
        const url = request.nextUrl.clone();
        url.pathname = "/login";
        url.search = "";
        return NextResponse.redirect(url);
      }
    } catch {
      const url = request.nextUrl.clone();
      url.pathname = "/admin/login";
      url.search = "";
      return NextResponse.redirect(url);
    }
  }

  return NextResponse.next();
}

export const config = {
  matcher: [
    "/((?!_next/static|_next/image|favicon.ico|.*\\.(?:svg|png|jpg|jpeg|gif|webp)$).*)",
  ],
};
