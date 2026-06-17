import { prisma } from "@/lib/db/client";
import type { CreateCompanyInput, UpdateCompanyInput, CompanyQueryInput } from "@/lib/validators/company";
import { NotFoundError, ConflictError } from "@/lib/errors";
import { createPaginationMeta, type PaginatedResult } from "@/lib/response";
import { getPrismaPagination } from "@/lib/utils/pagination";
import type { Company } from "@prisma/client";

export class CompanyService {
  async create(input: CreateCompanyInput): Promise<Company> {
    const existingSlug = await prisma.company.findUnique({
      where: { slug: input.slug },
    });

    if (existingSlug) {
      throw new ConflictError("Company with this slug already exists");
    }

    return prisma.company.create({
      data: {
        name: input.name,
        slug: input.slug,
        email: input.email,
        phone: input.phone,
        address: input.address,
      },
    });
  }

  async getById(id: string): Promise<Company> {
    const company = await prisma.company.findUnique({
      where: { id },
      include: {
        subscription: { include: { plan: true } },
        _count: { select: { users: true, projects: true } },
      },
    });

    if (!company) {
      throw new NotFoundError("Company", id);
    }

    return company;
  }

  async update(id: string, input: UpdateCompanyInput): Promise<Company> {
    const existing = await prisma.company.findUnique({ where: { id } });
    if (!existing) {
      throw new NotFoundError("Company", id);
    }

    return prisma.company.update({
      where: { id },
      data: input,
    });
  }

  async delete(id: string): Promise<void> {
    const existing = await prisma.company.findUnique({ where: { id } });
    if (!existing) {
      throw new NotFoundError("Company", id);
    }

    await prisma.company.update({
      where: { id },
      data: { isActive: false },
    });
  }

  async list(input: CompanyQueryInput): Promise<PaginatedResult<Company>> {
    const where: Record<string, unknown> = {};

    if (input.search) {
      where.OR = [
        { name: { contains: input.search, mode: "insensitive" } },
        { slug: { contains: input.search, mode: "insensitive" } },
        { email: { contains: input.search, mode: "insensitive" } },
      ];
    }

    const prismaPagination = getPrismaPagination(input);

    const [items, total] = await Promise.all([
      prisma.company.findMany({
        where,
        ...prismaPagination,
        include: {
          subscription: { select: { status: true, plan: { select: { name: true } } } },
          _count: { select: { users: true, projects: true } },
        },
      }),
      prisma.company.count({ where }),
    ]);

    return {
      items,
      meta: createPaginationMeta(input.page, input.pageSize, total),
    };
  }
}
