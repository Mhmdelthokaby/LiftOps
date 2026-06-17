import { NextRequest, NextResponse } from "next/server";
import { verifyAccessToken, verifyRefreshToken, createTokenPair, createAdminTokenPair } from "./jwt";
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
    type?: "user" | "admin";
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
      type: payload.type ?? "user",
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

    const accessToken = await getAccessTokenFromCookies();
    let isAdmin = false;
    if (accessToken) {
      try {
        const payload = await verifyAccessToken(accessToken);
        isAdmin = payload.type === "admin";
      } catch {
        // token may be expired, proceed with refresh token
      }
    }

    const refreshToken = await getRefreshTokenFromCookies();
    if (!refreshToken) return null;

    const { sub: ownerId } = await verifyRefreshToken(refreshToken);

    if (isAdmin) {
      const admin = await prisma.platformAdmin.findUnique({
        where: { id: ownerId },
        select: { id: true, email: true, role: true, firstName: true, lastName: true, isActive: true },
      });

      if (!admin || !admin.isActive) return null;

      const storedToken = await prisma.platformAdminRefreshToken.findFirst({
        where: { token: refreshToken, adminId: ownerId, revokedAt: null },
      });

      if (!storedToken) return null;

      const tokenPair = await createAdminTokenPair(admin);
      const response = NextResponse.next();

      response.cookies.set(createAccessTokenCookie(tokenPair.accessToken, tokenPair.accessTokenExpiresAt));
      response.cookies.set(createRefreshTokenCookie(tokenPair.refreshToken, tokenPair.refreshTokenExpiresAt));

      await prisma.platformAdminRefreshToken.update({
        where: { id: storedToken.id },
        data: { token: tokenPair.refreshToken },
      });

      (request as AuthenticatedRequest).auth = {
        userId: admin.id,
        email: admin.email,
        companyId: "",
        role: admin.role,
        firstName: admin.firstName,
        lastName: admin.lastName,
        type: "admin",
      };

      return { response, renewed: true };
    }

    const user = await prisma.appUser.findUnique({
      where: { id: ownerId },
      select: { id: true, email: true, companyId: true, role: true, firstName: true, lastName: true, isActive: true },
    });

    if (!user || !user.isActive) return null;

    const storedToken = await prisma.refreshToken.findFirst({
      where: { token: refreshToken, userId: ownerId, revokedAt: null },
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
      type: "user",
    };

    return { response, renewed: true };
  } catch {
    return null;
  }
}
