"use client"

import { useEffect, useState } from "react"
import Link from "next/link"
import { usePathname } from "next/navigation"
import { Building2, CreditCard, FileText, LayoutGrid, LogOut, Menu, Users, X } from "lucide-react"
import { cn } from "@/lib/utils"
import { Button } from "@/components/ui/button"
import { logout } from "@/lib/auth-client"
import { getUser, type User } from "@/lib/user"

const NAVIGATION_ITEMS = [
  { id: "dashboard", label: "Dashboard", href: "/admin/dashboard", icon: LayoutGrid },
  { id: "companies", label: "Companies", href: "/admin/companies", icon: Building2 },
  { id: "plans", label: "Plans", href: "/admin/plans", icon: FileText },
  { id: "subscriptions", label: "Subscriptions", href: "/admin/subscriptions", icon: CreditCard },
  { id: "users", label: "Users", href: "/admin/users", icon: Users },
]

export default function AdminRootLayout({ children }: { children: React.ReactNode }) {
  const pathname = usePathname()
  const [sidebarOpen, setSidebarOpen] = useState(true)
  const [user, setUser] = useState<User | null>(null)
  useEffect(() => {
    setUser(getUser())
  }, [])

  return (
    <div className="admin flex h-screen bg-background text-foreground">
      <aside
        className={cn(
          "fixed inset-y-0 left-0 z-50 flex flex-col border-r border-border bg-sidebar transition-all duration-200",
          sidebarOpen ? "w-64" : "w-0 md:w-20"
        )}
      >
        <div className="flex h-16 items-center justify-between border-b border-border px-4">
          <h1 className={cn("text-lg font-bold", sidebarOpen ? "block" : "hidden md:hidden")}>LiftOps</h1>
          <Button variant="ghost" size="icon" onClick={() => setSidebarOpen(!sidebarOpen)} className="md:hidden">
            {sidebarOpen ? <X className="h-4 w-4" /> : <Menu className="h-4 w-4" />}
          </Button>
        </div>

        <nav className="flex-1 space-y-1 overflow-y-auto px-2 py-4">
          {NAVIGATION_ITEMS.map((item) => {
            const Icon = item.icon
            const isActive = pathname === item.href || pathname.startsWith(`${item.href}/`)
            return (
              <Link
                key={item.id}
                href={item.href}
                className={cn(
                  "flex items-center gap-3 rounded-lg px-3 py-2 text-sm font-medium transition-colors",
                  isActive
                    ? "bg-primary text-primary-foreground"
                    : "text-muted-foreground hover:bg-accent hover:text-accent-foreground"
                )}
              >
                <Icon className="h-5 w-5 shrink-0" />
                <span className={sidebarOpen ? "block" : "hidden md:hidden"}>{item.label}</span>
              </Link>
            )
          })}
        </nav>

        <div className="border-t border-border p-4 space-y-2">
          <div className={cn("flex items-center gap-2 rounded-lg bg-accent p-2", sidebarOpen ? "" : "justify-center")}>
            <div className="h-8 w-8 shrink-0 rounded-full bg-primary" />
            {sidebarOpen && (
              <div className="min-w-0 flex-1">
                <p className="truncate text-sm font-medium">{user?.name ?? "Platform admin"}</p>
                <p className="truncate text-xs text-muted-foreground">{user?.email ?? ""}</p>
              </div>
            )}
          </div>
          <Button
            type="button"
            variant="outline"
            size="sm"
            className={cn(
              "w-full gap-0",
              sidebarOpen ? "justify-start" : "justify-center md:px-2"
            )}
            onClick={() => logout("/admin/login")}
            title="Log out"
          >
            <LogOut className="h-4 w-4 shrink-0" />
            {sidebarOpen ? <span className="ml-2">Log out</span> : null}
          </Button>
        </div>
      </aside>

      <main
        className={cn("flex-1 overflow-auto transition-all duration-200", sidebarOpen ? "md:ml-64" : "md:ml-20")}
      >
        {children}
      </main>
    </div>
  )
}
