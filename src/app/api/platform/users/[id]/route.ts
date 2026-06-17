import { NextRequest } from "next/server";
import { prisma } from "@/lib/db/client";
import { response, handleError } from "@/lib/response";
import { ForbiddenError } from "@/lib/errors";
import { authenticate } from "@/lib/auth/middleware";

type RouteParams = { params: Promise<{ id: string }> };

export async function PATCH(request: NextRequest, { params }: RouteParams) {
  try {
    const auth = await authenticate(request);
    if (auth.auth.type !== "admin") {
      throw new ForbiddenError("Only platform admins can manage users");
    }

    const { id } = await params;
    const body = await request.json();

    const user = await prisma.appUser.update({
      where: { id },
      data: {
        ...(body.isActive !== undefined ? { isActive: body.isActive } : {}),
        ...(body.role ? { role: body.role } : {}),
      },
      select: { id: true, email: true, firstName: true, lastName: true, role: true, isActive: true },
    });

    return response.ok(user, "User updated successfully");
  } catch (error) {
    return handleError(error);
  }
}
