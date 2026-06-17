import { cn } from "@/lib/utils"
import type { CompanyStatus, SubscriptionStatus } from "@/types/admin"

const companyStatusStyles: Record<CompanyStatus, { label: string; className: string }> = {
  Active: { label: "Active", className: "bg-green-600" },
  Inactive: { label: "Inactive", className: "bg-red-600" },
  Suspended: { label: "Suspended", className: "bg-amber-600" },
  SuspendedByAdmin: { label: "Suspended (admin)", className: "bg-red-700" },
  Deleted: { label: "Deleted", className: "bg-zinc-600" },
}

export function CompanyStatusBadge({ status }: { status: CompanyStatus | string }) {
  const normalized = status as CompanyStatus
  const config = companyStatusStyles[normalized] ?? {
    label: String(status),
    className: "bg-zinc-500",
  }
  return (
    <span
      className={cn(
        "inline-flex items-center rounded-full px-3 py-1 text-xs font-medium text-white",
        config.className
      )}
    >
      {config.label}
    </span>
  )
}

const subscriptionStyles: Record<SubscriptionStatus, { label: string; className: string }> = {
  Trial: { label: "Trial", className: "bg-blue-600" },
  Active: { label: "Active", className: "bg-green-600" },
  PastDue: { label: "Past due", className: "bg-amber-600" },
  Cancelled: { label: "Cancelled", className: "bg-zinc-600" },
  Expired: { label: "Expired", className: "bg-red-600" },
}

export function SubscriptionStatusBadge({ status }: { status: SubscriptionStatus | string }) {
  const normalized = status as SubscriptionStatus
  const config = subscriptionStyles[normalized] ?? {
    label: String(status),
    className: "bg-zinc-500",
  }
  return (
    <span
      className={cn(
        "inline-flex items-center rounded-full px-3 py-1 text-xs font-medium text-white",
        config.className
      )}
    >
      {config.label}
    </span>
  )
}

export function PlanCodeBadge({ code, name }: { code: string; name?: string }) {
  return (
    <span className="inline-flex items-center rounded-full bg-primary/15 px-3 py-1 text-xs font-medium text-primary">
      {name ?? code}
    </span>
  )
}

export function RoleBadge({ role }: { role: string }) {
  const isPlatform = role === "PlatformAdmin"
  return (
    <span
      className={cn(
        "inline-flex items-center rounded-full px-3 py-1 text-xs font-medium text-white",
        isPlatform ? "bg-red-600" : "bg-purple-600"
      )}
    >
      {role}
    </span>
  )
}
