import { NextRequest } from "next/server";
import { TicketService } from "@/lib/services/tickets";
import { assignTicketSchema } from "@/lib/validators/tickets";
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
    const parsed = assignTicketSchema.safeParse(body);
    if (!parsed.success) throw new ValidationError(parsed.error.flatten());

    const ticket = await ticketService.assign(id, parsed.data);
    return response.ok(ticket, "Technician assigned successfully");
  } catch (error) {
    return handleError(error);
  }
}
