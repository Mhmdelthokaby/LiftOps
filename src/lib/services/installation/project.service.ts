import { prisma } from "@/lib/db/client";
import type { CreateProjectInput, UpdateProjectInput, ProjectQueryInput } from "@/lib/validators/installation";
import { NotFoundError, ConflictError } from "@/lib/errors";
import { createPaginationMeta, type PaginatedResult } from "@/lib/response";
import type { Project } from "@prisma/client";

export class ProjectService {
  async create(companyId: string, input: CreateProjectInput): Promise<Project> {
    const existing = await prisma.project.findUnique({
      where: { companyId_code: { companyId, code: input.code } },
    });
    if (existing) throw new ConflictError("Project code already exists");

    return prisma.project.create({
      data: { ...input, companyId, startDate: input.startDate ? new Date(input.startDate) : null, endDate: input.endDate ? new Date(input.endDate) : null },
      include: { customer: true, elevators: true },
    });
  }

  async getById(id: string): Promise<Project> {
    const project = await prisma.project.findUnique({
      where: { id },
      include: {
        customer: true,
        stages: { orderBy: { order: "asc" } },
        elevators: { include: { stage: true } },
      },
    });
    if (!project) throw new NotFoundError("Project", id);
    return project;
  }

  async update(id: string, input: UpdateProjectInput): Promise<Project> {
    const existing = await prisma.project.findUnique({ where: { id } });
    if (!existing) throw new NotFoundError("Project", id);

    const data: Record<string, unknown> = { ...input };
    if (input.startDate) data.startDate = new Date(input.startDate);
    if (input.endDate) data.endDate = new Date(input.endDate);

    return prisma.project.update({ where: { id }, data, include: { customer: true, elevators: true } });
  }

  async list(companyId: string, input: ProjectQueryInput): Promise<PaginatedResult<Project>> {
    const where: Record<string, unknown> = { companyId };
    if (input.status) where.status = input.status;
    if (input.search) {
      where.OR = [
        { name: { contains: input.search, mode: "insensitive" } },
        { code: { contains: input.search, mode: "insensitive" } },
      ];
    }

    const skip = (input.page - 1) * input.pageSize;
    const [items, total] = await Promise.all([
      prisma.project.findMany({
        where,
        skip,
        take: input.pageSize,
        include: { customer: { select: { id: true, name: true } }, _count: { select: { elevators: true, stages: true } } },
        orderBy: { createdAt: "desc" },
      }),
      prisma.project.count({ where }),
    ]);

    return { items, meta: createPaginationMeta(input.page, input.pageSize, total) };
  }

  async delete(id: string): Promise<void> {
    const existing = await prisma.project.findUnique({ where: { id } });
    if (!existing) throw new NotFoundError("Project", id);
    await prisma.project.update({ where: { id }, data: { status: "CANCELLED" } });
  }
}
