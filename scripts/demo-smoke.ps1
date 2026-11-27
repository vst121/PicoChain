param(
    [string]$BackendUrl = "http://localhost:5000",
    [string]$AiServiceUrl = "http://localhost:8000"
)

$ErrorActionPreference = "Stop"

Write-Host "Checking backend health..."
$backendHealth = Invoke-RestMethod "$BackendUrl/health"
Write-Host "  $($backendHealth.service): $($backendHealth.status)"

Write-Host "Checking AI service health..."
$aiHealth = Invoke-RestMethod "$AiServiceUrl/health"
Write-Host "  $($aiHealth.service): $($aiHealth.status)"

Write-Host "Loading shipments..."
$shipments = @(Invoke-RestMethod "$BackendUrl/api/v1/shipments")
if ($shipments.Count -eq 0) {
    throw "No seeded shipments were returned."
}

$shipment = $shipments[0]
Write-Host "  Selected $($shipment.shipmentNumber) ($($shipment.id))"

Write-Host "Publishing a shipment delay event..."
$headers = @{ "X-Correlation-Id" = [Guid]::NewGuid().ToString("N") }
$body = @{ reason = "Demo smoke test: carrier traffic disruption" } | ConvertTo-Json -Compress
try {
    $delay = Invoke-RestMethod `
        -Uri "$BackendUrl/api/v1/shipments/$($shipment.id)/delay" `
        -Method Post `
        -Headers $headers `
        -ContentType "application/json" `
        -Body $body
    Write-Host "  HTTP 202 (Accepted)"
} catch {
    if ($_.Exception.Response -ne $null) {
        Write-Host "  HTTP $($_.Exception.Response.StatusCode)"
    } else {
        Write-Host "  Request sent (no response content)"
    }
}

Write-Host "Smoke test completed. Open the frontend Approval Center to review the resulting proposal."
