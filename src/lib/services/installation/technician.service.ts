import { prisma } from "@/lib/db/client";
import type { CreateTechnicianInput, UpdateTechnicianInput } from "@/lib/validators/installation";
import { NotFoundError, ConflictError } from "@/lib/errors";
import type { Technician } from "@prisma/client";

export class TechnicianService {
  async create(companyId: string, input: CreateTechnicianInput): Promise<Technician> {
    const existing = await prisma.technician.findUnique({
      where: { userId: input.userId },
    });
    if (existing) throw new ConflictError("User is already registered as a technician");

    return prisma.technician.create({
      data: { ...input, companyId },
      include: { user: { select: { id: true, firstName: true, lastName: true, email: true } } },
    });
  }

  async getById(id: string): Promise<Technician> {
    const technician = await prisma.technician.findUnique({
      where: { id },
      include: {
        user: { select: { id: true, firstName: true, lastName: true, email: true, phone: true } },
      },
    });
    if (!technician) throw new NotFoundError("Technician", id);
    return technician;
  }

  async update(id: string, input: UpdateTechnicianInput): Promise<Technician> {
    const existing = await prisma.technician.findUnique({ where: { id } });
    if (!existing) throw new NotFoundError("Technician", id);
    return prisma.technician.update({ where: { id }, data: input });
  }

  async list(companyId: string): Promise<Technician[]> {
    return prisma.technician.findMany({
      where: { companyId },
      include: {
        user: { select: { id: true, firstName: true, lastName: true, email: true } },
      },
      orderBy: { createdAt: "desc" },
    });
  }
}
