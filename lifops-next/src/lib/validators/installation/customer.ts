import { z } from "zod";

export const createCustomerSchema = z.object({
  name: z.string().min(1, "Customer name is required").max(200),
  contactPerson: z.string().min(1).max(200),
  phone: z.string().optional(),
  email: z.string().email().optional(),
  address: z.string().min(1, "Address is required"),
  notes: z.string().optional(),
});

export const updateCustomerSchema = createCustomerSchema.partial();

export type CreateCustomerInput = z.infer<typeof createCustomerSchema>;
export type UpdateCustomerInput = z.infer<typeof updateCustomerSchema>;
