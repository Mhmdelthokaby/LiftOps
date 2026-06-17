import { NextRequest } from "next/server";
import { MaintenanceContractService } from "@/lib/services/maintenance";
import { response, handleError } from "@/lib/response";
import { authenticate } from "@/lib/auth/middleware";

type RouteParams = { params: Promise<{ id: string }> };
const contractService = new MaintenanceContractService();

export async function GET(request: NextRequest, { params }: RouteParams) {
  try {
    await authenticate(request);
    const { id } = await params;
    const contract = await contractService.getById(id);
    return response.ok(contract);
  } catch (error) {
    return handleError(error);
  }
}
