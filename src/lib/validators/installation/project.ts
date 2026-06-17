import { z } from "zod";

export const createProjectSchema = z.object({
  customerId: z.string().uuid("Invalid customer ID"),
  name: z.string().min(1, "Project name is required").max(200),
  code: z.string().min(1, "Project code is required").max(50),
  address: z.string().min(1, "Address is required"),
  startDate: z.string().datetime().optional(),
  endDate: z.string().datetime().optional(),
  notes: z.string().optional(),
});

export const updateProjectSchema = createProjectSchema.partial();

export const projectQuerySchema = z.object({
  page: z.coerce.number().int().positive().default(1),
  pageSize: z.coerce.number().int().positive().max(100).default(20),
  status: z.enum(["PENDING", "IN_PROGRESS", "COMPLETED", "ON_HOLD", "CANCELLED"]).optional(),
  search: z.string().optional(),
});

export type CreateProjectInput = z.infer<typeof createProjectSchema>;
export type UpdateProjectInput = z.infer<typeof updateProjectSchema>;
export type ProjectQueryInput = z.infer<typeof projectQuerySchema>;
