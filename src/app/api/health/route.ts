import { response } from "@/lib/response";

export async function GET() {
  return response.ok({
    status: "healthy",
    timestamp: new Date().toISOString(),
    version: "1.0.0",
  });
}
