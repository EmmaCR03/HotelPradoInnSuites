# Archivos que debes actualizar en PradoPubli

Ya que hiciste los pasos 1 y 2 (compilar y publicar), ahora necesitas **republicar** para que los cambios se reflejen en tu carpeta `PradoPubli`.

## ⚠️ IMPORTANTE: Debes republicar

Los cambios que hice están en el **código fuente**, por lo que necesitas:

1. **Recompilar** el proyecto en modo Release
2. **Republicar** para que los archivos actualizados se copien a `PradoPubli`

## Archivos que cambiaron y necesitas actualizar:

### 1. **App_Start/Startup.Auth.cs** ⭐ CRÍTICO
   - Este archivo tiene la nueva configuración de OWIN que evita bucles de redirección
   - **Ubicación en publicación:** `PradoPubli\App_Start\Startup.Auth.cs`
   - **Debe tener:** La función `OnApplyRedirect` que evita redirecciones infinitas

### 2. **Controllers/AccountController.cs** ⭐ CRÍTICO
   - Mejorado el manejo de bucles de redirección
   - **Ubicación en publicación:** `PradoPubli\Controllers\AccountController.cs`
   - **Debe tener:** La función `SanitizeReturnUrl` mejorada

### 3. **bin/HotelPrado.UI.dll** ⭐ CRÍTICO
   - Este archivo contiene el código compilado de los cambios anteriores
   - **Ubicación en publicación:** `PradoPubli\bin\HotelPrado.UI.dll`
   - Se actualiza automáticamente al recompilar

### 4. **Web.config** (si aplica)
   - Si tienes `Web.Release.config`, se transforma automáticamente en `Web.config` al publicar
   - Verifica que no tenga la configuración de `httpErrors` que causa bucles

## Pasos para actualizar:

### Opción A: Republicar desde Visual Studio (RECOMENDADO)

1. En Visual Studio, asegúrate de estar en modo **Release**
2. Click derecho en `HotelPrado.UI` → **Publish**
3. Selecciona tu perfil de publicación (o crea uno nuevo apuntando a `PradoPubli`)
4. Click en **Publish**
5. Esto sobrescribirá los archivos antiguos con los nuevos

### Opción B: Copiar archivos manualmente

Si prefieres copiar solo los archivos cambiados:

1. **Desde tu proyecto fuente**, copia estos archivos a `PradoPubli`:
   - `HotelPrado.UI\App_Start\Startup.Auth.cs` → `PradoPubli\App_Start\Startup.Auth.cs`
   - `HotelPrado.UI\Controllers\AccountController.cs` → `PradoPubli\Controllers\AccountController.cs`

2. **Recompila el proyecto** y copia:
   - `HotelPrado.UI\bin\Release\HotelPrado.UI.dll` → `PradoPubli\bin\HotelPrado.UI.dll`
   - También copia todas las DLLs de `bin\Release\` a `PradoPubli\bin\`

## Verificación rápida:

Después de actualizar, verifica que estos archivos existan en `PradoPubli`:

- ✅ `App_Start\Startup.Auth.cs` (debe tener `OnApplyRedirect`)
- ✅ `Controllers\AccountController.cs` (debe tener `SanitizeReturnUrl` mejorado)
- ✅ `bin\HotelPrado.UI.dll` (fecha de modificación reciente)
- ✅ `Web.config` (sin `httpErrors` problemático)

## Después de actualizar PradoPubli:

1. **Sube los archivos actualizados al servidor** (FileZilla o el método que uses)
2. **Asegúrate de sobrescribir** los archivos antiguos en el servidor
3. **Prueba las URLs:**
   - `/Ping` → Debe responder "OK"
   - `/Account/Login` → Debe cargar sin redirección infinita
   - `/Account/Register` → Debe cargar sin redirección infinita
