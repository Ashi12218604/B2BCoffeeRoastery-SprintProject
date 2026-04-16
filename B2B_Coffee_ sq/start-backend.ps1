$services = @(
    "ApiGateway\ApiGateway",
    "AuthService\AuthService.API",
    "NotificationService\NotificationService.API",
    "ProductService\ProductService.API",
    "InventoryService\InventoryService.API",
    "OrderService\OrderService.API",
    "DeliveryService\DeliveryService.API",
    "ChatbotService\ChatbotService.API"
)

foreach ($service in $services) {
    Write-Host "Starting $service..."
    Start-Process -FilePath "dotnet" -ArgumentList "run --project `"$service\$([System.IO.Path]::GetFileName($service)).csproj`"" -WindowStyle Minimized
}

Write-Host "All backend services + API Gateway started!"
