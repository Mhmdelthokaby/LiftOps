import { prisma } from "@/lib/db/client";
import { hashPassword, verifyPassword, createAdminTokenPair, verifyRefreshToken } from "@/lib/auth";
import type { LoginInput } from "@/lib/auth/schemas";
import { UnauthorizedError, ConflictError } from "@/lib/errors";

export class AdminAuthService {
  async login(input: LoginInput) {
    const admin = await prisma.platformAdmin.findFirst({
      where: { email: input.email },
    });

    if (!admin || !admin.isActive) {
      throw new UnauthorizedError("Invalid email or password");
    }

    const isValid = await verifyPassword(input.password, admin.passwordHash);
    if (!isValid) {
      throw new UnauthorizedError("Invalid email or password");
    }

    const tokenPair = await createAdminTokenPair({
      id: admin.id,
      email: admin.email,
      role: admin.role,
      firstName: admin.firstName,
      lastName: admin.lastName,
    });

    await prisma.platformAdminRefreshToken.create({
      data: {
        token: tokenPair.refreshToken,
        adminId: admin.id,
        expiresAt: new Date(tokenPair.refreshTokenExpiresAt * 1000),
      },
    });

    await prisma.platformAdmin.update({
      where: { id: admin.id },
      data: { lastLoginAt: new Date() },
    });

    return {
      user: {
        id: admin.id,
        email: admin.email,
        firstName: admin.firstName,
        lastName: admin.lastName,
        role: admin.role,
        companyId: "",
      },
      ...tokenPair,
    };
  }

  async refresh(refreshToken: string) {
    let adminId: string;
    try {
      const payload = await verifyRefreshToken(refreshToken);
      adminId = payload.sub;
    } catch {
      throw new UnauthorizedError("Invalid refresh token");
    }

    const storedToken = await prisma.platformAdminRefreshToken.findFirst({
      where: { token: refreshToken, revokedAt: null },
    });

    if (!storedToken || storedToken.adminId !== adminId) {
      throw new UnauthorizedError("Refresh token has been revoked");
    }

    if (storedToken.expiresAt < new Date()) {
      await prisma.platformAdminRefreshToken.update({
        where: { id: storedToken.id },
        data: { revokedAt: new Date() },
      });
      throw new UnauthorizedError("Refresh token has expired");
    }

    const admin = await prisma.platformAdmin.findUnique({
      where: { id: adminId },
    });

    if (!admin || !admin.isActive) {
      throw new UnauthorizedError("Account is disabled");
    }

    const tokenPair = await createAdminTokenPair({
      id: admin.id,
      email: admin.email,
      role: admin.role,
      firstName: admin.firstName,
      lastName: admin.lastName,
    });

    await prisma.platformAdminRefreshToken.update({
      where: { id: storedToken.id },
      data: { token: tokenPair.refreshToken },
    });

    return {
      user: {
        id: admin.id,
        email: admin.email,
        firstName: admin.firstName,
        lastName: admin.lastName,
        role: admin.role,
        companyId: "",
      },
      ...tokenPair,
    };
  }

  async logout(refreshToken?: string) {
    if (refreshToken) {
      await prisma.platformAdminRefreshToken.updateMany({
        where: { token: refreshToken, revokedAt: null },
        data: { revokedAt: new Date() },
      });
    }
  }
}
