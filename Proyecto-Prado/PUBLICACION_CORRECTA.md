# 🔧 Guía de Publicación Correcta para Smarterasp

## ⚠️ Problema Identificado

El error 500 puede ser causado por problemas en la forma de publicación. Asegúrate de seguir estos pasos EXACTAMENTE.

## ✅ Método Correcto de Publicación

### Paso 1: Limpiar TODO antes de publicar

1. **Cierra Visual Studio** completamente
2. **Elimina** las carpetas temporales:
   - `HotelPrado.UI\bin\Release` (si existe)
   - `HotelPrado.UI\obj\Release` (si existe)
3. **Abre Visual Studio** nuevamente
4. **Abre** el proyecto

### Paso 2: Configurar para Release

1. En la barra superior, cambia de **Debug** a **Release**
2. **Build > Clean Solution** (limpia todo)
3. **Build > Rebuild Solution** (recompila todo desde cero)
4. Verifica que **NO hay errores** en la ventana "Error List"

### Paso 3: Publicar CORRECTAMENTE

1. **Click derecho** en `HotelPrado.UI` → **Publish**
2. Si tienes un perfil guardado:
   - Selecciónalo
   - Click en **Publish**
3. Si NO tienes perfil:
   - Click en **New** o **Create new profile**
   - Selecciona **Folder**
   - **Publish method:** Folder
   - **Target location:** `C:\Users\emmag\OneDrive\Documentos\PradoPubli` (o la carpeta que uses)
   - Click en **Publish**

### Paso 4: VERIFICAR que se copiaron TODOS los archivos

**Ejecutar el script de verificación:** (desde PowerShell, estando en `PradoPubli`)
```powershell
cd C:\Users\emmag\OneDrive\Documentos\PradoPubli
& "C:\Users\emmag\source\repos\HotelPradoInnSuites\Proyecto-Prado\VERIFICAR_PUBLICACION.ps1"
```

Después de publicar, verifica que en `PradoPubli` tienes:

#### ✅ Carpetas OBLIGATORIAS:
- [ ] `bin/` - Con TODAS las DLLs (debe tener muchas, no solo HotelPrado.UI.dll)
- [ ] `Content/` - Con todos los CSS
- [ ] `Scripts/` - Con todos los JS
- [ ] `Views/` - Con TODAS las carpetas y vistas
- [ ] `Views/Account/` - Debe existir
- [ ] `Views/Account/Register.cshtml` - Debe existir
- [ ] `Views/Account/Login.cshtml` - Debe existir
- [ ] `Img/` - Con todas las imágenes

#### ✅ Archivos OBLIGATORIOS:
- [ ] `Web.config` - Debe existir
- [ ] `Global.asax` - Debe existir
- [ ] `favicon.ico` - Debe existir

### Paso 5: Verificar el contenido de /bin

Abre la carpeta `PradoPubli\bin\` y verifica que tenga **MUCHAS** DLLs, incluyendo:

- ✅ `HotelPrado.UI.dll` (tu proyecto)
- ✅ `HotelPrado.Abstracciones.dll`
- ✅ `HotelPrado.AccesoADatos.dll`
- ✅ `HotelPrado.LN.dll`
- ✅ `Microsoft.Owin.*.dll` (varias)
- ✅ `System.Web.Mvc.dll`
- ✅ `EntityFramework.dll`
- ✅ Y muchas más...

**Si faltan DLLs, la publicación NO fue correcta.**

## 🚨 Problemas Comunes en la Publicación

### Problema 1: No se copian todas las DLLs
**Solución:** 
- Asegúrate de que todos los proyectos referenciados estén compilados
- Verifica que las referencias de NuGet estén correctas
- Usa **Rebuild Solution** en lugar de solo Build

### Problema 2: No se copian las Views
**Solución:**
- Verifica que las vistas tengan **Build Action: Content** en sus propiedades
- Asegúrate de que la carpeta `Views` esté incluida en el proyecto

### Problema 3: Web.config no se transforma correctamente
**Solución:**
- Verifica que `Web.Release.config` exista
- Asegúrate de que las transformaciones estén correctas

## 📤 Subir al Servidor CORRECTAMENTE

### Opción A: Subir TODO (RECOMENDADO)

1. Conecta con FileZilla
2. Ve a `/site1` en el servidor
3. **Selecciona TODO** el contenido de `PradoPubli` (Ctrl+A)
4. **Arrastra** todo al servidor
5. Cuando pregunte, elige **"Sobrescribir todo"** o **"Overwrite all"**

### Opción B: Subir solo lo necesario (si ya subiste antes)

1. Sube `bin/HotelPrado.UI.dll` (el más importante)
2. Sube `bin/` completa (por si faltan DLLs)
3. Sube `Views/Account/Register.cshtml` (por si falta)
4. Verifica `Web.config` (pero NO lo sobrescribas sin verificar la cadena de conexión)

## 🔍 Verificación Final en el Servidor

Después de subir, verifica en el servidor que:

1. `/site1/bin/` tiene MUCHAS DLLs (no solo una)
2. `/site1/Views/Account/Register.cshtml` existe
3. `/site1/Web.config` tiene la cadena de conexión correcta

## 🧪 Prueba Paso a Paso

1. Visita `/Ping` → Debe responder "OK"
2. Visita `/Diagnostico` → Debe mostrar estado de OWIN y BD
3. Visita `/Account/Register` → Debe cargar o mostrar error detallado

## 💡 Si SIGUE fallando después de esto:

1. **Revisa los logs del servidor** en el panel de Smarterasp
2. **Verifica** que la cadena de conexión en `Web.config` del servidor sea correcta
3. **Asegúrate** de que todas las DLLs estén en `/bin`
4. **Comparte** el mensaje de error detallado que ahora debería aparecer

---

**La clave está en asegurarse de que TODOS los archivos se copien durante la publicación.**
