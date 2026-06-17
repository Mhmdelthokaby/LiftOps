import { prisma } from "@/lib/db/client";
import type {
  CreateTicketInput,
  UpdateTicketInput,
  AssignTicketInput,
  ResolveTicketInput,
  TicketQueryInput,
} from "@/lib/validators/tickets";
import { NotFoundError } from "@/lib/errors";
import { createPaginationMeta, type PaginatedResult } from "@/lib/response";
import type { Ticket } from "@prisma/client";

export class TicketService {
  async create(companyId: string, reportedBy: string, input: CreateTicketInput): Promise<Ticket> {
    return prisma.ticket.create({
      data: {
        ...input,
        companyId,
        reportedBy,
      },
      include: {
        elevator: { select: { id: true, serialNumber: true } },
        reporter: { select: { id: true, firstName: true, lastName: true } },
      },
    });
  }

  async getById(id: string): Promise<Ticket> {
    const ticket = await prisma.ticket.findUnique({
      where: { id },
      include: {
        elevator: true,
        reporter: { select: { id: true, firstName: true, lastName: true, email: true } },
        assignee: { select: { id: true, firstName: true, lastName: true } },
        spareParts: { include: { item: true } },
      },
    });
    if (!ticket) throw new NotFoundError("Ticket", id);
    return ticket;
  }

  async update(id: string, input: UpdateTicketInput): Promise<Ticket> {
    const existing = await prisma.ticket.findUnique({ where: { id } });
    if (!existing) throw new NotFoundError("Ticket", id);
    return prisma.ticket.update({ where: { id }, data: input });
  }

  async assign(id: string, input: AssignTicketInput): Promise<Ticket> {
    const existing = await prisma.ticket.findUnique({ where: { id } });
    if (!existing) throw new NotFoundError("Ticket", id);

    return prisma.ticket.update({
      where: { id },
      data: {
        assignedTo: input.technicianId,
        status: "ASSIGNED",
      },
      include: {
        assignee: { select: { id: true, firstName: true, lastName: true } },
      },
    });
  }

  async resolve(id: string, input: ResolveTicketInput): Promise<Ticket> {
    const existing = await prisma.ticket.findUnique({ where: { id } });
    if (!existing) throw new NotFoundError("Ticket", id);

    const result = await prisma.ticket.update({
      where: { id },
      data: {
        status: "RESOLVED",
        resolvedAt: new Date(),
      },
    });

    if (input.spareParts && input.spareParts.length > 0) {
      await prisma.usedSparePart.createMany({
        data: input.spareParts.map((sp) => ({
          visitId: id,
          itemId: sp.itemId,
          quantity: sp.quantity,
        })),
      });
    }

    return result;
  }

  async close(id: string): Promise<Ticket> {
    const existing = await prisma.ticket.findUnique({ where: { id } });
    if (!existing) throw new NotFoundError("Ticket", id);

    return prisma.ticket.update({
      where: { id },
      data: { status: "CLOSED", closedAt: new Date() },
    });
  }

  async list(companyId: string, input: TicketQueryInput): Promise<PaginatedResult<Ticket>> {
    const where: Record<string, unknown> = { companyId };
    if (input.status) where.status = input.status;
    if (input.priority) where.priority = input.priority;
    if (input.elevatorId) where.elevatorId = input.elevatorId;
    if (input.isEmergency !== undefined) where.isEmergency = input.isEmergency;

    const skip = (input.page - 1) * input.pageSize;
    const [items, total] = await Promise.all([
      prisma.ticket.findMany({
        where,
        skip,
        take: input.pageSize,
        include: {
          elevator: { select: { id: true, serialNumber: true } },
          reporter: { select: { id: true, firstName: true, lastName: true } },
          assignee: { select: { id: true, firstName: true, lastName: true } },
        },
        orderBy: { createdAt: "desc" },
      }),
      prisma.ticket.count({ where }),
    ]);

    return { items, meta: createPaginationMeta(input.page, input.pageSize, total) };
  }
}
