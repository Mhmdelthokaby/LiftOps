import { prisma } from "@/lib/db/client";
import type { CreateItemInput, UpdateItemInput, ItemQueryInput } from "@/lib/validators/inventory";
import { NotFoundError } from "@/lib/errors";
import { createPaginationMeta, type PaginatedResult } from "@/lib/response";
import type { Item } from "@prisma/client";

export class ItemService {
  async create(companyId: string, input: CreateItemInput): Promise<Item> {
    return prisma.item.create({
      data: { ...input, companyId },
      include: { category: { select: { id: true, name: true } } },
    });
  }

  async getById(id: string): Promise<Item> {
    const item = await prisma.item.findUnique({
      where: { id },
      include: { category: true, _count: { select: { usedIn: true } } },
    });
    if (!item) throw new NotFoundError("Item", id);
    return item;
  }

  async update(id: string, input: UpdateItemInput): Promise<Item> {
    const existing = await prisma.item.findUnique({ where: { id } });
    if (!existing) throw new NotFoundError("Item", id);
    return prisma.item.update({ where: { id }, data: input, include: { category: { select: { id: true, name: true } } } });
  }

  async delete(id: string): Promise<void> {
    const existing = await prisma.item.findUnique({ where: { id } });
    if (!existing) throw new NotFoundError("Item", id);
    await prisma.item.update({ where: { id }, data: { isActive: false } });
  }

  async list(companyId: string, input: ItemQueryInput): Promise<PaginatedResult<Item>> {
    const where: Record<string, unknown> = { companyId };

    if (input.search) {
      where.OR = [
        { name: { contains: input.search, mode: "insensitive" } },
        { sku: { contains: input.search, mode: "insensitive" } },
        { supplier: { contains: input.search, mode: "insensitive" } },
      ];
    }
    if (input.categoryId) where.categoryId = input.categoryId;
    if (input.lowStock) {
      where.AND = [{ quantity: { lte: prisma.item.fields.minQuantity } }];
    }

    const skip = (input.page - 1) * input.pageSize;
    const [items, total] = await Promise.all([
      prisma.item.findMany({
        where,
        skip,
        take: input.pageSize,
        include: { category: { select: { id: true, name: true } } },
        orderBy: { createdAt: "desc" },
      }),
      prisma.item.count({ where }),
    ]);

    return { items, meta: createPaginationMeta(input.page, input.pageSize, total) };
  }

  async getTotalValue(companyId: string): Promise<number> {
    const items = await prisma.item.findMany({
      where: { companyId, isActive: true },
      select: { quantity: true, unitCost: true },
    });

    return items.reduce((sum, item) => sum + (item.quantity * (item.unitCost?.toNumber() ?? 0)), 0);
  }
}
