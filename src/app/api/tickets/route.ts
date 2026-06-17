import { NextRequest } from "next/server";
import { TicketService } from "@/lib/services/tickets";
import { createTicketSchema, ticketQuerySchema } from "@/lib/validators/tickets";
import { response, handleError } from "@/lib/response";
import { ValidationError } from "@/lib/errors";
import { authenticate } from "@/lib/auth/middleware";

const ticketService = new TicketService();

export async function GET(request: NextRequest) {
  try {
    const auth = await authenticate(request);
    const query = ticketQuerySchema.parse(Object.fromEntries(request.nextUrl.searchParams));
    const result = await ticketService.list(auth.auth.companyId, query);
    return response.paginated(result.items, result.meta);
  } catch (error) {
    return handleError(error);
  }
}

export async function POST(request: NextRequest) {
  try {
    const auth = await authenticate(request);
    const body = await request.json();
    const parsed = createTicketSchema.safeParse(body);
    if (!parsed.success) throw new ValidationError(parsed.error.flatten());

    const ticket = await ticketService.create(auth.auth.companyId, auth.auth.userId, parsed.data);
    return response.created(ticket, "Ticket created successfully");
  } catch (error) {
    return handleError(error);
  }
}
