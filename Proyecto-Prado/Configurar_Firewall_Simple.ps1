# Ejecutar este comando en PowerShell como Administrador:
# New-NetFirewallRule -DisplayName "IIS Express - Hotel Prado" -Direction Inbound -LocalPort 44380 -Protocol TCP -Action Allow

New-NetFirewallRule -DisplayName "IIS Express - Hotel Prado" -Direction Inbound -LocalPort 44380 -Protocol TCP -Action Allow

Write-Host "Regla de firewall creada exitosamente!" -ForegroundColor Green
Write-Host "El puerto 44380 ahora está abierto para conexiones entrantes." -ForegroundColor Yellow

