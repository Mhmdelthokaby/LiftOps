import { prisma } from "@/lib/db/client";
import type { ScheduleVisitInput, CompleteVisitInput, UpdateVisitStatusInput } from "@/lib/validators/maintenance";
import { NotFoundError } from "@/lib/errors";
import type { MaintenanceVisit } from "@prisma/client";
import { startOfMonth, endOfMonth } from "date-fns";

export class MaintenanceVisitService {
  async schedule(companyId: string, input: ScheduleVisitInput): Promise<MaintenanceVisit> {
    return prisma.maintenanceVisit.create({
      data: {
        ...input,
        companyId,
        scheduledDate: new Date(input.scheduledDate),
      },
      include: { elevator: true },
    });
  }

  async getById(id: string): Promise<MaintenanceVisit> {
    const visit = await prisma.maintenanceVisit.findUnique({
      where: { id },
      include: {
        elevator: true,
        contract: true,
        technician: { select: { id: true, firstName: true, lastName: true } },
        spareParts: { include: { item: true } },
      },
    });
    if (!visit) throw new NotFoundError("MaintenanceVisit", id);
    return visit;
  }

  async complete(id: string, input: CompleteVisitInput): Promise<MaintenanceVisit> {
    const visit = await prisma.maintenanceVisit.findUnique({ where: { id } });
    if (!visit) throw new NotFoundError("MaintenanceVisit", id);

    return prisma.maintenanceVisit.update({
      where: { id },
      data: {
        status: "COMPLETED",
        completedDate: new Date(),
        checklistData: (input.checklistData ?? undefined) as never,
        notes: input.notes,
      },
      include: { elevator: true, contract: true },
    });
  }

  async updateStatus(id: string, input: UpdateVisitStatusInput): Promise<MaintenanceVisit> {
    const visit = await prisma.maintenanceVisit.findUnique({ where: { id } });
    if (!visit) throw new NotFoundError("MaintenanceVisit", id);

    return prisma.maintenanceVisit.update({
      where: { id },
      data: {
        status: input.status,
        notes: input.notes,
        completedDate: input.status === "COMPLETED" ? new Date() : undefined,
      },
    });
  }

  async listByContract(contractId: string): Promise<MaintenanceVisit[]> {
    return prisma.maintenanceVisit.findMany({
      where: { contractId },
      include: { elevator: true, technician: { select: { id: true, firstName: true, lastName: true } } },
      orderBy: { scheduledDate: "asc" },
    });
  }

  async listByElevator(elevatorId: string): Promise<MaintenanceVisit[]> {
    return prisma.maintenanceVisit.findMany({
      where: { elevatorId },
      include: { contract: true, technician: { select: { id: true, firstName: true, lastName: true } } },
      orderBy: { scheduledDate: "desc" },
    });
  }

  async getMonthlySchedule(companyId: string, month: number, year: number): Promise<MaintenanceVisit[]> {
    const start = startOfMonth(new Date(year, month - 1));
    const end = endOfMonth(new Date(year, month - 1));

    return prisma.maintenanceVisit.findMany({
      where: {
        companyId,
        scheduledDate: { gte: start, lte: end },
      },
      include: {
        elevator: { select: { id: true, serialNumber: true, type: true, floors: true } },
        contract: { select: { id: true, startDate: true, endDate: true, isActive: true } },
        technician: { select: { id: true, firstName: true, lastName: true } },
      },
      orderBy: { scheduledDate: "asc" },
    });
  }

  async getStatistics(companyId: string, month: number, year: number) {
    const start = startOfMonth(new Date(year, month - 1));
    const end = endOfMonth(new Date(year, month - 1));

    const visits = await prisma.maintenanceVisit.findMany({
      where: { companyId, scheduledDate: { gte: start, lte: end } },
    });

    return {
      total: visits.length,
      scheduled: visits.filter((v) => v.status === "SCHEDULED").length,
      inProgress: visits.filter((v) => v.status === "IN_PROGRESS").length,
      completed: visits.filter((v) => v.status === "COMPLETED").length,
      cancelled: visits.filter((v) => v.status === "CANCELLED").length,
      noShow: visits.filter((v) => v.status === "NO_SHOW").length,
    };
  }
}
