import { NextRequest } from "next/server";
import { EmergencyService } from "@/lib/services/emergency";
import { updateEmergencyTicketSchema } from "@/lib/validators/emergency";
import { response, handleError } from "@/lib/response";
import { ValidationError } from "@/lib/errors";
import { authenticate } from "@/lib/auth/middleware";

type RouteParams = { params: Promise<{ id: string }> };
const emergencyService = new EmergencyService();

export async function GET(request: NextRequest, { params }: RouteParams) {
  try {
    await authenticate(request);
    const { id } = await params;
    const ticket = await emergencyService.getById(id);
    return response.ok(ticket);
  } catch (error) {
    return handleError(error);
  }
}

export async function PUT(request: NextRequest, { params }: RouteParams) {
  try {
    await authenticate(request);
    const { id } = await params;
    const body = await request.json();
    const parsed = updateEmergencyTicketSchema.safeParse(body);
    if (!parsed.success) throw new ValidationError(parsed.error.flatten());
    const ticket = await emergencyService.update(id, parsed.data);
    return response.ok(ticket, "Emergency ticket updated");
  } catch (error) {
    return handleError(error);
  }
}
