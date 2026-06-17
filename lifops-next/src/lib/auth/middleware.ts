import { NextRequest, NextResponse } from "next/server";
import { verifyAccessToken, verifyRefreshToken, createTokenPair } from "./jwt";
import { getAccessTokenFromCookies, getRefreshTokenFromCookies, createAccessTokenCookie, createRefreshTokenCookie } from "./session";
import { prisma } from "@/lib/db/client";
import { UnauthorizedError, ForbiddenError } from "@/lib/errors";

export type AuthenticatedRequest = NextRequest & {
  auth: {
    userId: string;
    email: string;
    companyId: string;
    role: string;
    firstName: string;
    lastName: string;
  };
};

export async function authenticate(request: NextRequest): Promise<AuthenticatedRequest> {
  const authHeader = request.headers.get("Authorization");
  let token: string | null = null;

  if (authHeader?.startsWith("Bearer ")) {
    token = authHeader.slice(7);
  } else {
    token = await getAccessTokenFromCookies();
  }

  if (!token) {
    throw new UnauthorizedError("No authentication token provided");
  }

  try {
    const payload = await verifyAccessToken(token);
    (request as AuthenticatedRequest).auth = {
      userId: payload.sub,
      email: payload.email,
      companyId: payload.companyId,
      role: payload.role,
      firstName: payload.firstName,
      lastName: payload.lastName,
    };
    return request as AuthenticatedRequest;
  } catch {
    throw new UnauthorizedError("Invalid or expired token");
  }
}

export function authorize(...allowedRoles: string[]) {
  return (request: AuthenticatedRequest): void => {
    if (allowedRoles.length === 0) return;
    if (!allowedRoles.includes(request.auth.role)) {
      throw new ForbiddenError(`Requires one of roles: ${allowedRoles.join(", ")}`);
    }
  };
}

export async function tryRefreshAccessToken(request: NextRequest): Promise<{ response: NextResponse; renewed: boolean } | null> {
  try {
    const authHeader = request.headers.get("Authorization");
    if (authHeader?.startsWith("Bearer ")) {
      return null;
    }

    const refreshToken = await getRefreshTokenFromCookies();
    if (!refreshToken) return null;

    const { sub: userId } = await verifyRefreshToken(refreshToken);

    const user = await prisma.appUser.findUnique({
      where: { id: userId },
      select: { id: true, email: true, companyId: true, role: true, firstName: true, lastName: true, isActive: true },
    });

    if (!user || !user.isActive) return null;

    const storedToken = await prisma.refreshToken.findFirst({
      where: { token: refreshToken, userId, revokedAt: null },
    });

    if (!storedToken) return null;

    const tokenPair = await createTokenPair(user);
    const response = NextResponse.next();

    response.cookies.set(createAccessTokenCookie(tokenPair.accessToken, tokenPair.accessTokenExpiresAt));
    response.cookies.set(createRefreshTokenCookie(tokenPair.refreshToken, tokenPair.refreshTokenExpiresAt));

    await prisma.refreshToken.update({
      where: { id: storedToken.id },
      data: { token: tokenPair.refreshToken },
    });

    (request as AuthenticatedRequest).auth = {
      userId: user.id,
      email: user.email,
      companyId: user.companyId,
      role: user.role,
      firstName: user.firstName,
      lastName: user.lastName,
    };

    return { response, renewed: true };
  } catch {
    return null;
  }
}
