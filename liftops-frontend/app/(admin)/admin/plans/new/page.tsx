"use client"

import { useEffect, useState } from "react"
import Link from "next/link"
import { useRouter, useSearchParams } from "next/navigation"
import { toast } from "sonner"
import { Button } from "@/components/ui/button"
import { Card } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Checkbox } from "@/components/ui/checkbox"
import { PageHeader } from "@/components/admin/page-header"
import { createPlan, getPlanById, updatePlan } from "@/lib/api"
import type { Plan } from "@/types/admin"

const defaults: Omit<Plan, "id"> = {
  name: "",
  code: "",
  monthlyPrice: 0,
  yearlyPrice: 0,
  trialDays: 14,
  isActive: true,
  maxUsers: 10,
  maxElevators: 50,
  maxMaintenanceContracts: 100,
  maxInstallationProjects: 50,
  allowEmergencyModule: true,
  allowFaultsModule: true,
  allowFinanceModule: true,
  allowInventoryModule: true,
  allowApiAccess: false,
}

export default function AdminPlanFormPage() {
  const router = useRouter()
  const searchParams = useSearchParams()
  const editId = searchParams.get("edit")
  const [loading, setLoading] = useState(Boolean(editId))
  const [saving, setSaving] = useState(false)
  const [form, setForm] = useState<Omit<Plan, "id"> & { id?: string }>(defaults)

  useEffect(() => {
    if (!editId) {
      setLoading(false)
      return
    }
    let cancelled = false
    getPlanById(editId)
      .then((p) => {
        if (cancelled) return
        setForm({
          id: p.id,
          name: p.name,
          code: p.code,
          monthlyPrice: p.monthlyPrice,
          yearlyPrice: p.yearlyPrice,
          trialDays: p.trialDays,
          isActive: p.isActive,
          maxUsers: p.maxUsers,
          maxElevators: p.maxElevators,
          maxMaintenanceContracts: p.maxMaintenanceContracts,
          maxInstallationProjects: p.maxInstallationProjects,
          allowEmergencyModule: p.allowEmergencyModule,
          allowFaultsModule: p.allowFaultsModule,
          allowFinanceModule: p.allowFinanceModule,
          allowInventoryModule: p.allowInventoryModule,
          allowApiAccess: p.allowApiAccess,
        })
      })
      .catch((e: unknown) => {
        toast.error(e instanceof Error ? e.message : "Failed to load plan")
        router.push("/admin/plans")
      })
      .finally(() => {
        if (!cancelled) setLoading(false)
      })
    return () => {
      cancelled = true
    }
  }, [editId, router])

  const update = <K extends keyof typeof form>(key: K, value: (typeof form)[K]) => {
    setForm((f) => ({ ...f, [key]: value }))
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    if (!form.name.trim() || !form.code.trim()) {
      toast.error("Name and code are required.")
      return
    }
    if (form.maxUsers <= 0) {
      toast.error("Max users must be greater than zero.")
      return
    }
    if (form.monthlyPrice < 0 || form.yearlyPrice < 0) {
      toast.error("Prices cannot be negative.")
      return
    }
    setSaving(true)
    try {
      const payload: Partial<Plan> = { ...form }
      delete payload.id
      if (form.id) {
        await updatePlan(form.id, payload)
        toast.success("Plan updated")
      } else {
        await createPlan(payload)
        toast.success("Plan created")
      }
      router.push("/admin/plans")
    } catch (err: unknown) {
      toast.error(err instanceof Error ? err.message : "Save failed")
    } finally {
      setSaving(false)
    }
  }

  if (loading) {
    return (
      <div className="p-8">
        <p className="text-sm text-muted-foreground">Loading plan…</p>
      </div>
    )
  }

  return (
    <div className="flex h-full flex-col">
      <PageHeader
        title={form.id ? "Edit plan" : "Create plan"}
        actions={
          <Button type="button" variant="outline" asChild>
            <Link href="/admin/plans">Cancel</Link>
          </Button>
        }
      />
      <div className="flex-1 overflow-auto p-8">
        <Card className="mx-auto max-w-2xl p-6">
          <form onSubmit={handleSubmit} className="space-y-6">
            <div className="grid grid-cols-2 gap-4">
              <div className="space-y-2">
                <Label htmlFor="name">Name</Label>
                <Input id="name" value={form.name} onChange={(e) => update("name", e.target.value)} required />
              </div>
              <div className="space-y-2">
                <Label htmlFor="code">Code</Label>
                <Input id="code" value={form.code} onChange={(e) => update("code", e.target.value)} required />
              </div>
            </div>
            <div className="grid grid-cols-2 gap-4">
              <div className="space-y-2">
                <Label htmlFor="m">Monthly price</Label>
                <Input
                  id="m"
                  type="number"
                  min={0}
                  step="0.01"
                  value={form.monthlyPrice}
                  onChange={(e) => update("monthlyPrice", Number(e.target.value))}
                />
              </div>
              <div className="space-y-2">
                <Label htmlFor="y">Yearly price</Label>
                <Input
                  id="y"
                  type="number"
                  min={0}
                  step="0.01"
                  value={form.yearlyPrice}
                  onChange={(e) => update("yearlyPrice", Number(e.target.value))}
                />
              </div>
            </div>
            <div className="grid grid-cols-2 gap-4">
              <div className="space-y-2">
                <Label htmlFor="trial">Trial days</Label>
                <Input
                  id="trial"
                  type="number"
                  min={0}
                  value={form.trialDays}
                  onChange={(e) => update("trialDays", Number(e.target.value))}
                />
              </div>
              <div className="flex items-center gap-2 pt-8">
                <Checkbox
                  id="active"
                  checked={form.isActive}
                  onCheckedChange={(c) => update("isActive", Boolean(c))}
                />
                <Label htmlFor="active">Active</Label>
              </div>
            </div>
            <div className="grid grid-cols-2 gap-4">
              <div className="space-y-2">
                <Label htmlFor="mu">Max users</Label>
                <Input
                  id="mu"
                  type="number"
                  min={1}
                  value={form.maxUsers}
                  onChange={(e) => update("maxUsers", Number(e.target.value))}
                />
              </div>
              <div className="space-y-2">
                <Label htmlFor="me">Max elevators</Label>
                <Input
                  id="me"
                  type="number"
                  min={0}
                  value={form.maxElevators}
                  onChange={(e) => update("maxElevators", Number(e.target.value))}
                />
              </div>
              <div className="space-y-2">
                <Label htmlFor="mmc">Max maintenance contracts</Label>
                <Input
                  id="mmc"
                  type="number"
                  min={0}
                  value={form.maxMaintenanceContracts}
                  onChange={(e) => update("maxMaintenanceContracts", Number(e.target.value))}
                />
              </div>
              <div className="space-y-2">
                <Label htmlFor="mip">Max installation projects</Label>
                <Input
                  id="mip"
                  type="number"
                  min={0}
                  value={form.maxInstallationProjects}
                  onChange={(e) => update("maxInstallationProjects", Number(e.target.value))}
                />
              </div>
            </div>
            <div className="space-y-3">
              <p className="text-sm font-medium">Modules</p>
              {(
                [
                  ["allowEmergencyModule", "Emergency"],
                  ["allowFaultsModule", "Faults"],
                  ["allowFinanceModule", "Finance"],
                  ["allowInventoryModule", "Inventory"],
                  ["allowApiAccess", "API access"],
                ] as const
              ).map(([key, label]) => (
                <div key={key} className="flex items-center gap-2">
                  <Checkbox
                    id={key}
                    checked={form[key]}
                    onCheckedChange={(c) => update(key, Boolean(c))}
                  />
                  <Label htmlFor={key}>{label}</Label>
                </div>
              ))}
            </div>
            <Button type="submit" disabled={saving}>
              {saving ? "Saving…" : form.id ? "Save changes" : "Create plan"}
            </Button>
          </form>
        </Card>
      </div>
    </div>
  )
}
