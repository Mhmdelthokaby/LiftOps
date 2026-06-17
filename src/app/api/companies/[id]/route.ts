import { NextRequest } from "next/server";
import { CompanyService } from "@/lib/services/company.service";
import { updateCompanySchema } from "@/lib/validators/company";
import { response, handleError } from "@/lib/response";
import { ValidationError, ForbiddenError } from "@/lib/errors";
import { authenticate } from "@/lib/auth/middleware";

type RouteParams = { params: Promise<{ id: string }> };

const companyService = new CompanyService();

export async function GET(request: NextRequest, { params }: RouteParams) {
  try {
    const auth = await authenticate(request);
    const { id } = await params;

    if (auth.auth.role !== "SUPER_ADMIN" && auth.auth.companyId !== id) {
      throw new ForbiddenError("Access denied");
    }

    const company = await companyService.getById(id);
    return response.ok(company);
  } catch (error) {
    return handleError(error);
  }
}

export async function PUT(request: NextRequest, { params }: RouteParams) {
  try {
    const auth = await authenticate(request);
    const { id } = await params;

    if (auth.auth.role !== "SUPER_ADMIN" && auth.auth.companyId !== id) {
      throw new ForbiddenError("Access denied");
    }

    const body = await request.json();
    const parsed = updateCompanySchema.safeParse(body);

    if (!parsed.success) {
      throw new ValidationError(parsed.error.flatten());
    }

    const company = await companyService.update(id, parsed.data);
    return response.ok(company, "Company updated successfully");
  } catch (error) {
    return handleError(error);
  }
}

export async function DELETE(request: NextRequest, { params }: RouteParams) {
  try {
    const auth = await authenticate(request);
    if (auth.auth.role !== "SUPER_ADMIN") {
      throw new ForbiddenError("Only platform admins can delete companies");
    }

    const { id } = await params;
    await companyService.delete(id);
    return response.noContent();
  } catch (error) {
    return handleError(error);
  }
}
