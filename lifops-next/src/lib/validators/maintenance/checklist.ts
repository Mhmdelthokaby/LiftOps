import { z } from "zod";

export const createChecklistSchema = z.object({
  name: z.string().min(1, "Checklist name is required").max(200),
  items: z.array(
    z.object({
      name: z.string().min(1),
      type: z.enum(["CHECKBOX", "TEXT", "NUMBER", "PERCENTAGE"]).default("CHECKBOX"),
      required: z.boolean().default(true),
    })
  ).min(1, "At least one checklist item is required"),
});

export const updateChecklistSchema = createChecklistSchema.partial();

export type CreateChecklistInput = z.infer<typeof createChecklistSchema>;
export type UpdateChecklistInput = z.infer<typeof updateChecklistSchema>;
