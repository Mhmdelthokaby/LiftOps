import { prisma } from "@/lib/db/client";

export class DashboardService {
  async getCompanyOverview(companyId: string) {
    const [projects, elevators, maintenanceVisits, tickets, inventoryItems] = await Promise.all([
      prisma.project.findMany({
        where: { companyId },
        select: { status: true },
      }),
      prisma.elevator.findMany({
        where: { companyId },
        select: { status: true },
      }),
      prisma.maintenanceVisit.findMany({
        where: { companyId },
        select: { status: true },
      }),
      prisma.ticket.findMany({
        where: { companyId },
        select: { status: true, priority: true },
      }),
      prisma.item.findMany({
        where: { companyId, isActive: true },
        select: { quantity: true, minQuantity: true, unitCost: true },
      }),
    ]);

    return {
      projects: {
        total: projects.length,
        byStatus: this.groupBy(projects, "status"),
      },
      elevators: {
        total: elevators.length,
        byStatus: this.groupBy(elevators, "status"),
      },
      maintenance: {
        totalVisits: maintenanceVisits.length,
        byStatus: this.groupBy(maintenanceVisits, "status"),
      },
      tickets: {
        total: tickets.length,
        open: tickets.filter((t) => t.status === "OPEN" || t.status === "ASSIGNED" || t.status === "IN_PROGRESS").length,
        critical: tickets.filter((t) => t.priority === "CRITICAL" && t.status !== "RESOLVED" && t.status !== "CLOSED").length,
        byStatus: this.groupBy(tickets, "status"),
      },
      inventory: {
        totalItems: inventoryItems.length,
        lowStock: inventoryItems.filter((i) => i.quantity <= i.minQuantity).length,
        totalValue: inventoryItems.reduce((sum, i) => sum + (i.quantity * (i.unitCost?.toNumber() ?? 0)), 0),
      },
    };
  }

  private groupBy<T extends Record<string, unknown>>(items: T[], key: string): Record<string, number> {
    return items.reduce(
      (acc, item) => {
        const val = String(item[key] ?? "unknown");
        acc[val] = (acc[val] ?? 0) + 1;
        return acc;
      },
      {} as Record<string, number>
    );
  }
}
