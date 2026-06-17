import { NextResponse } from "next/server";
import { AppError } from "@/lib/errors";

export type PaginationMeta = {
  page: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
};

export type ApiResponse<T = unknown> = {
  data?: T;
  message?: string;
  success: boolean;
  succeeded: boolean;
  errors?: unknown;
  meta?: PaginationMeta;
};

function createResponse<T>(
  statusCode: number,
  success: boolean,
  data?: T,
  message?: string,
  errors?: unknown,
  meta?: PaginationMeta
): NextResponse<ApiResponse<T>> {
  return NextResponse.json(
    { data, message, success, succeeded: success, errors, meta },
    { status: statusCode }
  );
}

export const response = {
  ok: <T>(data: T, message?: string) =>
    createResponse(200, true, data, message),

  created: <T>(data: T, message = "Created successfully") =>
    createResponse(201, true, data, message),

  noContent: () => new NextResponse(null, { status: 204 }),

  badRequest: (message: string, errors?: unknown) =>
    createResponse(400, false, undefined, message, errors),

  unauthorized: (message = "Authentication required") =>
    createResponse(401, false, undefined, message),

  forbidden: (message = "Insufficient permissions") =>
    createResponse(403, false, undefined, message),

  notFound: (message = "Resource not found") =>
    createResponse(404, false, undefined, message),

  conflict: (message: string) =>
    createResponse(409, false, undefined, message),

  error: (message: string, statusCode = 500) =>
    createResponse(statusCode, false, undefined, message),

  paginated: <T>(data: T, meta: PaginationMeta, message?: string) =>
    createResponse(200, true, data, message, undefined, meta),
};

export function handleError(error: unknown): NextResponse<ApiResponse> {
  console.error("[API Error]:", error);

  if (error instanceof AppError) {
    return createResponse(
      error.statusCode,
      false,
      undefined,
      error.message,
      error.details
    );
  }

  if (error && typeof error === "object" && "errors" in error) {
    return createResponse(400, false, undefined, "Validation failed", error);
  }

  return createResponse(500, false, undefined, "An unexpected error occurred");
}

export type PaginatedResult<T> = {
  items: T[];
  meta: PaginationMeta;
};

export function createPaginationMeta(
  page: number,
  pageSize: number,
  totalItems: number
): PaginationMeta {
  return {
    page,
    pageSize,
    totalItems,
    totalPages: Math.ceil(totalItems / pageSize),
  };
}
