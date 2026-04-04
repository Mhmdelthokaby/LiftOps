import { NextResponse } from "next/server"
import { cookies } from "next/headers"

const accessCookie = "liftops_access"
const refreshCookie = "liftops_refresh"

export async function POST() {
  const jar = await cookies()
  jar.delete(accessCookie)
  jar.delete(refreshCookie)
  return NextResponse.json({ ok: true })
}
