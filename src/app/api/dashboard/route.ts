import { NextRequest } from "next/server";
import { DashboardService } from "@/lib/services/dashboard";
import { response, handleError } from "@/lib/response";
import { authenticate } from "@/lib/auth/middleware";

const dashboardService = new DashboardService();

export async function GET(request: NextRequest) {
  try {
    const auth = await authenticate(request);
    const overview = await dashboardService.getCompanyOverview(auth.auth.companyId);
    return response.ok(overview);
  } catch (error) {
    return handleError(error);
  }
}
