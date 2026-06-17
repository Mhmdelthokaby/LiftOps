import { cn } from "@/lib/utils"

interface UsageBarProps {
  current: number
  max: number
  label?: string
  showPercentage?: boolean
}

export function UsageBar({ current, max, label, showPercentage = true }: UsageBarProps) {
  const safeMax = max <= 0 ? 1 : max
  const percentage = (current / safeMax) * 100
  const clampedPercentage = Math.min(percentage, 100)

  const getColorClass = () => {
    if (percentage < 70) return "bg-green-500"
    if (percentage < 90) return "bg-amber-500"
    return "bg-red-500"
  }

  return (
    <div className="space-y-1">
      {label && <p className="text-xs font-medium text-muted-foreground">{label}</p>}
      <div className="flex items-center gap-2">
        <div className="h-2 flex-1 overflow-hidden rounded-full bg-muted">
          <div className={cn("h-full transition-all", getColorClass())} style={{ width: `${clampedPercentage}%` }} />
        </div>
        {showPercentage && (
          <span className="w-12 text-right text-xs font-medium text-muted-foreground">
            {Math.round(percentage)}%
          </span>
        )}
      </div>
      {label && (
        <p className="text-xs text-muted-foreground">
          {current.toLocaleString()} / {max.toLocaleString()}
        </p>
      )}
    </div>
  )
}
