"use client"

import { Card } from "@/components/ui/card"
import { Skeleton } from "@/components/ui/skeleton"
import { PageHeader } from "@/components/admin/page-header"
import { RevenueTrendChart, CompaniesByPlanChart } from "@/components/admin/charts"
import { AdminErrorState } from "@/components/admin/admin-error-state"
import { useDashboardStats } from "@/hooks/use-admin-data"
import { Empty, EmptyDescription, EmptyHeader, EmptyTitle } from "@/components/ui/empty"

export default function AdminDashboardPage() {
  const { data, isLoading, error, refetch } = useDashboardStats()

  if (isLoading) {
    return (
      <div className="flex h-full flex-col">
        <PageHeader title="Dashboard" description="Platform overview" />
        <div className="flex-1 space-y-8 p-8">
          <div className="grid grid-cols-1 gap-4 md:grid-cols-2 lg:grid-cols-4">
            {Array.from({ length: 4 }).map((_, i) => (
              <Skeleton key={i} className="h-28 w-full" />
            ))}
          </div>
          <div className="grid grid-cols-1 gap-6 lg:grid-cols-2">
            <Skeleton className="h-[340px] w-full" />
            <Skeleton className="h-[340px] w-full" />
          </div>
        </div>
      </div>
    )
  }

  if (error) {
    return (
      <div className="flex h-full flex-col">
        <PageHeader title="Dashboard" description="Platform overview" />
        <div className="p-8">
          <AdminErrorState message={error} onRetry={refetch} />
        </div>
      </div>
    )
  }

  if (!data) {
    return null
  }

  const revenueTrend = data.revenueTrend ?? []
  const companiesByPlan = (data.companiesByPlan ?? []).map((c) => ({
    name: c.planName,
    value: c.count,
  }))

  const stats = [
    { label: "Total companies", value: data.totalCompanies, color: "text-blue-600" },
    { label: "Active subscriptions", value: data.activeSubscriptions, color: "text-green-600" },
    { label: "Trial companies", value: data.trialCompanies, color: "text-amber-600" },
    {
      label: "Monthly revenue",
      value: `$${data.monthlyRevenue.toLocaleString()}`,
      color: "text-emerald-600",
    },
  ]

  return (
    <div className="flex h-full flex-col">
      <PageHeader title="Dashboard" description="Overview of your LiftOps platform" />

      <div className="flex-1 overflow-auto">
        <div className="space-y-8 p-8">
          <div className="grid grid-cols-1 gap-4 md:grid-cols-2 lg:grid-cols-4">
            {stats.map((stat) => (
              <Card key={stat.label} className="p-6">
                <p className="text-sm font-medium text-muted-foreground">{stat.label}</p>
                <p className={`mt-2 text-3xl font-bold ${stat.color}`}>{stat.value}</p>
              </Card>
            ))}
          </div>

          <div className="grid grid-cols-1 gap-6 lg:grid-cols-2">
            <Card className="p-6">
              <h3 className="mb-4 text-lg font-semibold">Revenue trend</h3>
              {revenueTrend.length === 0 ? (
                <Empty>
                  <EmptyHeader>
                    <EmptyTitle>No revenue data</EmptyTitle>
                    <EmptyDescription>Revenue trend will appear when data is available.</EmptyDescription>
                  </EmptyHeader>
                </Empty>
              ) : (
                <RevenueTrendChart data={revenueTrend} />
              )}
            </Card>
            <Card className="p-6">
              <h3 className="mb-4 text-lg font-semibold">Companies by plan</h3>
              {companiesByPlan.length === 0 ? (
                <Empty>
                  <EmptyHeader>
                    <EmptyTitle>No plan distribution</EmptyTitle>
                    <EmptyDescription>Chart will populate when companies are assigned to plans.</EmptyDescription>
                  </EmptyHeader>
                </Empty>
              ) : (
                <CompaniesByPlanChart data={companiesByPlan} />
              )}
            </Card>
          </div>
        </div>
      </div>
    </div>
  )
}
