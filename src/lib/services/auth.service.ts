import { prisma } from "@/lib/db/client";
import { hashPassword, verifyPassword, createTokenPair, verifyRefreshToken } from "@/lib/auth";
import type { LoginInput, RegisterInput } from "@/lib/auth/schemas";
import { UnauthorizedError, ConflictError, ForbiddenError } from "@/lib/errors";

export class AuthService {
  async login(input: LoginInput) {
    const user = await prisma.appUser.findFirst({
      where: { email: input.email },
      include: { company: true },
    });

    if (!user || !user.isActive) {
      throw new UnauthorizedError("Invalid email or password");
    }

    const isValid = await verifyPassword(input.password, user.passwordHash);
    if (!isValid) {
      throw new UnauthorizedError("Invalid email or password");
    }

    const tokenPair = await createTokenPair({
      id: user.id,
      email: user.email,
      companyId: user.companyId,
      role: user.role,
      firstName: user.firstName,
      lastName: user.lastName,
    });

    await prisma.refreshToken.create({
      data: {
        token: tokenPair.refreshToken,
        userId: user.id,
        expiresAt: new Date(tokenPair.refreshTokenExpiresAt * 1000),
      },
    });

    await prisma.appUser.update({
      where: { id: user.id },
      data: { lastLoginAt: new Date() },
    });

    return {
      user: {
        id: user.id,
        email: user.email,
        firstName: user.firstName,
        lastName: user.lastName,
        role: user.role,
        companyId: user.companyId,
        companyName: user.company.name,
        avatarUrl: user.avatarUrl,
      },
      ...tokenPair,
    };
  }

  async register(input: RegisterInput) {
    const existing = await prisma.appUser.findFirst({
      where: { email: input.email },
    });

    if (existing) {
      throw new ConflictError("User with this email already exists");
    }

    const passwordHash = await hashPassword(input.password);

    const user = await prisma.appUser.create({
      data: {
        email: input.email,
        passwordHash,
        firstName: input.firstName,
        lastName: input.lastName,
        phone: input.phone,
        companyId: input.companyId ?? "",
        role: "ADMIN",
      },
    });

    return {
      id: user.id,
      email: user.email,
      firstName: user.firstName,
      lastName: user.lastName,
    };
  }

  async refresh(refreshToken: string) {
    let userId: string;
    try {
      const payload = await verifyRefreshToken(refreshToken);
      userId = payload.sub;
    } catch {
      throw new UnauthorizedError("Invalid refresh token");
    }

    const storedToken = await prisma.refreshToken.findFirst({
      where: { token: refreshToken, revokedAt: null },
      include: { user: { include: { company: true } } },
    });

    if (!storedToken || storedToken.userId !== userId) {
      throw new UnauthorizedError("Refresh token has been revoked");
    }

    if (storedToken.expiresAt < new Date()) {
      await prisma.refreshToken.update({
        where: { id: storedToken.id },
        data: { revokedAt: new Date() },
      });
      throw new UnauthorizedError("Refresh token has expired");
    }

    const user = storedToken.user;
    if (!user.isActive) {
      throw new ForbiddenError("Account is disabled");
    }

    const tokenPair = await createTokenPair({
      id: user.id,
      email: user.email,
      companyId: user.companyId,
      role: user.role,
      firstName: user.firstName,
      lastName: user.lastName,
    });

    await prisma.refreshToken.update({
      where: { id: storedToken.id },
      data: { token: tokenPair.refreshToken },
    });

    return {
      user: {
        id: user.id,
        email: user.email,
        firstName: user.firstName,
        lastName: user.lastName,
        role: user.role,
        companyId: user.companyId,
        companyName: user.company.name,
        avatarUrl: user.avatarUrl,
      },
      ...tokenPair,
    };
  }

  async logout(refreshToken?: string) {
    if (refreshToken) {
      await prisma.refreshToken.updateMany({
        where: { token: refreshToken, revokedAt: null },
        data: { revokedAt: new Date() },
      });
    }
  }

  async getProfile(userId: string) {
    const user = await prisma.appUser.findUnique({
      where: { id: userId },
      include: { company: true },
    });

    if (!user) {
      throw new UnauthorizedError("User not found");
    }

    return {
      id: user.id,
      email: user.email,
      firstName: user.firstName,
      lastName: user.lastName,
      role: user.role,
      companyId: user.companyId,
      companyName: user.company.name,
      phone: user.phone,
      avatarUrl: user.avatarUrl,
      isActive: user.isActive,
      lastLoginAt: user.lastLoginAt,
      createdAt: user.createdAt,
    };
  }
}
