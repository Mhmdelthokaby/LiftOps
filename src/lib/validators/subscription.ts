import { z } from "zod";

export const createPlanSchema = z.object({
  name: z.string().min(1, "Plan name is required").max(200),
  description: z.string().optional(),
  price: z.coerce.number().positive("Price must be positive"),
  billingCycle: z.enum(["MONTHLY", "QUARTERLY", "YEARLY"]),
  features: z.array(z.string()).default([]),
  maxUsers: z.coerce.number().int().positive().default(10),
  maxProjects: z.coerce.number().int().positive().default(50),
});

export const updatePlanSchema = z.object({
  name: z.string().min(1).max(200).optional(),
  description: z.string().optional(),
  price: z.coerce.number().positive().optional(),
  features: z.array(z.string()).optional(),
  maxUsers: z.coerce.number().int().positive().optional(),
  maxProjects: z.coerce.number().int().positive().optional(),
  isActive: z.boolean().optional(),
});

export const changePlanSchema = z.object({
  planId: z.string().uuid("Invalid plan ID"),
});

export type CreatePlanInput = z.infer<typeof createPlanSchema>;
export type UpdatePlanInput = z.infer<typeof updatePlanSchema>;
export type ChangePlanInput = z.infer<typeof changePlanSchema>;
