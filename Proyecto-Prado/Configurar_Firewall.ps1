# Script para configurar el Firewall de Windows para IIS Express
# Ejecutar como Administrador: PowerShell -ExecutionPolicy Bypass -File Configurar_Firewall.ps1

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Configurando Firewall para Hotel Prado" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Verificar si se ejecuta como administrador
$isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)

if (-not $isAdmin) {
    Write-Host "ERROR: Este script debe ejecutarse como Administrador" -ForegroundColor Red
    Write-Host "Clic derecho en PowerShell -> Ejecutar como administrador" -ForegroundColor Yellow
    pause
    exit
}

# Puerto del proyecto
$port = 44380

Write-Host "Agregando regla de firewall para el puerto $port..." -ForegroundColor Yellow

# Eliminar regla existente si existe
$existingRule = Get-NetFirewallRule -DisplayName "IIS Express - Hotel Prado" -ErrorAction SilentlyContinue
if ($existingRule) {
    Write-Host "Eliminando regla existente..." -ForegroundColor Yellow
    Remove-NetFirewallRule -DisplayName "IIS Express - Hotel Prado"
}

# Crear nueva regla para HTTP
try {
    New-NetFirewallRule -DisplayName "IIS Express - Hotel Prado HTTP" `
        -Direction Inbound `
        -LocalPort $port `
        -Protocol TCP `
        -Action Allow `
        -Profile Any | Out-Null
    Write-Host "✓ Regla HTTP creada exitosamente" -ForegroundColor Green
} catch {
    Write-Host "✗ Error al crear regla HTTP: $_" -ForegroundColor Red
}

# Crear nueva regla para HTTPS
try {
    New-NetFirewallRule -DisplayName "IIS Express - Hotel Prado HTTPS" `
        -Direction Inbound `
        -LocalPort $port `
        -Protocol TCP `
        -Action Allow `
        -Profile Any | Out-Null
    Write-Host "✓ Regla HTTPS creada exitosamente" -ForegroundColor Green
} catch {
    Write-Host "✗ Error al crear regla HTTPS: $_" -ForegroundColor Red
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Configuración completada!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Tu aplicación será accesible en:" -ForegroundColor Yellow
Write-Host "  - https://192.168.0.2:$port" -ForegroundColor White
Write-Host "  - https://localhost:$port" -ForegroundColor White
Write-Host ""
Write-Host "Desde otros dispositivos en la red, usa:" -ForegroundColor Yellow
Write-Host "  https://192.168.0.2:$port" -ForegroundColor White
Write-Host ""
pause

