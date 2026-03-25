# Script para configurar acceso desde teléfono
# ⚠️ IMPORTANTE: Ejecutar como Administrador
# Clic derecho en este archivo → "Ejecutar con PowerShell"

# Verificar si es Administrador
$isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)

if (-not $isAdmin) {
    Write-Host "========================================" -ForegroundColor Red
    Write-Host "❌ ERROR: Se requieren permisos de Administrador" -ForegroundColor Red
    Write-Host "========================================" -ForegroundColor Red
    Write-Host ""
    Write-Host "Por favor:" -ForegroundColor Yellow
    Write-Host "1. Cierra esta ventana" -ForegroundColor White
    Write-Host "2. Clic derecho en este archivo" -ForegroundColor White
    Write-Host "3. Selecciona 'Ejecutar con PowerShell'" -ForegroundColor White
    Write-Host "4. O abre PowerShell como Administrador manualmente" -ForegroundColor White
    Write-Host ""
    Write-Host "Presiona cualquier tecla para salir..."
    $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
    exit
}

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Configuración de Acceso desde Teléfono" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Configurar Firewall para puerto 44380 (HTTPS)
Write-Host "Configurando Firewall para puerto 44380..." -ForegroundColor Yellow

try {
    # Verificar si la regla ya existe
    $existingRule = Get-NetFirewallRule -DisplayName "IIS Express - Hotel Prado" -ErrorAction SilentlyContinue
    
    if ($existingRule) {
        Write-Host "La regla del firewall ya existe. Eliminando..." -ForegroundColor Yellow
        Remove-NetFirewallRule -DisplayName "IIS Express - Hotel Prado"
    }
    
    # Crear nueva regla
    New-NetFirewallRule -DisplayName "IIS Express - Hotel Prado" -Direction Inbound -LocalPort 44380 -Protocol TCP -Action Allow -ErrorAction Stop
    
    Write-Host "✅ Firewall configurado correctamente!" -ForegroundColor Green
    Write-Host ""
    
    # Obtener IP local
    Write-Host "Obteniendo tu IP local..." -ForegroundColor Yellow
    $ipAddress = (Get-NetIPAddress -AddressFamily IPv4 | Where-Object {$_.IPAddress -like "192.168.*"} | Select-Object -First 1).IPAddress
    
    if ($ipAddress) {
        Write-Host ""
        Write-Host "========================================" -ForegroundColor Cyan
        Write-Host "✅ CONFIGURACIÓN COMPLETA" -ForegroundColor Green
        Write-Host "========================================" -ForegroundColor Cyan
        Write-Host ""
        Write-Host "Tu IP local es: $ipAddress" -ForegroundColor Green
        Write-Host ""
        Write-Host "Para acceder desde tu teléfono:" -ForegroundColor Yellow
        Write-Host "  https://$ipAddress:44380" -ForegroundColor White
        Write-Host ""
        Write-Host "IMPORTANTE:" -ForegroundColor Yellow
        Write-Host "1. Modifica applicationhost.config (ver instrucciones)" -ForegroundColor White
        Write-Host "2. Asegúrate de estar en la misma red WiFi" -ForegroundColor White
        Write-Host "3. Ejecuta el proyecto en Visual Studio (F5)" -ForegroundColor White
        Write-Host ""
    } else {
        Write-Host "⚠️ No se pudo detectar la IP automáticamente." -ForegroundColor Yellow
        Write-Host "Ejecuta 'ipconfig' en CMD para obtener tu IP." -ForegroundColor White
    }
    
} catch {
    Write-Host "❌ Error al configurar firewall: $_" -ForegroundColor Red
    Write-Host ""
    Write-Host "Asegúrate de ejecutar este script como Administrador." -ForegroundColor Yellow
    Write-Host "Clic derecho en PowerShell → Ejecutar como administrador" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "Presiona cualquier tecla para salir..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")

