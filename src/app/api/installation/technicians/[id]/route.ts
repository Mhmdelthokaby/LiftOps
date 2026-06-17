import { NextRequest } from "next/server";
import { TechnicianService } from "@/lib/services/installation";
import { response, handleError } from "@/lib/response";
import { authenticate } from "@/lib/auth/middleware";

type RouteParams = { params: Promise<{ id: string }> };
const technicianService = new TechnicianService();

export async function GET(request: NextRequest, { params }: RouteParams) {
  try {
    await authenticate(request);
    const { id } = await params;
    const technician = await technicianService.getById(id);
    return response.ok(technician);
  } catch (error) {
    return handleError(error);
  }
}
