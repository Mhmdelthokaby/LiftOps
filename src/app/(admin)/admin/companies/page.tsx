"use client"

import { useCallback, useEffect, useMemo, useState } from "react"
import Link from "next/link"
import { useRouter, useSearchParams } from "next/navigation"
import { Eye, EyeOff, Pencil, Plus, Trash2 } from "lucide-react"
import { toast } from "sonner"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select"
import {
  Dialog,
  DialogContent,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog"
import { PageHeader } from "@/components/admin/page-header"
import { DataTable, type Column } from "@/components/admin/data-table"
import { CompanyStatusBadge, PlanCodeBadge } from "@/components/admin/badges"
import { EditCompanyDialog } from "@/components/admin/companies/edit-company-dialog"
import { AdminErrorState } from "@/components/admin/admin-error-state"
import { useCompanies, usePlans } from "@/hooks/use-admin-data"
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
import { createCompany, softDeleteCompany } from "@/lib/api"
import { generatePowerPass } from "@/lib/power-pass"
import type { Company, CompanyStatus } from "@/types/admin"
import { Empty, EmptyDescription, EmptyHeader, EmptyTitle } from "@/components/ui/empty"

const STATUSES: Array<CompanyStatus | ""> = [
  "",
  "Active",
  "Inactive",
  "Suspended",
  "SuspendedByAdmin",
  "Deleted",
]

export default function AdminCompaniesPage() {
  const router = useRouter()
  const searchParams = useSearchParams()
  const page = Math.max(1, Number(searchParams.get("page") ?? "1") || 1)
  const pageSize = Math.min(100, Math.max(1, Number(searchParams.get("pageSize") ?? "10") || 10))
  const statusParam = searchParams.get("status") ?? ""
  const planIdParam = searchParams.get("planId") ?? ""
  const searchParam = searchParams.get("search") ?? ""

  const [searchInput, setSearchInput] = useState(searchParam)
  const [debouncedSearch, setDebouncedSearch] = useState(searchParam)
  const [createOpen, setCreateOpen] = useState(false)
  const [createName, setCreateName] = useState("")
  const [createEmail, setCreateEmail] = useState("")
  const [createPlanId, setCreatePlanId] = useState("")
  const [createPassword, setCreatePassword] = useState("")
  const [showCreatePassword, setShowCreatePassword] = useState(false)
  const [creating, setCreating] = useState(false)
  const [editCompany, setEditCompany] = useState<Company | null>(null)
  const [deleteTarget, setDeleteTarget] = useState<Company | null>(null)
  const [deleteLoading, setDeleteLoading] = useState(false)

  const resetCreateDialog = () => {
    setCreateName("")
    setCreateEmail("")
    setCreatePlanId("")
    setCreatePassword("")
    setShowCreatePassword(false)
  }

  useEffect(() => {
    const t = setTimeout(() => setDebouncedSearch(searchInput.trim()), 300)
    return () => clearTimeout(t)
  }, [searchInput])

  const updateQuery = useCallback(
    (updates: Record<string, string | undefined>) => {
      const next = new URLSearchParams(searchParams.toString())
      for (const [k, v] of Object.entries(updates)) {
        if (v === undefined || v === "") next.delete(k)
        else next.set(k, v)
      }
      router.push(`/admin/companies?${next.toString()}`)
    },
    [router, searchParams]
  )

  useEffect(() => {
    if (debouncedSearch === searchParam) return
    updateQuery({ search: debouncedSearch || undefined, page: "1" })
  }, [debouncedSearch, searchParam, updateQuery])

  const filters = useMemo(
    () => ({
      page,
      pageSize,
      search: debouncedSearch || undefined,
      status: statusParam || undefined,
      planId: planIdParam || undefined,
    }),
    [page, pageSize, debouncedSearch, statusParam, planIdParam]
  )

  const { data, isLoading, error, refetch } = useCompanies(filters)
  const { data: plans } = usePlans()

  const totalPages = data ? Math.max(1, Math.ceil(data.totalCount / data.pageSize)) : 1

  const handleConfirmDelete = async () => {
    if (!deleteTarget) return
    setDeleteLoading(true)
    try {
      await softDeleteCompany(deleteTarget.id)
      toast.success("Company deactivated. Data is retained.")
      setDeleteTarget(null)
      refetch()
    } catch (e: unknown) {
      toast.error(e instanceof Error ? e.message : "Deactivate failed")
    } finally {
      setDeleteLoading(false)
    }
  }

  const columns: Column<Company>[] = [
    {
      id: "name",
      header: "Name",
      cell: (row) => <span className="font-medium">{row.name}</span>,
      sortable: true,
      sortFn: (a, b) => a.name.localeCompare(b.name),
      width: "w-44",
    },
    {
      id: "status",
      header: "Status",
      cell: (row) => <CompanyStatusBadge status={row.status} />,
      sortable: true,
      sortFn: (a, b) => a.status.localeCompare(b.status),
    },
    {
      id: "plan",
      header: "Plan",
      cell: (row) => {
        const label = row.subscriptionPlan ?? row.planName
        return label ? <PlanCodeBadge code={label} name={label} /> : <span>—</span>
      },
    },
    {
      id: "createdAt",
      header: "Created",
      cell: (row) => new Date(row.createdAt).toLocaleDateString(),
      sortable: true,
      sortFn: (a, b) => new Date(a.createdAt).getTime() - new Date(b.createdAt).getTime(),
    },
    {
      id: "actions",
      header: "",
      cell: (row) => (
        <div className="flex items-center justify-end gap-1">
          <Button type="button" variant="ghost" size="icon" className="h-8 w-8" asChild title="View">
            <Link href={`/admin/companies/${row.id}`}>
              <Eye className="h-4 w-4" />
            </Link>
          </Button>
          <Button
            type="button"
            variant="ghost"
            size="icon"
            className="h-8 w-8"
            title="Edit"
            onClick={() => setEditCompany(row)}
          >
            <Pencil className="h-4 w-4" />
          </Button>
          <Button
            type="button"
            variant="ghost"
            size="icon"
            className="h-8 w-8 text-destructive hover:text-destructive"
            title="Deactivate"
            disabled={row.status === "Deleted"}
            onClick={() => setDeleteTarget(row)}
          >
            <Trash2 className="h-4 w-4" />
          </Button>
        </div>
      ),
      width: "w-28",
    },
  ]

  const handleGeneratePowerPass = async () => {
    const password = generatePowerPass()
    setCreatePassword(password)
    try {
      await navigator.clipboard.writeText(password)
      toast.success("Strong password generated and copied!")
    } catch {
      toast.info("Password generated — copy it from the field (clipboard unavailable).")
    }
  }

  const handleCreate = async () => {
    if (!createName.trim() || !createEmail.trim() || !createPlanId) {
      toast.error("Company name, main user email, and plan are required.")
      return
    }
    if (createPassword.length < 8) {
      toast.error("Password must be at least 8 characters (use Generate PowerPass or type your own).")
      return
    }
    setCreating(true)
    try {
      await createCompany({
        companyName: createName.trim(),
        adminEmail: createEmail.trim(),
        planId: createPlanId,
        password: createPassword,
      })
      toast.success("Company created. The main user can sign in with that email and password.")
      setCreateOpen(false)
      resetCreateDialog()
      refetch()
    } catch (e: unknown) {
      toast.error(e instanceof Error ? e.message : "Failed to create company")
    } finally {
      setCreating(false)
    }
  }

  return (
    <div className="flex h-full flex-col">
      <PageHeader
        title="Companies"
        description="Manage tenants and subscriptions"
        actions={
          <Dialog
            open={createOpen}
            onOpenChange={(open) => {
              setCreateOpen(open)
              if (!open) resetCreateDialog()
            }}
          >
            <DialogTrigger asChild>
              <Button type="button">
                <Plus className="mr-2 h-4 w-4" />
                Add company
              </Button>
            </DialogTrigger>
            <DialogContent className="sm:max-w-md">
              <DialogHeader>
                <DialogTitle>New company</DialogTitle>
                <p className="text-sm text-muted-foreground">
                  Creates the tenant and one primary user (Manager). They sign in with the email and password below.
                </p>
              </DialogHeader>
              <div className="space-y-4 py-2">
                <div className="space-y-2">
                  <Label htmlFor="cname">Company name</Label>
                  <Input
                    id="cname"
                    value={createName}
                    onChange={(e) => setCreateName(e.target.value)}
                    placeholder="Acme Lifts"
                    autoComplete="organization"
                  />
                </div>
                <div className="space-y-2">
                  <Label htmlFor="cemail">Main user email</Label>
                  <Input
                    id="cemail"
                    type="email"
                    value={createEmail}
                    onChange={(e) => setCreateEmail(e.target.value)}
                    placeholder="admin@acmelifts.com"
                    autoComplete="off"
                  />
                  <p className="text-xs text-muted-foreground">
                    Login for the company&apos;s first Manager account (same as billing contact on the backend).
                  </p>
                </div>
                <div className="space-y-2">
                  <Label htmlFor="cpass">Password for main user</Label>
                  <div className="flex flex-col gap-2 sm:flex-row sm:items-start">
                    <div className="relative flex-1">
                      <Input
                        id="cpass"
                        type={showCreatePassword ? "text" : "password"}
                        value={createPassword}
                        onChange={(e) => setCreatePassword(e.target.value)}
                        placeholder="Min. 8 characters"
                        autoComplete="new-password"
                        className="pr-10"
                      />
                      <Button
                        type="button"
                        variant="ghost"
                        size="sm"
                        className="absolute right-0 top-0 h-full px-3 py-2 hover:bg-transparent"
                        onClick={() => setShowCreatePassword(!showCreatePassword)}
                        aria-label={showCreatePassword ? "Hide password" : "Show password"}
                      >
                        {showCreatePassword ? (
                          <EyeOff className="h-4 w-4 text-muted-foreground" />
                        ) : (
                          <Eye className="h-4 w-4 text-muted-foreground" />
                        )}
                      </Button>
                    </div>
                    <Button
                      type="button"
                      variant="outline"
                      className="shrink-0 whitespace-nowrap"
                      onClick={handleGeneratePowerPass}
                    >
                      Generate PowerPass
                    </Button>
                  </div>
                  <p className="text-xs text-muted-foreground">
                    Use Generate PowerPass for a strong 20-character password (also copied to clipboard).
                  </p>
                </div>
                <div className="space-y-2">
                  <Label>Subscription plan</Label>
                  <Select value={createPlanId} onValueChange={setCreatePlanId}>
                    <SelectTrigger>
                      <SelectValue placeholder="Select plan" />
                    </SelectTrigger>
                    <SelectContent>
                      {(plans ?? []).map((p) => (
                        <SelectItem key={p.id} value={p.id}>
                          {p.name} ({p.code})
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>
              </div>
              <DialogFooter>
                <Button type="button" variant="outline" onClick={() => setCreateOpen(false)}>
                  Cancel
                </Button>
                <Button type="button" onClick={handleCreate} disabled={creating}>
                  {creating ? "Creating…" : "Create"}
                </Button>
              </DialogFooter>
            </DialogContent>
          </Dialog>
        }
      />

      <div className="flex-1 overflow-auto p-8">
        <div className="mb-6 flex flex-wrap items-end gap-4">
          <div className="space-y-2">
            <Label>Search</Label>
            <Input
              className="w-64"
              placeholder="Company name or email…"
              value={searchInput}
              onChange={(e) => setSearchInput(e.target.value)}
            />
          </div>
          <div className="space-y-2">
            <Label>Status</Label>
            <Select
              value={statusParam || "__all__"}
              onValueChange={(v) => updateQuery({ status: v === "__all__" ? undefined : v, page: "1" })}
            >
              <SelectTrigger className="w-44">
                <SelectValue placeholder="All" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="__all__">All</SelectItem>
                {STATUSES.filter(Boolean).map((s) => (
                  <SelectItem key={s} value={s as string}>
                    {s}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>
          <div className="space-y-2">
            <Label>Plan</Label>
            <Select
              value={planIdParam || "__all__"}
              onValueChange={(v) => updateQuery({ planId: v === "__all__" ? undefined : v, page: "1" })}
            >
              <SelectTrigger className="w-48">
                <SelectValue placeholder="All plans" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="__all__">All plans</SelectItem>
                {(plans ?? []).map((p) => (
                  <SelectItem key={p.id} value={p.id}>
                    {p.name}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>
        </div>

        {isLoading && <p className="text-sm text-muted-foreground">Loading companies…</p>}
        {error && <AdminErrorState message={error} onRetry={refetch} />}
        {!isLoading && !error && data && data.items.length === 0 && (
          <Empty>
            <EmptyHeader>
              <EmptyTitle>No companies</EmptyTitle>
              <EmptyDescription>Adjust filters or create a new company.</EmptyDescription>
            </EmptyHeader>
          </Empty>
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

      <EditCompanyDialog
        company={editCompany}
        open={editCompany !== null}
        onOpenChange={(open) => {
          if (!open) setEditCompany(null)
        }}
        onSaved={() => refetch()}
      />

      <AlertDialog open={deleteTarget !== null} onOpenChange={(open) => !open && setDeleteTarget(null)}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Deactivate company?</AlertDialogTitle>
            <AlertDialogDescription>
              This will deactivate the tenant and hide it from the default company list. Historical data stays in
              the database for audits and possible reactivation.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel disabled={deleteLoading}>Cancel</AlertDialogCancel>
            <AlertDialogAction
              className="bg-destructive text-destructive-foreground hover:bg-destructive/90"
              disabled={deleteLoading}
              onClick={(e) => {
                e.preventDefault()
                void handleConfirmDelete()
              }}
            >
              {deleteLoading ? "Working…" : "Deactivate"}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  )
}
