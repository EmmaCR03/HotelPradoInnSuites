# Archivos que debes actualizar en Smarterasp

Después de republicar tu proyecto, estos son los archivos específicos que debes subir/reemplazar en tu servidor de Smarterasp:

## 📁 Archivos CRÍTICOS que DEBES actualizar:

### 1. **bin/HotelPrado.UI.dll** ⭐ MÁS IMPORTANTE
   - **Ubicación en tu PC:** `PradoPubli\bin\HotelPrado.UI.dll`
   - **Ubicación en servidor:** `/site1/bin/HotelPrado.UI.dll` (o la carpeta donde tengas tu sitio)
   - **¿Por qué?** Este DLL contiene el código compilado de `Startup.Auth.cs` y `AccountController.cs` con los cambios para evitar bucles de redirección
   - **Acción:** REEMPLAZAR completamente el archivo antiguo

### 2. **Controllers/AccountController.cs** ⭐ IMPORTANTE
   - **Ubicación en tu PC:** `PradoPubli\Controllers\AccountController.cs`
   - **Ubicación en servidor:** `/site1/Controllers/AccountController.cs`
   - **¿Por qué?** Contiene las mejoras en `SanitizeReturnUrl` para evitar bucles
   - **Acción:** REEMPLAZAR completamente el archivo antiguo

### 3. **Web.config** (si cambió)
   - **Ubicación en tu PC:** `PradoPubli\Web.config`
   - **Ubicación en servidor:** `/site1/Web.config`
   - **¿Por qué?** Puede tener cambios de configuración
   - **⚠️ IMPORTANTE:** Antes de reemplazar, **COPIA** el Web.config del servidor y verifica que tenga la cadena de conexión correcta de producción
   - **Acción:** Comparar y actualizar solo si es necesario (NO sobrescribir la cadena de conexión de producción)

## 📋 Lista de verificación para subir:

### Opción A: Subir solo los archivos cambiados (RÁPIDO)

1. Conecta con FileZilla a tu servidor Smarterasp
2. Navega a la carpeta de tu sitio (ej: `/site1`)
3. Sube estos archivos específicos:
   - ✅ `bin/HotelPrado.UI.dll` → `/site1/bin/HotelPrado.UI.dll`
   - ✅ `Controllers/AccountController.cs` → `/site1/Controllers/AccountController.cs`
4. **Sobrescribe** los archivos antiguos cuando FileZilla pregunte

### Opción B: Subir todo el contenido de PradoPubli (COMPLETO)

1. Conecta con FileZilla a tu servidor Smarterasp
2. Navega a la carpeta de tu sitio (ej: `/site1`)
3. Selecciona **TODO** el contenido de `PradoPubli` en tu PC
4. Arrastra y suelta en `/site1` del servidor
5. Cuando pregunte si sobrescribir, elige **"Sí a todos"** o **"Overwrite all"**

## ⚠️ IMPORTANTE - Antes de subir Web.config:

**NO sobrescribas el Web.config sin verificar primero:**

1. **Descarga** el `Web.config` actual del servidor
2. **Abre** el `Web.config` de `PradoPubli` en tu PC
3. **Compara** las cadenas de conexión:
   - El del servidor debe tener: `Data Source=SQL5106.site4now.net` (o tu servidor SQL)
   - El de tu PC puede tener una cadena diferente
4. **Si son diferentes:** Copia la cadena de conexión del servidor y pégala en el Web.config de PradoPubli antes de subirlo
5. **O mejor:** Edita el Web.config directamente en el servidor usando el File Manager de Smarterasp

## 🔍 Cómo verificar que se subieron correctamente:

Después de subir los archivos, verifica en el servidor:

1. **Fecha de modificación:**
   - `bin/HotelPrado.UI.dll` debe tener fecha de HOY
   - `Controllers/AccountController.cs` debe tener fecha de HOY

2. **Tamaño del archivo:**
   - Compara el tamaño de `HotelPrado.UI.dll` en tu PC vs servidor (deben ser iguales)

## 📝 Resumen rápido:

| Archivo | Acción | Prioridad |
|---------|--------|-----------|
| `bin/HotelPrado.UI.dll` | REEMPLAZAR | 🔴 CRÍTICO |
| `Controllers/AccountController.cs` | REEMPLAZAR | 🟡 IMPORTANTE |
| `Web.config` | VERIFICAR antes de reemplazar | 🟢 CUIDADO |

## 🚀 Después de subir:

1. **Limpia la caché del navegador** (Ctrl+Shift+Delete)
2. **Prueba estas URLs:**
   - `tu-dominio.com/Ping` → Debe responder "OK"
   - `tu-dominio.com/Account/Login` → Debe cargar sin redirección infinita
   - `tu-dominio.com/Account/Register` → Debe cargar sin redirección infinita

## 💡 Tip:

Si tienes dudas, usa la **Opción B** (subir todo) porque es más segura y te asegura que todos los archivos estén actualizados.
