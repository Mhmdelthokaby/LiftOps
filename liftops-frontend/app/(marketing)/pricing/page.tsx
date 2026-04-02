import Link from "next/link"
import { MarketingNavbar } from "@/components/marketing/navbar"
import { Button } from "@/components/ui/button"
import { Check } from "lucide-react"

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
      <main className="min-h-screen bg-background">
        <section className="relative overflow-hidden px-6 py-32 sm:px-8 sm:py-40 lg:px-12 lg:py-48">
          <div className="mx-auto max-w-6xl text-center">
            <h1 className="mb-8 text-balance text-5xl font-bold leading-tight text-foreground sm:text-6xl lg:text-7xl">
              Simple, Transparent Pricing
            </h1>
            <p className="mx-auto max-w-3xl text-lg leading-relaxed text-foreground/70 sm:text-xl">
              All plans include a 14-day free trial with full access. No credit card required to start.
            </p>
          </div>
        </section>

        <section className="bg-foreground/2 px-6 py-24 sm:px-8 sm:py-32 lg:px-12">
          <div className="mx-auto max-w-6xl">
            <div className="grid grid-cols-1 gap-8 md:grid-cols-3 lg:gap-6">
              {plans.map((plan, index) => (
                <div
                  key={index}
                  className={`relative rounded-2xl border transition-all ${
                    plan.highlighted
                      ? "border-primary bg-gradient-to-br from-primary/5 to-primary/2 shadow-xl ring-1 ring-primary lg:scale-105"
                      : "border-border/50 bg-card shadow-sm hover:border-primary/30 hover:shadow-lg"
                  }`}
                >
                  {plan.highlighted && (
                    <div className="absolute -top-4 left-0 right-0 text-center">
                      <span className="inline-block rounded-full bg-accent px-4 py-1.5 text-sm font-semibold text-accent-foreground">
                        Most Popular
                      </span>
                    </div>
                  )}
                  <div className="p-8">
                    <h3 className="text-2xl font-bold text-foreground">{plan.name}</h3>
                    <p className="mt-2 text-sm text-foreground/70">{plan.description}</p>
                    <div className="mt-8">
                      <span className="text-5xl font-bold text-foreground">{plan.price}</span>
                      {plan.period && <span className="ml-1 text-foreground/70">{plan.period}</span>}
                    </div>
                    <Button
                      asChild
                      size="lg"
                      className={`mt-10 w-full rounded-full font-semibold ${
                        plan.highlighted
                          ? "bg-accent text-accent-foreground hover:bg-accent/90"
                          : "border border-foreground/20 text-foreground hover:bg-foreground/5"
                      }`}
                      variant={plan.highlighted ? "default" : "outline"}
                    >
                      <Link href="/login">{plan.cta}</Link>
                    </Button>
                    <div className="mt-10 space-y-4 border-t border-border/30 pt-10">
                      {plan.features.map((feature, idx) => (
                        <div key={idx} className="flex items-start gap-3">
                          <Check className="mt-0.5 h-5 w-5 flex-shrink-0 text-primary" />
                          <span className="text-sm text-foreground/70">{feature}</span>
                        </div>
                      ))}
                    </div>
                  </div>
                </div>
              ))}
            </div>
          </div>
        </section>

        <section className="px-6 py-24 sm:px-8 sm:py-32 lg:px-12">
          <div className="mx-auto max-w-6xl">
            <div className="mb-16 text-center">
              <h2 className="mb-4 text-4xl font-bold text-foreground sm:text-5xl">Common Questions</h2>
              <p className="text-lg text-foreground/60">Everything you need to know about our plans</p>
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
                <div key={index} className="rounded-2xl border border-border/50 bg-card p-8">
                  <h3 className="mb-3 text-lg font-semibold text-foreground">{item.q}</h3>
                  <p className="text-foreground/70">{item.a}</p>
                </div>
              ))}
            </div>
          </div>
        </section>

        <section className="bg-foreground/2 px-6 py-24 sm:px-8 sm:py-32 lg:px-12">
          <div className="mx-auto max-w-4xl">
            <div className="rounded-3xl bg-gradient-to-br from-primary to-primary/80 p-12 text-center shadow-xl sm:p-16">
              <h2 className="mb-4 text-4xl font-bold text-primary-foreground sm:text-5xl">Ready to Get Started?</h2>
              <p className="mx-auto mb-10 max-w-2xl text-lg text-primary-foreground/90">
                Start your 14-day free trial today. No credit card required.
              </p>
              <div className="flex flex-col justify-center gap-4 sm:flex-row">
                <Button
                  asChild
                  size="lg"
                  className="rounded-full bg-accent px-8 text-base font-semibold text-accent-foreground hover:bg-accent/90"
                >
                  <Link href="/login">Start Free Trial</Link>
                </Button>
                <Button
                  asChild
                  variant="outline"
                  size="lg"
                  className="rounded-full border-primary-foreground px-8 font-semibold text-primary-foreground hover:bg-primary-foreground/10"
                >
                  <Link href="/contact">Talk to Sales</Link>
                </Button>
              </div>
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
