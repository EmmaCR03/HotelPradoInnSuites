# Ajusta el script generado por SSMS para ejecutarlo en db_ac52da_pradoinn
# Uso: .\AjustarScriptMigracion.ps1 "C:\ruta\a\HotelPrado_Completo.sql"
# O arrastra el .sql sobre este script.

param(
    [Parameter(Mandatory = $true)]
    [string]$RutaScript
)

if (-not (Test-Path $RutaScript)) {
    Write-Host "No se encuentra el archivo: $RutaScript" -ForegroundColor Red
    exit 1
}

$salida = [System.IO.Path]::GetDirectoryName($RutaScript) + "\HotelPrado_ListoParaCloud.sql"
Write-Host "Leyendo script (puede tardar si es muy grande)..." -ForegroundColor Yellow

$content = Get-Content -Path $RutaScript -Raw -Encoding UTF8

# 1) Reemplazar nombre de la base en todo el archivo
$content = $content -replace '\[HotelPrado\]', '[db_ac52da_pradoinn]'

# 2) Cortar desde el PRIMER "CREATE TABLE" (así quitamos SIEMPRE CREATE DATABASE y todos los ALTER DATABASE)
$idx = $content.IndexOf("CREATE TABLE")
if ($idx -lt 0) {
    Write-Host "No se encontró CREATE TABLE en el archivo. Revisa el script." -ForegroundColor Red
    exit 1
}
# Dejar solo desde el primer CREATE TABLE y poner USE al inicio
$content = "USE [db_ac52da_pradoinn]`r`nGO`r`n`r`n" + $content.Substring($idx)

Write-Host "Escribiendo archivo listo: $salida" -ForegroundColor Green
Set-Content -Path $salida -Value $content -Encoding UTF8 -NoNewline

Write-Host "Listo. Abre en SSMS este archivo y ejecútalo contra sql5106.site4now.net (base db_ac52da_pradoinn)." -ForegroundColor Cyan
