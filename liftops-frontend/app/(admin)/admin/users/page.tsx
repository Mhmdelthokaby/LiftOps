"use client"

import { useCallback, useEffect, useMemo, useState } from "react"
import Link from "next/link"
import { useRouter, useSearchParams } from "next/navigation"
import { Shield } from "lucide-react"
import { toast } from "sonner"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { PageHeader } from "@/components/admin/page-header"
import { DataTable, type Column } from "@/components/admin/data-table"
import { RoleBadge } from "@/components/admin/badges"
import { AdminErrorState } from "@/components/admin/admin-error-state"
import { ConfirmationDialog } from "@/components/admin/confirmation-dialog"
import { useGlobalUsers } from "@/hooks/use-admin-data"
import { impersonateUser } from "@/lib/api"
import { beginImpersonation } from "@/lib/impersonation"
import type { AdminUser } from "@/types/admin"

export default function AdminUsersPage() {
  const router = useRouter()
  const searchParams = useSearchParams()
  const page = Math.max(1, Number(searchParams.get("page") ?? "1") || 1)
  const pageSize = Math.min(100, Math.max(1, Number(searchParams.get("pageSize") ?? "10") || 10))
  const companyIdParam = searchParams.get("companyId") ?? ""
  const searchParam = searchParams.get("search") ?? ""

  const [searchInput, setSearchInput] = useState(searchParam)
  const [debouncedSearch, setDebouncedSearch] = useState(searchParam)

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
      router.push(`/admin/users?${next.toString()}`)
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
      companyId: companyIdParam || undefined,
    }),
    [page, pageSize, debouncedSearch, companyIdParam]
  )

  const { data, isLoading, error, refetch } = useGlobalUsers(filters)
  const totalPages = data ? Math.max(1, Math.ceil(data.totalCount / data.pageSize)) : 1

  const displayName = (u: AdminUser) => {
    const n = [u.firstName, u.lastName].filter(Boolean).join(" ").trim()
    return n || u.email
  }

  const handleImpersonate = async (user: AdminUser) => {
    const access = localStorage.getItem("accessToken")
    const refresh = localStorage.getItem("refreshToken")
    const userJson = localStorage.getItem("user")
    if (!access || !refresh || !userJson) {
      toast.error("Session not available.")
      return
    }
    try {
      const res = await impersonateUser(user.id)
      beginImpersonation({
        originalAccessToken: access,
        originalRefreshToken: refresh,
        originalUserJson: userJson,
        impersonationAuth: res,
        displayUserName: displayName(user),
        displayCompanyName: user.companyName ?? "Unknown company",
      })
      toast.success("Now impersonating user")
      router.push("/dashboard")
    } catch (e: unknown) {
      toast.error(e instanceof Error ? e.message : "Impersonation failed")
    }
  }

  const columns: Column<AdminUser>[] = [
    {
      id: "name",
      header: "User",
      cell: (row) => (
        <div>
          <p className="font-medium">{displayName(row)}</p>
          <p className="text-xs text-muted-foreground">{row.email}</p>
        </div>
      ),
      sortable: true,
      sortFn: (a, b) => displayName(a).localeCompare(displayName(b)),
    },
    {
      id: "company",
      header: "Company",
      cell: (row) =>
        row.companyName ? (
          <Link href={`/admin/companies/${row.companyId}`} className="text-primary hover:underline">
            {row.companyName}
          </Link>
        ) : (
          row.companyId
        ),
    },
    {
      id: "role",
      header: "Role",
      cell: (row) => <RoleBadge role={row.role} />,
    },
    {
      id: "status",
      header: "Status",
      cell: (row) => (
        <span
          className={
            row.isActive
              ? "text-green-600"
              : "text-muted-foreground"
          }
        >
          {row.isActive ? "Active" : "Inactive"}
        </span>
      ),
    },
    {
      id: "lastLogin",
      header: "Last login",
      cell: (row) => (row.lastLogin ? new Date(row.lastLogin).toLocaleString() : "—"),
    },
    {
      id: "actions",
      header: "Actions",
      cell: (row) => (
        <ConfirmationDialog
          title="Impersonate user"
          description={`You will sign in as ${displayName(row)}. This is audited.`}
          actionLabel="Impersonate"
          onConfirm={() => handleImpersonate(row)}
          trigger={
            <Button type="button" variant="outline" size="sm">
              <Shield className="mr-2 h-4 w-4" />
              Impersonate
            </Button>
          }
        />
      ),
    },
  ]

  return (
    <div className="flex h-full flex-col">
      <PageHeader
        title="Users"
        description={data ? `${data.totalCount} users` : "Global user directory"}
      />

      <div className="flex-1 space-y-6 overflow-auto p-8">
        <div className="flex flex-wrap gap-4">
          <div className="space-y-2">
            <Label>Search</Label>
            <Input
              className="w-72"
              placeholder="Email or name…"
              value={searchInput}
              onChange={(e) => setSearchInput(e.target.value)}
            />
          </div>
          {companyIdParam && (
            <Button type="button" variant="outline" onClick={() => updateQuery({ companyId: undefined, page: "1" })}>
              Clear company filter
            </Button>
          )}
        </div>

        {isLoading && <p className="text-sm text-muted-foreground">Loading…</p>}
        {error && <AdminErrorState message={error} onRetry={refetch} />}
        {!isLoading && !error && data && data.items.length === 0 && (
          <p className="text-sm text-muted-foreground">No users found.</p>
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
