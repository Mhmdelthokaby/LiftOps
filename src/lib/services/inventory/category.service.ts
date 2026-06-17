import { prisma } from "@/lib/db/client";
import type { CreateCategoryInput, UpdateCategoryInput } from "@/lib/validators/inventory";
import { NotFoundError, ConflictError } from "@/lib/errors";
import type { ItemCategory } from "@prisma/client";

export class CategoryService {
  async create(companyId: string, input: CreateCategoryInput): Promise<ItemCategory> {
    const existing = await prisma.itemCategory.findUnique({
      where: { companyId_name: { companyId, name: input.name } },
    });
    if (existing) throw new ConflictError("Category with this name already exists");

    return prisma.itemCategory.create({
      data: { ...input, companyId },
    });
  }

  async update(id: string, input: UpdateCategoryInput): Promise<ItemCategory> {
    const existing = await prisma.itemCategory.findUnique({ where: { id } });
    if (!existing) throw new NotFoundError("ItemCategory", id);
    return prisma.itemCategory.update({ where: { id }, data: input });
  }

  async list(companyId: string): Promise<ItemCategory[]> {
    return prisma.itemCategory.findMany({
      where: { companyId },
      include: { _count: { select: { items: true } } },
      orderBy: { name: "asc" },
    });
  }
}
