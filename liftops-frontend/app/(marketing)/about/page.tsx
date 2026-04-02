import Link from "next/link"
import Image from "next/image"
import { MarketingNavbar } from "@/components/marketing/navbar"
import { Button } from "@/components/ui/button"

export default function AboutPage() {
  return (
    <>
      <MarketingNavbar />
      <main className="min-h-screen bg-background">
        <section className="relative overflow-hidden px-6 py-32 sm:px-8 sm:py-40 lg:px-12 lg:py-48">
          <div className="mx-auto max-w-6xl text-center">
            <h1 className="mb-8 text-balance text-5xl font-bold leading-tight text-foreground sm:text-6xl lg:text-7xl">
              About LiftOps
            </h1>
            <p className="mx-auto max-w-3xl text-lg leading-relaxed text-foreground/70 sm:text-xl">
              We&apos;re building the modern operating system for elevator companies, designed by operators for operators.
            </p>
          </div>
        </section>

        <section className="bg-foreground/2 px-6 py-24 sm:px-8 sm:py-32 lg:px-12">
          <div className="mx-auto max-w-6xl">
            <div className="grid grid-cols-1 items-center gap-12 lg:grid-cols-2 lg:gap-16">
              <div>
                <h2 className="mb-6 text-4xl font-bold text-foreground sm:text-5xl">Our Mission</h2>
                <p className="mb-6 text-lg leading-relaxed text-foreground/70">
                  We believe elevator operations deserve better software. For decades, the industry has relied on spreadsheets,
                  fragmented tools, and manual workflows. We&apos;re changing that.
                </p>
                <p className="text-lg leading-relaxed text-foreground/70">
                  LiftOps was founded to streamline every aspect of elevator operations—from complex installations to emergency
                  dispatch—with a platform built specifically for this industry.
                </p>
              </div>
              <div className="relative h-80 overflow-hidden rounded-2xl border border-border/50">
                <Image src="/about-illustration.jpg" alt="LiftOps Team" fill className="object-cover" />
              </div>
            </div>
          </div>
        </section>

        <section className="px-6 py-24 sm:px-8 sm:py-32 lg:px-12">
          <div className="mx-auto max-w-6xl">
            <div className="mb-16 text-center">
              <h2 className="mb-4 text-4xl font-bold text-foreground sm:text-5xl">Our Values</h2>
              <p className="text-lg text-foreground/60">What drives everything we do</p>
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
                  className="group rounded-2xl border border-border/50 bg-card p-8 transition-all hover:border-primary/30 hover:shadow-lg hover:shadow-primary/5"
                >
                  <h3 className="mb-4 text-2xl font-semibold text-foreground">{value.title}</h3>
                  <p className="leading-relaxed text-foreground/70">{value.description}</p>
                </div>
              ))}
            </div>
          </div>
        </section>

        <section className="bg-foreground/2 px-6 py-24 sm:px-8 sm:py-32 lg:px-12">
          <div className="mx-auto max-w-6xl">
            <div className="grid grid-cols-2 gap-8 md:grid-cols-4">
              {[
                { stat: "500+", label: "Companies Trust Us" },
                { stat: "2024", label: "Founded" },
                { stat: "50K+", label: "Active Users" },
                { stat: "99.9%", label: "Uptime" },
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
          <div className="mx-auto max-w-4xl">
            <div className="rounded-3xl bg-gradient-to-br from-primary to-primary/80 p-12 text-center shadow-xl sm:p-16">
              <h2 className="mb-4 text-4xl font-bold text-primary-foreground sm:text-5xl">Want to Learn More?</h2>
              <p className="mx-auto mb-10 max-w-2xl text-lg text-primary-foreground/90">
                Get in touch with our team to see how LiftOps can transform your operations.
              </p>
              <Button
                asChild
                size="lg"
                className="rounded-full bg-accent px-8 text-base font-semibold text-accent-foreground hover:bg-accent/90"
              >
                <Link href="/contact">Contact Us</Link>
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
