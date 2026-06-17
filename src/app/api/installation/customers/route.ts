import { NextRequest } from "next/server";
import { CustomerService } from "@/lib/services/installation";
import { createCustomerSchema } from "@/lib/validators/installation";
import { response, handleError } from "@/lib/response";
import { ValidationError } from "@/lib/errors";
import { authenticate } from "@/lib/auth/middleware";

const customerService = new CustomerService();

export async function GET(request: NextRequest) {
  try {
    const auth = await authenticate(request);
    const { searchParams } = request.nextUrl;
    const page = parseInt(searchParams.get("page") ?? "1");
    const pageSize = parseInt(searchParams.get("pageSize") ?? "20");
    const search = searchParams.get("search") ?? undefined;

    const result = await customerService.list(auth.auth.companyId, page, pageSize, search);
    return response.paginated(result.items, result.meta);
  } catch (error) {
    return handleError(error);
  }
}

export async function POST(request: NextRequest) {
  try {
    const auth = await authenticate(request);
    const body = await request.json();
    const parsed = createCustomerSchema.safeParse(body);
    if (!parsed.success) throw new ValidationError(parsed.error.flatten());

    const customer = await customerService.create(auth.auth.companyId, parsed.data);
    return response.created(customer, "Customer created successfully");
  } catch (error) {
    return handleError(error);
  }
}
