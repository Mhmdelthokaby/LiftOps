import { NextRequest } from "next/server";
import { prisma } from "@/lib/db/client";
import { response, handleError } from "@/lib/response";
import { NotFoundError } from "@/lib/errors";
import { authenticate } from "@/lib/auth/middleware";

type RouteParams = { params: Promise<{ id: string }> };

export async function POST(request: NextRequest, { params }: RouteParams) {
  try {
    await authenticate(request);
    const { id } = await params;
    const project = await prisma.project.findUnique({ where: { id } });
    if (!project) throw new NotFoundError("Quotation", id);

    await prisma.project.update({ where: { id }, data: { status: "IN_PROGRESS" } });
    return response.ok(null, "Quotation approved");
  } catch (error) {
    return handleError(error);
  }
}
