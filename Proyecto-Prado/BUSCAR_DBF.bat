@echo off
echo ============================================
echo   BUSCADOR DE ARCHIVOS DBF
echo ============================================
echo.
echo Buscando archivos DBF en ubicaciones comunes...
echo.

REM Buscar en la carpeta actual y subcarpetas
echo [1] Buscando en: C:\Users\emmag\OneDrive\Escritorio\exe
dir /s /b "C:\Users\emmag\OneDrive\Escritorio\exe\*.DBF" 2>nul | findstr /v "FOXUSER"
echo.

REM Buscar en C:\ (solo primera capa)
echo [2] Buscando en C:\ (primer nivel)
dir /b "C:\*.DBF" 2>nul | findstr /v "FOXUSER"
echo.

REM Buscar en unidad M: si existe
if exist M:\ (
    echo [3] Buscando en unidad M:\
    dir /s /b "M:\*.DBF" 2>nul | findstr /v "FOXUSER"
    echo.
)

REM Buscar en otras ubicaciones comunes
echo [4] Buscando en otras ubicaciones comunes...
dir /s /b "C:\Program Files\*.DBF" 2>nul | findstr /v "FOXUSER"
dir /s /b "C:\Program Files (x86)\*.DBF" 2>nul | findstr /v "FOXUSER"
dir /s /b "D:\*.DBF" 2>nul | findstr /v "FOXUSER"
echo.

echo ============================================
echo   BUSQUEDA COMPLETA
echo ============================================
echo.
echo Si no encontraste archivos DBF, los datos pueden estar:
echo   1. En una unidad de red (\\server\delicias)
echo   2. En otra computadora
echo   3. En una carpeta diferente
echo.
echo Pregunta a los dueños del hotel:
echo   - Donde esta la base de datos del sistema antiguo
echo   - Si tienen una copia de los archivos DBF
echo   - Si pueden exportar los datos a Excel o CSV
echo.
pause






