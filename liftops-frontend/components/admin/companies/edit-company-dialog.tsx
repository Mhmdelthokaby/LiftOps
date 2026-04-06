"use client"

import { useEffect, useState } from "react"
import { toast } from "sonner"
import { Button } from "@/components/ui/button"
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Switch } from "@/components/ui/switch"
import { updateCompany } from "@/lib/api-platform"
import type { Company } from "@/types/admin"
import { SubscriptionPlanSelect } from "./subscription-plan-select"

export interface EditCompanyDialogProps {
  company: Pick<Company, "id" | "name" | "planId"> & {
    isActive?: boolean
  } | null
  open: boolean
  onOpenChange: (open: boolean) => void
  onSaved: () => void
}

export function EditCompanyDialog({ company, open, onOpenChange, onSaved }: EditCompanyDialogProps) {
  const [name, setName] = useState("")
  const [isActive, setIsActive] = useState(true)
  const [planId, setPlanId] = useState<string>("")
  const [saving, setSaving] = useState(false)

  useEffect(() => {
    if (!company || !open) return
    setName(company.name)
    setIsActive(company.isActive !== false)
    setPlanId(company.planId ?? "")
  }, [company, open])

  const handleSave = async () => {
    if (!company?.id || !name.trim()) {
      toast.error("Company name is required.")
      return
    }
    setSaving(true)
    try {
      await updateCompany(company.id, {
        name: name.trim(),
        isActive,
        subscriptionPlanId: planId || undefined,
      })
      toast.success("Company updated")
      onOpenChange(false)
      onSaved()
    } catch (e: unknown) {
      toast.error(e instanceof Error ? e.message : "Update failed")
    } finally {
      setSaving(false)
    }
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <DialogTitle>Edit company</DialogTitle>
          <DialogDescription>Update tenant display name, plan tier, and active status.</DialogDescription>
        </DialogHeader>
        <div className="space-y-4 py-2">
          <div className="space-y-2">
            <Label htmlFor="edit-co-name">Company name</Label>
            <Input
              id="edit-co-name"
              value={name}
              onChange={(e) => setName(e.target.value)}
              autoComplete="organization"
            />
          </div>
          <div className="space-y-2">
            <Label>Subscription plan</Label>
            <SubscriptionPlanSelect value={planId} onValueChange={setPlanId} />
          </div>
          <div className="flex items-center justify-between rounded-lg border p-3">
            <div>
              <p className="text-sm font-medium">Active</p>
              <p className="text-xs text-muted-foreground">Inactive companies cannot use the tenant app.</p>
            </div>
            <Switch checked={isActive} onCheckedChange={setIsActive} aria-label="Company active" />
          </div>
        </div>
        <DialogFooter>
          <Button type="button" variant="outline" onClick={() => onOpenChange(false)}>
            Cancel
          </Button>
          <Button type="button" onClick={handleSave} disabled={saving}>
            {saving ? "Saving…" : "Save"}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}
