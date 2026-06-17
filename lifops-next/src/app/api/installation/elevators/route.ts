import { NextRequest } from "next/server";
import { ElevatorService } from "@/lib/services/installation";
import { createElevatorSchema } from "@/lib/validators/installation";
import { response, handleError } from "@/lib/response";
import { ValidationError } from "@/lib/errors";
import { authenticate } from "@/lib/auth/middleware";

const elevatorService = new ElevatorService();

export async function GET(request: NextRequest) {
  try {
    const auth = await authenticate(request);
    const projectId = request.nextUrl.searchParams.get("projectId");

    if (projectId) {
      const elevators = await elevatorService.listByProject(projectId);
      return response.ok(elevators);
    }

    const elevators = await elevatorService.listByCompany(auth.auth.companyId);
    return response.ok(elevators);
  } catch (error) {
    return handleError(error);
  }
}

export async function POST(request: NextRequest) {
  try {
    const auth = await authenticate(request);
    const body = await request.json();
    const parsed = createElevatorSchema.safeParse(body);
    if (!parsed.success) throw new ValidationError(parsed.error.flatten());

    const elevator = await elevatorService.create(auth.auth.companyId, parsed.data);
    return response.created(elevator, "Elevator added successfully");
  } catch (error) {
    return handleError(error);
  }
}
