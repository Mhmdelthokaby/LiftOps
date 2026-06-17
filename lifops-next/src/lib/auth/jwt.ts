import { SignJWT, jwtVerify, type JWTPayload } from "jose";
import { env } from "@/config/env";

const secretKey = new TextEncoder().encode(env.JWT_SECRET);
const refreshSecretKey = new TextEncoder().encode(env.JWT_REFRESH_SECRET);

export interface TokenPayload extends JWTPayload {
  sub: string;
  email: string;
  companyId: string;
  role: string;
  firstName: string;
  lastName: string;
}

export interface TokenPair {
  accessToken: string;
  refreshToken: string;
  accessTokenExpiresAt: number;
  refreshTokenExpiresAt: number;
}

export async function signAccessToken(payload: Omit<TokenPayload, keyof JWTPayload>): Promise<string> {
  return new SignJWT({ ...payload } as unknown as JWTPayload)
    .setProtectedHeader({ alg: "HS256" })
    .setIssuedAt()
    .setExpirationTime(`${env.JWT_ACCESS_EXPIRY_MINUTES}m`)
    .setSubject(payload.sub)
    .sign(secretKey);
}

export async function signRefreshToken(userId: string): Promise<string> {
  return new SignJWT({})
    .setProtectedHeader({ alg: "HS256" })
    .setIssuedAt()
    .setExpirationTime(`${env.JWT_REFRESH_EXPIRY_DAYS}d`)
    .setSubject(userId)
    .sign(refreshSecretKey);
}

export async function verifyAccessToken(token: string): Promise<TokenPayload> {
  const { payload } = await jwtVerify(token, secretKey, {
    algorithms: ["HS256"],
  });
  return payload as unknown as TokenPayload;
}

export async function verifyRefreshToken(token: string): Promise<{ sub: string }> {
  const { payload } = await jwtVerify(token, refreshSecretKey, {
    algorithms: ["HS256"],
  });
  return { sub: payload.sub as string };
}

export async function createTokenPair(
  user: { id: string; email: string; companyId: string; role: string; firstName: string; lastName: string }
): Promise<TokenPair> {
  const now = Math.floor(Date.now() / 1000);
  const accessExpiry = now + env.JWT_ACCESS_EXPIRY_MINUTES * 60;
  const refreshExpiry = now + env.JWT_REFRESH_EXPIRY_DAYS * 86400;

  const accessToken = await signAccessToken({
    sub: user.id,
    email: user.email,
    companyId: user.companyId,
    role: user.role,
    firstName: user.firstName,
    lastName: user.lastName,
  });

  const refreshToken = await signRefreshToken(user.id);

  return {
    accessToken,
    refreshToken,
    accessTokenExpiresAt: accessExpiry,
    refreshTokenExpiresAt: refreshExpiry,
  };
}
