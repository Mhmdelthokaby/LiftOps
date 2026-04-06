export type CompanyStatus =
  | "Active"
  | "Inactive"
  | "Suspended"
  | "SuspendedByAdmin"
  | "Deleted"

export type SubscriptionStatus = "Trial" | "Active" | "PastDue" | "Cancelled" | "Expired"

export type BillingCycle = "Monthly" | "Yearly"

export interface Plan {
  id: string
  name: string
  code: string
  monthlyPrice: number
  yearlyPrice: number
  trialDays: number
  isActive: boolean
  maxUsers: number
  maxElevators: number
  maxMaintenanceContracts: number
  maxInstallationProjects: number
  allowEmergencyModule: boolean
  allowFaultsModule: boolean
  allowFinanceModule: boolean
  allowInventoryModule: boolean
  allowApiAccess: boolean
}

export interface Subscription {
  id: string
  companyId: string
  planId: string
  /** Display name from linked subscription plan (API). */
  planName?: string
  status: SubscriptionStatus
  currentPeriodStart: string
  currentPeriodEnd: string
  billingCycle: BillingCycle
}

/** Subscription row as returned by platform list endpoints (joined fields optional). */
export interface SubscriptionListItem extends Subscription {
  companyName?: string
  planName?: string
  planCode?: string
  userCount?: number
  planMaxUsers?: number
  planMonthlyPrice?: number
}

export interface Company {
  id: string
  name: string
  slug?: string
  contactEmail?: string
  /** Primary admin / billing email (list API). */
  adminEmail?: string
  isActive?: boolean
  isDeleted?: boolean
  status: CompanyStatus
  createdAt: string
  planName?: string
  planId?: string
  /** Display tier (Free / Pro / Enterprise style). */
  subscriptionPlan?: string | null
  currentUserCount: number
  currentElevatorCount: number
  subscription?: Subscription
  /** Limits from current plan (when API includes them). */
  planMaxUsers?: number
  planMaxElevators?: number
  /** Detail API: first Manager user. */
  defaultAdminName?: string | null
  defaultAdminEmail?: string | null
  defaultAdminPhone?: string | null
}

export interface AdminUser {
  id: string
  email: string
  firstName?: string
  lastName?: string
  /** Null for platform administrators without a tenant. */
  companyId: string | null
  companyName?: string
  role: string
  lastLogin?: string
  isActive: boolean
}

export interface PaginatedResult<T> {
  items: T[]
  totalCount: number
  page: number
  pageSize: number
}

export interface PlatformDashboardStats {
  totalCompanies: number
  activeSubscriptions: number
  monthlyRevenue: number
  trialCompanies: number
  revenueTrend: Array<{ month: string; revenue: number }>
  companiesByPlan: Array<{ planName: string; count: number }>
}

export interface CompaniesQueryParams {
  page?: number
  pageSize?: number
  search?: string
  status?: string
  planId?: string
}

export interface SubscriptionsQueryParams {
  page?: number
  pageSize?: number
  search?: string
  status?: string
  planId?: string
}

export interface GlobalUsersQueryParams {
  page?: number
  pageSize?: number
  search?: string
  companyId?: string
  role?: string
}

export interface CreateCompanyPayload {
  companyName: string
  adminEmail: string
  password: string
  /** When set, backend uses this plan; otherwise the default active plan (lowest monthly price) is used. */
  planId?: string
}

export interface UpdateCompanyPayload {
  name: string
  isActive: boolean
  subscriptionPlanId?: string
}
