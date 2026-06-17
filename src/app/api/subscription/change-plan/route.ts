import { NextRequest } from "next/server";
import { SubscriptionLifecycleService } from "@/lib/services/subscription.service";
import { changePlanSchema } from "@/lib/validators/subscription";
import { response, handleError } from "@/lib/response";
import { ValidationError } from "@/lib/errors";
import { authenticate } from "@/lib/auth/middleware";

const lifecycleService = new SubscriptionLifecycleService();

export async function POST(request: NextRequest) {
  try {
    const auth = await authenticate(request);
    const body = await request.json();
    const parsed = changePlanSchema.safeParse(body);

    if (!parsed.success) throw new ValidationError(parsed.error.flatten());

    const subscription = await lifecycleService.changePlan(auth.auth.companyId, parsed.data.planId);
    return response.ok(subscription, "Plan changed successfully");
  } catch (error) {
    return handleError(error);
  }
}
