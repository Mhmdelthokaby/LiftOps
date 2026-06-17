import { PrismaClient } from "@prisma/client";

const globalForPrisma = globalThis as unknown as {
  prisma: PrismaClient | undefined;
};

export const prisma =
  globalForPrisma.prisma ??
  new PrismaClient({
    log:
      process.env.NODE_ENV === "development"
        ? ["query", "error", "warn"]
        : ["error"],
  });

if (process.env.NODE_ENV !== "production") {
  globalForPrisma.prisma = prisma;
}

const MODELS_WITH_TENANT = [
  "AppUser",
  "Project",
  "Elevator",
  "Customer",
  "Stage",
  "Technician",
  "MaintenanceContract",
  "MaintenanceVisit",
  "MaintenanceChecklist",
  "Ticket",
  "Item",
  "ItemCategory",
  "Subscription",
] as const;

type ModelName = (typeof MODELS_WITH_TENANT)[number];

export function createTenantClient(companyId: string) {
  return prisma.$extends({
    query: {
      $allModels: {
        async $allOperations({ args, query, model, operation }) {
          if (!(MODELS_WITH_TENANT as readonly string[]).includes(model)) {
            return query(args);
          }

          if (model === "Company") {
            const where = "where" in args ? args.where : {};
            if (["findUnique", "findFirst"].includes(operation)) {
              return query({ ...args, where: { ...where, id: companyId } });
            }
            if (["update", "delete"].includes(operation)) {
              return query({ ...args, where: { ...where, id: companyId } });
            }
            return query(args);
          }

          const where = "where" in args ? (args.where as Record<string, unknown>) || {} : {};
          return query({ ...args, where: { ...where, companyId } } as never);
        },
      },
    },
  });
}
