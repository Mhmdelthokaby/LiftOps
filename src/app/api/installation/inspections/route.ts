import { NextRequest } from "next/server";
import { InspectionService } from "@/lib/services/installation";
import { response, handleError } from "@/lib/response";
import { ValidationError } from "@/lib/errors";
import { authenticate } from "@/lib/auth/middleware";

const inspectionService = new InspectionService();

export async function POST(request: NextRequest) {
  try {
    const auth = await authenticate(request);
    const body = await request.json();
    if (!body.customerId) throw new ValidationError("customerId is required");

    const project = await inspectionService.create(auth.auth.companyId, auth.auth.userId, body);
    return response.created(project, "Inspection created");
  } catch (error) {
    return handleError(error);
  }
}
