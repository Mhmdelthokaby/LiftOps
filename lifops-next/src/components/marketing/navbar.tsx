"use client"

import Link from "next/link"
import { Button } from "@/components/ui/button"
import { usePathname } from "next/navigation"
import { Sun, Moon } from "lucide-react"
import { useTheme } from "next-themes"
import { useEffect, useState } from "react"
import { Logo } from "./logo"

export function MarketingNavbar() {
  const pathname = usePathname()
  const { theme, setTheme } = useTheme()
  const [mounted, setMounted] = useState(false)

  useEffect(() => {
    setMounted(true)
  }, [])

  const isHomeActive = pathname === "/" || pathname === "/home"

  const isActive = (path: string) => pathname === path

  return (
    <nav className="sticky top-0 z-50 w-full border-b border-border/50 bg-background/98 backdrop-blur supports-[backdrop-filter]:bg-background/95">
      <div className="mx-auto flex max-w-7xl items-center justify-between px-6 py-3.5 sm:px-8 lg:px-12">
        <Link href="/" className="flex flex-shrink-0 items-center gap-2.5 hover:opacity-90 transition-opacity">
          <Logo size={28} />
          <span className="hidden text-xl font-black sm:inline tracking-tight">
            <span className="text-foreground">Lift</span>
            <span className="text-accent">Ops</span>
          </span>
        </Link>

        <div className="hidden items-center gap-10 md:flex">
          <Link
            href="/"
            className={`text-sm font-medium transition-colors ${
              isHomeActive ? "text-primary font-semibold" : "text-foreground/60 hover:text-foreground/90"
            }`}
          >
            Home
          </Link>
          <Link
            href="/about"
            className={`text-sm font-medium transition-colors ${
              isActive("/about") ? "text-primary font-semibold" : "text-foreground/60 hover:text-foreground/90"
            }`}
          >
            About
          </Link>
          <Link
            href="/pricing"
            className={`text-sm font-medium transition-colors ${
              isActive("/pricing") ? "text-primary font-semibold" : "text-foreground/60 hover:text-foreground/90"
            }`}
          >
            Pricing
          </Link>
          <Link
            href="/contact"
            className={`text-sm font-medium transition-colors ${
              isActive("/contact") ? "text-primary font-semibold" : "text-foreground/60 hover:text-foreground/90"
            }`}
          >
            Contact
          </Link>
        </div>

        <div className="flex items-center gap-3">
          {mounted && (
            <Button
              variant="ghost"
              size="icon"
              onClick={() => setTheme(theme === "light" ? "dark" : "light")}
              className="relative rounded-full w-9 h-9 text-foreground/60 hover:text-foreground hover:bg-foreground/5 cursor-pointer transition-all duration-200"
            >
              <Sun className={`h-[1.2rem] w-[1.2rem] transition-all duration-300 ${theme === "light" ? "rotate-0 scale-100 opacity-100" : "rotate-90 scale-0 opacity-0 absolute"}`} />
              <Moon className={`h-[1.2rem] w-[1.2rem] transition-all duration-300 ${theme !== "light" ? "rotate-0 scale-100 opacity-100" : "-rotate-90 scale-0 opacity-0 absolute"}`} />
              <span className="sr-only">Toggle theme</span>
            </Button>
          )}

          <Button asChild variant="ghost" className="text-sm font-medium text-foreground/60 hover:text-foreground">
            <Link href="/login">Sign In</Link>
          </Button>
          <Button
            asChild
            className="rounded-full bg-accent text-sm font-medium text-accent-foreground transition-all hover:bg-accent/90 shadow-md shadow-accent/10 hover:shadow-accent/20"
          >
            <Link href="/login">Get Started</Link>
          </Button>
        </div>
      </div>
    </nav>
  )
}
