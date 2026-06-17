import { z } from "zod";

export const createContractSchema = z.object({
  customerId: z.string().uuid("Invalid customer ID"),
  startDate: z.string().datetime(),
  endDate: z.string().datetime(),
  visitFrequency: z.coerce.number().int().positive().default(1),
  value: z.coerce.number().positive(),
  notes: z.string().optional(),
});

export const updateContractSchema = createContractSchema.partial();

export type CreateContractInput = z.infer<typeof createContractSchema>;
export type UpdateContractInput = z.infer<typeof updateContractSchema>;
