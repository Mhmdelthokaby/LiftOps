import { NextRequest } from "next/server";
import { AuthService } from "@/lib/services/auth.service";
import { response, handleError } from "@/lib/response";
import { authenticate } from "@/lib/auth/middleware";
import type { AuthenticatedRequest } from "@/lib/auth/middleware";

const authService = new AuthService();

export async function GET(request: NextRequest) {
  try {
    const authenticated = await authenticate(request);
    const profile = await authService.getProfile(authenticated.auth.userId);
    return response.ok(profile);
  } catch (error) {
    return handleError(error);
  }
}
