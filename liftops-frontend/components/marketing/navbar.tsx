"use client"

import Link from "next/link"
import { Button } from "@/components/ui/button"
import { usePathname } from "next/navigation"

export function MarketingNavbar() {
  const pathname = usePathname()

  const isHomeActive = pathname === "/" || pathname === "/home"

  const isActive = (path: string) => pathname === path

  return (
    <nav className="sticky top-0 z-50 w-full border-b border-border/50 bg-background/98 backdrop-blur supports-[backdrop-filter]:bg-background/95">
      <div className="mx-auto flex max-w-7xl items-center justify-between px-6 py-3.5 sm:px-8 lg:px-12">
        <Link href="/" className="flex flex-shrink-0 items-center gap-2.5">
          <div className="flex h-9 w-9 items-center justify-center rounded-lg bg-primary">
            <span className="text-base font-bold text-primary-foreground">L</span>
          </div>
          <span className="hidden text-lg font-semibold text-foreground sm:inline">LiftOps</span>
        </Link>

        <div className="hidden items-center gap-10 md:flex">
          <Link
            href="/"
            className={`text-sm font-medium transition-colors ${
              isHomeActive ? "text-primary" : "text-foreground/60 hover:text-foreground/90"
            }`}
          >
            Home
          </Link>
          <Link
            href="/about"
            className={`text-sm font-medium transition-colors ${
              isActive("/about") ? "text-primary" : "text-foreground/60 hover:text-foreground/90"
            }`}
          >
            About
          </Link>
          <Link
            href="/pricing"
            className={`text-sm font-medium transition-colors ${
              isActive("/pricing") ? "text-primary" : "text-foreground/60 hover:text-foreground/90"
            }`}
          >
            Pricing
          </Link>
          <Link
            href="/contact"
            className={`text-sm font-medium transition-colors ${
              isActive("/contact") ? "text-primary" : "text-foreground/60 hover:text-foreground/90"
            }`}
          >
            Contact
          </Link>
        </div>

        <div className="flex items-center gap-3">
          <Button asChild variant="ghost" className="text-sm font-medium text-foreground/60 hover:text-foreground">
            <Link href="/login">Sign In</Link>
          </Button>
          <Button
            asChild
            className="rounded-full bg-accent text-sm font-medium text-accent-foreground transition-all hover:bg-accent/90"
          >
            <Link href="/login">Get Started</Link>
          </Button>
        </div>
      </div>
    </nav>
  )
}
