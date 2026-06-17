import { NextRequest } from "next/server";
import { ItemService } from "@/lib/services/inventory";
import { updateItemSchema } from "@/lib/validators/inventory";
import { response, handleError } from "@/lib/response";
import { ValidationError } from "@/lib/errors";
import { authenticate } from "@/lib/auth/middleware";

type RouteParams = { params: Promise<{ id: string }> };
const itemService = new ItemService();

export async function GET(request: NextRequest, { params }: RouteParams) {
  try {
    await authenticate(request);
    const { id } = await params;
    const item = await itemService.getById(id);
    return response.ok(item);
  } catch (error) {
    return handleError(error);
  }
}

export async function PUT(request: NextRequest, { params }: RouteParams) {
  try {
    await authenticate(request);
    const { id } = await params;
    const body = await request.json();
    const parsed = updateItemSchema.safeParse(body);
    if (!parsed.success) throw new ValidationError(parsed.error.flatten());
    const item = await itemService.update(id, parsed.data);
    return response.ok(item, "Item updated successfully");
  } catch (error) {
    return handleError(error);
  }
}

export async function DELETE(request: NextRequest, { params }: RouteParams) {
  try {
    await authenticate(request);
    const { id } = await params;
    await itemService.delete(id);
    return response.noContent();
  } catch (error) {
    return handleError(error);
  }
}
