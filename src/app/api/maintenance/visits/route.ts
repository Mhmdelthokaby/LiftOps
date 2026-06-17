import { NextRequest } from "next/server";
import { MaintenanceVisitService } from "@/lib/services/maintenance";
import { scheduleVisitSchema } from "@/lib/validators/maintenance";
import { response, handleError } from "@/lib/response";
import { ValidationError } from "@/lib/errors";
import { authenticate } from "@/lib/auth/middleware";

const visitService = new MaintenanceVisitService();

export async function POST(request: NextRequest) {
  try {
    const auth = await authenticate(request);
    const body = await request.json();
    const parsed = scheduleVisitSchema.safeParse(body);
    if (!parsed.success) throw new ValidationError(parsed.error.flatten());
    const visit = await visitService.schedule(auth.auth.companyId, parsed.data);
    return response.created(visit, "Visit scheduled successfully");
  } catch (error) {
    return handleError(error);
  }
}
