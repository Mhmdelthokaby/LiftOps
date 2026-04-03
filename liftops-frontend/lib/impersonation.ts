const ACTIVE = "liftops_impersonation_active"
const ORIG_ACCESS = "liftops_impersonation_original_access_token"
const ORIG_REFRESH = "liftops_impersonation_original_refresh_token"
const ORIG_USER = "liftops_impersonation_original_user"
const LABEL_USER = "liftops_impersonation_label_user"
const LABEL_COMPANY = "liftops_impersonation_label_company"

export function isImpersonatingSession(): boolean {
  if (typeof window === "undefined") return false
  return sessionStorage.getItem(ACTIVE) === "1"
}

export function getImpersonationBannerLabels(): { userLabel: string; companyLabel: string } {
  if (typeof window === "undefined") return { userLabel: "", companyLabel: "" }
  return {
    userLabel: sessionStorage.getItem(LABEL_USER) ?? "",
    companyLabel: sessionStorage.getItem(LABEL_COMPANY) ?? "",
  }
}

export function beginImpersonation(params: {
  originalAccessToken: string
  originalRefreshToken: string
  originalUserJson: string
  impersonationAuth: {
    token: string
    refreshToken: string
    refreshTokenExpiry: string
    name: string
    email: string
    roles: string[]
  }
  displayUserName: string
  displayCompanyName: string
}): void {
  sessionStorage.setItem(ORIG_ACCESS, params.originalAccessToken)
  sessionStorage.setItem(ORIG_REFRESH, params.originalRefreshToken)
  sessionStorage.setItem(ORIG_USER, params.originalUserJson)
  sessionStorage.setItem(LABEL_USER, params.displayUserName)
  sessionStorage.setItem(LABEL_COMPANY, params.displayCompanyName)
  sessionStorage.setItem(ACTIVE, "1")

  localStorage.setItem("accessToken", params.impersonationAuth.token)
  localStorage.setItem("refreshToken", params.impersonationAuth.refreshToken)
  localStorage.setItem(
    "user",
    JSON.stringify({
      name: params.impersonationAuth.name,
      email: params.impersonationAuth.email,
      roles: params.impersonationAuth.roles,
    })
  )
}

export function endImpersonation(): void {
  const access = sessionStorage.getItem(ORIG_ACCESS)
  const refresh = sessionStorage.getItem(ORIG_REFRESH)
  const userJson = sessionStorage.getItem(ORIG_USER)

  sessionStorage.removeItem(ACTIVE)
  sessionStorage.removeItem(ORIG_ACCESS)
  sessionStorage.removeItem(ORIG_REFRESH)
  sessionStorage.removeItem(ORIG_USER)
  sessionStorage.removeItem(LABEL_USER)
  sessionStorage.removeItem(LABEL_COMPANY)

  if (access) localStorage.setItem("accessToken", access)
  if (refresh) localStorage.setItem("refreshToken", refresh)
  if (userJson) localStorage.setItem("user", userJson)
}
