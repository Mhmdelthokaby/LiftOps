import { PrismaClient, UserRole, SubscriptionStatus, BillingCycle } from "@prisma/client";
import bcrypt from "bcryptjs";

const prisma = new PrismaClient();

async function main() {
  console.log("Seeding database...");

  const company = await prisma.company.upsert({
    where: { id: "00000000-0000-0000-0000-000000000000" },
    update: {},
    create: {
      id: "00000000-0000-0000-0000-000000000000",
      name: "LiftOps Platform",
      slug: "lifops-platform",
      isActive: true,
    },
  });

  const plan = await prisma.subscriptionPlan.upsert({
    where: { id: "00000000-0000-0000-0000-000000000001" },
    update: {},
    create: {
      id: "00000000-0000-0000-0000-000000000001",
      name: "Professional",
      description: "For mid-size elevator companies",
      price: 99.99,
      billingCycle: BillingCycle.MONTHLY,
      features: JSON.stringify([
        "unlimited_elevators",
        "maintenance_tracking",
        "fault_management",
        "inventory_management",
        "basic_analytics",
      ]),
      maxUsers: 25,
      maxProjects: 100,
      isActive: true,
    },
  });

  const passwordHash = await bcrypt.hash("Admin@123", 12);
  const admin = await prisma.appUser.upsert({
    where: {
      companyId_email: {
        companyId: company.id,
        email: "admin@lifops.com",
      },
    },
    update: {},
    create: {
      id: "00000000-0000-0000-0000-000000000002",
      companyId: company.id,
      email: "admin@lifops.com",
      passwordHash,
      firstName: "Platform",
      lastName: "Admin",
      role: UserRole.SUPER_ADMIN,
      isActive: true,
      emailVerifiedAt: new Date(),
    },
  });

  await prisma.subscription.upsert({
    where: { companyId: company.id },
    update: {},
    create: {
      companyId: company.id,
      planId: plan.id,
      status: SubscriptionStatus.ACTIVE,
      startDate: new Date(),
      currentPeriodEnd: new Date(Date.now() + 365 * 24 * 60 * 60 * 1000),
    },
  });

  console.log("Seed completed!");
  console.log({ company: company.name, admin: admin.email });
}

main()
  .catch((e) => {
    console.error(e);
    process.exit(1);
  })
  .finally(async () => {
    await prisma.$disconnect();
  });
