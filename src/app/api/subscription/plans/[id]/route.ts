import { NextRequest } from "next/server";
import { SubscriptionPlanService } from "@/lib/services/subscription.service";
import { updatePlanSchema } from "@/lib/validators/subscription";
import { response, handleError } from "@/lib/response";
import { ValidationError, ForbiddenError } from "@/lib/errors";
import { authenticate } from "@/lib/auth/middleware";

type RouteParams = { params: Promise<{ id: string }> };

const planService = new SubscriptionPlanService();

export async function GET(request: NextRequest, { params }: RouteParams) {
  try {
    const { id } = await params;
    const plan = await planService.getById(id);
    return response.ok(plan);
  } catch (error) {
    return handleError(error);
  }
}

export async function PUT(request: NextRequest, { params }: RouteParams) {
  try {
    const auth = await authenticate(request);
    if (auth.auth.type !== "admin") {
      throw new ForbiddenError("Only platform admins can update plans");
    }

    const { id } = await params;
    const body = await request.json();
    const parsed = updatePlanSchema.safeParse(body);
    if (!parsed.success) throw new ValidationError(parsed.error.flatten());

    const plan = await planService.update(id, parsed.data);
    return response.ok(plan, "Plan updated successfully");
  } catch (error) {
    return handleError(error);
  }
}
