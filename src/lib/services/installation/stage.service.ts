import { prisma } from "@/lib/db/client";
import type { CreateStageInput, UpdateStageInput } from "@/lib/validators/installation";
import { NotFoundError } from "@/lib/errors";
import type { Stage } from "@prisma/client";

export class StageService {
  async create(companyId: string, input: CreateStageInput): Promise<Stage> {
    return prisma.stage.create({
      data: { ...input, companyId },
      include: { project: { select: { id: true, name: true, code: true } } },
    });
  }

  async getById(id: string): Promise<Stage> {
    const stage = await prisma.stage.findUnique({
      where: { id },
      include: { project: true, elevators: true },
    });
    if (!stage) throw new NotFoundError("Stage", id);
    return stage;
  }

  async update(id: string, input: UpdateStageInput): Promise<Stage> {
    const existing = await prisma.stage.findUnique({ where: { id } });
    if (!existing) throw new NotFoundError("Stage", id);
    return prisma.stage.update({ where: { id }, data: input });
  }

  async delete(id: string): Promise<void> {
    const existing = await prisma.stage.findUnique({ where: { id } });
    if (!existing) throw new NotFoundError("Stage", id);
    await prisma.stage.delete({ where: { id } });
  }

  async listByProject(projectId: string): Promise<Stage[]> {
    return prisma.stage.findMany({
      where: { projectId },
      include: { elevators: true },
      orderBy: { order: "asc" },
    });
  }
}
