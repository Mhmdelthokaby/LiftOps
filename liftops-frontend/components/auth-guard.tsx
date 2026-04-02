"use client"

import { useEffect, useState } from "react"
import { useRouter, usePathname } from "next/navigation"
import { isAuthenticated, getValidToken } from "@/lib/auth"
import { getPostLoginRedirectPath, isPublicPath } from "@/lib/navigation"
import {
  canViewClients,
  canViewProjects,
  canViewInstallation,
  canViewInventory,
  canManageTechnicians,
  canViewMaintenance,
  canViewEmergency,
  canViewFinance,
  canViewSettings,
  isTechnician,
} from "@/lib/user"
import { Loader2 } from "lucide-react"

interface AuthGuardProps {
  children: React.ReactNode
}

// Route to role checker mapping
const routeAccessMap: Record<string, () => boolean> = {
  "/dashboard": () => !isTechnician(),
  "/technician/visits": () => isTechnician(),
  "/clients": canViewClients,
  "/projects": canViewProjects,
  "/installation": canViewInstallation,
  "/inventory": canViewInventory,
  "/technicians": canManageTechnicians,
  "/maintenance": canViewMaintenance,
  "/emergency": canViewEmergency,
  "/finance": canViewFinance,
  "/settings": canViewSettings,
}

function isRouteOrChild(pathname: string, route: string): boolean {
  return pathname === route || pathname.startsWith(`${route}/`)
}

function canAccessRoute(pathname: string): boolean {
  const exact = routeAccessMap[pathname]
  if (exact) return exact()

  const entries = Object.entries(routeAccessMap)
    .filter(([route]) => route !== "/")
    .sort((a, b) => b[0].length - a[0].length)

  for (const [route, checker] of entries) {
    if (isRouteOrChild(pathname, route)) {
      return checker()
    }
  }

  return true
}

export function AuthGuard({ children }: AuthGuardProps) {
  const router = useRouter()
  const pathname = usePathname()
  const [isChecking, setIsChecking] = useState(true)
  const [isAuthorized, setIsAuthorized] = useState(false)

  useEffect(() => {
    const checkAuth = async () => {
      if (isPublicPath(pathname)) {
        if (pathname === "/login") {
          if (isAuthenticated()) {
            const token = await getValidToken()
            if (token) {
              const userStr = localStorage.getItem("user")
              let roles: string[] = []
              if (userStr) {
                try {
                  roles = JSON.parse(userStr).roles ?? []
                } catch {
                  roles = []
                }
              }
              router.replace(getPostLoginRedirectPath(roles))
              setIsChecking(false)
              return
            }
          }
        }
        setIsAuthorized(true)
        setIsChecking(false)
        return
      }

      if (!isAuthenticated()) {
        router.push("/login")
        return
      }

      const token = await getValidToken()
      if (!token) {
        router.push("/login")
        return
      }

      if (!canAccessRoute(pathname)) {
        const userStr = localStorage.getItem("user")
        if (userStr) {
          try {
            const user = JSON.parse(userStr)
            const roles = user.roles || []
            router.push(getPostLoginRedirectPath(roles))
          } catch {
            router.push("/dashboard")
          }
        } else {
          router.push("/dashboard")
        }
        return
      }

      setIsAuthorized(true)
      setIsChecking(false)
    }

    checkAuth()
  }, [pathname, router])

  if (isChecking) {
    return (
      <div className="flex min-h-screen items-center justify-center">
        <div className="flex flex-col items-center gap-4">
          <Loader2 className="h-8 w-8 animate-spin text-primary" />
          <p className="text-sm text-muted-foreground">Checking authentication...</p>
        </div>
      </div>
    )
  }

  if (!isAuthorized && !isPublicPath(pathname)) {
    return null
  }

  return <>{children}</>
}
