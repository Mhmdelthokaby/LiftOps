import { NextRequest, NextResponse } from "next/server";
import { AdminAuthService } from "@/lib/services/admin-auth.service";
import { handleError } from "@/lib/response";
import { createAccessTokenCookie, createRefreshTokenCookie } from "@/lib/auth/session";

const adminAuthService = new AdminAuthService();

export async function POST(request: NextRequest) {
  try {
    const body = await request.json();
    const { token, refreshToken } = body as { token?: string; refreshToken?: string };

    if (!refreshToken) {
      return NextResponse.json({ message: "Refresh token is required" }, { status: 400 });
    }

    const result = await adminAuthService.refresh(refreshToken);

    const res = NextResponse.json({
      token: result.accessToken,
      refreshToken: result.refreshToken,
      refreshTokenExpiry: new Date(result.refreshTokenExpiresAt * 1000).toISOString(),
      name: `${result.user.firstName} ${result.user.lastName}`.trim(),
      email: result.user.email,
      roles: [result.user.role],
    });

    res.cookies.set(createAccessTokenCookie(result.accessToken, result.accessTokenExpiresAt));
    res.cookies.set(createRefreshTokenCookie(result.refreshToken, result.refreshTokenExpiresAt));

    return res;
  } catch (error) {
    return handleError(error);
  }
}
