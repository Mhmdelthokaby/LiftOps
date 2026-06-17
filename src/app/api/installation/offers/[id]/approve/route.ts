import { NextRequest } from "next/server";
import { OfferService } from "@/lib/services/installation";
import { response, handleError } from "@/lib/response";
import { authenticate } from "@/lib/auth/middleware";

type RouteParams = { params: Promise<{ id: string }> };
const offerService = new OfferService();

export async function POST(request: NextRequest, { params }: RouteParams) {
  try {
    await authenticate(request);
    const { id } = await params;
    await offerService.approve(id);
    return response.ok(null, "Offer approved");
  } catch (error) {
    return handleError(error);
  }
}
