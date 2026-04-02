import Link from "next/link"
import Image from "next/image"
import { Button } from "@/components/ui/button"
import { MarketingNavbar } from "@/components/marketing/navbar"
import { ArrowRight, CheckCircle2, Zap, Users, BarChart3 } from "lucide-react"

export default function MarketingHomePage() {
  return (
    <>
      <MarketingNavbar />
      <main className="min-h-screen bg-background">
        <section className="relative overflow-hidden px-6 py-32 sm:px-8 sm:py-40 lg:px-12 lg:py-48">
          <div className="mx-auto max-w-6xl">
            <div className="mb-10 flex justify-center">
              <div className="inline-flex items-center gap-2 rounded-full bg-primary/10 px-4 py-1.5 text-sm font-medium text-primary">
                <span className="relative flex h-2 w-2">
                  <span className="absolute inline-flex h-full w-full animate-ping rounded-full bg-primary opacity-75"></span>
                  <span className="relative inline-flex h-2 w-2 rounded-full bg-primary"></span>
                </span>
                Trusted by 500+ elevator companies
              </div>
            </div>

            <h1 className="mb-8 text-balance text-center text-5xl font-bold leading-tight text-foreground sm:text-6xl lg:text-7xl">
              Streamline Elevator Operations
            </h1>

            <p className="mx-auto mb-12 max-w-3xl text-center text-lg leading-relaxed text-foreground/70 sm:text-xl">
              Complete workflow management for installations, maintenance scheduling, emergency response, and inventory
              tracking. Everything your team needs in one platform.
            </p>

            <div className="mb-16 flex flex-col items-center justify-center gap-4 sm:flex-row">
              <Button
                asChild
                size="lg"
                className="rounded-full bg-primary px-8 text-base font-semibold text-primary-foreground hover:bg-primary/90"
              >
                <Link href="/login">
                  Get Started <ArrowRight className="ml-2 h-4 w-4" />
                </Link>
              </Button>
              <Button
                asChild
                variant="outline"
                size="lg"
                className="rounded-full border-foreground/20 px-8 text-base font-semibold hover:bg-foreground/5"
              >
                <Link href="#features">See Features</Link>
              </Button>
            </div>

            <div className="relative h-96 w-full overflow-hidden rounded-2xl border border-border/50 shadow-xl">
              <Image
                src="/hero-illustration.jpg"
                alt="LiftOps Dashboard"
                fill
                className="object-cover"
                priority
              />
            </div>
          </div>
        </section>

        <section id="features" className="bg-foreground/2 px-6 py-24 sm:px-8 sm:py-32 lg:px-12">
          <div className="mx-auto max-w-6xl">
            <div className="mb-16 text-center">
              <h2 className="mb-4 text-4xl font-bold text-foreground sm:text-5xl">Everything you need</h2>
              <p className="mx-auto max-w-2xl text-lg text-foreground/60">
                Powerful tools designed specifically for elevator operations teams
              </p>
            </div>

            <div className="grid grid-cols-1 gap-6 md:grid-cols-2">
              {[
                {
                  icon: Zap,
                  title: "Installation Tracking",
                  description:
                    "Monitor elevator installation projects from start to completion with real-time progress updates and team collaboration tools.",
                  image: "/installation-vector.jpg",
                },
                {
                  icon: Users,
                  title: "Maintenance Scheduling",
                  description:
                    "Automate maintenance contracts and schedule technician visits across all locations with intelligent conflict detection.",
                  image: "/maintenance-vector.jpg",
                },
                {
                  icon: BarChart3,
                  title: "Emergency Response",
                  description:
                    "Rapid fault logging and emergency dispatch coordination for elevator breakdowns with priority alert routing.",
                  image: "/emergency-vector.jpg",
                },
                {
                  icon: CheckCircle2,
                  title: "Inventory Management",
                  description:
                    "Track parts inventory and manage procurement across your entire operation with automated stock level alerts.",
                  image: "/inventory-vector.jpg",
                },
              ].map((feature, index) => {
                const Icon = feature.icon
                return (
                  <div
                    key={index}
                    className="group overflow-hidden rounded-2xl border border-border/50 bg-card transition-all hover:border-primary/30 hover:shadow-lg hover:shadow-primary/5"
                  >
                    <div className="relative h-48 w-full bg-primary/5">
                      <Image src={feature.image} alt={feature.title} fill className="object-cover" />
                    </div>
                    <div className="p-8">
                      <div className="mb-4 inline-flex h-12 w-12 items-center justify-center rounded-lg bg-primary/10 transition-colors group-hover:bg-primary/15">
                        <Icon className="h-6 w-6 text-primary" />
                      </div>
                      <h3 className="mb-3 text-lg font-semibold text-foreground">{feature.title}</h3>
                      <p className="text-sm leading-relaxed text-foreground/70">{feature.description}</p>
                    </div>
                  </div>
                )
              })}
            </div>
          </div>
        </section>

        <section className="px-6 py-24 sm:px-8 sm:py-32 lg:px-12">
          <div className="mx-auto max-w-6xl">
            <div className="grid grid-cols-2 gap-8 md:grid-cols-4">
              {[
                { stat: "500+", label: "Companies" },
                { stat: "50K+", label: "Technicians" },
                { stat: "99.9%", label: "Uptime" },
                { stat: "24/7", label: "Support" },
              ].map((item, index) => (
                <div key={index} className="text-center">
                  <p className="mb-2 text-3xl font-bold text-primary sm:text-4xl">{item.stat}</p>
                  <p className="text-sm text-foreground/60">{item.label}</p>
                </div>
              ))}
            </div>
          </div>
        </section>

        <section className="px-6 py-24 sm:px-8 sm:py-32 lg:px-12">
          <div className="mx-auto max-w-6xl">
            <div className="mb-16 text-center">
              <h2 className="mb-4 text-4xl font-bold text-foreground sm:text-5xl">Powerful Dashboard</h2>
              <p className="mx-auto max-w-2xl text-lg text-foreground/60">
                Visualize your entire elevator operations in one unified dashboard
              </p>
            </div>
            <div className="relative h-96 w-full overflow-hidden rounded-2xl border border-border/50 shadow-2xl">
              <Image src="/dashboard-vector.jpg" alt="LiftOps Dashboard" fill className="object-cover" />
            </div>
          </div>
        </section>

        <section className="bg-foreground/2 px-6 py-24 sm:px-8 sm:py-32 lg:px-12">
          <div className="mx-auto max-w-4xl">
            <div className="rounded-3xl bg-gradient-to-br from-primary to-primary/80 p-12 text-center shadow-xl sm:p-16">
              <h2 className="mb-4 text-4xl font-bold text-primary-foreground sm:text-5xl">
                Ready to transform your operations?
              </h2>
              <p className="mx-auto mb-10 max-w-2xl text-lg text-primary-foreground/90">
                Join hundreds of elevator companies streamlining their workflows with LiftOps
              </p>
              <Button
                asChild
                size="lg"
                className="rounded-full bg-accent px-8 text-base font-semibold text-accent-foreground hover:bg-accent/90"
              >
                <Link href="/login">
                  Start Your Free Trial <ArrowRight className="ml-2 h-4 w-4" />
                </Link>
              </Button>
            </div>
          </div>
        </section>

        <footer className="border-t border-border/30 bg-background px-6 py-12 sm:px-8 lg:px-12">
          <div className="mx-auto max-w-6xl">
            <div className="mb-8 flex items-center gap-2">
              <div className="flex h-8 w-8 items-center justify-center rounded-lg bg-primary">
                <span className="text-sm font-bold text-primary-foreground">L</span>
              </div>
              <span className="text-lg font-semibold text-foreground">LiftOps</span>
            </div>
            <p className="max-w-2xl text-sm text-foreground/50">
              Platform for elevator companies to streamline installations, maintenance, emergency response, and inventory
              management.
            </p>
            <div className="mt-8 flex flex-col gap-6 border-t border-border/30 pt-8 text-sm text-foreground/60 sm:flex-row">
              <p>&copy; 2024 LiftOps. All rights reserved.</p>
              <div className="flex gap-6">
                <Link href="#" className="transition-colors hover:text-foreground">
                  Privacy
                </Link>
                <Link href="#" className="transition-colors hover:text-foreground">
                  Terms
                </Link>
                <Link href="/contact" className="transition-colors hover:text-foreground">
                  Contact
                </Link>
              </div>
            </div>
          </div>
        </footer>
      </main>
    </>
  )
}
