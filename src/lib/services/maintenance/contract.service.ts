import { prisma } from "@/lib/db/client";
import type { CreateContractInput, UpdateContractInput } from "@/lib/validators/maintenance";
import { NotFoundError } from "@/lib/errors";
import { createPaginationMeta, type PaginatedResult } from "@/lib/response";
import type { MaintenanceContract } from "@prisma/client";

export class MaintenanceContractService {
  async create(companyId: string, input: CreateContractInput): Promise<MaintenanceContract> {
    return prisma.maintenanceContract.create({
      data: {
        ...input,
        companyId,
        startDate: new Date(input.startDate),
        endDate: new Date(input.endDate),
      },
      include: { _count: { select: { visits: true } } },
    });
  }

  async getById(id: string): Promise<MaintenanceContract> {
    const contract = await prisma.maintenanceContract.findUnique({
      where: { id },
      include: {
        visits: {
          include: { elevator: true, technician: { select: { id: true, firstName: true, lastName: true } } },
          orderBy: { scheduledDate: "desc" },
        },
        _count: { select: { visits: true } },
      },
    });
    if (!contract) throw new NotFoundError("MaintenanceContract", id);
    return contract;
  }

  async list(companyId: string, page = 1, pageSize = 20): Promise<PaginatedResult<MaintenanceContract>> {
    const skip = (page - 1) * pageSize;
    const [items, total] = await Promise.all([
      prisma.maintenanceContract.findMany({
        where: { companyId },
        skip,
        take: pageSize,
        include: { _count: { select: { visits: true } } },
        orderBy: { createdAt: "desc" },
      }),
      prisma.maintenanceContract.count({ where: { companyId } }),
    ]);
    return { items, meta: createPaginationMeta(page, pageSize, total) };
  }
}
