import { z } from "zod";

export const createTechnicianSchema = z.object({
  userId: z.string().uuid("Invalid user ID"),
  specialty: z.string().optional(),
});

export const updateTechnicianSchema = z.object({
  specialty: z.string().optional(),
  isAvailable: z.boolean().optional(),
});

export type CreateTechnicianInput = z.infer<typeof createTechnicianSchema>;
export type UpdateTechnicianInput = z.infer<typeof updateTechnicianSchema>;
