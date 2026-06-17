import { z } from "zod";
import { appConfig } from "@/config/app";

export const paginationSchema = z.object({
  page: z.coerce.number().int().positive().default(1),
  pageSize: z
    .coerce
    .number()
    .int()
    .positive()
    .max(appConfig.pagination.maxPageSize)
    .default(appConfig.pagination.defaultPageSize),
  sortBy: z.string().optional(),
  sortOrder: z.enum(["asc", "desc"]).default("desc"),
  search: z.string().optional(),
});

export type PaginationInput = z.infer<typeof paginationSchema>;

export function parsePagination(searchParams: URLSearchParams): PaginationInput {
  const raw: Record<string, string> = {};
  searchParams.forEach((value, key) => {
    raw[key] = value;
  });
  return paginationSchema.parse(raw);
}

export function getPrismaPagination(input: PaginationInput) {
  const skip = (input.page - 1) * input.pageSize;

  return {
    skip,
    take: input.pageSize,
    orderBy: input.sortBy
      ? { [input.sortBy]: input.sortOrder }
      : { createdAt: input.sortOrder as "asc" | "desc" },
  };
}
