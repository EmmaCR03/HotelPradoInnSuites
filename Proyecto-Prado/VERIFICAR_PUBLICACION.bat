@echo off
REM Ejecuta el script de verificacion con permisos (sin cambiar politica del sistema)
cd /d "C:\Users\emmag\OneDrive\Documentos\PradoPubli"
powershell -ExecutionPolicy Bypass -File "%~dp0VERIFICAR_PUBLICACION.ps1"
pause
