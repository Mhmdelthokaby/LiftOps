import { prisma } from "@/lib/db/client";
import type { CreateElevatorInput, UpdateElevatorInput } from "@/lib/validators/installation";
import { NotFoundError, ConflictError } from "@/lib/errors";
import type { Elevator } from "@prisma/client";

export class ElevatorService {
  async create(companyId: string, input: CreateElevatorInput): Promise<Elevator> {
    const existing = await prisma.elevator.findUnique({
      where: { companyId_serialNumber: { companyId, serialNumber: input.serialNumber } },
    });
    if (existing) throw new ConflictError("Elevator serial number already exists");

    return prisma.elevator.create({
      data: { ...input, companyId },
      include: { project: { select: { id: true, name: true, code: true } } },
    });
  }

  async getById(id: string): Promise<Elevator> {
    const elevator = await prisma.elevator.findUnique({
      where: { id },
      include: {
        project: { select: { id: true, name: true, code: true } },
        stage: true,
        maintenanceVisits: { orderBy: { scheduledDate: "desc" }, take: 5 },
      },
    });
    if (!elevator) throw new NotFoundError("Elevator", id);
    return elevator;
  }

  async update(id: string, input: UpdateElevatorInput): Promise<Elevator> {
    const existing = await prisma.elevator.findUnique({ where: { id } });
    if (!existing) throw new NotFoundError("Elevator", id);

    return prisma.elevator.update({
      where: { id },
      data: input,
      include: { project: { select: { id: true, name: true, code: true } } },
    });
  }

  async delete(id: string): Promise<void> {
    const existing = await prisma.elevator.findUnique({ where: { id } });
    if (!existing) throw new NotFoundError("Elevator", id);
    await prisma.elevator.delete({ where: { id } });
  }

  async listByProject(projectId: string): Promise<Elevator[]> {
    return prisma.elevator.findMany({
      where: { projectId },
      include: { stage: true, _count: { select: { maintenanceVisits: true, tickets: true } } },
      orderBy: { createdAt: "asc" },
    });
  }

  async listByCompany(companyId: string): Promise<Elevator[]> {
    return prisma.elevator.findMany({
      where: { companyId },
      include: { project: { select: { name: true, code: true } } },
      orderBy: { createdAt: "desc" },
    });
  }
}
