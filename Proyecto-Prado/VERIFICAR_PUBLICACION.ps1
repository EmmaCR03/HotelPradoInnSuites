# Script para verificar que la publicación se hizo correctamente
# Ejecutar desde PowerShell en la carpeta PradoPubli

Write-Host "=== Verificación de Publicación ===" -ForegroundColor Cyan
Write-Host ""

$errores = 0

# Verificar carpetas obligatorias
Write-Host "Verificando carpetas..." -ForegroundColor Yellow
$carpetas = @("bin", "Content", "Scripts", "Views", "Img", "Views\Account")
foreach ($carpeta in $carpetas) {
    if (Test-Path $carpeta) {
        Write-Host "  [OK] $carpeta existe" -ForegroundColor Green
    } else {
        Write-Host "  [ERROR] $carpeta NO existe" -ForegroundColor Red
        $errores++
    }
}

# Verificar archivos obligatorios
Write-Host ""
Write-Host "Verificando archivos..." -ForegroundColor Yellow
$archivos = @("Web.config", "Global.asax", "favicon.ico", "Views\Account\Register.cshtml", "Views\Account\Login.cshtml")
foreach ($archivo in $archivos) {
    if (Test-Path $archivo) {
        Write-Host "  [OK] $archivo existe" -ForegroundColor Green
    } else {
        Write-Host "  [ERROR] $archivo NO existe" -ForegroundColor Red
        $errores++
    }
}

# Verificar DLLs en /bin
Write-Host ""
Write-Host "Verificando DLLs en /bin..." -ForegroundColor Yellow
if (Test-Path "bin") {
    $dlls = Get-ChildItem -Path "bin" -Filter "*.dll" | Measure-Object
    Write-Host "  Total de DLLs encontradas: $($dlls.Count)" -ForegroundColor Cyan
    
    if ($dlls.Count -lt 20) {
        Write-Host "  [ADVERTENCIA] Parece que faltan DLLs (debería haber más de 20)" -ForegroundColor Yellow
    }
    
    # Verificar DLLs críticas
    $dllsCriticas = @(
        "HotelPrado.UI.dll",
        "HotelPrado.Abstracciones.dll",
        "HotelPrado.AccesoADatos.dll",
        "HotelPrado.LN.dll",
        "Microsoft.Owin.dll",
        "Microsoft.Owin.Host.SystemWeb.dll",
        "Microsoft.Owin.Security.Cookies.dll",
        "System.Web.Mvc.dll",
        "EntityFramework.dll"
    )
    
    Write-Host ""
    Write-Host "  Verificando DLLs críticas..." -ForegroundColor Cyan
    foreach ($dll in $dllsCriticas) {
        if (Test-Path "bin\$dll") {
            Write-Host "    [OK] $dll" -ForegroundColor Green
        } else {
            Write-Host "    [ERROR] $dll NO encontrada" -ForegroundColor Red
            $errores++
        }
    }
} else {
    Write-Host "  [ERROR] Carpeta bin no existe" -ForegroundColor Red
    $errores++
}

# Resumen
Write-Host ""
Write-Host "=== RESUMEN ===" -ForegroundColor Cyan
if ($errores -eq 0) {
    Write-Host "  [OK] La publicación parece correcta" -ForegroundColor Green
} else {
    Write-Host "  [ERROR] Se encontraron $errores problemas" -ForegroundColor Red
    Write-Host "  Debes republicar el proyecto" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "Presiona cualquier tecla para continuar..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
