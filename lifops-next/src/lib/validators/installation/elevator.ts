import { z } from "zod";

export const createElevatorSchema = z.object({
  projectId: z.string().uuid("Invalid project ID"),
  stageId: z.string().uuid().optional(),
  serialNumber: z.string().min(1, "Serial number is required"),
  brand: z.string().optional(),
  model: z.string().optional(),
  type: z.string().optional(),
  capacity: z.coerce.number().int().positive().optional(),
  floors: z.coerce.number().int().positive().optional(),
  notes: z.string().optional(),
});

export const updateElevatorSchema = createElevatorSchema.omit({ projectId: true }).partial();

export type CreateElevatorInput = z.infer<typeof createElevatorSchema>;
export type UpdateElevatorInput = z.infer<typeof updateElevatorSchema>;
