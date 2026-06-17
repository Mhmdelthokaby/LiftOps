import { prisma } from "@/lib/db/client";
import type { CreateChecklistInput, UpdateChecklistInput } from "@/lib/validators/maintenance";
import { NotFoundError } from "@/lib/errors";
import type { MaintenanceChecklist } from "@prisma/client";

export class MaintenanceChecklistService {
  async create(companyId: string, input: CreateChecklistInput): Promise<MaintenanceChecklist> {
    return prisma.maintenanceChecklist.create({
      data: {
        name: input.name,
        companyId,
        items: JSON.stringify(input.items),
      },
    });
  }

  async update(id: string, input: UpdateChecklistInput): Promise<MaintenanceChecklist> {
    const existing = await prisma.maintenanceChecklist.findUnique({ where: { id } });
    if (!existing) throw new NotFoundError("MaintenanceChecklist", id);

    const data: Record<string, unknown> = {};
    if (input.name) data.name = input.name;
    if (input.items) data.items = JSON.stringify(input.items);

    return prisma.maintenanceChecklist.update({ where: { id }, data });
  }

  async list(companyId: string, includeInactive = false): Promise<MaintenanceChecklist[]> {
    return prisma.maintenanceChecklist.findMany({
      where: { companyId, ...(includeInactive ? {} : { isActive: true }) },
      orderBy: { createdAt: "desc" },
    });
  }
}
