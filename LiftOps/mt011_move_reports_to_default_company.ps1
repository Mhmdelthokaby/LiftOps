param(
    [string]$DefaultCompanyId = "00000000-0000-0000-0000-000000000000"
)

$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$reportsRoot = Join-Path $root "wwwroot/reports"
$tenantRoot = Join-Path $reportsRoot $DefaultCompanyId

if (-not (Test-Path $reportsRoot)) {
    Write-Host "reports folder not found: $reportsRoot"
    exit 0
}

if (-not (Test-Path $tenantRoot)) {
    New-Item -ItemType Directory -Path $tenantRoot | Out-Null
}

$children = Get-ChildItem -Path $reportsRoot -Directory
foreach ($child in $children) {
    if ($child.Name -eq $DefaultCompanyId) {
        continue
    }

    $target = Join-Path $tenantRoot $child.Name
    if (Test-Path $target) {
        Write-Host "Skipping existing target: $target"
        continue
    }

    Move-Item -Path $child.FullName -Destination $target
    Write-Host "Moved $($child.FullName) -> $target"
}

Write-Host "MT-011 report move complete."
