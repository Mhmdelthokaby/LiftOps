import Link from "next/link"
import Image from "next/image"
import { Button } from "@/components/ui/button"
import { MarketingNavbar } from "@/components/marketing/navbar"
import { ArrowRight, CheckCircle2, Zap, Users, BarChart3 } from "lucide-react"
import { Logo } from "@/components/marketing/logo"

export default function MarketingHomePage() {
  return (
    <>
      <MarketingNavbar />
      <main className="min-h-screen bg-background relative overflow-hidden">
        {/* Futuristic Industrial Background Glow */}
        <div className="absolute top-0 left-1/2 -translate-x-1/2 -z-10 h-[600px] w-full max-w-7xl overflow-hidden opacity-40 pointer-events-none">
          <div className="absolute top-[-150px] left-[25%] h-[450px] w-[450px] rounded-full bg-accent/15 blur-[100px] animate-pulse" />
          <div className="absolute top-[-80px] right-[20%] h-[350px] w-[350px] rounded-full bg-primary/10 blur-[80px]" />
        </div>

        <section className="relative px-6 py-28 sm:px-8 sm:py-36 lg:px-12 lg:py-44">
          <div className="mx-auto max-w-6xl">
            <div className="mb-10 flex justify-center">
              <div className="inline-flex items-center gap-2 rounded-full bg-primary/10 px-4 py-1.5 text-sm font-medium text-primary border border-primary/20">
                <span className="relative flex h-2 w-2">
                  <span className="absolute inline-flex h-full w-full animate-ping rounded-full bg-primary opacity-75"></span>
                  <span className="relative inline-flex h-2 w-2 rounded-full bg-primary"></span>
                </span>
                Trusted by 500+ elevator companies
              </div>
            </div>

            <h1 className="mb-8 text-balance text-center text-5xl font-bold leading-tight text-foreground sm:text-6xl lg:text-7xl tracking-tight">
              Streamline Elevator Operations
            </h1>

            <p className="mx-auto mb-12 max-w-3xl text-center text-lg leading-relaxed text-foreground/75 sm:text-xl">
              Complete workflow management for installations, maintenance scheduling, emergency response, and inventory
              tracking. Everything your team needs in one platform.
            </p>

            <div className="mb-16 flex flex-col items-center justify-center gap-4 sm:flex-row">
              <Button
                asChild
                size="lg"
                className="rounded-full bg-primary px-8 text-base font-semibold text-primary-foreground hover:bg-primary/95 cursor-pointer shadow-lg shadow-primary/10 transition-all duration-200"
              >
                <Link href="/login">
                  Get Started <ArrowRight className="ml-2 h-4 w-4" />
                </Link>
              </Button>
              <Button
                asChild
                variant="outline"
                size="lg"
                className="rounded-full border-foreground/20 px-8 text-base font-semibold hover:bg-foreground/5 cursor-pointer"
              >
                <Link href="#features">See Features</Link>
              </Button>
            </div>

            <div className="relative h-96 w-full overflow-hidden rounded-2xl border border-border shadow-xl">
              <Image
                src="/hero-illustration.jpg"
                alt="LiftOps Dashboard"
                fill
                className="object-cover opacity-90"
                priority
              />
              <div className="absolute inset-0 bg-gradient-to-t from-background/40 to-transparent" />
            </div>
          </div>
        </section>

        <section id="features" className="bg-secondary/40 px-6 py-24 sm:px-8 sm:py-32 lg:px-12 border-y border-border/50">
          <div className="mx-auto max-w-6xl">
            <div className="mb-16 text-center">
              <h2 className="mb-4 text-4xl font-bold text-foreground sm:text-5xl tracking-tight">Everything you need</h2>
              <p className="mx-auto max-w-2xl text-lg text-foreground/70">
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
                    className="group overflow-hidden rounded-2xl border border-border bg-card transition-all duration-300 hover:border-accent/40 hover:shadow-xl hover:shadow-accent/5"
                  >
                    <div className="relative h-48 w-full bg-accent/5 overflow-hidden">
                      <Image src={feature.image} alt={feature.title} fill className="object-cover transition-transform duration-500 group-hover:scale-105" />
                      <div className="absolute inset-0 bg-gradient-to-t from-card/35 to-transparent" />
                    </div>
                    <div className="p-8">
                      <div className="mb-4 inline-flex h-12 w-12 items-center justify-center rounded-lg bg-primary/10 transition-colors group-hover:bg-accent/10">
                        <Icon className="h-6 w-6 text-primary group-hover:text-accent transition-colors duration-200" />
                      </div>
                      <h3 className="mb-3 text-lg font-semibold text-foreground group-hover:text-accent transition-colors duration-200">{feature.title}</h3>
                      <p className="text-sm leading-relaxed text-foreground/75">{feature.description}</p>
                    </div>
                  </div>
                )
              })}
            </div>
          </div>
        </section>

        <section className="px-6 py-24 sm:px-8 sm:py-32 lg:px-12 border-b border-border/30">
          <div className="mx-auto max-w-6xl">
            <div className="grid grid-cols-2 gap-8 md:grid-cols-4">
              {[
                { stat: "500+", label: "Companies" },
                { stat: "50K+", label: "Technicians" },
                { stat: "99.9%", label: "Uptime" },
                { stat: "24/7", label: "Support" },
              ].map((item, index) => (
                <div key={index} className="text-center group">
                  <p className="mb-2 text-4xl font-extrabold text-primary sm:text-5xl tracking-tight transition-transform duration-300 group-hover:scale-105">{item.stat}</p>
                  <p className="text-sm font-medium text-foreground/70 uppercase tracking-wider">{item.label}</p>
                </div>
              ))}
            </div>
          </div>
        </section>

        <section className="px-6 py-24 sm:px-8 sm:py-32 lg:px-12 bg-secondary/20">
          <div className="mx-auto max-w-6xl">
            <div className="mb-16 text-center">
              <h2 className="mb-4 text-4xl font-bold text-foreground sm:text-5xl tracking-tight">Powerful Dashboard</h2>
              <p className="mx-auto max-w-2xl text-lg text-foreground/70">
                Visualize your entire elevator operations in one unified dashboard
              </p>
            </div>
            <div className="relative h-96 w-full overflow-hidden rounded-2xl border border-border shadow-2xl">
              <Image src="/dashboard-vector.jpg" alt="LiftOps Dashboard" fill className="object-cover opacity-95" />
              <div className="absolute inset-0 bg-gradient-to-t from-background/30 to-transparent" />
            </div>
          </div>
        </section>

        <section className="px-6 py-24 sm:px-8 sm:py-32 lg:px-12">
          <div className="mx-auto max-w-4xl">
            <div className="rounded-3xl bg-gradient-to-br from-secondary to-card border border-border p-12 text-center shadow-2xl sm:p-16 relative overflow-hidden">
              {/* Subtle background glow */}
              <div className="absolute -right-10 -bottom-10 h-40 w-40 rounded-full bg-accent/10 blur-[50px] pointer-events-none" />
              <div className="absolute -left-10 -top-10 h-40 w-40 rounded-full bg-primary/10 blur-[50px] pointer-events-none" />

              <h2 className="mb-4 text-4xl font-bold text-foreground sm:text-5xl tracking-tight">
                Ready to transform your operations?
              </h2>
              <p className="mx-auto mb-10 max-w-2xl text-lg text-foreground/75">
                Join hundreds of elevator companies streamlining their workflows with LiftOps
              </p>
              <Button
                asChild
                size="lg"
                className="rounded-full bg-accent px-8 text-base font-semibold text-accent-foreground hover:bg-accent/95 cursor-pointer shadow-lg shadow-accent/25 transition-transform duration-200 hover:scale-[1.02]"
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
            <div className="mb-8 flex items-center gap-3">
              <Logo size={36} />
              <div className="flex flex-col">
                <span className="text-xl font-black tracking-tight leading-none">
                  <span className="text-foreground">Lift</span>
                  <span className="text-accent">Ops</span>
                </span>
                <div className="h-[1px] w-full bg-accent/25 my-1" />
                <span className="text-[8px] font-bold tracking-[0.18em] text-accent uppercase leading-none">
                  Elevator Management
                </span>
              </div>
            </div>
            <p className="max-w-2xl text-sm text-foreground/60 leading-relaxed">
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
