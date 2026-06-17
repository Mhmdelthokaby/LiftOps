"use client"

import { useMemo, useState } from "react"
import { ChevronDown, ChevronUp } from "lucide-react"
import { Button } from "@/components/ui/button"
import { Checkbox } from "@/components/ui/checkbox"
import { cn } from "@/lib/utils"

export interface Column<T> {
  id: string
  header: string
  cell: (row: T) => React.ReactNode
  width?: string
  sortable?: boolean
  sortFn?: (a: T, b: T) => number
}

export interface ServerPaginationProps {
  page: number
  totalPages: number
  onPageChange: (page: number) => void
}

interface DataTableProps<T extends { id: string }> {
  columns: Column<T>[]
  data: T[]
  selectable?: boolean
  onSelectionChange?: (selectedIds: string[]) => void
  pageSize?: number
  /** When set, all rows are shown and pagination controls call parent. */
  serverPagination?: ServerPaginationProps
}

export function DataTable<T extends { id: string }>({
  columns,
  data,
  selectable = false,
  onSelectionChange,
  pageSize = 10,
  serverPagination,
}: DataTableProps<T>) {
  const [sortColumn, setSortColumn] = useState<string | null>(null)
  const [sortDirection, setSortDirection] = useState<"asc" | "desc">("asc")
  const [selectedIds, setSelectedIds] = useState<Set<string>>(new Set())
  const [currentPage, setCurrentPage] = useState(0)

  const sortedData = useMemo(() => {
    if (!sortColumn) return data
    const column = columns.find((c) => c.id === sortColumn)
    if (!column?.sortFn) return data
    const sorted = [...data].sort(column.sortFn)
    return sortDirection === "desc" ? sorted.reverse() : sorted
  }, [data, sortColumn, sortDirection, columns])

  const paginatedData = useMemo(() => {
    if (serverPagination) return sortedData
    const start = currentPage * pageSize
    return sortedData.slice(start, start + pageSize)
  }, [sortedData, currentPage, pageSize, serverPagination])

  const totalPages = serverPagination
    ? serverPagination.totalPages
    : Math.max(1, Math.ceil(sortedData.length / pageSize))

  const displayPageIndex = serverPagination ? serverPagination.page - 1 : currentPage

  const handleSort = (columnId: string) => {
    if (sortColumn === columnId) {
      setSortDirection(sortDirection === "asc" ? "desc" : "asc")
    } else {
      setSortColumn(columnId)
      setSortDirection("asc")
    }
  }

  const handleSelectAll = (checked: boolean) => {
    const ids: string[] = paginatedData.map((row) => row.id)
    const newSelectedIds = checked ? new Set<string>(ids) : new Set<string>()
    setSelectedIds(newSelectedIds)
    onSelectionChange?.(Array.from(newSelectedIds))
  }

  const handleSelectRow = (id: string, checked: boolean) => {
    const next = new Set(selectedIds)
    if (checked) next.add(id)
    else next.delete(id)
    setSelectedIds(next)
    onSelectionChange?.(Array.from(next))
  }

  const isAllSelected =
    paginatedData.length > 0 && paginatedData.every((row) => selectedIds.has(row.id))

  const goPrev = () => {
    if (serverPagination) {
      serverPagination.onPageChange(Math.max(1, serverPagination.page - 1))
    } else {
      setCurrentPage((p) => Math.max(0, p - 1))
    }
  }

  const goNext = () => {
    if (serverPagination) {
      serverPagination.onPageChange(Math.min(serverPagination.totalPages, serverPagination.page + 1))
    } else {
      setCurrentPage((p) => Math.min(totalPages - 1, p + 1))
    }
  }

  return (
    <div className="space-y-4">
      <div className="overflow-x-auto rounded-lg border border-border">
        <table className="w-full">
          <thead>
            <tr className="border-b border-border bg-muted">
              {selectable && (
                <th className="w-12 px-4 py-3">
                  <Checkbox checked={isAllSelected} onCheckedChange={(c) => handleSelectAll(Boolean(c))} />
                </th>
              )}
              {columns.map((column) => (
                <th key={column.id} className={cn("px-4 py-3 text-left text-sm font-semibold", column.width)}>
                  {column.sortable ? (
                    <button
                      type="button"
                      onClick={() => handleSort(column.id)}
                      className="flex items-center gap-2 text-muted-foreground hover:text-foreground"
                    >
                      {column.header}
                      {sortColumn === column.id &&
                        (sortDirection === "asc" ? (
                          <ChevronUp className="h-4 w-4" />
                        ) : (
                          <ChevronDown className="h-4 w-4" />
                        ))}
                    </button>
                  ) : (
                    column.header
                  )}
                </th>
              ))}
            </tr>
          </thead>
          <tbody>
            {paginatedData.length === 0 ? (
              <tr>
                <td
                  colSpan={columns.length + (selectable ? 1 : 0)}
                  className="px-4 py-8 text-center text-sm text-muted-foreground"
                >
                  No data found
                </td>
              </tr>
            ) : (
              paginatedData.map((row) => (
                <tr key={row.id} className="border-b border-border transition-colors hover:bg-muted/50">
                  {selectable && (
                    <td className="w-12 px-4 py-3">
                      <Checkbox
                        checked={selectedIds.has(row.id)}
                        onCheckedChange={(c) => handleSelectRow(row.id, Boolean(c))}
                      />
                    </td>
                  )}
                  {columns.map((column) => (
                    <td key={`${row.id}-${column.id}`} className={cn("px-4 py-3 text-sm", column.width)}>
                      {column.cell(row)}
                    </td>
                  ))}
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>

      {totalPages > 1 && (
        <div className="flex items-center justify-between">
          <p className="text-sm text-muted-foreground">
            Page {displayPageIndex + 1} of {totalPages}
          </p>
          <div className="flex gap-2">
            <Button type="button" variant="outline" size="sm" onClick={goPrev} disabled={displayPageIndex <= 0}>
              Previous
            </Button>
            <Button
              type="button"
              variant="outline"
              size="sm"
              onClick={goNext}
              disabled={displayPageIndex >= totalPages - 1}
            >
              Next
            </Button>
          </div>
        </div>
      )}
    </div>
  )
}
