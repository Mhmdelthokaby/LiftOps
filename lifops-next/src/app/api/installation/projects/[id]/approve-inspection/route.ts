import { NextRequest } from "next/server";
import { InspectionService } from "@/lib/services/installation";
import { response, handleError } from "@/lib/response";
import { authenticate } from "@/lib/auth/middleware";

type RouteParams = { params: Promise<{ id: string }> };
const inspectionService = new InspectionService();

export async function POST(request: NextRequest, { params }: RouteParams) {
  try {
    await authenticate(request);
    const { id } = await params;
    const project = await inspectionService.approve(id);
    return response.ok(project, "Inspection approved");
  } catch (error) {
    return handleError(error);
  }
}
