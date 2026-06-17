import { z } from "zod";

export const createStageSchema = z.object({
  projectId: z.string().uuid("Invalid project ID"),
  name: z.string().min(1, "Stage name is required").max(200),
  order: z.coerce.number().int().positive(),
});

export const updateStageSchema = createStageSchema.partial();

export type CreateStageInput = z.infer<typeof createStageSchema>;
export type UpdateStageInput = z.infer<typeof updateStageSchema>;
