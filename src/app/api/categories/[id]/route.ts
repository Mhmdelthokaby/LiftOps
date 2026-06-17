import { NextRequest } from "next/server";
import { prisma } from "@/lib/db/client";
import { response, handleError } from "@/lib/response";
import { NotFoundError } from "@/lib/errors";
import { authenticate } from "@/lib/auth/middleware";

type RouteParams = { params: Promise<{ id: string }> };

export async function PUT(request: NextRequest, { params }: RouteParams) {
  try {
    await authenticate(request);
    const { id } = await params;
    const body = await request.json();

    const existing = await prisma.itemCategory.findUnique({ where: { id } });
    if (!existing) throw new NotFoundError("Category", id);

    const category = await prisma.itemCategory.update({ where: { id }, data: body });
    return response.ok(category, "Category updated");
  } catch (error) {
    return handleError(error);
  }
}

export async function DELETE(request: NextRequest, { params }: RouteParams) {
  try {
    await authenticate(request);
    const { id } = await params;

    const existing = await prisma.itemCategory.findUnique({ where: { id } });
    if (!existing) throw new NotFoundError("Category", id);

    await prisma.itemCategory.update({ where: { id }, data: { isActive: false } });
    return response.noContent();
  } catch (error) {
    return handleError(error);
  }
}
