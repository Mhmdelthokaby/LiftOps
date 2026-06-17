import { prisma } from "@/lib/db/client";
import type {
  CreateEmergencyTicketInput,
  UpdateEmergencyTicketInput,
  ResolveEmergencyInput,
} from "@/lib/validators/emergency";
import { NotFoundError } from "@/lib/errors";
import type { Ticket } from "@prisma/client";

export class EmergencyService {
  async create(companyId: string, reportedBy: string, input: CreateEmergencyTicketInput): Promise<Ticket> {
    return prisma.ticket.create({
      data: {
        companyId,
        elevatorId: input.elevatorId,
        reportedBy,
        priority: "CRITICAL",
        isEmergency: true,
        title: input.title,
        description: input.description,
      },
      include: {
        elevator: { select: { id: true, serialNumber: true, type: true, floors: true } },
        reporter: { select: { id: true, firstName: true, lastName: true } },
      },
    });
  }

  async getById(id: string): Promise<Ticket> {
    const ticket = await prisma.ticket.findFirst({
      where: { id, isEmergency: true },
      include: {
        elevator: true,
        reporter: { select: { id: true, firstName: true, lastName: true, email: true } },
        assignee: { select: { id: true, firstName: true, lastName: true } },
      },
    });
    if (!ticket) throw new NotFoundError("EmergencyTicket", id);
    return ticket;
  }

  async update(id: string, input: UpdateEmergencyTicketInput): Promise<Ticket> {
    const existing = await prisma.ticket.findFirst({ where: { id, isEmergency: true } });
    if (!existing) throw new NotFoundError("EmergencyTicket", id);
    return prisma.ticket.update({ where: { id }, data: input });
  }

  async assign(id: string, technicianId: string): Promise<Ticket> {
    const existing = await prisma.ticket.findFirst({ where: { id, isEmergency: true } });
    if (!existing) throw new NotFoundError("EmergencyTicket", id);
    return prisma.ticket.update({
      where: { id },
      data: { assignedTo: technicianId, status: "ASSIGNED" },
      include: { assignee: { select: { id: true, firstName: true, lastName: true } } },
    });
  }

  async resolve(id: string, input: ResolveEmergencyInput): Promise<Ticket> {
    const existing = await prisma.ticket.findFirst({ where: { id, isEmergency: true } });
    if (!existing) throw new NotFoundError("EmergencyTicket", id);
    return prisma.ticket.update({
      where: { id },
      data: { status: "RESOLVED", resolvedAt: new Date(), description: input.notes },
    });
  }

  async list(companyId: string): Promise<Ticket[]> {
    return prisma.ticket.findMany({
      where: { companyId, isEmergency: true },
      include: {
        elevator: { select: { id: true, serialNumber: true } },
        reporter: { select: { id: true, firstName: true, lastName: true } },
        assignee: { select: { id: true, firstName: true, lastName: true } },
      },
      orderBy: { createdAt: "desc" },
    });
  }

  async listOpen(companyId: string): Promise<Ticket[]> {
    return prisma.ticket.findMany({
      where: {
        companyId,
        isEmergency: true,
        status: { in: ["OPEN", "ASSIGNED", "IN_PROGRESS"] },
      },
      include: {
        elevator: { select: { id: true, serialNumber: true, location: true } },
        reporter: { select: { id: true, firstName: true, lastName: true } },
        assignee: { select: { id: true, firstName: true, lastName: true } },
      },
      orderBy: [{ priority: "desc" }, { createdAt: "asc" }],
    });
  }
}
