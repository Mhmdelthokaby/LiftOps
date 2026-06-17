import { NextRequest } from "next/server";
import { CustomerService } from "@/lib/services/installation";
import { updateCustomerSchema } from "@/lib/validators/installation";
import { response, handleError } from "@/lib/response";
import { ValidationError } from "@/lib/errors";
import { authenticate } from "@/lib/auth/middleware";

type RouteParams = { params: Promise<{ id: string }> };
const customerService = new CustomerService();

export async function GET(request: NextRequest, { params }: RouteParams) {
  try {
    await authenticate(request);
    const { id } = await params;
    const customer = await customerService.getById(id);
    return response.ok(customer);
  } catch (error) {
    return handleError(error);
  }
}

export async function PUT(request: NextRequest, { params }: RouteParams) {
  try {
    await authenticate(request);
    const { id } = await params;
    const body = await request.json();
    const parsed = updateCustomerSchema.safeParse(body);
    if (!parsed.success) throw new ValidationError(parsed.error.flatten());

    const customer = await customerService.update(id, parsed.data);
    return response.ok(customer, "Customer updated successfully");
  } catch (error) {
    return handleError(error);
  }
}

export async function DELETE(request: NextRequest, { params }: RouteParams) {
  try {
    await authenticate(request);
    const { id } = await params;
    await customerService.delete(id);
    return response.noContent();
  } catch (error) {
    return handleError(error);
  }
}
