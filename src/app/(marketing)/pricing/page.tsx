import Link from "next/link"
import { MarketingNavbar } from "@/components/marketing/navbar"
import { Button } from "@/components/ui/button"
import { Check } from "lucide-react"
import { Logo } from "@/components/marketing/logo"

export default function PricingPage() {
  const plans = [
    {
      name: "Starter",
      description: "Perfect for small teams",
      price: "$299",
      period: "/month",
      features: [
        "Up to 5 users",
        "Basic installation tracking",
        "Limited maintenance scheduling",
        "Email support",
        "Single tenant company",
      ],
      cta: "Get Started",
      highlighted: false,
    },
    {
      name: "Professional",
      description: "For growing companies",
      price: "$799",
      period: "/month",
      features: [
        "Up to 50 users",
        "Full installation management",
        "Advanced maintenance scheduling",
        "Emergency response tracking",
        "Inventory management",
        "Priority support",
        "Custom integrations",
      ],
      cta: "Start Free Trial",
      highlighted: true,
    },
    {
      name: "Enterprise",
      description: "For large operations",
      price: "Custom",
      period: "",
      features: [
        "Unlimited users",
        "All Professional features",
        "Multi-tenant isolation",
        "Advanced analytics",
        "Dedicated account manager",
        "24/7 phone support",
        "Custom development",
        "SLA guarantees",
      ],
      cta: "Schedule Demo",
      highlighted: false,
    },
  ]

  return (
    <>
      <MarketingNavbar />
      <main className="min-h-screen bg-background relative overflow-hidden">
        {/* Futuristic Industrial Background Glow */}
        <div className="absolute top-0 left-1/2 -translate-x-1/2 -z-10 h-[500px] w-full max-w-7xl overflow-hidden opacity-30 pointer-events-none">
          <div className="absolute top-[-100px] right-[20%] h-[400px] w-[400px] rounded-full bg-accent/15 blur-[100px]" />
        </div>

        <section className="relative px-6 py-28 sm:px-8 sm:py-36 lg:px-12 lg:py-44">
          <div className="mx-auto max-w-6xl text-center">
            <h1 className="mb-8 text-balance text-5xl font-bold leading-tight text-foreground sm:text-6xl lg:text-7xl tracking-tight">
              Simple, Transparent Pricing
            </h1>
            <p className="mx-auto max-w-3xl text-lg leading-relaxed text-foreground/75 sm:text-xl">
              All plans include a 14-day free trial with full access. No credit card required to start.
            </p>
          </div>
        </section>

        <section className="bg-secondary/40 px-6 py-24 sm:px-8 sm:py-32 lg:px-12 border-y border-border/50">
          <div className="mx-auto max-w-6xl">
            <div className="grid grid-cols-1 gap-8 md:grid-cols-3 lg:gap-6 items-stretch">
              {plans.map((plan, index) => (
                <div
                  key={index}
                  className={`relative rounded-2xl border transition-all duration-300 flex flex-col justify-between ${
                    plan.highlighted
                      ? "border-accent bg-gradient-to-br from-accent/5 to-accent/2 shadow-xl ring-1 ring-accent lg:scale-105 z-10"
                      : "border-border bg-card shadow-sm hover:border-accent/40 hover:shadow-lg hover:shadow-accent/5"
                  }`}
                >
                  {plan.highlighted && (
                    <div className="absolute -top-4 left-0 right-0 text-center">
                      <span className="inline-block rounded-full bg-accent px-4 py-1.5 text-sm font-semibold text-accent-foreground shadow-md">
                        Most Popular
                      </span>
                    </div>
                  )}
                  <div className="p-8 flex-grow flex flex-col justify-between">
                    <div>
                      <h3 className="text-2xl font-bold text-foreground">{plan.name}</h3>
                      <p className="mt-2 text-sm text-foreground/70">{plan.description}</p>
                      <div className="mt-8">
                        <span className="text-5xl font-extrabold text-foreground tracking-tight">{plan.price}</span>
                        {plan.period && <span className="ml-1 text-foreground/70">{plan.period}</span>}
                      </div>
                    </div>
                    <div>
                      <Button
                        asChild
                        size="lg"
                        className={`mt-10 w-full rounded-full font-semibold cursor-pointer transition-all duration-200 ${
                          plan.highlighted
                            ? "bg-accent text-accent-foreground hover:bg-accent/95 shadow-md shadow-accent/20 hover:scale-[1.02]"
                            : "border border-foreground/20 text-foreground hover:bg-foreground/5"
                        }`}
                        variant={plan.highlighted ? "default" : "outline"}
                      >
                        <Link href="/login">{plan.cta}</Link>
                      </Button>
                      <div className="mt-10 space-y-4 border-t border-border/30 pt-10">
                        {plan.features.map((feature, idx) => (
                          <div key={idx} className="flex items-start gap-3">
                            <Check className="mt-0.5 h-5 w-5 flex-shrink-0 text-accent" />
                            <span className="text-sm text-foreground/75">{feature}</span>
                          </div>
                        ))}
                      </div>
                    </div>
                  </div>
                </div>
              ))}
            </div>
          </div>
        </section>

        <section className="px-6 py-24 sm:px-8 sm:py-32 lg:px-12 border-b border-border/30">
          <div className="mx-auto max-w-6xl">
            <div className="mb-16 text-center">
              <h2 className="mb-4 text-4xl font-bold text-foreground sm:text-5xl tracking-tight">Common Questions</h2>
              <p className="text-lg text-foreground/70">Everything you need to know about our plans</p>
            </div>
            <div className="mx-auto grid max-w-4xl grid-cols-1 gap-8 md:grid-cols-2">
              {[
                {
                  q: "Do you offer a free trial?",
                  a: "Yes! All plans include a 14-day free trial with full access to features.",
                },
                {
                  q: "Can I change plans?",
                  a: "Absolutely. You can upgrade or downgrade anytime. Changes take effect at the start of your next billing cycle.",
                },
                {
                  q: "What happens to my data if I cancel?",
                  a: "Your data is securely retained for 30 days. You can export it at any time before that period.",
                },
                {
                  q: "Do you offer discounts for annual billing?",
                  a: "Yes! Annual plans include a 20% discount. Contact our sales team for more details.",
                },
              ].map((item, index) => (
                <div key={index} className="rounded-2xl border border-border bg-card p-8 transition-all duration-300 hover:border-accent/30 hover:shadow-md">
                  <h3 className="mb-3 text-lg font-semibold text-foreground">{item.q}</h3>
                  <p className="text-foreground/75 leading-relaxed">{item.a}</p>
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

              <h2 className="mb-4 text-4xl font-bold text-foreground sm:text-5xl tracking-tight">Ready to Get Started?</h2>
              <p className="mx-auto mb-10 max-w-2xl text-lg text-foreground/75">
                Start your 14-day free trial today. No credit card required.
              </p>
              <div className="flex flex-col justify-center gap-4 sm:flex-row relative z-10">
                <Button
                  asChild
                  size="lg"
                  className="rounded-full bg-accent px-8 text-base font-semibold text-accent-foreground hover:bg-accent/95 cursor-pointer shadow-lg shadow-accent/25 transition-transform duration-200 hover:scale-[1.02]"
                >
                  <Link href="/login">Start Free Trial</Link>
                </Button>
                <Button
                  asChild
                  variant="outline"
                  size="lg"
                  className="rounded-full border-foreground/20 px-8 text-base font-semibold hover:bg-foreground/5 cursor-pointer"
                >
                  <Link href="/contact">Talk to Sales</Link>
                </Button>
              </div>
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
