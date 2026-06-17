import { NextRequest } from "next/server";
import { StageService } from "@/lib/services/installation";
import { response, handleError } from "@/lib/response";
import { authenticate } from "@/lib/auth/middleware";

type RouteParams = { params: Promise<{ id: string }> };
const stageService = new StageService();

export async function GET(request: NextRequest, { params }: RouteParams) {
  try {
    await authenticate(request);
    const { id } = await params;
    const stage = await stageService.getById(id);
    return response.ok(stage);
  } catch (error) {
    return handleError(error);
  }
}
