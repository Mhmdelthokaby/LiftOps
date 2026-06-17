import { z } from "zod";

export const createEmergencyTicketSchema = z.object({
  elevatorId: z.string().uuid("Invalid elevator ID"),
  priority: z.enum(["LOW", "MEDIUM", "HIGH", "CRITICAL"]).default("CRITICAL"),
  title: z.string().min(1, "Title is required").max(200),
  description: z.string().min(1, "Description is required"),
  location: z.string().optional(),
  contact: z.string().optional(),
});

export const updateEmergencyTicketSchema = z.object({
  priority: z.enum(["LOW", "MEDIUM", "HIGH", "CRITICAL"]).optional(),
  title: z.string().min(1).max(200).optional(),
  description: z.string().min(1).optional(),
  location: z.string().optional(),
  contact: z.string().optional(),
});

export const assignEmergencyTechnicianSchema = z.object({
  technicianId: z.string().uuid("Invalid technician ID"),
});

export const resolveEmergencySchema = z.object({
  notes: z.string().min(1, "Resolution notes are required"),
});

export type CreateEmergencyTicketInput = z.infer<typeof createEmergencyTicketSchema>;
export type UpdateEmergencyTicketInput = z.infer<typeof updateEmergencyTicketSchema>;
export type AssignEmergencyTechnicianInput = z.infer<typeof assignEmergencyTechnicianSchema>;
export type ResolveEmergencyInput = z.infer<typeof resolveEmergencySchema>;
