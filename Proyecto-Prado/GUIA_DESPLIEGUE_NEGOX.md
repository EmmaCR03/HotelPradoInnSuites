# 🚀 Guía de Despliegue en Negox - Hotel Prado

## ✅ Compatibilidad con Negox

**¡Excelente noticia!** Negox es un proveedor de hosting especializado en aplicaciones **ASP.NET** y **.NET Core**, lo que lo hace **perfectamente compatible** con tu proyecto.

### Características de Negox:
- ✅ **Windows Server 2019** con **IIS 10**
- ✅ **.NET Framework 4.8.1** (compatible con tu proyecto)
- ✅ **SQL Server** para bases de datos
- ✅ **Panel de control Plesk** (similar a cPanel pero para Windows)
- ✅ **Integración con Git** para despliegue continuo
- ✅ **Soporte técnico especializado** en ASP.NET

---

## 📋 Requisitos Previos

Antes de comenzar, asegúrate de tener:

- [ ] Cuenta activa en Negox
- [ ] Acceso al panel de control Plesk
- [ ] Credenciales de acceso FTP (si aplica)
- [ ] Información de la base de datos SQL Server
- [ ] Proyecto compilado en modo **Release**

---

## 🔧 Paso 1: Preparar el Proyecto para Producción

### 1.1 Compilar en Modo Release

1. Abre el proyecto en **Visual Studio**
2. En la barra de herramientas, cambia la configuración de **Debug** a **Release**
3. Limpia la solución: `Build > Clean Solution`
4. Compila la solución: `Build > Build Solution`
5. Verifica que no hay errores de compilación

### 1.2 Publicar el Proyecto

1. Haz clic derecho en el proyecto **HotelPrado.UI**
2. Selecciona **Publish...**
3. Elige **Folder** como método de publicación
4. Selecciona una carpeta de destino (ej: `C:\Publish\HotelPrado`)
5. Haz clic en **Publish**

**Verifica que se generaron:**
- ✅ Carpeta `bin` con todas las DLLs
- ✅ `Web.config`
- ✅ `Global.asax` y `Global.asax.cs`
- ✅ Carpetas: `Content`, `Scripts`, `Views`, `Img`
- ✅ Todas las vistas `.cshtml`

---

## 🗄️ Paso 2: Configurar Base de Datos en Negox

### 2.1 Crear Base de Datos SQL Server

1. Inicia sesión en el **panel de control Plesk** de Negox
2. Navega a **Databases** > **SQL Server Databases**
3. Haz clic en **Add Database**
4. Configura:
   - **Database name**: `HotelPrado` (o el nombre que prefieras)
   - **Database user**: Crea un nuevo usuario o usa uno existente
   - **Password**: Genera una contraseña segura
5. Guarda las credenciales (las necesitarás para el `Web.config`)

### 2.2 Obtener Cadena de Conexión

En Plesk, la cadena de conexión típicamente tiene este formato:

```
Data Source=TU_SERVIDOR_SQL;Initial Catalog=HotelPrado;User ID=usuario;Password=contraseña;Connection Timeout=30;MultipleActiveResultSets=True
```

**Nota:** El nombre del servidor SQL generalmente es algo como: `localhost` o `sql.negox.com` o similar. Consulta con el soporte de Negox si no estás seguro.

### 2.3 Ejecutar Scripts de Base de Datos

1. Conéctate a la base de datos usando **SQL Server Management Studio (SSMS)** o la herramienta de Plesk
2. Ejecuta los scripts SQL necesarios:
   - Scripts de creación de tablas (si los tienes)
   - Scripts de migración de datos (si aplica)
3. Verifica que todas las tablas se crearon correctamente

---

## 📝 Paso 3: Configurar Web.config para Producción

### 3.1 Actualizar Cadena de Conexión

Abre el archivo `Web.config` de la carpeta de publicación y actualiza la cadena de conexión:

```xml
<connectionStrings>
  <add name="Contexto" 
       connectionString="Data Source=TU_SERVIDOR_SQL;Initial Catalog=HotelPrado;User ID=usuario;Password=contraseña;Connection Timeout=30;MultipleActiveResultSets=True;Pooling=true;Min Pool Size=5;Max Pool Size=100" 
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

### 3.2 Configurar para Producción

Asegúrate de que `Web.config` tenga estas configuraciones:

```xml
<system.web>
  <compilation debug="false" targetFramework="4.8.1" />
  <httpRuntime targetFramework="4.8.1" maxRequestLength="1048576" executionTimeout="600" />
  
  <!-- Configuración de errores personalizados -->
  <customErrors mode="RemoteOnly" defaultRedirect="~/Error">
    <error statusCode="404" redirect="~/Error/NotFound" />
    <error statusCode="500" redirect="~/Error/ServerError" />
  </customErrors>
  
  <!-- ... resto de configuración ... -->
</system.web>
```

**Importante:**
- ✅ `debug="false"` (siempre en producción)
- ✅ Cadena de conexión actualizada con datos de Negox
- ✅ Configuraciones de email actualizadas (si aplica)
- ✅ URLs de APIs externas actualizadas (si aplica)

---

## 📤 Paso 4: Subir Archivos a Negox

Negox ofrece varias formas de subir archivos. Elige la que prefieras:

### Opción A: Usando Plesk File Manager (Recomendado para principiantes)

1. Inicia sesión en **Plesk**
2. Navega a **Files** > **File Manager**
3. Ve a la carpeta donde está tu sitio (generalmente `httpdocs` o `wwwroot`)
4. **Sube todos los archivos** de la carpeta de publicación:
   - Selecciona todos los archivos y carpetas
   - Usa la función de **Upload** o arrastra y suelta
5. Asegúrate de mantener la estructura de carpetas

### Opción B: Usando FTP

1. Obtén las credenciales FTP de Negox (en Plesk: **Websites & Domains** > **FTP Access**)
2. Usa un cliente FTP (FileZilla, WinSCP, etc.)
3. Conéctate al servidor
4. Navega a la carpeta del sitio (generalmente `httpdocs`)
5. Sube todos los archivos de la carpeta de publicación

### Opción C: Usando Git (Recomendado para desarrollo continuo)

1. En Plesk, ve a **Git**
2. Configura tu repositorio Git
3. Plesk puede hacer deploy automático desde Git
4. Configura el branch y la carpeta de destino

**Archivos que DEBES subir:**
- ✅ Carpeta `bin` completa (con todas las DLLs)
- ✅ `Web.config`
- ✅ `Global.asax` y `Global.asax.cs`
- ✅ Carpeta `Content` (CSS, imágenes)
- ✅ Carpeta `Scripts` (JavaScript)
- ✅ Carpeta `Views` (todas las vistas `.cshtml`)
- ✅ Carpeta `Img` (imágenes del sitio)
- ✅ Cualquier otra carpeta de recursos

**Archivos que NO debes subir:**
- ❌ Archivos `.cs` (código fuente)
- ❌ Archivos `.csproj` (archivos de proyecto)
- ❌ Archivos `.sln` (archivos de solución)
- ❌ Carpetas `obj` y `bin` del proyecto original (solo la carpeta `bin` de la publicación)

---

## ⚙️ Paso 5: Configurar IIS en Plesk

### 5.1 Verificar Configuración del Sitio

1. En Plesk, ve a **Websites & Domains**
2. Selecciona tu dominio
3. Verifica que el **Document Root** apunta a la carpeta donde subiste los archivos

### 5.2 Configurar Application Pool

1. En Plesk, ve a **Websites & Domains** > tu dominio > **ASP.NET Settings**
2. Configura:
   - **ASP.NET version**: `.NET Framework 4.8.1` o `v4.0`
   - **Application Pool**: Crea uno nuevo o usa el existente
   - **Managed Pipeline Mode**: `Integrated`

### 5.3 Configurar Permisos

1. Asegúrate de que la carpeta tiene permisos de lectura
2. Si tienes carpeta `App_Data`, dale permisos de escritura al Application Pool Identity

---

## 🔍 Paso 6: Verificar y Probar

### 6.1 Verificación Inicial

1. Accede a tu sitio web desde el navegador
2. Verifica que la página principal carga
3. Revisa la consola del navegador (F12) para errores de JavaScript
4. Verifica que las imágenes cargan correctamente
5. Verifica que los estilos CSS se aplican

### 6.2 Pruebas Funcionales

- [ ] Probar login (si aplica)
- [ ] Probar funcionalidades principales
- [ ] Verificar conexión a base de datos
- [ ] Probar creación/lectura/actualización de datos
- [ ] Verificar que los logs se generan (si aplica)

### 6.3 Verificar Logs

1. En Plesk, ve a **Logs** > **IIS Logs**
2. Revisa si hay errores
3. También revisa los logs de la aplicación (si los tienes configurados)

---

## 🔒 Paso 7: Configurar Seguridad

### 7.1 Configurar HTTPS (Recomendado)

1. En Plesk, ve a **SSL/TLS Certificates**
2. Instala un certificado SSL (Let's Encrypt es gratuito)
3. Activa **Force HTTPS** para redirigir HTTP a HTTPS

### 7.2 Revisar Configuración de Seguridad

- [ ] Verificar que las contraseñas no están en texto plano
- [ ] Verificar que los archivos sensibles no son accesibles públicamente
- [ ] Revisar permisos de archivos y carpetas
- [ ] Configurar autenticación/autorización correctamente

---

## 🆘 Solución de Problemas Comunes

### Error 500 - Internal Server Error

**Posibles causas:**
- Application Pool no configurado correctamente
- Permisos de carpetas incorrectos
- Error en `Web.config`

**Solución:**
1. Revisa los logs de IIS en Plesk
2. Verifica que el Application Pool está configurado para .NET Framework 4.8.1
3. Verifica permisos de carpetas
4. Revisa `Web.config` por errores de sintaxis

### Error de Base de Datos

**Posibles causas:**
- Cadena de conexión incorrecta
- SQL Server no permite conexiones remotas
- Firewall bloqueando conexiones

**Solución:**
1. Verifica la cadena de conexión en `Web.config`
2. Contacta a Negox para verificar que SQL Server permite conexiones
3. Verifica que el firewall permite conexiones al puerto 1433 (SQL Server)

### Archivos DLL no encontrados

**Posibles causas:**
- DLLs faltantes en la carpeta `bin`
- Referencias de NuGet no incluidas

**Solución:**
1. Verifica que todas las DLLs están en la carpeta `bin`
2. Re-publica el proyecto asegurándote de incluir todas las dependencias
3. Verifica que las referencias de NuGet están configuradas como "Copy Local = True"

### Error 404 - Página no encontrada

**Posibles causas:**
- Rutas no configuradas correctamente
- Módulo de URL Rewrite no instalado

**Solución:**
1. Verifica la configuración de rutas en `RouteConfig.cs`
2. Contacta a Negox para verificar que el módulo de URL Rewrite está instalado en IIS

### Imágenes o CSS no cargan

**Posibles causas:**
- Rutas incorrectas
- Archivos no subidos correctamente

**Solución:**
1. Verifica que todas las carpetas (`Content`, `Scripts`, `Img`) se subieron correctamente
2. Verifica las rutas en las vistas (deben ser relativas o usar `@Url.Content()`)

---

## 📞 Contacto con Negox

Si encuentras problemas durante el despliegue:

1. **Soporte Técnico de Negox**: Consulta su sitio web para información de contacto
2. **Documentación de Plesk**: Plesk tiene documentación extensa disponible
3. **Foros de la Comunidad**: Negox puede tener foros o comunidad de usuarios

**Información que debes tener lista:**
- Tu dominio
- Tipo de error que estás viendo
- Logs de error (si los tienes)
- Pasos que ya intentaste

---

## ✅ Checklist Final de Despliegue

### Antes de Subir:
- [ ] Proyecto compilado en modo **Release**
- [ ] `Web.config` configurado para producción
- [ ] Cadena de conexión actualizada con datos de Negox
- [ ] `debug="false"` en Web.config
- [ ] Base de datos creada en Negox
- [ ] Scripts de base de datos ejecutados
- [ ] Todas las dependencias NuGet incluidas en `bin`

### Archivos Subidos:
- [ ] Todos los archivos de la carpeta de publicación
- [ ] Carpeta `bin` completa (con todas las DLLs)
- [ ] `Web.config`
- [ ] Carpetas: `Content`, `Scripts`, `Views`, `Img`, etc.
- [ ] `Global.asax` y `Global.asax.cs`

### Configuración del Servidor:
- [ ] IIS configurado correctamente en Plesk
- [ ] Application Pool configurado para .NET Framework 4.8.1
- [ ] Permisos de carpetas correctos
- [ ] Base de datos accesible desde el servidor
- [ ] SSL/HTTPS configurado (recomendado)

### Pruebas:
- [ ] Sitio carga correctamente
- [ ] Login funciona (si aplica)
- [ ] Base de datos funciona
- [ ] Imágenes y CSS cargan
- [ ] JavaScript funciona
- [ ] Funcionalidades principales trabajan
- [ ] No hay errores en los logs

---

## 🔄 Despliegue Continuo con Git (Opcional)

Si quieres configurar despliegue automático desde Git:

1. En Plesk, ve a **Git**
2. Agrega tu repositorio Git (GitHub, Bitbucket, etc.)
3. Configura el branch (generalmente `main` o `master`)
4. Configura la carpeta de destino
5. Configura el comando de build (si es necesario)
6. Plesk puede hacer deploy automático cada vez que hagas push

**Ventajas:**
- ✅ Despliegue automático
- ✅ Control de versiones
- ✅ Rollback fácil
- ✅ Mejor para trabajo en equipo

---

## 📝 Notas Finales

- **Siempre haz backup** antes de hacer cambios importantes
- **Prueba primero en un subdominio** si es posible antes de cambiar el dominio principal
- **Mantén el sitio viejo funcionando** hasta estar 100% seguro de que el nuevo funciona
- **Documenta todos los cambios** que hagas
- **Monitorea el sitio** durante las primeras 24-48 horas después del despliegue

---

## 🔗 Recursos Adicionales

- [Documentación de Negox](https://www.negox.com)
- [Documentación de Plesk](https://docs.plesk.com/)
- [Despliegue de ASP.NET MVC](https://docs.microsoft.com/en-us/aspnet/mvc/overview/deployment/)
- [Configuración de IIS](https://docs.microsoft.com/en-us/iis/)

---

## 📅 Información del Despliegue

**Fecha de Despliegue**: _______________________
**Responsable**: _______________________
**Versión Desplegada**: _______________________
**Dominio**: _______________________
**Base de Datos**: _______________________

---

¡Éxito con tu despliegue en Negox! 🎉



