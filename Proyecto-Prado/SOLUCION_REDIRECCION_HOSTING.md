# Solución para Problema de Redirección Infinita en Hosting

## Cambios Realizados

Se han realizado los siguientes cambios para solucionar el problema de redirección infinita (`ERR_TOO_MANY_REDIRECTS`) en el hosting de Smarterasp:

### 1. **Configuración de OWIN (Startup.Auth.cs)**
   - ✅ Agregada configuración de cookies más robusta para hosting
   - ✅ Implementado `OnApplyRedirect` para evitar redirecciones cuando ya estamos en Login/Register
   - ✅ Configurado `CookieSecureOption.SameAsRequest` para funcionar en HTTP y HTTPS
   - ✅ Establecido tiempo de expiración de cookies (30 días)

### 2. **Web.Release.config**
   - ✅ Comentada la configuración de `httpErrors` que causaba bucles de redirección
   - ✅ Ahora se usa solo `customErrors` para manejo de errores

### 3. **AccountController**
   - ✅ Mejorado `SanitizeReturnUrl` para detectar y evitar más tipos de bucles
   - ✅ Agregado contador de redirecciones para detectar bucles
   - ✅ Mejorado manejo de errores sin redirecciones infinitas

## Instrucciones para Publicar

### Paso 1: Compilar en Modo Release
1. En Visual Studio, cambia la configuración a **Release**
2. Limpia la solución: `Build > Clean Solution`
3. Compila: `Build > Build Solution`

### Paso 2: Publicar con FileZilla o Folder

#### Opción A: Publicar con Folder (Recomendado)
1. Click derecho en el proyecto `HotelPrado.UI`
2. Selecciona **Publish**
3. Elige **Folder** como método de publicación
4. Configura la carpeta de destino (ej: `C:\Publish\HotelPrado`)
5. Click en **Publish**

#### Opción B: Publicar con FileZilla
1. Publica localmente primero (Opción A)
2. Conecta FileZilla a tu servidor Smarterasp
3. Sube todos los archivos de la carpeta de publicación a la carpeta del sitio web

### Paso 3: Archivos Importantes a Verificar en el Servidor

Asegúrate de que estos archivos estén correctamente en el servidor:

#### ✅ Web.config
- Debe tener la cadena de conexión correcta de producción
- `customErrors mode="Off"` o `"RemoteOnly"` para debugging inicial
- `owin:AutomaticAppStartup` debe estar en `true`

#### ✅ Bin/
- Todos los archivos DLL deben estar presentes
- Especialmente: `Microsoft.Owin.*.dll`, `System.Web.Mvc.dll`, etc.

#### ✅ App_Start/
- `Startup.cs` debe estar presente
- `Startup.Auth.cs` debe estar presente
- `RouteConfig.cs` debe estar presente

#### ✅ Views/
- Todas las carpetas de vistas deben estar presentes
- Especialmente `Views/Account/Login.cshtml` y `Register.cshtml`

### Paso 4: Verificar Base de Datos

Asegúrate de que la base de datos en producción tenga:
- ✅ Tablas de Identity: `AspNetUsers`, `AspNetRoles`, `AspNetUserRoles`, etc.
- ✅ Cadena de conexión correcta en `Web.config`
- ✅ Permisos correctos para el usuario de la base de datos

### Paso 5: Probar la Aplicación

Después de publicar, prueba estas URLs en orden:

1. **`/Ping`** - Debe responder "OK" (prueba básica de que la app funciona)
2. **`/Diagnostico`** - Debe mostrar estado de OWIN y BD
3. **`/Account/Login`** - Debe cargar la página de login sin redirecciones
4. **`/Account/Register`** - Debe cargar la página de registro sin redirecciones

## Solución de Problemas

### Si aún hay redirección infinita:

1. **Limpiar cookies del navegador** para el dominio
2. **Verificar que OWIN se inicializa correctamente:**
   - Visita `/Diagnostico` y verifica que OWIN muestre "OK"
3. **Verificar logs del servidor:**
   - En Smarterasp, revisa los logs de errores de IIS
4. **Verificar que no haya configuración duplicada:**
   - Asegúrate de que no haya `<authentication mode="Forms">` en Web.config (debe usar solo OWIN)

### Si la página de Login carga pero no funciona:

1. Verifica la conexión a la base de datos con `/Diagnostico`
2. Verifica que las tablas de Identity existan en la BD
3. Revisa los logs de errores en el servidor

### Si hay errores 500:

1. Cambia `customErrors mode="Off"` temporalmente para ver el error detallado
2. Revisa los logs de IIS en el panel de Smarterasp
3. Verifica que todos los DLLs estén presentes en `/bin`

## Configuración Recomendada para Producción

Una vez que todo funcione, cambia en `Web.Release.config`:

```xml
<customErrors mode="RemoteOnly" defaultRedirect="~/Error">
```

Esto mostrará errores detallados solo localmente y páginas de error personalizadas en producción.

## Notas Importantes

- ⚠️ **NUNCA** publiques con `debug="true"` en producción (afecta el rendimiento)
- ⚠️ Asegúrate de que `owin:AutomaticAppStartup` esté en `true`
- ⚠️ Verifica que la cadena de conexión sea la correcta de producción
- ⚠️ Si cambias algo en `Startup.cs` o `Startup.Auth.cs`, debes recompilar y republicar

## Contacto y Soporte

Si después de seguir estos pasos aún tienes problemas:
1. Revisa los logs de errores en el panel de Smarterasp
2. Prueba `/Diagnostico` para ver el estado de OWIN y BD
3. Verifica que todos los archivos se hayan subido correctamente
