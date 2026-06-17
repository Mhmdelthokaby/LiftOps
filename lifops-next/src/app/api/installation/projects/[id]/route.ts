import { NextRequest } from "next/server";
import { ProjectService, ElevatorService, StageService } from "@/lib/services/installation";
import { updateProjectSchema } from "@/lib/validators/installation";
import { response, handleError } from "@/lib/response";
import { ValidationError } from "@/lib/errors";
import { authenticate } from "@/lib/auth/middleware";

type RouteParams = { params: Promise<{ id: string }> };
const projectService = new ProjectService();
const elevatorService = new ElevatorService();
const stageService = new StageService();

export async function GET(request: NextRequest, { params }: RouteParams) {
  try {
    await authenticate(request);
    const { id } = await params;
    const project = await projectService.getById(id);
    return response.ok(project);
  } catch (error) {
    return handleError(error);
  }
}

export async function PUT(request: NextRequest, { params }: RouteParams) {
  try {
    await authenticate(request);
    const { id } = await params;
    const body = await request.json();
    const parsed = updateProjectSchema.safeParse(body);
    if (!parsed.success) throw new ValidationError(parsed.error.flatten());

    const project = await projectService.update(id, parsed.data);
    return response.ok(project, "Project updated successfully");
  } catch (error) {
    return handleError(error);
  }
}

export async function DELETE(request: NextRequest, { params }: RouteParams) {
  try {
    await authenticate(request);
    const { id } = await params;
    await projectService.delete(id);
    return response.noContent();
  } catch (error) {
    return handleError(error);
  }
}
