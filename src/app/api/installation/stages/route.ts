import { NextRequest } from "next/server";
import { StageService } from "@/lib/services/installation";
import { createStageSchema } from "@/lib/validators/installation";
import { response, handleError } from "@/lib/response";
import { ValidationError } from "@/lib/errors";
import { authenticate } from "@/lib/auth/middleware";

const stageService = new StageService();

export async function GET(request: NextRequest) {
  try {
    await authenticate(request);
    const projectId = request.nextUrl.searchParams.get("projectId");
    if (!projectId) return response.badRequest("projectId query parameter is required");

    const stages = await stageService.listByProject(projectId);
    return response.ok(stages);
  } catch (error) {
    return handleError(error);
  }
}

export async function POST(request: NextRequest) {
  try {
    const auth = await authenticate(request);
    const body = await request.json();
    const parsed = createStageSchema.safeParse(body);
    if (!parsed.success) throw new ValidationError(parsed.error.flatten());

    const stage = await stageService.create(auth.auth.companyId, parsed.data);
    return response.created(stage, "Stage created successfully");
  } catch (error) {
    return handleError(error);
  }
}
