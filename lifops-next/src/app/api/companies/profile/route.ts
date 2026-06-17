import { NextRequest } from "next/server";
import { CompanyService } from "@/lib/services/company.service";
import { updateCompanySchema } from "@/lib/validators/company";
import { response, handleError } from "@/lib/response";
import { ValidationError } from "@/lib/errors";
import { authenticate } from "@/lib/auth/middleware";

const companyService = new CompanyService();

export async function GET(request: NextRequest) {
  try {
    const auth = await authenticate(request);
    const company = await companyService.getById(auth.auth.companyId);
    return response.ok({
      id: company.id,
      name: company.name,
      slug: company.slug,
      email: company.email,
      phone: company.phone,
      address: company.address,
      isActive: company.isActive,
      subscription: company.subscription,
    });
  } catch (error) {
    return handleError(error);
  }
}

export async function PUT(request: NextRequest) {
  try {
    const auth = await authenticate(request);
    const body = await request.json();
    const parsed = updateCompanySchema.safeParse(body);

    if (!parsed.success) {
      throw new ValidationError(parsed.error.flatten());
    }

    // Tenants cannot change their own active status or plan
    const sanitized = { ...parsed.data };
    delete sanitized.isActive;

    const company = await companyService.update(auth.auth.companyId, sanitized);
    return response.ok(company, "Company profile updated successfully");
  } catch (error) {
    return handleError(error);
  }
}
