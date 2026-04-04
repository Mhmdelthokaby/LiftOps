import { NextResponse } from "next/server"
import { getApiBaseUrlValue } from "@/lib/api-config"

const accessCookie = "liftops_access"
const refreshCookie = "liftops_refresh"

/**
 * BFF login: calls the .NET API and mirrors tokens into httpOnly cookies for middleware + returns JSON for localStorage.
 */
export async function POST(request: Request) {
  let body: unknown
  try {
    body = await request.json()
  } catch {
    return NextResponse.json({ message: "Invalid JSON body" }, { status: 400 })
  }

  const base = getApiBaseUrlValue()
  const upstream = await fetch(`${base}/api/auth/login`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(body),
  })

  const data = (await upstream.json().catch(() => null)) as Record<string, unknown> | null
  if (!upstream.ok) {
    return NextResponse.json(
      data ?? { message: upstream.statusText || "Login failed" },
      { status: upstream.status }
    )
  }

  const token = data?.token as string | undefined
  const refreshToken = data?.refreshToken as string | undefined
  if (!token) {
    return NextResponse.json({ message: "Invalid response from auth service" }, { status: 502 })
  }

  const res = NextResponse.json(data)

  const secure = process.env.NODE_ENV === "production"
  res.cookies.set(accessCookie, token, {
    httpOnly: true,
    secure: secure,
    sameSite: "lax",
    path: "/",
    maxAge: 60 * 60,
  })

  if (refreshToken) {
    res.cookies.set(refreshCookie, refreshToken, {
      httpOnly: true,
      secure: secure,
      sameSite: "lax",
      path: "/",
      maxAge: 60 * 60 * 24 * 7,
    })
  }

  return res
}
