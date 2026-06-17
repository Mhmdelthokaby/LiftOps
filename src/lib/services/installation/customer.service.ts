import { prisma } from "@/lib/db/client";
import type { CreateCustomerInput, UpdateCustomerInput } from "@/lib/validators/installation";
import { NotFoundError } from "@/lib/errors";
import { createPaginationMeta, type PaginatedResult } from "@/lib/response";
import type { Customer } from "@prisma/client";

export class CustomerService {
  async create(companyId: string, input: CreateCustomerInput): Promise<Customer> {
    return prisma.customer.create({
      data: { ...input, companyId },
    });
  }

  async getById(id: string): Promise<Customer> {
    const customer = await prisma.customer.findUnique({
      where: { id },
      include: { _count: { select: { projects: true } } },
    });
    if (!customer) throw new NotFoundError("Customer", id);
    return customer;
  }

  async update(id: string, input: UpdateCustomerInput): Promise<Customer> {
    const existing = await prisma.customer.findUnique({ where: { id } });
    if (!existing) throw new NotFoundError("Customer", id);
    return prisma.customer.update({ where: { id }, data: input });
  }

  async delete(id: string): Promise<void> {
    const existing = await prisma.customer.findUnique({ where: { id } });
    if (!existing) throw new NotFoundError("Customer", id);
    await prisma.customer.delete({ where: { id } });
  }

  async list(
    companyId: string,
    page = 1,
    pageSize = 20,
    search?: string
  ): Promise<PaginatedResult<Customer>> {
    const where: Record<string, unknown> = { companyId };
    if (search) {
      where.OR = [
        { name: { contains: search, mode: "insensitive" } },
        { contactPerson: { contains: search, mode: "insensitive" } },
        { email: { contains: search, mode: "insensitive" } },
      ];
    }

    const skip = (page - 1) * pageSize;
    const [items, total] = await Promise.all([
      prisma.customer.findMany({
        where,
        skip,
        take: pageSize,
        orderBy: { createdAt: "desc" },
      }),
      prisma.customer.count({ where }),
    ]);

    return { items, meta: createPaginationMeta(page, pageSize, total) };
  }
}
