import { prisma } from "@/lib/db/client";
import { NotFoundError } from "@/lib/errors";

type InspectionData = {
  customerId: string;
  shaftType?: string;
  shaftWidth?: number;
  shaftDepth?: number;
  lastFloorHeight?: number;
  pitDepth?: number;
  travelHeight?: number;
  technicalNotes?: string;
  notes?: string;
};

export class InspectionService {
  async create(companyId: string, adminId: string, data: InspectionData) {
    const customer = await prisma.customer.findUnique({ where: { id: data.customerId } });
    if (!customer) throw new NotFoundError("Customer", data.customerId);

    return prisma.project.create({
      data: {
        companyId,
        customerId: data.customerId,
        name: `Inspection - ${customer.name}`,
        code: `INS-${Date.now().toString(36).toUpperCase()}`,
        address: customer.address,
        status: "PENDING",
        notes: data.notes,
      },
      include: { customer: true },
    });
  }

  async approve(projectId: string) {
    const project = await prisma.project.findUnique({ where: { id: projectId } });
    if (!project) throw new NotFoundError("Project", projectId);

    return prisma.project.update({
      where: { id: projectId },
      data: { status: "IN_PROGRESS" },
    });
  }

  async reject(projectId: string) {
    const project = await prisma.project.findUnique({ where: { id: projectId } });
    if (!project) throw new NotFoundError("Project", projectId);

    return prisma.project.update({
      where: { id: projectId },
      data: { status: "CANCELLED" },
    });
  }
}
