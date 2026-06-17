import { NextRequest } from "next/server";
import { CategoryService } from "@/lib/services/inventory";
import { createCategorySchema } from "@/lib/validators/inventory";
import { response, handleError } from "@/lib/response";
import { ValidationError } from "@/lib/errors";
import { authenticate } from "@/lib/auth/middleware";

const categoryService = new CategoryService();

export async function GET(request: NextRequest) {
  try {
    const auth = await authenticate(request);
    const categories = await categoryService.list(auth.auth.companyId);
    return response.ok(categories);
  } catch (error) {
    return handleError(error);
  }
}

export async function POST(request: NextRequest) {
  try {
    const auth = await authenticate(request);
    const body = await request.json();
    const parsed = createCategorySchema.safeParse(body);
    if (!parsed.success) throw new ValidationError(parsed.error.flatten());
    const category = await categoryService.create(auth.auth.companyId, parsed.data);
    return response.created(category, "Category created successfully");
  } catch (error) {
    return handleError(error);
  }
}
