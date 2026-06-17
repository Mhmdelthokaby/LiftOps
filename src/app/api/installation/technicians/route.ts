import { NextRequest } from "next/server";
import { TechnicianService } from "@/lib/services/installation";
import { createTechnicianSchema } from "@/lib/validators/installation";
import { response, handleError } from "@/lib/response";
import { ValidationError } from "@/lib/errors";
import { authenticate } from "@/lib/auth/middleware";

const technicianService = new TechnicianService();

export async function GET(request: NextRequest) {
  try {
    const auth = await authenticate(request);
    const technicians = await technicianService.list(auth.auth.companyId);
    return response.ok(technicians);
  } catch (error) {
    return handleError(error);
  }
}

export async function POST(request: NextRequest) {
  try {
    const auth = await authenticate(request);
    const body = await request.json();
    const parsed = createTechnicianSchema.safeParse(body);
    if (!parsed.success) throw new ValidationError(parsed.error.flatten());

    const technician = await technicianService.create(auth.auth.companyId, parsed.data);
    return response.created(technician, "Technician registered successfully");
  } catch (error) {
    return handleError(error);
  }
}
