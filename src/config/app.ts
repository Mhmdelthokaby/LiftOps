import { env } from "./env";

export const appConfig = {
  name: env.NEXT_PUBLIC_APP_NAME,
  url: env.NEXT_PUBLIC_APP_URL,
  api: {
    prefix: "/api/v1",
    version: "1.0.0",
  },
  auth: {
    accessTokenExpiry: `${env.JWT_ACCESS_EXPIRY_MINUTES}m`,
    refreshTokenExpiry: `${env.JWT_REFRESH_EXPIRY_DAYS}d`,
    cookieNames: {
      accessToken: "liftops_access",
      refreshToken: "liftops_refresh",
    },
  },
  pagination: {
    defaultPageSize: 20,
    maxPageSize: 100,
  },
  platformAdmin: {
    companyId: env.PLATFORM_ADMIN_COMPANY_ID ?? "00000000-0000-0000-0000-000000000000",
  },
} as const;
