import { NextRequest } from "next/server";
import { prisma } from "@/lib/db/client";
import { response, handleError } from "@/lib/response";
import { ValidationError } from "@/lib/errors";
import { authenticate } from "@/lib/auth/middleware";

export async function POST(request: NextRequest) {
  try {
    const auth = await authenticate(request);
    const body = await request.json();
    if (!body.projectId || !body.price) throw new ValidationError("projectId and price are required");

    const quotation = await prisma.project.update({
      where: { id: body.projectId },
      data: { notes: body.notes },
    });

    return response.created(quotation, "Quotation created");
  } catch (error) {
    return handleError(error);
  }
}
