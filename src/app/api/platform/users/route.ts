import { NextRequest } from "next/server";
import { prisma } from "@/lib/db/client";
import { response, handleError } from "@/lib/response";
import { ForbiddenError } from "@/lib/errors";
import { authenticate } from "@/lib/auth/middleware";

export async function GET(request: NextRequest) {
  try {
    const auth = await authenticate(request);
    if (auth.auth.role !== "SUPER_ADMIN") {
      throw new ForbiddenError("Only platform admins can access this resource");
    }

    const { searchParams } = request.nextUrl;
    const page = parseInt(searchParams.get("page") ?? "1");
    const pageSize = parseInt(searchParams.get("pageSize") ?? "20");
    const companyId = searchParams.get("companyId");
    const skip = (page - 1) * pageSize;

    const where: Record<string, unknown> = {};
    if (companyId) where.companyId = companyId;

    const [users, total] = await Promise.all([
      prisma.appUser.findMany({
        where,
        skip,
        take: pageSize,
        select: {
          id: true,
          email: true,
          firstName: true,
          lastName: true,
          role: true,
          isActive: true,
          lastLoginAt: true,
          createdAt: true,
          company: { select: { id: true, name: true } },
        },
        orderBy: { createdAt: "desc" },
      }),
      prisma.appUser.count({ where }),
    ]);

    return response.ok({
      items: users,
      totalCount: total,
      page,
      pageSize,
    });
  } catch (error) {
    return handleError(error);
  }
}
