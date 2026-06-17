import { NextRequest } from "next/server";
import { CompanyService } from "@/lib/services/company.service";
import { createCompanySchema, companyQuerySchema } from "@/lib/validators/company";
import { response, handleError } from "@/lib/response";
import { ValidationError, ForbiddenError } from "@/lib/errors";
import { authenticate } from "@/lib/auth/middleware";

const companyService = new CompanyService();

export async function GET(request: NextRequest) {
  try {
    const auth = await authenticate(request);
    if (auth.auth.type !== "admin") {
      throw new ForbiddenError("Only platform admins can list companies");
    }

    const { searchParams } = request.nextUrl;
    const parsed = companyQuerySchema.parse(Object.fromEntries(searchParams));
    const result = await companyService.list(parsed);
    return response.ok({
      items: result.items,
      totalCount: result.meta.totalItems,
      page: result.meta.page,
      pageSize: result.meta.pageSize,
    });
  } catch (error) {
    return handleError(error);
  }
}

export async function POST(request: NextRequest) {
  try {
    const auth = await authenticate(request);
    if (auth.auth.type !== "admin") {
      throw new ForbiddenError("Only platform admins can create companies");
    }

    const body = await request.json();
    const parsed = createCompanySchema.safeParse(body);

    if (!parsed.success) {
      throw new ValidationError(parsed.error.flatten());
    }

    const company = await companyService.create(parsed.data);
    return response.created(company, "Company created successfully");
  } catch (error) {
    return handleError(error);
  }
}
