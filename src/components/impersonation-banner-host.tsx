"use client"

import { useEffect, useState } from "react"
import { usePathname, useRouter } from "next/navigation"
import { Button } from "@/components/ui/button"
import { endImpersonation, getImpersonationBannerLabels, isImpersonatingSession } from "@/lib/impersonation"

export function ImpersonationBannerHost() {
  const router = useRouter()
  const pathname = usePathname()
  const [visible, setVisible] = useState(false)
  const [labels, setLabels] = useState({ userLabel: "", companyLabel: "" })

  useEffect(() => {
    const sync = () => {
      setVisible(isImpersonatingSession())
      setLabels(getImpersonationBannerLabels())
    }
    sync()
    const onStorage = (e: StorageEvent) => {
      if (e.key?.startsWith("liftops_impersonation")) sync()
    }
    window.addEventListener("storage", onStorage)
    return () => window.removeEventListener("storage", onStorage)
  }, [pathname])

  if (!visible) return null

  return (
    <div className="flex items-center justify-between gap-4 border-b border-amber-500/40 bg-amber-500/15 px-4 py-2 text-sm">
      <p className="text-amber-950 dark:text-amber-100">
        You are impersonating <strong>{labels.userLabel || "user"}</strong>
        {labels.companyLabel ? (
          <>
            {" "}
            from <strong>{labels.companyLabel}</strong>
          </>
        ) : null}
        .
      </p>
      <Button
        type="button"
        size="sm"
        variant="secondary"
        onClick={() => {
          endImpersonation()
          setVisible(false)
          router.push("/admin/companies")
          router.refresh()
        }}
      >
        Exit impersonation
      </Button>
    </div>
  )
}
