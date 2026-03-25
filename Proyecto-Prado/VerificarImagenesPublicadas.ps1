# Verifica que las imágenes estén en la carpeta de publicación antes de subir al host
# Ejecutar DESPUÉS de publicar desde Visual Studio (Build > Publish)

$CarpetaPublicada = "C:\Users\emmag\OneDrive\Documentos\PradoPubli"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Verificar imágenes en carpeta publicada" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

if (-not (Test-Path $CarpetaPublicada)) {
    Write-Host "ERROR: No existe la carpeta de publicación: $CarpetaPublicada" -ForegroundColor Red
    Write-Host "Publica primero desde Visual Studio (Build > Publish)." -ForegroundColor Yellow
    exit 1
}

$imgBase = Join-Path $CarpetaPublicada "Img\images"
$apartamentos = Join-Path $imgBase "Apartamentos"
$servicios = Join-Path $imgBase "Servicios"

$ok = $true

if (-not (Test-Path $imgBase)) {
    Write-Host "FALTA: No existe Img\images en la publicación." -ForegroundColor Red
    $ok = $false
} else {
    Write-Host "OK: Img\images existe." -ForegroundColor Green
}

if (-not (Test-Path $apartamentos)) {
    Write-Host "FALTA: No existe Img\images\Apartamentos." -ForegroundColor Red
    $ok = $false
} else {
    $count = (Get-ChildItem -Path $apartamentos -File -ErrorAction SilentlyContinue).Count
    Write-Host "OK: Img\images\Apartamentos existe ($count archivos)." -ForegroundColor Green
}

if (-not (Test-Path $servicios)) {
    Write-Host "FALTA: No existe Img\images\Servicios." -ForegroundColor Red
    $ok = $false
} else {
    $count = (Get-ChildItem -Path $servicios -File -ErrorAction SilentlyContinue).Count
    Write-Host "OK: Img\images\Servicios existe ($count archivos)." -ForegroundColor Green
}

Write-Host ""
if (-not $ok) {
    Write-Host "Las imágenes NO están en la publicación." -ForegroundColor Red
    Write-Host ""
    Write-Host "Haz esto:" -ForegroundColor Yellow
    Write-Host "  1. En Visual Studio: Build > Clean Solution" -ForegroundColor White
    Write-Host "  2. Luego: Build > Publish (tu perfil FolderProfile)" -ForegroundColor White
    Write-Host "  3. Vuelve a ejecutar este script para verificar." -ForegroundColor White
    exit 1
}

Write-Host "Todo listo. Ahora ejecuta SubirAlHosting.ps1 para subir al host." -ForegroundColor Green
Write-Host ""
