import { NextRequest } from "next/server";
import { SubscriptionLifecycleService } from "@/lib/services/subscription.service";
import { response, handleError } from "@/lib/response";

const lifecycleService = new SubscriptionLifecycleService();

export async function POST(request: NextRequest) {
  try {
    const body = await request.json();
    const { type, data } = body;

    await lifecycleService.handleWebhook(type, data?.object ?? {});
    return response.ok(null, "Webhook processed");
  } catch (error) {
    return handleError(error);
  }
}
