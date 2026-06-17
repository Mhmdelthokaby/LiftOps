import { NextRequest } from "next/server";
import { EmergencyService } from "@/lib/services/emergency";
import { response, handleError } from "@/lib/response";
import { authenticate } from "@/lib/auth/middleware";

const emergencyService = new EmergencyService();

export async function GET(request: NextRequest) {
  try {
    const auth = await authenticate(request);
    const tickets = await emergencyService.listOpen(auth.auth.companyId);
    return response.ok(tickets);
  } catch (error) {
    return handleError(error);
  }
}
