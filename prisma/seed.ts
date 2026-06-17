import { PrismaClient, SubscriptionStatus, BillingCycle } from "@prisma/client";
import bcrypt from "bcryptjs";

const prisma = new PrismaClient();

async function main() {
  console.log("Seeding database...");

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
  const admin = await prisma.platformAdmin.upsert({
    where: { email: "admin@lifops.com" },
    update: {},
    create: {
      email: "admin@lifops.com",
      passwordHash,
      firstName: "Platform",
      lastName: "Admin",
      role: "SUPER_ADMIN",
      isActive: true,
    },
  });

  console.log("Seed completed!");
  console.log({ admin: admin.email });
}

main()
  .catch((e) => {
    console.error(e);
    process.exit(1);
  })
  .finally(async () => {
    await prisma.$disconnect();
  });
