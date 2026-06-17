import { NextRequest } from "next/server";
import { MaintenanceVisitService } from "@/lib/services/maintenance";
import { completeVisitSchema, updateVisitStatusSchema } from "@/lib/validators/maintenance";
import { response, handleError } from "@/lib/response";
import { ValidationError } from "@/lib/errors";
import { authenticate } from "@/lib/auth/middleware";

type RouteParams = { params: Promise<{ id: string }> };
const visitService = new MaintenanceVisitService();

export async function GET(request: NextRequest, { params }: RouteParams) {
  try {
    await authenticate(request);
    const { id } = await params;
    const visit = await visitService.getById(id);
    return response.ok(visit);
  } catch (error) {
    return handleError(error);
  }
}

export async function PUT(request: NextRequest, { params }: RouteParams) {
  try {
    await authenticate(request);
    const { id } = await params;
    const body = await request.json();

    if (body.status) {
      const parsed = updateVisitStatusSchema.safeParse(body);
      if (!parsed.success) throw new ValidationError(parsed.error.flatten());
      const visit = await visitService.updateStatus(id, parsed.data);
      return response.ok(visit, "Visit status updated");
    }

    const parsed = completeVisitSchema.safeParse(body);
    if (!parsed.success) throw new ValidationError(parsed.error.flatten());
    const visit = await visitService.complete(id, parsed.data);
    return response.ok(visit, "Visit completed");
  } catch (error) {
    return handleError(error);
  }
}
