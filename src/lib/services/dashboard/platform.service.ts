import { prisma } from "@/lib/db/client";

export class PlatformDashboardService {
  async getStats() {
    const [companies, subscriptions, plans] = await Promise.all([
      prisma.company.findMany({
        select: { id: true, isActive: true },
      }),
      prisma.subscription.findMany({
        select: { status: true, planId: true },
      }),
      prisma.subscriptionPlan.findMany({
        select: { id: true, name: true, price: true },
      }),
    ]);

    const totalCompanies = companies.length;
    const activeSubscriptions = subscriptions.filter((s) => s.status === "ACTIVE").length;
    const trialCompanies = subscriptions.filter((s) => s.status === "TRIAL").length;

    const planMap = new Map(plans.map((p) => [p.id, p]));
    const monthlyRevenue = subscriptions
      .filter((s) => s.status === "ACTIVE")
      .reduce((sum, s) => {
        const plan = s.planId ? planMap.get(s.planId) : undefined;
        return sum + (plan?.price.toNumber() ?? 0);
      }, 0);

    const planCounts = new Map<string, number>();
    for (const s of subscriptions) {
      if (s.planId) {
        planCounts.set(s.planId, (planCounts.get(s.planId) ?? 0) + 1);
      }
    }
    const companiesByPlan = Array.from(planCounts.entries()).map(([planId, count]) => ({
      planName: planMap.get(planId)?.name ?? "Unknown",
      count,
    }));

    return {
      totalCompanies,
      activeSubscriptions,
      monthlyRevenue,
      trialCompanies,
      revenueTrend: [],
      companiesByPlan,
    };
  }
}
