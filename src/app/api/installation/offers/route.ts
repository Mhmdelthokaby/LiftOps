import { NextRequest } from "next/server";
import { OfferService } from "@/lib/services/installation";
import { prisma } from "@/lib/db/client";
import { response, handleError } from "@/lib/response";
import { ValidationError } from "@/lib/errors";
import { authenticate } from "@/lib/auth/middleware";

const offerService = new OfferService();

export async function POST(request: NextRequest) {
  try {
    const auth = await authenticate(request);
    const body = await request.json();
    if (!body.inspectionRequestId) throw new ValidationError("inspectionRequestId is required");

    const result = await offerService.create(auth.auth.companyId, body);
    return response.created(result, "Offer created");
  } catch (error) {
    return handleError(error);
  }
}

export async function GET(request: NextRequest) {
  try {
    const auth = await authenticate(request);
    const status = request.nextUrl.searchParams.get("status");

    const where: Record<string, unknown> = { companyId: auth.auth.companyId };
    if (status) where.projectStatus = status;

    const offers = await prisma.project.findMany({
      where,
      include: {
        customer: true,
        elevators: true,
      },
      orderBy: { createdAt: "desc" },
    });

    return response.ok(offers);
  } catch (error) {
    return handleError(error);
  }
}
