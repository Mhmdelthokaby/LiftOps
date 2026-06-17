import { NextRequest } from "next/server";
import { TicketService } from "@/lib/services/tickets";
import { updateTicketSchema } from "@/lib/validators/tickets";
import { response, handleError } from "@/lib/response";
import { ValidationError } from "@/lib/errors";
import { authenticate } from "@/lib/auth/middleware";

type RouteParams = { params: Promise<{ id: string }> };
const ticketService = new TicketService();

export async function GET(request: NextRequest, { params }: RouteParams) {
  try {
    await authenticate(request);
    const { id } = await params;
    const ticket = await ticketService.getById(id);
    return response.ok(ticket);
  } catch (error) {
    return handleError(error);
  }
}

export async function PUT(request: NextRequest, { params }: RouteParams) {
  try {
    await authenticate(request);
    const { id } = await params;
    const body = await request.json();
    const parsed = updateTicketSchema.safeParse(body);
    if (!parsed.success) throw new ValidationError(parsed.error.flatten());

    const ticket = await ticketService.update(id, parsed.data);
    return response.ok(ticket, "Ticket updated successfully");
  } catch (error) {
    return handleError(error);
  }
}

export async function PATCH(request: NextRequest, { params }: RouteParams) {
  try {
    await authenticate(request);
    const { id } = await params;
    const ticket = await ticketService.close(id);
    return response.ok(ticket, "Ticket closed");
  } catch (error) {
    return handleError(error);
  }
}
