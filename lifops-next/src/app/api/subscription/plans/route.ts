import { NextRequest } from "next/server";
import { SubscriptionPlanService } from "@/lib/services/subscription.service";
import { createPlanSchema } from "@/lib/validators/subscription";
import { response, handleError } from "@/lib/response";
import { ValidationError, ForbiddenError } from "@/lib/errors";
import { authenticate } from "@/lib/auth/middleware";

const planService = new SubscriptionPlanService();

export async function GET() {
  try {
    const plans = await planService.list();
    return response.ok(plans);
  } catch (error) {
    return handleError(error);
  }
}

export async function POST(request: NextRequest) {
  try {
    const auth = await authenticate(request);
    if (auth.auth.role !== "SUPER_ADMIN") {
      throw new ForbiddenError("Only platform admins can create subscription plans");
    }

    const body = await request.json();
    const parsed = createPlanSchema.safeParse(body);
    if (!parsed.success) throw new ValidationError(parsed.error.flatten());

    const plan = await planService.create(parsed.data);
    return response.created(plan, "Plan created successfully");
  } catch (error) {
    return handleError(error);
  }
}
