"use client"

import Link from "next/link"
import { Plus, Trash2 } from "lucide-react"
import { toast } from "sonner"
import { Button } from "@/components/ui/button"
import { Card } from "@/components/ui/card"
import { PageHeader } from "@/components/admin/page-header"
import { PlanCodeBadge } from "@/components/admin/badges"
import { ConfirmationDialog } from "@/components/admin/confirmation-dialog"
import { AdminErrorState } from "@/components/admin/admin-error-state"
import { Skeleton } from "@/components/ui/skeleton"
import { usePlans } from "@/hooks/use-admin-data"
import { deletePlan } from "@/lib/api"
import type { Plan } from "@/types/admin"
import { Empty, EmptyDescription, EmptyHeader, EmptyTitle } from "@/components/ui/empty"

function planFeatures(p: Plan): string[] {
  const f: string[] = []
  if (p.allowEmergencyModule) f.push("Emergency")
  if (p.allowFaultsModule) f.push("Faults")
  if (p.allowFinanceModule) f.push("Finance")
  if (p.allowInventoryModule) f.push("Inventory")
  if (p.allowApiAccess) f.push("API access")
  return f
}

export default function AdminPlansPage() {
  const { data, isLoading, error, refetch } = usePlans()

  const handleDelete = async (plan: Plan) => {
    try {
      await deletePlan(plan.id)
      toast.success("Plan deleted")
      refetch()
    } catch (e: unknown) {
      toast.error(e instanceof Error ? e.message : "Delete failed")
    }
  }

  if (isLoading) {
    return (
      <div className="flex h-full flex-col">
        <PageHeader title="Plans" />
        <div className="grid grid-cols-1 gap-6 p-8 md:grid-cols-3">
          {Array.from({ length: 3 }).map((_, i) => (
            <Skeleton key={i} className="h-64 w-full" />
          ))}
        </div>
      </div>
    )
  }

  if (error) {
    return (
      <div className="flex h-full flex-col">
        <PageHeader title="Plans" />
        <div className="p-8">
          <AdminErrorState message={error} onRetry={refetch} />
        </div>
      </div>
    )
  }

  const plans = data ?? []

  return (
    <div className="flex h-full flex-col">
      <PageHeader
        title="Plans"
        description="Subscription plans and entitlements"
        actions={
          <Button type="button" asChild>
            <Link href="/admin/plans/new">
              <Plus className="mr-2 h-4 w-4" />
              Create plan
            </Link>
          </Button>
        }
      />

      <div className="flex-1 overflow-auto p-8">
        {plans.length === 0 ? (
          <Empty>
            <EmptyHeader>
              <EmptyTitle>No plans</EmptyTitle>
              <EmptyDescription>Create a plan to assign to companies.</EmptyDescription>
            </EmptyHeader>
          </Empty>
        ) : (
          <div className="grid grid-cols-1 gap-6 md:grid-cols-3">
            {plans.map((plan) => (
              <Card key={plan.id} className="flex flex-col p-6">
                <div className="mb-4 flex items-start justify-between gap-2">
                  <div>
                    <h3 className="text-lg font-semibold">{plan.name}</h3>
                    <PlanCodeBadge code={plan.code} />
                    {!plan.isActive && (
                      <p className="mt-1 text-xs text-muted-foreground">Inactive</p>
                    )}
                  </div>
                </div>

                <div className="flex-1 space-y-4 text-sm">
                  <div>
                    <p className="text-2xl font-bold">${plan.monthlyPrice}</p>
                    <p className="text-muted-foreground">/ month · ${plan.yearlyPrice} / year</p>
                    <p className="text-muted-foreground">Trial: {plan.trialDays} days</p>
                  </div>
                  <div>
                    <p className="font-medium">Limits</p>
                    <ul className="mt-1 space-y-1 text-muted-foreground">
                      <li>Users: {plan.maxUsers}</li>
                      <li>Elevators: {plan.maxElevators}</li>
                      <li>Maintenance contracts: {plan.maxMaintenanceContracts}</li>
                      <li>Installation projects: {plan.maxInstallationProjects}</li>
                    </ul>
                  </div>
                  <div>
                    <p className="font-medium">Modules</p>
                    <ul className="mt-1 space-y-1">
                      {planFeatures(plan).map((x) => (
                        <li key={x} className="flex items-center text-muted-foreground">
                          <span className="mr-2 h-1.5 w-1.5 rounded-full bg-primary" />
                          {x}
                        </li>
                      ))}
                      {planFeatures(plan).length === 0 && (
                        <li className="text-muted-foreground">No optional modules</li>
                      )}
                    </ul>
                  </div>
                </div>

                <div className="mt-6 flex gap-2 border-t border-border pt-4">
                  <Button type="button" variant="outline" size="sm" className="flex-1" asChild>
                    <Link href={`/admin/plans/new?edit=${plan.id}`}>Edit</Link>
                  </Button>
                  <ConfirmationDialog
                    title="Delete plan"
                    description={`Delete ${plan.name}? This cannot be undone.`}
                    actionLabel="Delete"
                    isDestructive
                    onConfirm={() => handleDelete(plan)}
                    trigger={
                      <Button type="button" variant="outline" size="sm" className="flex-1">
                        <Trash2 className="mr-2 h-4 w-4" />
                        Delete
                      </Button>
                    }
                  />
                </div>
              </Card>
            ))}
          </div>
        )}
      </div>
    </div>
  )
}
