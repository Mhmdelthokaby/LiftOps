"use client"

import { useCallback, useEffect, useState } from "react"
import {
  getCompanies,
  getCompanyById,
  getGlobalUsers,
  getPlans,
  getPlatformDashboard,
  getSubscriptions,
  getActivePlans,
} from "@/lib/api"

export function useActivePlans() {
  const [data, setData] = useState<Plan[] | null>(null)
  const [isLoading, setIsLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [reloadToken, setReloadToken] = useState(0)
  const refetch = useCallback(() => setReloadToken((t) => t + 1), [])

  useEffect(() => {
    let cancelled = false
    setIsLoading(true)
    setError(null)
    getActivePlans()
      .then((d) => {
        if (!cancelled) setData(d)
      })
      .catch((e: unknown) => {
        if (!cancelled) {
          setError(e instanceof Error ? e.message : "Failed to load active plans")
        }
      })
      .finally(() => {
        if (!cancelled) setIsLoading(false)
      })
    return () => {
      cancelled = true
    }
  }, [reloadToken])

  return { data, isLoading, error, refetch }
}
import type {
  AdminUser,
  CompaniesQueryParams,
  Company,
  GlobalUsersQueryParams,
  PaginatedResult,
  Plan,
  PlatformDashboardStats,
  SubscriptionListItem,
  SubscriptionsQueryParams,
} from "@/types/admin"

export function useDashboardStats() {
  const [data, setData] = useState<PlatformDashboardStats | null>(null)
  const [isLoading, setIsLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [reloadToken, setReloadToken] = useState(0)
  const refetch = useCallback(() => setReloadToken((t) => t + 1), [])

  useEffect(() => {
    let cancelled = false
    setIsLoading(true)
    setError(null)
    getPlatformDashboard()
      .then((d) => {
        if (!cancelled) setData(d)
      })
      .catch((e: unknown) => {
        if (!cancelled) {
          setError(e instanceof Error ? e.message : "Failed to load dashboard")
        }
      })
      .finally(() => {
        if (!cancelled) setIsLoading(false)
      })
    return () => {
      cancelled = true
    }
  }, [reloadToken])

  return { data, isLoading, error, refetch }
}

export function useCompanies(filters: CompaniesQueryParams) {
  const [data, setData] = useState<PaginatedResult<Company> | null>(null)
  const [isLoading, setIsLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [reloadToken, setReloadToken] = useState(0)
  const refetch = useCallback(() => setReloadToken((t) => t + 1), [])

  useEffect(() => {
    let cancelled = false
    setIsLoading(true)
    setError(null)
    getCompanies(filters)
      .then((d) => {
        if (!cancelled) setData(d)
      })
      .catch((e: unknown) => {
        if (!cancelled) {
          setError(e instanceof Error ? e.message : "Failed to load companies")
        }
      })
      .finally(() => {
        if (!cancelled) setIsLoading(false)
      })
    return () => {
      cancelled = true
    }
  }, [
    filters.page,
    filters.pageSize,
    filters.search,
    filters.status,
    filters.planId,
    reloadToken,
  ])

  return { data, isLoading, error, refetch }
}

export function useCompanyDetails(id: string | undefined) {
  const [data, setData] = useState<Company | null>(null)
  const [isLoading, setIsLoading] = useState(Boolean(id))
  const [error, setError] = useState<string | null>(null)
  const [reloadToken, setReloadToken] = useState(0)
  const refetch = useCallback(() => setReloadToken((t) => t + 1), [])

  useEffect(() => {
    if (!id) {
      setData(null)
      setIsLoading(false)
      setError(null)
      return
    }
    let cancelled = false
    setIsLoading(true)
    setError(null)
    getCompanyById(id)
      .then((d) => {
        if (!cancelled) setData(d)
      })
      .catch((e: unknown) => {
        if (!cancelled) {
          setError(e instanceof Error ? e.message : "Failed to load company")
        }
      })
      .finally(() => {
        if (!cancelled) setIsLoading(false)
      })
    return () => {
      cancelled = true
    }
  }, [id, reloadToken])

  return { data, isLoading, error, refetch }
}

export function usePlans() {
  const [data, setData] = useState<Plan[] | null>(null)
  const [isLoading, setIsLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [reloadToken, setReloadToken] = useState(0)
  const refetch = useCallback(() => setReloadToken((t) => t + 1), [])

  useEffect(() => {
    let cancelled = false
    setIsLoading(true)
    setError(null)
    getPlans()
      .then((d) => {
        if (!cancelled) setData(d)
      })
      .catch((e: unknown) => {
        if (!cancelled) {
          setError(e instanceof Error ? e.message : "Failed to load plans")
        }
      })
      .finally(() => {
        if (!cancelled) setIsLoading(false)
      })
    return () => {
      cancelled = true
    }
  }, [reloadToken])

  return { data, isLoading, error, refetch }
}

export function useSubscriptions(filters: SubscriptionsQueryParams) {
  const [data, setData] = useState<PaginatedResult<SubscriptionListItem> | null>(null)
  const [isLoading, setIsLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [reloadToken, setReloadToken] = useState(0)
  const refetch = useCallback(() => setReloadToken((t) => t + 1), [])

  useEffect(() => {
    let cancelled = false
    setIsLoading(true)
    setError(null)
    getSubscriptions(filters)
      .then((d) => {
        if (!cancelled) setData(d)
      })
      .catch((e: unknown) => {
        if (!cancelled) {
          setError(e instanceof Error ? e.message : "Failed to load subscriptions")
        }
      })
      .finally(() => {
        if (!cancelled) setIsLoading(false)
      })
    return () => {
      cancelled = true
    }
  }, [
    filters.page,
    filters.pageSize,
    filters.search,
    filters.status,
    filters.planId,
    reloadToken,
  ])

  return { data, isLoading, error, refetch }
}

export function useGlobalUsers(filters: GlobalUsersQueryParams) {
  const [data, setData] = useState<PaginatedResult<AdminUser> | null>(null)
  const [isLoading, setIsLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [reloadToken, setReloadToken] = useState(0)
  const refetch = useCallback(() => setReloadToken((t) => t + 1), [])

  useEffect(() => {
    let cancelled = false
    setIsLoading(true)
    setError(null)
    getGlobalUsers(filters)
      .then((d) => {
        if (!cancelled) setData(d)
      })
      .catch((e: unknown) => {
        if (!cancelled) {
          setError(e instanceof Error ? e.message : "Failed to load users")
        }
      })
      .finally(() => {
        if (!cancelled) setIsLoading(false)
      })
    return () => {
      cancelled = true
    }
  }, [
    filters.page,
    filters.pageSize,
    filters.search,
    filters.companyId,
    filters.role,
    reloadToken,
  ])

  return { data, isLoading, error, refetch }
}
