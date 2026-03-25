# Script para subir archivos al hosting vía FTP
# Uso: .\SubirAlHosting.ps1

param(
    [Parameter(Mandatory=$true)]
    [string]$FtpServer,
    
    [Parameter(Mandatory=$true)]
    [string]$FtpUsuario,
    
    [Parameter(Mandatory=$true)]
    [string]$FtpPassword,
    
    [Parameter(Mandatory=$false)]
    [string]$CarpetaLocal = "C:\Users\emmag\OneDrive\Documentos\PradoPubli",
    
    [Parameter(Mandatory=$false)]
    [string]$CarpetaRemota = "/"
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Subida de archivos al hosting FTP" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Verificar que la carpeta local existe
if (-not (Test-Path $CarpetaLocal)) {
    Write-Host "ERROR: La carpeta local no existe: $CarpetaLocal" -ForegroundColor Red
    exit 1
}

Write-Host "Carpeta local: $CarpetaLocal" -ForegroundColor Green
Write-Host "Servidor FTP: $FtpServer" -ForegroundColor Green
Write-Host "Carpeta remota: $CarpetaRemota" -ForegroundColor Green
Write-Host ""

# Crear función para subir archivos recursivamente
function SubirArchivoFtp {
    param(
        [string]$LocalPath,
        [string]$RemotePath,
        [string]$Server,
        [string]$User,
        [string]$Pass
    )
    
    $files = Get-ChildItem -Path $LocalPath -File
    $folders = Get-ChildItem -Path $LocalPath -Directory
    
    # Subir archivos en la carpeta actual
    foreach ($file in $files) {
        $remoteFile = "$RemotePath/$($file.Name)"
        $localFile = $file.FullName
        
        try {
            $ftpUri = "ftp://$Server$remoteFile"
            $ftpRequest = [System.Net.FtpWebRequest]::Create($ftpUri)
            $ftpRequest.Credentials = New-Object System.Net.NetworkCredential($User, $Pass)
            $ftpRequest.Method = [System.Net.WebRequestMethods+Ftp]::UploadFile
            $ftpRequest.UseBinary = $true
            $ftpRequest.UsePassive = $true
            
            $fileContent = [System.IO.File]::ReadAllBytes($localFile)
            $ftpRequest.ContentLength = $fileContent.Length
            
            $requestStream = $ftpRequest.GetRequestStream()
            $requestStream.Write($fileContent, 0, $fileContent.Length)
            $requestStream.Close()
            
            $response = $ftpRequest.GetResponse()
            $response.Close()
            
            Write-Host "  ✓ Subido: $($file.Name)" -ForegroundColor Green
        }
        catch {
            Write-Host "  ✗ Error subiendo $($file.Name): $_" -ForegroundColor Red
        }
    }
    
    # Crear carpetas y subir contenido recursivamente
    foreach ($folder in $folders) {
        $remoteFolder = "$RemotePath/$($folder.Name)"
        
        try {
            # Intentar crear la carpeta remota
            $ftpUri = "ftp://$Server$remoteFolder"
            $ftpRequest = [System.Net.FtpWebRequest]::Create($ftpUri)
            $ftpRequest.Credentials = New-Object System.Net.NetworkCredential($User, $Pass)
            $ftpRequest.Method = [System.Net.WebRequestMethods+Ftp]::MakeDirectory
            $ftpRequest.UsePassive = $true
            
            try {
                $response = $ftpRequest.GetResponse()
                $response.Close()
            }
            catch {
                # La carpeta puede que ya exista, continuar
            }
            
            Write-Host "📁 Carpeta: $($folder.Name)" -ForegroundColor Yellow
            SubirArchivoFtp -LocalPath $folder.FullName -RemotePath $remoteFolder -Server $Server -User $User -Pass $Pass
        }
        catch {
            Write-Host "  ✗ Error creando carpeta $($folder.Name): $_" -ForegroundColor Red
        }
    }
}

# Iniciar subida
Write-Host "Iniciando subida de archivos..." -ForegroundColor Cyan
Write-Host ""

SubirArchivoFtp -LocalPath $CarpetaLocal -RemotePath $CarpetaRemota -Server $FtpServer -User $FtpUsuario -Pass $FtpPassword

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Subida completada" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
