import { z } from "zod";

export const createTicketSchema = z.object({
  elevatorId: z.string().uuid("Invalid elevator ID"),
  priority: z.enum(["LOW", "MEDIUM", "HIGH", "CRITICAL"]).default("MEDIUM"),
  isEmergency: z.boolean().default(false),
  title: z.string().min(1, "Title is required").max(200),
  description: z.string().min(1, "Description is required"),
});

export const updateTicketSchema = z.object({
  priority: z.enum(["LOW", "MEDIUM", "HIGH", "CRITICAL"]).optional(),
  title: z.string().min(1).max(200).optional(),
  description: z.string().min(1).optional(),
});

export const assignTicketSchema = z.object({
  technicianId: z.string().uuid("Invalid technician ID"),
});

export const resolveTicketSchema = z.object({
  resolution: z.string().min(1, "Resolution notes are required"),
  spareParts: z
    .array(
      z.object({
        itemId: z.string().uuid(),
        quantity: z.coerce.number().int().positive(),
      })
    )
    .optional(),
});

export const ticketQuerySchema = z.object({
  page: z.coerce.number().int().positive().default(1),
  pageSize: z.coerce.number().int().positive().max(100).default(20),
  status: z.enum(["OPEN", "ASSIGNED", "IN_PROGRESS", "RESOLVED", "CLOSED", "REOPENED"]).optional(),
  priority: z.enum(["LOW", "MEDIUM", "HIGH", "CRITICAL"]).optional(),
  elevatorId: z.string().uuid().optional(),
  isEmergency: z.coerce.boolean().optional(),
});

export type CreateTicketInput = z.infer<typeof createTicketSchema>;
export type UpdateTicketInput = z.infer<typeof updateTicketSchema>;
export type AssignTicketInput = z.infer<typeof assignTicketSchema>;
export type ResolveTicketInput = z.infer<typeof resolveTicketSchema>;
export type TicketQueryInput = z.infer<typeof ticketQuerySchema>;
