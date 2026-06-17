"use client"

import { AlertCircle } from "lucide-react"
import { Button } from "@/components/ui/button"
import { Card } from "@/components/ui/card"

interface AdminErrorStateProps {
  message: string
  onRetry?: () => void
}

export function AdminErrorState({ message, onRetry }: AdminErrorStateProps) {
  return (
    <Card className="border-destructive/40 p-8">
      <div className="flex flex-col items-center gap-4 text-center">
        <AlertCircle className="h-10 w-10 text-destructive" />
        <p className="text-sm text-muted-foreground">{message}</p>
        {onRetry && (
          <Button type="button" variant="outline" onClick={onRetry}>
            Retry
          </Button>
        )}
      </div>
    </Card>
  )
}
