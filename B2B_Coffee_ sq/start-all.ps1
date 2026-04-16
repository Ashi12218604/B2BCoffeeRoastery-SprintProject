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

Write-Host "Starting Frontend (Angular)..."
Start-Process -FilePath "cmd.exe" -ArgumentList "/C cd Frontend\EmberBean && npx ng serve --open" -WindowStyle Normal

Write-Host "All services started! (Backend is minimized, Angular is opening in a new window)"
