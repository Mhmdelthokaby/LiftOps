import { NextRequest } from "next/server";
import { AuthService } from "@/lib/services/auth.service";
import { registerSchema } from "@/lib/auth/schemas";
import { response, handleError } from "@/lib/response";
import { ValidationError } from "@/lib/errors";

const authService = new AuthService();

export async function POST(request: NextRequest) {
  try {
    const body = await request.json();
    const parsed = registerSchema.safeParse(body);

    if (!parsed.success) {
      throw new ValidationError(parsed.error.flatten());
    }

    const result = await authService.register(parsed.data);

    return response.created(result, "User registered successfully");
  } catch (error) {
    return handleError(error);
  }
}
