import { NextRequest } from "next/server";
import { ItemService } from "@/lib/services/inventory";
import { createItemSchema, itemQuerySchema } from "@/lib/validators/inventory";
import { response, handleError } from "@/lib/response";
import { ValidationError } from "@/lib/errors";
import { authenticate } from "@/lib/auth/middleware";

const itemService = new ItemService();

export async function GET(request: NextRequest) {
  try {
    const auth = await authenticate(request);
    const query = itemQuerySchema.parse(Object.fromEntries(request.nextUrl.searchParams));
    const result = await itemService.list(auth.auth.companyId, query);
    return response.paginated(result.items, result.meta);
  } catch (error) {
    return handleError(error);
  }
}

export async function POST(request: NextRequest) {
  try {
    const auth = await authenticate(request);
    const body = await request.json();
    const parsed = createItemSchema.safeParse(body);
    if (!parsed.success) throw new ValidationError(parsed.error.flatten());
    const item = await itemService.create(auth.auth.companyId, parsed.data);
    return response.created(item, "Item added successfully");
  } catch (error) {
    return handleError(error);
  }
}
