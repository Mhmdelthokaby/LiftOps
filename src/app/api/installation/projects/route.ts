import { NextRequest } from "next/server";
import { ProjectService } from "@/lib/services/installation";
import { createProjectSchema, projectQuerySchema } from "@/lib/validators/installation";
import { response, handleError } from "@/lib/response";
import { ValidationError } from "@/lib/errors";
import { authenticate } from "@/lib/auth/middleware";

const projectService = new ProjectService();

export async function GET(request: NextRequest) {
  try {
    const auth = await authenticate(request);
    const query = projectQuerySchema.parse(Object.fromEntries(request.nextUrl.searchParams));
    const result = await projectService.list(auth.auth.companyId, query);
    return response.paginated(result.items, result.meta);
  } catch (error) {
    return handleError(error);
  }
}

export async function POST(request: NextRequest) {
  try {
    const auth = await authenticate(request);
    const body = await request.json();
    const parsed = createProjectSchema.safeParse(body);
    if (!parsed.success) throw new ValidationError(parsed.error.flatten());

    const project = await projectService.create(auth.auth.companyId, parsed.data);
    return response.created(project, "Project created successfully");
  } catch (error) {
    return handleError(error);
  }
}
