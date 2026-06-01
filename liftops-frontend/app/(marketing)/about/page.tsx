import Link from "next/link"
import Image from "next/image"
import { MarketingNavbar } from "@/components/marketing/navbar"
import { Button } from "@/components/ui/button"
import { Logo } from "@/components/marketing/logo"

export default function AboutPage() {
  return (
    <>
      <MarketingNavbar />
      <main className="min-h-screen bg-background relative overflow-hidden">
        {/* Futuristic Industrial Background Glow */}
        <div className="absolute top-0 left-1/2 -translate-x-1/2 -z-10 h-[500px] w-full max-w-7xl overflow-hidden opacity-30 pointer-events-none">
          <div className="absolute top-[-100px] left-[20%] h-[400px] w-[400px] rounded-full bg-accent/15 blur-[100px]" />
        </div>

        <section className="relative px-6 py-28 sm:px-8 sm:py-36 lg:px-12 lg:py-44">
          <div className="mx-auto max-w-6xl text-center">
            <h1 className="mb-8 text-balance text-5xl font-bold leading-tight text-foreground sm:text-6xl lg:text-7xl tracking-tight">
              About LiftOps
            </h1>
            <p className="mx-auto max-w-3xl text-lg leading-relaxed text-foreground/75 sm:text-xl">
              We&apos;re building the modern operating system for elevator companies, designed by operators for operators.
            </p>
          </div>
        </section>

        <section className="bg-secondary/40 px-6 py-24 sm:px-8 sm:py-32 lg:px-12 border-y border-border/50">
          <div className="mx-auto max-w-6xl">
            <div className="grid grid-cols-1 items-center gap-12 lg:grid-cols-2 lg:gap-16">
              <div>
                <h2 className="mb-6 text-4xl font-bold text-foreground sm:text-5xl tracking-tight">Our Mission</h2>
                <p className="mb-6 text-lg leading-relaxed text-foreground/75">
                  We believe elevator operations deserve better software. For decades, the industry has relied on spreadsheets,
                  fragmented tools, and manual workflows. We&apos;re changing that.
                </p>
                <p className="text-lg leading-relaxed text-foreground/75">
                  LiftOps was founded to streamline every aspect of elevator operations—from complex installations to emergency
                  dispatch—with a platform built specifically for this industry.
                </p>
              </div>
              <div className="relative h-80 overflow-hidden rounded-2xl border border-border">
                <Image src="/about-illustration.jpg" alt="LiftOps Team" fill className="object-cover opacity-90" />
                <div className="absolute inset-0 bg-gradient-to-t from-background/30 to-transparent" />
              </div>
            </div>
          </div>
        </section>

        <section className="px-6 py-24 sm:px-8 sm:py-32 lg:px-12 border-b border-border/30">
          <div className="mx-auto max-w-6xl">
            <div className="mb-16 text-center">
              <h2 className="mb-4 text-4xl font-bold text-foreground sm:text-5xl tracking-tight">Our Values</h2>
              <p className="text-lg text-foreground/70">What drives everything we do</p>
            </div>

            <div className="grid grid-cols-1 gap-8 md:grid-cols-3">
              {[
                {
                  title: "Reliability",
                  description:
                    "We understand that elevator operations are critical infrastructure. Our systems are built for 99.9% uptime and enterprise-grade security.",
                },
                {
                  title: "Innovation",
                  description:
                    "We continuously evolve our platform with the latest technology and industry best practices to keep you ahead.",
                },
                {
                  title: "Customer Success",
                  description:
                    "Your success is our success. We provide dedicated support and always listen to your feedback to improve our platform.",
                },
              ].map((value, index) => (
                <div
                  key={index}
                  className="group rounded-2xl border border-border bg-card p-8 transition-all duration-300 hover:border-accent/40 hover:shadow-xl hover:shadow-accent/5"
                >
                  <h3 className="mb-4 text-2xl font-semibold text-foreground group-hover:text-accent transition-colors duration-200">{value.title}</h3>
                  <p className="leading-relaxed text-foreground/75">{value.description}</p>
                </div>
              ))}
            </div>
          </div>
        </section>

        <section className="bg-secondary/20 px-6 py-24 sm:px-8 sm:py-32 lg:px-12 border-b border-border/30">
          <div className="mx-auto max-w-6xl">
            <div className="grid grid-cols-2 gap-8 md:grid-cols-4">
              {[
                { stat: "500+", label: "Companies Trust Us" },
                { stat: "2024", label: "Founded" },
                { stat: "50K+", label: "Active Users" },
                { stat: "99.9%", label: "Uptime" },
              ].map((item, index) => (
                <div key={index} className="text-center group">
                  <p className="mb-2 text-4xl font-extrabold text-primary sm:text-5xl tracking-tight transition-transform duration-300 group-hover:scale-105">{item.stat}</p>
                  <p className="text-sm font-medium text-foreground/70 uppercase tracking-wider">{item.label}</p>
                </div>
              ))}
            </div>
          </div>
        </section>

        <section className="px-6 py-24 sm:px-8 sm:py-32 lg:px-12">
          <div className="mx-auto max-w-4xl">
            <div className="rounded-3xl bg-gradient-to-br from-secondary to-card border border-border p-12 text-center shadow-2xl sm:p-16 relative overflow-hidden">
              {/* Subtle background glow */}
              <div className="absolute -right-10 -bottom-10 h-40 w-40 rounded-full bg-accent/10 blur-[50px] pointer-events-none" />
              <div className="absolute -left-10 -top-10 h-40 w-40 rounded-full bg-primary/10 blur-[50px] pointer-events-none" />

              <h2 className="mb-4 text-4xl font-bold text-foreground sm:text-5xl tracking-tight">Want to Learn More?</h2>
              <p className="mx-auto mb-10 max-w-2xl text-lg text-foreground/75">
                Get in touch with our team to see how LiftOps can transform your operations.
              </p>
              <Button
                asChild
                size="lg"
                className="rounded-full bg-accent px-8 text-base font-semibold text-accent-foreground hover:bg-accent/95 cursor-pointer shadow-lg shadow-accent/25 transition-transform duration-200 hover:scale-[1.02]"
              >
                <Link href="/contact">Contact Us</Link>
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
