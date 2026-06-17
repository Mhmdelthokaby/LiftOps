import { NextRequest } from "next/server";
import { ElevatorService } from "@/lib/services/installation";
import { updateElevatorSchema } from "@/lib/validators/installation";
import { response, handleError } from "@/lib/response";
import { ValidationError } from "@/lib/errors";
import { authenticate } from "@/lib/auth/middleware";

type RouteParams = { params: Promise<{ id: string }> };
const elevatorService = new ElevatorService();

export async function GET(request: NextRequest, { params }: RouteParams) {
  try {
    await authenticate(request);
    const { id } = await params;
    const elevator = await elevatorService.getById(id);
    return response.ok(elevator);
  } catch (error) {
    return handleError(error);
  }
}

export async function PUT(request: NextRequest, { params }: RouteParams) {
  try {
    await authenticate(request);
    const { id } = await params;
    const body = await request.json();
    const parsed = updateElevatorSchema.safeParse(body);
    if (!parsed.success) throw new ValidationError(parsed.error.flatten());

    const elevator = await elevatorService.update(id, parsed.data);
    return response.ok(elevator, "Elevator updated successfully");
  } catch (error) {
    return handleError(error);
  }
}

export async function DELETE(request: NextRequest, { params }: RouteParams) {
  try {
    await authenticate(request);
    const { id } = await params;
    await elevatorService.delete(id);
    return response.noContent();
  } catch (error) {
    return handleError(error);
  }
}
