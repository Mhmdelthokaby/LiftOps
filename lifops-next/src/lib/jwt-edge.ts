/**
 * Minimal JWT payload helpers for Edge middleware (no Node crypto).
 */

export function parseJwtPayloadJson(token: string): Record<string, unknown> | null {
  const parts = token.split(".")
  if (parts.length < 2) return null
  try {
    const segment = parts[1]
    const padded = segment.padEnd(segment.length + ((4 - (segment.length % 4)) % 4), "=")
    const b64 = padded.replace(/-/g, "+").replace(/_/g, "/")
    const json = atob(b64)
    return JSON.parse(json) as Record<string, unknown>
  } catch {
    return null
  }
}

const roleClaimLong = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"

export function rolesFromJwtPayload(payload: Record<string, unknown>): string[] {
  const raw = payload.role ?? payload[roleClaimLong]
  if (typeof raw === "string") return [raw]
  if (Array.isArray(raw)) return raw.filter((x): x is string => typeof x === "string")
  return []
}

export function isJwtExpired(payload: Record<string, unknown>, nowMs: number = Date.now()): boolean {
  const exp = payload.exp
  if (typeof exp !== "number") return true
  return nowMs >= exp * 1000
}
