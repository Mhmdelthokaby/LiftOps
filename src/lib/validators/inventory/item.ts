import { z } from "zod";

export const createItemSchema = z.object({
  categoryId: z.string().uuid().optional(),
  name: z.string().min(1, "Item name is required").max(200),
  sku: z.string().optional(),
  description: z.string().optional(),
  unit: z.string().optional(),
  quantity: z.coerce.number().int().min(0).default(0),
  minQuantity: z.coerce.number().int().min(0).default(5),
  unitCost: z.coerce.number().positive().optional(),
  supplier: z.string().optional(),
  location: z.string().optional(),
});

export const updateItemSchema = createItemSchema.partial();

export const itemQuerySchema = z.object({
  page: z.coerce.number().int().positive().default(1),
  pageSize: z.coerce.number().int().positive().max(100).default(20),
  search: z.string().optional(),
  categoryId: z.string().uuid().optional(),
  lowStock: z.coerce.boolean().optional(),
  sortBy: z.string().optional(),
  sortOrder: z.enum(["asc", "desc"]).default("desc"),
});

export type CreateItemInput = z.infer<typeof createItemSchema>;
export type UpdateItemInput = z.infer<typeof updateItemSchema>;
export type ItemQueryInput = z.infer<typeof itemQuerySchema>;
