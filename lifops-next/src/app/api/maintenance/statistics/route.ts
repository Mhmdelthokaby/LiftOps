import { NextRequest } from "next/server";
import { MaintenanceVisitService } from "@/lib/services/maintenance";
import { response, handleError } from "@/lib/response";
import { authenticate } from "@/lib/auth/middleware";

const visitService = new MaintenanceVisitService();

export async function GET(request: NextRequest) {
  try {
    const auth = await authenticate(request);
    const { searchParams } = request.nextUrl;
    const month = parseInt(searchParams.get("month") ?? new Date().getMonth().toString()) + 1;
    const year = parseInt(searchParams.get("year") ?? new Date().getFullYear().toString());

    if (month < 1 || month > 12) return response.badRequest("Month must be between 1 and 12");
    if (year < 2000 || year > 2100) return response.badRequest("Year must be between 2000 and 2100");

    const stats = await visitService.getStatistics(auth.auth.companyId, month, year);
    return response.ok(stats);
  } catch (error) {
    return handleError(error);
  }
}
