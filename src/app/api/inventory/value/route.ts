import { NextRequest } from "next/server";
import { ItemService } from "@/lib/services/inventory";
import { response, handleError } from "@/lib/response";
import { authenticate } from "@/lib/auth/middleware";

const itemService = new ItemService();

export async function GET(request: NextRequest) {
  try {
    const auth = await authenticate(request);
    const totalValue = await itemService.getTotalValue(auth.auth.companyId);
    return response.ok({ totalValue });
  } catch (error) {
    return handleError(error);
  }
}
