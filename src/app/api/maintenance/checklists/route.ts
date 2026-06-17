import { NextRequest } from "next/server";
import { MaintenanceChecklistService } from "@/lib/services/maintenance";
import { createChecklistSchema } from "@/lib/validators/maintenance";
import { response, handleError } from "@/lib/response";
import { ValidationError } from "@/lib/errors";
import { authenticate } from "@/lib/auth/middleware";

const checklistService = new MaintenanceChecklistService();

export async function GET(request: NextRequest) {
  try {
    const auth = await authenticate(request);
    const includeInactive = request.nextUrl.searchParams.get("includeInactive") === "true";
    const checklists = await checklistService.list(auth.auth.companyId, includeInactive);
    return response.ok(checklists);
  } catch (error) {
    return handleError(error);
  }
}

export async function POST(request: NextRequest) {
  try {
    const auth = await authenticate(request);
    const body = await request.json();
    const parsed = createChecklistSchema.safeParse(body);
    if (!parsed.success) throw new ValidationError(parsed.error.flatten());
    const checklist = await checklistService.create(auth.auth.companyId, parsed.data);
    return response.created(checklist, "Checklist created successfully");
  } catch (error) {
    return handleError(error);
  }
}
