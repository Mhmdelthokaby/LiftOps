import { cookies } from "next/headers";
import { appConfig } from "@/config/app";

const { accessToken: accessCookieName, refreshToken: refreshCookieName } = appConfig.auth.cookieNames;

export function createAccessTokenCookie(token: string, maxAge: number) {
  return {
    name: accessCookieName,
    value: token,
    httpOnly: true,
    secure: process.env.NODE_ENV === "production",
    sameSite: "lax" as const,
    path: "/",
    maxAge,
  };
}

export function createRefreshTokenCookie(token: string, maxAge: number) {
  return {
    name: refreshCookieName,
    value: token,
    httpOnly: true,
    secure: process.env.NODE_ENV === "production",
    sameSite: "lax" as const,
    path: "/api/auth",
    maxAge,
  };
}

export function createClearAccessTokenCookie() {
  return {
    name: accessCookieName,
    value: "",
    httpOnly: true,
    secure: process.env.NODE_ENV === "production",
    sameSite: "lax" as const,
    path: "/",
    maxAge: 0,
  };
}

export function createClearRefreshTokenCookie() {
  return {
    name: refreshCookieName,
    value: "",
    httpOnly: true,
    secure: process.env.NODE_ENV === "production",
    sameSite: "lax" as const,
    path: "/api/auth",
    maxAge: 0,
  };
}

export async function getAccessTokenFromCookies(): Promise<string | null> {
  const cookieStore = await cookies();
  const accessToken = cookieStore.get(accessCookieName);
  return accessToken?.value ?? null;
}

export async function getRefreshTokenFromCookies(): Promise<string | null> {
  const cookieStore = await cookies();
  const refreshToken = cookieStore.get(refreshCookieName);
  return refreshToken?.value ?? null;
}
