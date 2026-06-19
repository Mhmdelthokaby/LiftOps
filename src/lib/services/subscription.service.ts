import { prisma } from "@/lib/db/client";
import type { CreatePlanInput, UpdatePlanInput } from "@/lib/validators/subscription";
import { NotFoundError, ConflictError, ForbiddenError } from "@/lib/errors";
import type { SubscriptionPlan } from "@prisma/client";

export class SubscriptionPlanService {
  async create(input: CreatePlanInput): Promise<SubscriptionPlan> {
    return prisma.subscriptionPlan.create({
      data: {
        ...input,
        features: JSON.stringify(input.features),
      },
    });
  }

  async update(id: string, input: UpdatePlanInput): Promise<SubscriptionPlan> {
    const existing = await prisma.subscriptionPlan.findUnique({ where: { id } });
    if (!existing) throw new NotFoundError("SubscriptionPlan", id);

    const data: Record<string, unknown> = { ...input };
    if (input.features) {
      data.features = JSON.stringify(input.features);
    }

    return prisma.subscriptionPlan.update({ where: { id }, data });
  }

  async list(includeInactive = false): Promise<SubscriptionPlan[]> {
    return prisma.subscriptionPlan.findMany({
      where: includeInactive ? {} : { isActive: true },
      orderBy: { price: "asc" },
    });
  }

  async getById(id: string): Promise<SubscriptionPlan> {
    const plan = await prisma.subscriptionPlan.findUnique({ where: { id } });
    if (!plan) throw new NotFoundError("SubscriptionPlan", id);
    return plan;
  }

  async delete(id: string): Promise<void> {
    const existing = await prisma.subscriptionPlan.findUnique({ where: { id } });
    if (!existing) throw new NotFoundError("SubscriptionPlan", id);
    await prisma.subscriptionPlan.delete({ where: { id } });
  }
}

export class SubscriptionLifecycleService {
  async getCompanySubscription(companyId: string) {
    const subscription = await prisma.subscription.findUnique({
      where: { companyId },
      include: { plan: true },
    });

    if (!subscription) {
      // Auto-create trial subscription
      return prisma.subscription.create({
        data: {
          companyId,
          status: "TRIAL",
          startDate: new Date(),
          currentPeriodEnd: new Date(Date.now() + 14 * 24 * 60 * 60 * 1000),
        },
        include: { plan: true },
      });
    }

    return subscription;
  }

  async changePlan(companyId: string, planId: string) {
    const plan = await prisma.subscriptionPlan.findUnique({ where: { id: planId } });
    if (!plan || !plan.isActive) throw new NotFoundError("Active subscription plan", planId);

    const subscription = await this.getCompanySubscription(companyId);

    return prisma.subscription.update({
      where: { id: subscription.id },
      data: { planId, status: "ACTIVE" },
      include: { plan: true },
    });
  }

  async cancelSubscription(companyId: string) {
    const subscription = await prisma.subscription.findUnique({
      where: { companyId },
    });

    if (!subscription) throw new NotFoundError("Subscription");

    return prisma.subscription.update({
      where: { companyId },
      data: {
        status: "CANCELLED",
        cancelledAt: new Date(),
      },
    });
  }

  async handleWebhook(event: string, data: Record<string, unknown>) {
    switch (event) {
      case "customer.subscription.updated":
      case "customer.subscription.deleted":
        // Handle Stripe webhook events
        break;
      default:
        break;
    }
  }
}
