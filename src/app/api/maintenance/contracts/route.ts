import { NextRequest } from "next/server";
import { MaintenanceContractService } from "@/lib/services/maintenance";
import { createContractSchema } from "@/lib/validators/maintenance";
import { response, handleError } from "@/lib/response";
import { ValidationError } from "@/lib/errors";
import { authenticate } from "@/lib/auth/middleware";

const contractService = new MaintenanceContractService();

export async function GET(request: NextRequest) {
  try {
    const auth = await authenticate(request);
    const { searchParams } = request.nextUrl;
    const page = parseInt(searchParams.get("page") ?? "1");
    const pageSize = parseInt(searchParams.get("pageSize") ?? "20");
    const result = await contractService.list(auth.auth.companyId, page, pageSize);
    return response.paginated(result.items, result.meta);
  } catch (error) {
    return handleError(error);
  }
}

export async function POST(request: NextRequest) {
  try {
    const auth = await authenticate(request);
    const body = await request.json();
    const parsed = createContractSchema.safeParse(body);
    if (!parsed.success) throw new ValidationError(parsed.error.flatten());
    const contract = await contractService.create(auth.auth.companyId, parsed.data);
    return response.created(contract, "Contract created successfully");
  } catch (error) {
    return handleError(error);
  }
}
