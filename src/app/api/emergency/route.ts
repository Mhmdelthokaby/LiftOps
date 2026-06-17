import { NextRequest } from "next/server";
import { EmergencyService } from "@/lib/services/emergency";
import { createEmergencyTicketSchema } from "@/lib/validators/emergency";
import { response, handleError } from "@/lib/response";
import { ValidationError } from "@/lib/errors";
import { authenticate } from "@/lib/auth/middleware";

const emergencyService = new EmergencyService();

export async function GET(request: NextRequest) {
  try {
    const auth = await authenticate(request);
    const tickets = await emergencyService.list(auth.auth.companyId);
    return response.ok(tickets);
  } catch (error) {
    return handleError(error);
  }
}

export async function POST(request: NextRequest) {
  try {
    const auth = await authenticate(request);
    const body = await request.json();
    const parsed = createEmergencyTicketSchema.safeParse(body);
    if (!parsed.success) throw new ValidationError(parsed.error.flatten());

    const ticket = await emergencyService.create(auth.auth.companyId, auth.auth.userId, parsed.data);
    return response.created(ticket, "Emergency ticket created");
  } catch (error) {
    return handleError(error);
  }
}
