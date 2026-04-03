"use client"

import { useCallback, useMemo } from "react"
import Link from "next/link"
import { useRouter, useSearchParams } from "next/navigation"
import { AlertCircle } from "lucide-react"
import { PageHeader } from "@/components/admin/page-header"
import { DataTable, type Column } from "@/components/admin/data-table"
import { PlanCodeBadge, SubscriptionStatusBadge } from "@/components/admin/badges"
import { AdminErrorState } from "@/components/admin/admin-error-state"
import { useSubscriptions } from "@/hooks/use-admin-data"
import type { SubscriptionListItem } from "@/types/admin"

const TRIAL_WARNING_DAYS = 7

export default function AdminSubscriptionsPage() {
  const router = useRouter()
  const searchParams = useSearchParams()
  const page = Math.max(1, Number(searchParams.get("page") ?? "1") || 1)
  const pageSize = Math.min(100, Math.max(1, Number(searchParams.get("pageSize") ?? "10") || 10))
  const statusParam = searchParams.get("status") ?? ""
  const searchParam = searchParams.get("search") ?? ""

  const updateQuery = useCallback(
    (updates: Record<string, string | undefined>) => {
      const next = new URLSearchParams(searchParams.toString())
      for (const [k, v] of Object.entries(updates)) {
        if (v === undefined || v === "") next.delete(k)
        else next.set(k, v)
      }
      router.push(`/admin/subscriptions?${next.toString()}`)
    },
    [router, searchParams]
  )

  const filters = useMemo(
    () => ({
      page,
      pageSize,
      search: searchParam || undefined,
      status: statusParam || undefined,
    }),
    [page, pageSize, searchParam, statusParam]
  )

  const { data, isLoading, error, refetch } = useSubscriptions(filters)
  const totalPages = data ? Math.max(1, Math.ceil(data.totalCount / data.pageSize)) : 1

  const daysUntilEnd = (sub: SubscriptionListItem) =>
    Math.ceil((new Date(sub.currentPeriodEnd).getTime() - Date.now()) / (1000 * 60 * 60 * 24))

  const columns: Column<SubscriptionListItem>[] = [
    {
      id: "company",
      header: "Company",
      cell: (row) => (
        <Link
          href={`/admin/companies/${row.companyId}`}
          className="font-medium text-primary hover:underline"
        >
          {row.companyName ?? row.companyId}
        </Link>
      ),
      sortable: true,
      sortFn: (a, b) => (a.companyName ?? "").localeCompare(b.companyName ?? ""),
    },
    {
      id: "plan",
      header: "Plan",
      cell: (row) =>
        row.planName ? <PlanCodeBadge code={row.planCode ?? row.planName} name={row.planName} /> : "—",
    },
    {
      id: "status",
      header: "Status",
      cell: (row) => {
        const d = daysUntilEnd(row)
        const warn = row.status === "Trial" && d <= TRIAL_WARNING_DAYS && d > 0
        const expired = d <= 0
        return (
          <div className="flex items-center gap-2">
            {warn && <span className="h-2 w-2 animate-pulse rounded-full bg-amber-500" />}
            {expired && <AlertCircle className="h-4 w-4 text-red-500" />}
            <SubscriptionStatusBadge status={row.status} />
          </div>
        )
      },
    },
    {
      id: "users",
      header: "Users",
      cell: (row) => {
        const max = row.planMaxUsers ?? 0
        const u = row.userCount ?? 0
        const full = max > 0 && u >= max
        return (
          <span className={full ? "font-medium text-red-600" : ""}>
            {u}
            {max > 0 ? ` / ${max}` : ""}
          </span>
        )
      },
    },
    {
      id: "mrr",
      header: "MRR",
      cell: (row) => (row.planMonthlyPrice != null ? `$${row.planMonthlyPrice}` : "—"),
    },
    {
      id: "renewal",
      header: "Period end",
      cell: (row) => {
        const d = daysUntilEnd(row)
        if (d <= 0) return <span className="text-red-600">Ended</span>
        if (d <= TRIAL_WARNING_DAYS) return <span className="text-amber-600">{d} days</span>
        return new Date(row.currentPeriodEnd).toLocaleDateString()
      },
      sortable: true,
      sortFn: (a, b) =>
        new Date(a.currentPeriodEnd).getTime() - new Date(b.currentPeriodEnd).getTime(),
    },
  ]

  return (
    <div className="flex h-full flex-col">
      <PageHeader title="Subscriptions" description="All tenant subscriptions" />

      <div className="flex-1 space-y-6 overflow-auto p-8">
        {isLoading && <p className="text-sm text-muted-foreground">Loading…</p>}
        {error && <AdminErrorState message={error} onRetry={refetch} />}
        {!isLoading && !error && data && data.items.length === 0 && (
          <p className="text-sm text-muted-foreground">No subscriptions match your filters.</p>
        )}
        {!isLoading && !error && data && data.items.length > 0 && (
          <DataTable
            columns={columns}
            data={data.items}
            serverPagination={{
              page: data.page,
              totalPages,
              onPageChange: (p) => updateQuery({ page: String(p) }),
            }}
          />
        )}
      </div>
    </div>
  )
}
