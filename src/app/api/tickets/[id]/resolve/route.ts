import { NextRequest } from "next/server";
import { TicketService } from "@/lib/services/tickets";
import { resolveTicketSchema } from "@/lib/validators/tickets";
import { response, handleError } from "@/lib/response";
import { ValidationError } from "@/lib/errors";
import { authenticate } from "@/lib/auth/middleware";

type RouteParams = { params: Promise<{ id: string }> };
const ticketService = new TicketService();

export async function POST(request: NextRequest, { params }: RouteParams) {
  try {
    await authenticate(request);
    const { id } = await params;
    const body = await request.json();
    const parsed = resolveTicketSchema.safeParse(body);
    if (!parsed.success) throw new ValidationError(parsed.error.flatten());

    const ticket = await ticketService.resolve(id, parsed.data);
    return response.ok(ticket, "Ticket resolved successfully");
  } catch (error) {
    return handleError(error);
  }
}
