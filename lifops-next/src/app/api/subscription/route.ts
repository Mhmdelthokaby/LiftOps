import { NextRequest } from "next/server";
import { SubscriptionLifecycleService } from "@/lib/services/subscription.service";
import { response, handleError } from "@/lib/response";
import { authenticate } from "@/lib/auth/middleware";

const lifecycleService = new SubscriptionLifecycleService();

export async function GET(request: NextRequest) {
  try {
    const auth = await authenticate(request);
    const subscription = await lifecycleService.getCompanySubscription(auth.auth.companyId);
    return response.ok(subscription);
  } catch (error) {
    return handleError(error);
  }
}
