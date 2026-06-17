import { NextRequest, NextResponse } from "next/server";
import { AuthService } from "@/lib/services/auth.service";
import { loginSchema } from "@/lib/auth/schemas";
import { response, handleError } from "@/lib/response";
import { ValidationError } from "@/lib/errors";
import { createAccessTokenCookie, createRefreshTokenCookie } from "@/lib/auth/session";

const authService = new AuthService();

export async function POST(request: NextRequest) {
  try {
    const body = await request.json();
    const parsed = loginSchema.safeParse(body);

    if (!parsed.success) {
      throw new ValidationError(parsed.error.flatten());
    }

    const result = await authService.login(parsed.data);

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
