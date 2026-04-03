"use client"

import { useCallback, useEffect, useMemo, useState } from "react"
import Link from "next/link"
import { useRouter, useSearchParams } from "next/navigation"
import { Plus } from "lucide-react"
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
import { AdminErrorState } from "@/components/admin/admin-error-state"
import { useCompanies, usePlans } from "@/hooks/use-admin-data"
import { createCompany } from "@/lib/api"
import type { Company, CompanyStatus } from "@/types/admin"
import { Empty, EmptyDescription, EmptyHeader, EmptyTitle } from "@/components/ui/empty"

const STATUSES: Array<CompanyStatus | ""> = ["", "Active", "Suspended", "SuspendedByAdmin", "Deleted"]

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
  const [creating, setCreating] = useState(false)

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

  const columns: Column<Company>[] = [
    {
      id: "name",
      header: "Company",
      cell: (row) => (
        <Link href={`/admin/companies/${row.id}`} className="font-medium text-primary hover:underline">
          {row.name}
        </Link>
      ),
      sortable: true,
      sortFn: (a, b) => a.name.localeCompare(b.name),
      width: "w-48",
    },
    {
      id: "contact",
      header: "Contact",
      cell: (row) => (
        <div>
          <p className="text-xs text-muted-foreground">{row.contactEmail ?? "—"}</p>
        </div>
      ),
    },
    {
      id: "plan",
      header: "Plan",
      cell: (row) =>
        row.planName ? <PlanCodeBadge code={row.planName} name={row.planName} /> : <span>—</span>,
    },
    {
      id: "usage",
      header: "Users",
      cell: (row) => {
        const max = row.planMaxUsers
        const suffix = max != null && max > 0 ? ` / ${max}` : ""
        return (
          <span>
            {row.currentUserCount}
            {suffix}
          </span>
        )
      },
    },
    {
      id: "status",
      header: "Status",
      cell: (row) => <CompanyStatusBadge status={row.status} />,
      sortable: true,
      sortFn: (a, b) => a.status.localeCompare(b.status),
    },
    {
      id: "createdAt",
      header: "Created",
      cell: (row) => new Date(row.createdAt).toLocaleDateString(),
      sortable: true,
      sortFn: (a, b) => new Date(a.createdAt).getTime() - new Date(b.createdAt).getTime(),
    },
  ]

  const handleCreate = async () => {
    if (!createName.trim() || !createEmail.trim() || !createPlanId) {
      toast.error("Name, contact email, and plan are required.")
      return
    }
    setCreating(true)
    try {
      await createCompany({
        name: createName.trim(),
        contactEmail: createEmail.trim(),
        planId: createPlanId,
      })
      toast.success("Company created")
      setCreateOpen(false)
      setCreateName("")
      setCreateEmail("")
      setCreatePlanId("")
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
          <Dialog open={createOpen} onOpenChange={setCreateOpen}>
            <DialogTrigger asChild>
              <Button type="button">
                <Plus className="mr-2 h-4 w-4" />
                Add company
              </Button>
            </DialogTrigger>
            <DialogContent>
              <DialogHeader>
                <DialogTitle>New company</DialogTitle>
              </DialogHeader>
              <div className="space-y-4 py-2">
                <div className="space-y-2">
                  <Label htmlFor="cname">Name</Label>
                  <Input
                    id="cname"
                    value={createName}
                    onChange={(e) => setCreateName(e.target.value)}
                    placeholder="Acme Lifts"
                  />
                </div>
                <div className="space-y-2">
                  <Label htmlFor="cemail">Contact email</Label>
                  <Input
                    id="cemail"
                    type="email"
                    value={createEmail}
                    onChange={(e) => setCreateEmail(e.target.value)}
                    placeholder="billing@example.com"
                  />
                </div>
                <div className="space-y-2">
                  <Label>Plan</Label>
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
    </div>
  )
}
