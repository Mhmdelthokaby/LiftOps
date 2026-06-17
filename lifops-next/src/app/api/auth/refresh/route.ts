import { NextRequest, NextResponse } from "next/server";
import { AuthService } from "@/lib/services/auth.service";
import { refreshTokenSchema } from "@/lib/auth/schemas";
import { response, handleError } from "@/lib/response";
import { ValidationError } from "@/lib/errors";
import { getRefreshTokenFromCookies, createAccessTokenCookie, createRefreshTokenCookie } from "@/lib/auth/session";

const authService = new AuthService();

export async function POST(request: NextRequest) {
  try {
    let refreshToken: string | undefined;

    const body = await request.json().catch(() => ({}));
    const parsed = refreshTokenSchema.safeParse(body);

    if (parsed.success) {
      refreshToken = parsed.data.refreshToken;
    } else {
      const cookieToken = await getRefreshTokenFromCookies();
      if (cookieToken) {
        refreshToken = cookieToken;
      } else {
        throw new ValidationError(parsed.error.flatten());
      }
    }

    const result = await authService.refresh(refreshToken);

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
