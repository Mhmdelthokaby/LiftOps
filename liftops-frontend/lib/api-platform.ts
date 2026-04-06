import { apiClient, parseResponse } from "./api-client"
import type {
  AdminUser,
  Company,
  CompaniesQueryParams,
  CreateCompanyPayload,
  GlobalUsersQueryParams,
  PaginatedResult,
  Plan,
  PlatformDashboardStats,
  SubscriptionListItem,
  SubscriptionsQueryParams,
  UpdateCompanyPayload,
} from "@/types/admin"

function buildQuery(params: Record<string, string | number | undefined>): string {
  const u = new URLSearchParams()
  for (const [key, value] of Object.entries(params)) {
    if (value !== undefined && value !== "") {
      u.set(key, String(value))
    }
  }
  const s = u.toString()
  return s ? `?${s}` : ""
}

export async function getPlatformDashboard(): Promise<PlatformDashboardStats> {
  const response = await apiClient("/api/platform/dashboard", { method: "GET" })
  return parseResponse<PlatformDashboardStats>(response)
}

export async function getCompanies(
  params: CompaniesQueryParams
): Promise<PaginatedResult<Company>> {
  const qs = buildQuery({
    page: params.page,
    pageSize: params.pageSize,
    search: params.search,
    status: params.status,
    planId: params.planId,
  })
  const response = await apiClient(`/api/companies${qs}`, { method: "GET" })
  return parseResponse<PaginatedResult<Company>>(response)
}

export async function getCompanyById(id: string): Promise<Company> {
  const response = await apiClient(`/api/companies/${id}`, { method: "GET" })
  return parseResponse<Company>(response)
}

export async function updateCompany(id: string, data: UpdateCompanyPayload): Promise<void> {
  const response = await apiClient(`/api/companies/${id}`, {
    method: "PUT",
    body: JSON.stringify({
      name: data.name,
      isActive: data.isActive,
      subscriptionPlanId: data.subscriptionPlanId,
    }),
  })
  await parseVoid(response)
}

export async function getActivePlans(): Promise<Plan[]> {
  const response = await apiClient("/api/subscription-plans", { method: "GET" })
  return parseResponse<Plan[]>(response)
}

/** Soft delete: deactivates company and hides from default lists; data retained. */
export async function softDeleteCompany(id: string): Promise<void> {
  const response = await apiClient(`/api/companies/${id}`, { method: "DELETE" })
  await parseVoid(response)
}

/** Shape of `POST /api/companies` success body (company detail DTO + echoed password). */
export interface CreateCompanyApiResponse {
  company: Partial<Company> & Pick<Company, "id" | "name">
  initialPassword?: string
}

export async function createCompany(data: CreateCompanyPayload): Promise<CreateCompanyApiResponse> {
  const body: Record<string, string> = {
    companyName: data.companyName,
    adminEmail: data.adminEmail,
    password: data.password,
  }
  if (data.planId) {
    body.planId = data.planId
  }
  const response = await apiClient("/api/companies", {
    method: "POST",
    body: JSON.stringify(body),
  })
  return parseResponse<CreateCompanyApiResponse>(response)
}

async function parseVoid(response: Response): Promise<void> {
  if (!response.ok) {
    await parseResponse<unknown>(response)
    return
  }
  if (response.status === 204 || response.status === 205) return
  const text = await response.text()
  if (!text.trim()) return
  await parseResponse<unknown>(new Response(text, { status: response.status, headers: response.headers }))
}

export async function suspendCompany(id: string, reason: string): Promise<void> {
  const response = await apiClient(`/api/platform/companies/${id}/suspend`, {
    method: "PUT",
    body: JSON.stringify({ reason }),
  })
  await parseVoid(response)
}

export async function activateCompany(id: string): Promise<void> {
  const response = await apiClient(`/api/platform/companies/${id}/activate`, {
    method: "PUT",
  })
  await parseVoid(response)
}

export async function extendTrial(companyId: string, days: number): Promise<void> {
  const response = await apiClient(`/api/platform/subscriptions/${companyId}/extend-trial`, {
    method: "POST",
    body: JSON.stringify({ days }),
  })
  await parseVoid(response)
}

export async function getPlans(): Promise<Plan[]> {
  const response = await apiClient("/api/platform/plans", { method: "GET" })
  return parseResponse<Plan[]>(response)
}

export async function getPlanById(id: string): Promise<Plan> {
  const response = await apiClient(`/api/platform/plans/${id}`, { method: "GET" })
  return parseResponse<Plan>(response)
}

export async function createPlan(data: Partial<Plan>): Promise<Plan> {
  const response = await apiClient("/api/platform/plans", {
    method: "POST",
    body: JSON.stringify(data),
  })
  return parseResponse<Plan>(response)
}

export async function updatePlan(id: string, data: Partial<Plan>): Promise<Plan> {
  const response = await apiClient(`/api/platform/plans/${id}`, {
    method: "PUT",
    body: JSON.stringify(data),
  })
  return parseResponse<Plan>(response)
}

export async function deletePlan(id: string): Promise<void> {
  const response = await apiClient(`/api/platform/plans/${id}`, {
    method: "DELETE",
  })
  await parseVoid(response)
}

export async function getSubscriptions(
  params: SubscriptionsQueryParams
): Promise<PaginatedResult<SubscriptionListItem>> {
  const qs = buildQuery({
    page: params.page,
    pageSize: params.pageSize,
    search: params.search,
    status: params.status,
    planId: params.planId,
  })
  const response = await apiClient(`/api/platform/subscriptions${qs}`, { method: "GET" })
  return parseResponse<PaginatedResult<SubscriptionListItem>>(response)
}

export async function changePlan(companyId: string, newPlanId: string): Promise<void> {
  const response = await apiClient(`/api/platform/subscriptions/${companyId}/change-plan`, {
    method: "PUT",
    body: JSON.stringify({ newPlanId }),
  })
  await parseVoid(response)
}

export async function getGlobalUsers(
  params: GlobalUsersQueryParams
): Promise<PaginatedResult<AdminUser>> {
  const qs = buildQuery({
    page: params.page,
    pageSize: params.pageSize,
    search: params.search,
    companyId: params.companyId,
    role: params.role,
  })
  const response = await apiClient(`/api/platform/users${qs}`, { method: "GET" })
  return parseResponse<PaginatedResult<AdminUser>>(response)
}

export interface ImpersonateResponse {
  token: string
  refreshToken: string
  refreshTokenExpiry: string
  name: string
  email: string
  roles: string[]
}

export async function impersonateUser(userId: string): Promise<ImpersonateResponse> {
  const response = await apiClient("/api/platform/users/impersonate", {
    method: "POST",
    body: JSON.stringify({ userId }),
  })
  return parseResponse<ImpersonateResponse>(response)
}
