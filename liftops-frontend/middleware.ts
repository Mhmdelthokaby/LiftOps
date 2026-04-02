import { NextResponse } from 'next/server'
import type { NextRequest } from 'next/server'

// Documented public paths (client-side AuthGuard enforces; localStorage is not available here)
const publicRoutes = ["/", "/home", "/about", "/pricing", "/contact", "/login"]

export function middleware(request: NextRequest) {
    const { pathname } = request.nextUrl
    
    // Allow public routes
    if (publicRoutes.includes(pathname)) {
        return NextResponse.next()
    }
    
    // For protected routes, we'll let the client-side handle auth checking
    // since we need to check localStorage which is only available on client
    // The client-side auth guard will handle redirects
    return NextResponse.next()
}

export const config = {
    matcher: [
        /*
         * Match all request paths except for the ones starting with:
         * - api (API routes)
         * - _next/static (static files)
         * - _next/image (image optimization files)
         * - favicon.ico (favicon file)
         * - public files (public folder)
         */
        '/((?!api|_next/static|_next/image|favicon.ico|.*\\.(?:svg|png|jpg|jpeg|gif|webp)$).*)',
    ],
}
