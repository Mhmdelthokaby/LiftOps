import { NextRequest } from "next/server";
import { prisma } from "@/lib/db/client";
import { response, handleError } from "@/lib/response";
import { authenticate } from "@/lib/auth/middleware";

export async function GET(request: NextRequest) {
  try {
    const auth = await authenticate(request);
    const categories = await prisma.itemCategory.findMany({
      where: { companyId: auth.auth.companyId },
      include: { _count: { select: { items: true } } },
      orderBy: { name: "asc" },
    });
    return response.ok(categories);
  } catch (error) {
    return handleError(error);
  }
}

export async function POST(request: NextRequest) {
  try {
    const auth = await authenticate(request);
    const body = await request.json();
    const category = await prisma.itemCategory.create({
      data: { name: body.name, description: body.description, companyId: auth.auth.companyId },
    });
    return response.created(category, "Category created");
  } catch (error) {
    return handleError(error);
  }
}
