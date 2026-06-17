import { z } from "zod";

export const scheduleVisitSchema = z.object({
  contractId: z.string().uuid().optional(),
  elevatorId: z.string().uuid("Invalid elevator ID"),
  technicianId: z.string().uuid().optional(),
  scheduledDate: z.string().datetime(),
  notes: z.string().optional(),
});

export const completeVisitSchema = z.object({
  checklistData: z.record(z.unknown()).optional(),
  notes: z.string().optional(),
});

export const updateVisitStatusSchema = z.object({
  status: z.enum(["SCHEDULED", "IN_PROGRESS", "COMPLETED", "CANCELLED", "NO_SHOW"]),
  notes: z.string().optional(),
});

export type ScheduleVisitInput = z.infer<typeof scheduleVisitSchema>;
export type CompleteVisitInput = z.infer<typeof completeVisitSchema>;
export type UpdateVisitStatusInput = z.infer<typeof updateVisitStatusSchema>;
