import type { Metadata } from "next"

export const metadata: Metadata = {
  title: "LiftOps | Elevator Operations Platform",
  description:
    "Complete workflow management for installations, maintenance scheduling, emergency response, and inventory tracking.",
}

export default function MarketingLayout({ children }: { children: React.ReactNode }) {
  return <div className="marketing">{children}</div>
}
