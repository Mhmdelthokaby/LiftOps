import { NextRequest } from "next/server";
import { PlatformDashboardService } from "@/lib/services/dashboard/platform.service";
import { response, handleError } from "@/lib/response";
import { authenticate } from "@/lib/auth/middleware";
import { ForbiddenError } from "@/lib/errors";

const platformService = new PlatformDashboardService();

export async function GET(request: NextRequest) {
  try {
    const auth = await authenticate(request);
    if (auth.auth.type !== "admin") {
      throw new ForbiddenError("Only platform administrators can access this endpoint");
    }
    const stats = await platformService.getStats();
    return response.ok(stats);
  } catch (error) {
    return handleError(error);
  }
}
