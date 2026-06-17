import { NextRequest } from "next/server";
import { EmergencyService } from "@/lib/services/emergency";
import { assignEmergencyTechnicianSchema } from "@/lib/validators/emergency";
import { response, handleError } from "@/lib/response";
import { ValidationError } from "@/lib/errors";
import { authenticate } from "@/lib/auth/middleware";

type RouteParams = { params: Promise<{ id: string }> };
const emergencyService = new EmergencyService();

export async function POST(request: NextRequest, { params }: RouteParams) {
  try {
    await authenticate(request);
    const { id } = await params;
    const body = await request.json();
    const parsed = assignEmergencyTechnicianSchema.safeParse(body);
    if (!parsed.success) throw new ValidationError(parsed.error.flatten());
    const ticket = await emergencyService.assign(id, parsed.data.technicianId);
    return response.ok(ticket, "Technician dispatched");
  } catch (error) {
    return handleError(error);
  }
}
