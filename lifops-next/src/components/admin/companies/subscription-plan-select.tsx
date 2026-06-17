"use client"

import { useEffect, useState } from "react"
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select"
import { useActivePlans } from "@/hooks/use-admin-data"

interface SubscriptionPlanSelectProps {
  value?: string
  onValueChange: (value: string) => void
  placeholder?: string
  className?: string
  disabled?: boolean
}

export function SubscriptionPlanSelect({
  value,
  onValueChange,
  placeholder = "Select a plan",
  className,
  disabled
}: SubscriptionPlanSelectProps) {
  const { data: plans, isLoading: loading } = useActivePlans()

  return (
    <Select
      value={value}
      onValueChange={onValueChange}
      disabled={disabled || loading}
    >
      <SelectTrigger className={className}>
        <SelectValue placeholder={loading ? "Loading plans..." : placeholder} />
      </SelectTrigger>
      <SelectContent>
        {(plans || []).map((plan) => (
          <SelectItem key={plan.id} value={plan.id}>
            {plan.name} - ${plan.monthlyPrice}/mo
          </SelectItem>
        ))}
      </SelectContent>
    </Select>
  )
}
