import { NextRequest, NextResponse } from "next/server";
import { AuthService } from "@/lib/services/auth.service";
import { response, handleError } from "@/lib/response";
import { createClearAccessTokenCookie, createClearRefreshTokenCookie, getRefreshTokenFromCookies } from "@/lib/auth/session";

const authService = new AuthService();

export async function POST(request: NextRequest) {
  try {
    const refreshToken = await getRefreshTokenFromCookies();
    await authService.logout(refreshToken ?? undefined);

    const res = NextResponse.json(response.ok(null, "Logged out successfully"), { status: 200 });
    res.cookies.set(createClearAccessTokenCookie());
    res.cookies.set(createClearRefreshTokenCookie());

    return res;
  } catch (error) {
    return handleError(error);
  }
}
