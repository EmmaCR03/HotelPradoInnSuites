# 🚀 Métodos Rápidos para Subir Archivos al Hosting

Ya tienes los archivos publicados en `C:\Users\emmag\OneDrive\Documentos\PradoPubli`. Aquí tienes varias formas **rápidas** de subirlos sin hacerlo de uno en uno:

---

## ✅ Opción 1: Cliente FTP con Arrastrar y Soltar (MÁS FÁCIL)

### FileZilla (Gratis y Recomendado)

1. **Descargar FileZilla**: https://filezilla-project.org/download.php?type=client
2. **Instalar** y abrir FileZilla
3. **Conectar al servidor FTP**:
   - Archivo → Gestor de sitios → Nuevo sitio
   - **Host**: Tu servidor FTP (ej: `ftp.tudominio.com` o la IP)
   - **Protocolo**: FTP - File Transfer Protocol
   - **Tipo de acceso**: Normal
   - **Usuario** y **Contraseña**: Las credenciales FTP que te dio tu hosting
   - Guardar contraseña: ✓
   - Conectar

4. **Subir archivos**:
   - **Lado izquierdo**: Navega a `C:\Users\emmag\OneDrive\Documentos\PradoPubli`
   - **Lado derecho**: Navega a la carpeta del sitio en el servidor (ej: `/httpdocs` o `/wwwroot`)
   - **Selecciona TODAS las carpetas y archivos** en el lado izquierdo (Ctrl+A)
   - **Arrastra y suelta** al lado derecho, o clic derecho → Subir
   - FileZilla subirá todo automáticamente manteniendo la estructura de carpetas

**Ventajas**: 
- ✅ Muy fácil de usar
- ✅ Muestra progreso de cada archivo
- ✅ Puedes pausar/reanudar
- ✅ Sube carpetas completas automáticamente

---

## ✅ Opción 2: WinSCP (Alternativa a FileZilla)

1. **Descargar WinSCP**: https://winscp.net/eng/download.php
2. **Instalar** y abrir
3. **Nuevo sitio**:
   - Protocolo: FTP
   - Nombre de host: Tu servidor FTP
   - Usuario y contraseña
   - Guardar → Conectar

4. **Subir**:
   - Arrastra la carpeta `PradoPubli` completa desde el explorador de Windows al panel derecho de WinSCP
   - O selecciona todo dentro de PradoPubli y arrastra

---

## ✅ Opción 3: Script PowerShell Automático

He creado un script `SubirAlHosting.ps1` que puedes usar:

### Uso del Script:

1. **Abrir PowerShell** como Administrador
2. **Ejecutar**:
   ```powershell
   cd "C:\Users\emmag\source\repos\HotelPradoInnSuites\Proyecto-Prado"
   .\SubirAlHosting.ps1 -FtpServer "tu-servidor-ftp.com" -FtpUsuario "tu-usuario" -FtpPassword "tu-contraseña" -CarpetaRemota "/httpdocs"
   ```

**Ejemplo real** (ajusta con tus datos):
```powershell
.\SubirAlHosting.ps1 -FtpServer "ftp.pradoinn-001-site1.stempurl.com" -FtpUsuario "usuario123" -FtpPassword "pass123" -CarpetaRemota "/"
```

El script subirá todos los archivos automáticamente manteniendo la estructura.

---

## ✅ Opción 4: Comprimir y Subir ZIP (Si el hosting lo permite)

1. **Comprimir la carpeta**:
   - Clic derecho en `PradoPubli` → Enviar a → Carpeta comprimida (ZIP)
   - Se creará `PradoPubli.zip`

2. **Subir el ZIP**:
   - Usa FileZilla o el File Manager del hosting para subir solo el ZIP
   - Es mucho más rápido (1 archivo vs miles)

3. **Descomprimir en el servidor**:
   - Si tu hosting tiene File Manager con opción de descomprimir:
     - Sube el ZIP
     - Clic derecho → Extraer/Unzip
   - Si no, necesitarás descomprimir localmente y usar FTP

**Nota**: Algunos hostings bloquean archivos ZIP grandes o requieren descomprimir manualmente.

---

## ✅ Opción 5: File Manager del Hosting (Si tiene opción de subir múltiples)

Algunos paneles de control (cPanel, Plesk) tienen File Manager con opción de:
- **Subir múltiples archivos** a la vez
- **Arrastrar y soltar** desde el navegador
- **Subir carpeta completa** (depende del hosting)

**Pasos**:
1. Entra al File Manager de tu hosting
2. Ve a la carpeta del sitio
3. Busca la opción "Upload" o "Subir"
4. Arrastra toda la carpeta `PradoPubli` o selecciona múltiples archivos

---

## 📋 Comparación de Métodos

| Método | Velocidad | Facilidad | Recomendado |
|--------|-----------|-----------|-------------|
| **FileZilla** | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐⭐ | ✅ **SÍ** |
| **WinSCP** | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐ | ✅ SÍ |
| **Script PowerShell** | ⭐⭐⭐⭐ | ⭐⭐⭐ | Si te gusta automatizar |
| **ZIP + Descomprimir** | ⭐⭐⭐⭐⭐ | ⭐⭐⭐ | Si el hosting lo permite |
| **File Manager** | ⭐⭐⭐ | ⭐⭐⭐⭐ | Depende del hosting |

---

## 🎯 Recomendación Final

**Usa FileZilla** (Opción 1):
- Es gratis y muy fácil
- Sube carpetas completas automáticamente
- Muestra progreso en tiempo real
- Puedes pausar y reanudar si se corta la conexión
- Funciona con cualquier hosting FTP

---

## ⚠️ Importante al Subir

1. **Mantén la estructura de carpetas**: No subas la carpeta "PradoPubli", sino **todo lo que hay dentro** de PradoPubli
2. **Verifica que se subieron**:
   - Carpeta `bin` con todas las DLLs
   - `Web.config`
   - Carpetas: `Content`, `Scripts`, `Views`, `Img`
3. **Después de subir**: Edita `Web.config` en el servidor para actualizar la cadena de conexión

---

## 🔧 Obtener Credenciales FTP

Si no tienes las credenciales FTP, búscalas en:
- **Panel de control** de tu hosting (cPanel, Plesk, etc.)
- Sección **FTP Accounts** o **FTP Access**
- O contacta al soporte de tu hosting

**Ejemplo de credenciales**:
- Servidor: `ftp.tudominio.com` o `ftp.pradoinn-001-site1.stempurl.com`
- Usuario: `usuario@tudominio.com` o similar
- Contraseña: La que configuraste
- Puerto: `21` (por defecto)
