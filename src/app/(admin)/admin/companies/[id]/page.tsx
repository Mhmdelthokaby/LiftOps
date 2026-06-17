"use client"

import { useCallback, useEffect, useState } from "react"
import Link from "next/link"
import { useParams, useRouter } from "next/navigation"
import { ArrowLeft, CalendarClock, Pencil, RefreshCw } from "lucide-react"
import { toast } from "sonner"
import { Button } from "@/components/ui/button"
import { Card } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Textarea } from "@/components/ui/textarea"
import {
  Dialog,
  DialogContent,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog"
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select"
import {
  Tooltip,
  TooltipContent,
  TooltipProvider,
  TooltipTrigger,
} from "@/components/ui/tooltip"
import { PageHeader } from "@/components/admin/page-header"
import { UsageBar } from "@/components/admin/usage-bar"
import { CompanyStatusBadge, PlanCodeBadge, SubscriptionStatusBadge } from "@/components/admin/badges"
import { ConfirmationDialog } from "@/components/admin/confirmation-dialog"
import { AdminErrorState } from "@/components/admin/admin-error-state"
import { Skeleton } from "@/components/ui/skeleton"
import { EditCompanyDialog } from "@/components/admin/companies/edit-company-dialog"
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from "@/components/ui/alert-dialog"
import { useCompanyDetails, usePlans } from "@/hooks/use-admin-data"
import {
  activateCompany,
  changePlan,
  extendTrial,
  getPlanById,
  softDeleteCompany,
  suspendCompany,
} from "@/lib/api"
import type { Plan } from "@/types/admin"

export default function AdminCompanyDetailPage() {
  const params = useParams()
  const router = useRouter()
  const id = typeof params.id === "string" ? params.id : params.id?.[0]
  const { data: company, isLoading, error, refetch } = useCompanyDetails(id)
  const { data: plans } = usePlans()

  const [planLimits, setPlanLimits] = useState<Plan | null>(null)
  const [suspendReason, setSuspendReason] = useState("")
  const [suspendOpen, setSuspendOpen] = useState(false)
  const [trialDays, setTrialDays] = useState(14)
  const [trialOpen, setTrialOpen] = useState(false)
  const [newPlanId, setNewPlanId] = useState("")
  const [planOpen, setPlanOpen] = useState(false)
  const [actionLoading, setActionLoading] = useState(false)
  const [editOpen, setEditOpen] = useState(false)
  const [deactivateOpen, setDeactivateOpen] = useState(false)
  const [deactivateLoading, setDeactivateLoading] = useState(false)

  const loadPlanLimits = useCallback(async (planId: string | undefined) => {
    if (!planId) {
      setPlanLimits(null)
      return
    }
    try {
      const p = await getPlanById(planId)
      setPlanLimits(p)
    } catch {
      setPlanLimits(null)
    }
  }, [])

  useEffect(() => {
    const pid = company?.subscription?.planId ?? company?.planId
    void loadPlanLimits(pid)
  }, [company, loadPlanLimits])

  const maxUsers = company?.planMaxUsers ?? planLimits?.maxUsers ?? 0
  const maxElevators = company?.planMaxElevators ?? planLimits?.maxElevators ?? 0
  const atUserLimit = maxUsers > 0 && company && company.currentUserCount >= maxUsers

  const handleSuspend = async () => {
    if (!id || !suspendReason.trim()) {
      toast.error("Please provide a suspension reason.")
      return
    }
    setActionLoading(true)
    try {
      await suspendCompany(id, suspendReason.trim())
      toast.success("Company suspended")
      setSuspendOpen(false)
      setSuspendReason("")
      refetch()
    } catch (e: unknown) {
      toast.error(e instanceof Error ? e.message : "Suspend failed")
    } finally {
      setActionLoading(false)
    }
  }

  const handleActivate = async () => {
    if (!id) return
    setActionLoading(true)
    try {
      await activateCompany(id)
      toast.success("Company activated")
      refetch()
    } catch (e: unknown) {
      toast.error(e instanceof Error ? e.message : "Activate failed")
    } finally {
      setActionLoading(false)
    }
  }

  const handleExtendTrial = async () => {
    if (!id || trialDays <= 0) {
      toast.error("Enter a positive number of days.")
      return
    }
    setActionLoading(true)
    try {
      await extendTrial(id, trialDays)
      toast.success("Trial extended")
      setTrialOpen(false)
      refetch()
    } catch (e: unknown) {
      toast.error(e instanceof Error ? e.message : "Extend trial failed")
    } finally {
      setActionLoading(false)
    }
  }

  const handleDeactivate = async () => {
    if (!id) return
    setDeactivateLoading(true)
    try {
      await softDeleteCompany(id)
      toast.success("Company deactivated")
      setDeactivateOpen(false)
      router.push("/admin/companies")
    } catch (e: unknown) {
      toast.error(e instanceof Error ? e.message : "Deactivate failed")
    } finally {
      setDeactivateLoading(false)
    }
  }

  const handleChangePlan = async () => {
    if (!id || !newPlanId) {
      toast.error("Select a plan.")
      return
    }
    setActionLoading(true)
    try {
      await changePlan(id, newPlanId)
      toast.success("Plan updated")
      setPlanOpen(false)
      setNewPlanId("")
      refetch()
    } catch (e: unknown) {
      toast.error(e instanceof Error ? e.message : "Change plan failed")
    } finally {
      setActionLoading(false)
    }
  }

  if (isLoading) {
    return (
      <div className="flex h-full flex-col">
        <PageHeader title="Company" />
        <div className="space-y-4 p-8">
          <Skeleton className="h-10 w-64" />
          <Skeleton className="h-48 w-full" />
        </div>
      </div>
    )
  }

  if (error || !company) {
    return (
      <div className="flex h-full flex-col">
        <PageHeader title="Company" />
        <div className="p-8">
          <AdminErrorState
            message={error ?? "Company not found"}
            onRetry={() => (id ? refetch() : router.push("/admin/companies"))}
          />
          <Button type="button" variant="outline" className="mt-4" asChild>
            <Link href="/admin/companies">
              <ArrowLeft className="mr-2 h-4 w-4" />
              Back to companies
            </Link>
          </Button>
        </div>
      </div>
    )
  }

  const sub = company.subscription
  const subscriptionPlanLabel =
    (sub?.planName && sub.planName.trim()) ||
    company.subscriptionPlan?.trim() ||
    company.planName?.trim() ||
    null

  return (
    <TooltipProvider>
      <div className="flex h-full flex-col">
        <PageHeader
          title={company.name}
          description={company.slug ? `Slug: ${company.slug}` : "Company details"}
          actions={
            <div className="flex flex-wrap gap-2">
              <Button type="button" variant="outline" asChild>
                <Link href="/admin/companies">
                  <ArrowLeft className="mr-2 h-4 w-4" />
                  Back
                </Link>
              </Button>
              {company.status !== "Deleted" && (
                <>
                  <Button type="button" variant="outline" onClick={() => setEditOpen(true)}>
                    <Pencil className="mr-2 h-4 w-4" />
                    Edit company
                  </Button>
                  <Button type="button" variant="destructive" onClick={() => setDeactivateOpen(true)}>
                    Deactivate
                  </Button>
                </>
              )}
            </div>
          }
        />

        <div className="flex-1 space-y-8 overflow-auto p-8">
          <div className="grid grid-cols-1 gap-6 lg:grid-cols-3">
            <Card className="p-6 lg:col-span-2">
              <h3 className="mb-4 text-lg font-semibold">Company</h3>
              <div className="grid grid-cols-2 gap-4 text-sm">
                <div>
                  <p className="text-muted-foreground">Plan tier</p>
                  <p className="font-medium">{company.subscriptionPlan ?? company.planName ?? "—"}</p>
                </div>
                <div>
                  <p className="text-muted-foreground">Active flag</p>
                  <p className="font-medium">{company.isActive === false ? "No" : "Yes"}</p>
                </div>
                <div>
                  <p className="text-muted-foreground">Billing / contact email</p>
                  <p className="font-medium">{company.contactEmail ?? "—"}</p>
                </div>
                <div>
                  <p className="text-muted-foreground">Status</p>
                  <div className="mt-1">
                    <CompanyStatusBadge status={company.status} />
                  </div>
                </div>
                <div>
                  <p className="text-muted-foreground">Created</p>
                  <p className="font-medium">{new Date(company.createdAt).toLocaleString()}</p>
                </div>
                <div>
                  <p className="text-muted-foreground">Company ID</p>
                  <p className="font-mono text-xs break-all">{company.id}</p>
                </div>
              </div>
            </Card>


            <Card className="p-6">
              <h3 className="mb-4 text-lg font-semibold">Subscription</h3>
              {sub ? (
                <div className="space-y-3 text-sm">
                  <div>
                    <p className="text-muted-foreground">Plan</p>
                    <div className="mt-1">
                      {subscriptionPlanLabel ? (
                        <PlanCodeBadge code={subscriptionPlanLabel} name={subscriptionPlanLabel} />
                      ) : (
                        <span className="font-mono text-xs text-muted-foreground" title="Plan id">
                          {sub.planId}
                        </span>
                      )}
                    </div>
                  </div>
                  <div>
                    <p className="text-muted-foreground">Status</p>
                    <div className="mt-1">
                      <SubscriptionStatusBadge status={sub.status} />
                    </div>
                  </div>
                  <div>
                    <p className="text-muted-foreground">Period</p>
                    <p className="font-medium">
                      {new Date(sub.currentPeriodStart).toLocaleDateString()} →{" "}
                      {new Date(sub.currentPeriodEnd).toLocaleDateString()}
                    </p>
                  </div>
                  <div>
                    <p className="text-muted-foreground">Billing</p>
                    <p className="font-medium">{sub.billingCycle}</p>
                  </div>
                </div>
              ) : (
                <p className="text-muted-foreground">No subscription on record.</p>
              )}
            </Card>
          </div>

          <Card className="p-6">
            <h3 className="mb-4 text-lg font-semibold">Primary admin (Manager)</h3>
            <div className="grid grid-cols-1 gap-4 text-sm sm:grid-cols-3">
              <div>
                <p className="text-muted-foreground">Name</p>
                <p className="font-medium">{company.defaultAdminName ?? "—"}</p>
              </div>
              <div>
                <p className="text-muted-foreground">Email</p>
                <p className="font-medium">{company.defaultAdminEmail ?? "—"}</p>
              </div>
              <div>
                <p className="text-muted-foreground">Phone</p>
                <p className="font-medium">{company.defaultAdminPhone ?? "—"}</p>
              </div>
            </div>
          </Card>

          {(maxUsers > 0 || maxElevators > 0) && (
            <Card className="p-6">
              <h3 className="mb-6 text-lg font-semibold">Usage</h3>
              <div className="grid grid-cols-1 gap-8 md:grid-cols-2">
                {maxUsers > 0 && (
                  <UsageBar current={company.currentUserCount} max={maxUsers} label="Users" />
                )}
                {maxElevators > 0 && (
                  <UsageBar
                    current={company.currentElevatorCount}
                    max={maxElevators}
                    label="Elevators"
                  />
                )}
              </div>
            </Card>
          )}

          <div className="flex flex-wrap gap-3">
            <Dialog open={trialOpen} onOpenChange={setTrialOpen}>
              <DialogTrigger asChild>
                <Button type="button" variant="outline">
                  <CalendarClock className="mr-2 h-4 w-4" />
                  Extend trial
                </Button>
              </DialogTrigger>
              <DialogContent>
                <DialogHeader>
                  <DialogTitle>Extend trial</DialogTitle>
                </DialogHeader>
                <div className="space-y-2 py-2">
                  <Label htmlFor="days">Additional days</Label>
                  <Input
                    id="days"
                    type="number"
                    min={1}
                    value={trialDays}
                    onChange={(e) => setTrialDays(Number(e.target.value))}
                  />
                </div>
                <DialogFooter>
                  <Button type="button" variant="outline" onClick={() => setTrialOpen(false)}>
                    Cancel
                  </Button>
                  <Button type="button" onClick={handleExtendTrial} disabled={actionLoading}>
                    Apply
                  </Button>
                </DialogFooter>
              </DialogContent>
            </Dialog>

            <Dialog open={planOpen} onOpenChange={setPlanOpen}>
              <DialogTrigger asChild>
                <Button type="button" variant="outline">
                  <RefreshCw className="mr-2 h-4 w-4" />
                  Change plan
                </Button>
              </DialogTrigger>
              <DialogContent>
                <DialogHeader>
                  <DialogTitle>Change plan</DialogTitle>
                </DialogHeader>
                <div className="space-y-2 py-2">
                  <Label>New plan</Label>
                  <Select value={newPlanId} onValueChange={setNewPlanId}>
                    <SelectTrigger>
                      <SelectValue placeholder="Select plan" />
                    </SelectTrigger>
                    <SelectContent>
                      {(plans ?? [])
                        .filter((p) => p.isActive)
                        .map((p) => (
                          <SelectItem key={p.id} value={p.id}>
                            {p.name} ({p.code})
                          </SelectItem>
                        ))}
                    </SelectContent>
                  </Select>
                </div>
                <DialogFooter>
                  <Button type="button" variant="outline" onClick={() => setPlanOpen(false)}>
                    Cancel
                  </Button>
                  <Button type="button" onClick={handleChangePlan} disabled={actionLoading}>
                    Save
                  </Button>
                </DialogFooter>
              </DialogContent>
            </Dialog>

            <Dialog open={suspendOpen} onOpenChange={setSuspendOpen}>
              <DialogTrigger asChild>
                <Button type="button" variant="destructive" disabled={company.status === "Deleted"}>
                  Suspend company
                </Button>
              </DialogTrigger>
              <DialogContent>
                <DialogHeader>
                  <DialogTitle>Suspend company</DialogTitle>
                </DialogHeader>
                <Textarea
                  placeholder="Reason (required)"
                  value={suspendReason}
                  onChange={(e) => setSuspendReason(e.target.value)}
                  rows={4}
                />
                <DialogFooter>
                  <Button type="button" variant="outline" onClick={() => setSuspendOpen(false)}>
                    Cancel
                  </Button>
                  <Button type="button" variant="destructive" onClick={handleSuspend} disabled={actionLoading}>
                    Suspend
                  </Button>
                </DialogFooter>
              </DialogContent>
            </Dialog>

            {company.status !== "Active" && company.status !== "Deleted" && (
              <Button type="button" variant="secondary" onClick={handleActivate} disabled={actionLoading}>
                Activate company
              </Button>
            )}

            {atUserLimit ? (
              <Tooltip>
                <TooltipTrigger asChild>
                  <span>
                    <Button type="button" variant="outline" disabled>
                      Add user
                    </Button>
                  </span>
                </TooltipTrigger>
                <TooltipContent>Plan user limit reached. Upgrade plan or remove users.</TooltipContent>
              </Tooltip>
            ) : (
              <Button type="button" variant="outline" asChild>
                <Link href={`/admin/users?companyId=${company.id}`}>Add user</Link>
              </Button>
            )}
          </div>
        </div>

        <EditCompanyDialog
          company={company}
          open={editOpen}
          onOpenChange={setEditOpen}
          onSaved={() => refetch()}
        />

        <AlertDialog open={deactivateOpen} onOpenChange={setDeactivateOpen}>
          <AlertDialogContent>
            <AlertDialogHeader>
              <AlertDialogTitle>Deactivate this company?</AlertDialogTitle>
              <AlertDialogDescription>
                The tenant will be hidden from default lists and marked inactive. Operational data is not deleted.
              </AlertDialogDescription>
            </AlertDialogHeader>
            <AlertDialogFooter>
              <AlertDialogCancel disabled={deactivateLoading}>Cancel</AlertDialogCancel>
              <AlertDialogAction
                className="bg-destructive text-destructive-foreground hover:bg-destructive/90"
                disabled={deactivateLoading}
                onClick={(e) => {
                  e.preventDefault()
                  void handleDeactivate()
                }}
              >
                {deactivateLoading ? "Working…" : "Deactivate"}
              </AlertDialogAction>
            </AlertDialogFooter>
          </AlertDialogContent>
        </AlertDialog>
      </div>
    </TooltipProvider>
  )
}
