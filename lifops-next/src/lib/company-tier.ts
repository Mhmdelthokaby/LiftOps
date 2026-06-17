/** Map API plan labels to edit-form tier (must match backend UpdateCompanyRequest). */
export type SubscriptionTier = "Free" | "Pro" | "Enterprise"

export function inferSubscriptionTier(planLabel?: string | null): SubscriptionTier {
  const p = (planLabel ?? "").toLowerCase()
  if (p.includes("enterprise") || p === "ent") return "Enterprise"
  if (p.includes("pro") || p.includes("professional")) return "Pro"
  if (p.includes("free") || p.includes("basic") || p.includes("starter")) return "Free"
  return "Pro"
}
