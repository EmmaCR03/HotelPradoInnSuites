# Guía de Despliegue en cPanel - Hotel Prado

## ⚠️ Consideraciones Importantes

Este proyecto es una aplicación **ASP.NET MVC 5** con **.NET Framework 4.8.1**, lo cual requiere:
- **Servidor Windows** con **IIS (Internet Information Services)**
- **SQL Server** para la base de datos
- **.NET Framework 4.8.1** instalado en el servidor

### Problema con cPanel Tradicional

**cPanel tradicionalmente funciona en servidores Linux**, que NO son compatibles con aplicaciones .NET Framework. Sin embargo, existen opciones:

---

## ✅ Opciones de Despliegue

### Opción 1: cPanel en Windows Server (Recomendado si ya tienes cPanel)

Algunos proveedores de hosting ofrecen **cPanel en Windows Server**. Si tu proveedor lo ofrece:

#### Requisitos del Hosting:
- Windows Server 2016 o superior
- IIS 10 o superior
- .NET Framework 4.8.1 instalado
- SQL Server (puede ser SQL Server Express o completo)
- cPanel para Windows

#### Pasos para Desplegar:

1. **Preparar el Proyecto para Producción**
   ```bash
   # En Visual Studio:
   # 1. Cambiar configuración a "Release"
   # 2. Build > Publish > Seleccionar "Folder"
   # 3. Elegir una carpeta de destino
   ```

2. **Configurar Web.config para Producción**
   - Actualizar la cadena de conexión con los datos del servidor
   - Cambiar `debug="true"` a `debug="false"`
   - Configurar configuraciones específicas del servidor

3. **Subir Archivos vía cPanel File Manager o FTP**
   - Subir todos los archivos de la carpeta de publicación
   - Asegurarse de incluir la carpeta `bin` con todas las DLLs

4. **Configurar Base de Datos**
   - Crear la base de datos en SQL Server
   - Ejecutar scripts de migración si es necesario
   - Actualizar la cadena de conexión en Web.config

5. **Configurar IIS en cPanel**
   - Crear un nuevo sitio web o aplicación
   - Apuntar al directorio donde subiste los archivos
   - Configurar el Application Pool para .NET Framework 4.8.1

---

### Opción 2: Subdominio para Testing (Recomendado)

Esta es la mejor opción para probar antes de cambiar el dominio principal.

#### Paso 1: Crear Subdominio en cPanel

1. Inicia sesión en **cPanel**
2. Ve a **Subdominios** (Subdomains)
3. Crea un nuevo subdominio, por ejemplo:
   - **Nombre**: `nuevo` o `v2` o `test`
   - **Dominio**: `tudominio.com`
   - **Resultado**: `nuevo.tudominio.com`
4. El subdominio apuntará a una carpeta, por ejemplo: `public_html/nuevo`

#### Paso 2: Subir el Proyecto al Subdominio

1. **Preparar el proyecto en modo Release**
   - En Visual Studio: Build > Configuration Manager > Release
   - Build > Publish > Folder Profile
   - Seleccionar carpeta de destino

2. **Subir archivos al subdominio**
   - Usa **File Manager** de cPanel o **FTP**
   - Sube todos los archivos a la carpeta del subdominio (ej: `public_html/nuevo`)
   - Asegúrate de incluir:
     - Todos los archivos `.dll` en la carpeta `bin`
     - `Web.config`
     - Todas las carpetas: `Content`, `Scripts`, `Views`, `Img`, etc.

3. **Configurar Base de Datos**
   - Crea una nueva base de datos para el subdominio (o usa la misma si quieres datos compartidos)
   - Actualiza la cadena de conexión en `Web.config`

#### Paso 3: Configurar IIS para el Subdominio

Si tu hosting tiene acceso a configuración de IIS:

1. Abre **IIS Manager** (si tienes acceso)
2. Crea un nuevo **Application Pool**:
   - Nombre: `HotelPradoNuevo`
   - .NET CLR Version: `.NET CLR Version v4.0`
   - Managed Pipeline Mode: `Integrated`

3. Crea un nuevo **Website** o **Application**:
   - Nombre: `HotelPrado-Nuevo`
   - Physical Path: Apunta a la carpeta del subdominio
   - Binding: 
     - Type: `http` o `https`
     - Host name: `nuevo.tudominio.com`
   - Application Pool: Selecciona el pool creado arriba

#### Paso 4: Verificar Permisos

- Asegúrate de que la carpeta tenga permisos de lectura/ejecución
- La carpeta `App_Data` (si existe) necesita permisos de escritura

---

### Opción 3: Hosting Windows con Plesk (Alternativa)

Si tu proveedor no ofrece cPanel en Windows, considera:

- **Plesk**: Panel de control similar a cPanel pero para Windows
- **Hosting Windows tradicional**: Con acceso directo a IIS

---

## 📋 Checklist de Despliegue

### Antes de Subir:

- [ ] Proyecto compilado en modo **Release**
- [ ] `Web.config` configurado para producción
- [ ] Cadena de conexión actualizada con datos del servidor
- [ ] `debug="false"` en Web.config
- [ ] Base de datos creada y scripts ejecutados
- [ ] Todas las dependencias NuGet incluidas en `bin`

### Archivos a Subir:

- [ ] Todos los archivos de la carpeta de publicación
- [ ] Carpeta `bin` completa (con todas las DLLs)
- [ ] `Web.config`
- [ ] Carpetas: `Content`, `Scripts`, `Views`, `Img`, etc.
- [ ] `Global.asax` y `Global.asax.cs`

### Configuración del Servidor:

- [ ] IIS configurado correctamente
- [ ] Application Pool configurado para .NET Framework 4.8.1
- [ ] Permisos de carpetas correctos
- [ ] Base de datos accesible desde el servidor
- [ ] Firewall configurado (si es necesario)

---

## 🔄 Migración del Subdominio al Dominio Principal

Una vez que hayas probado y estés seguro de que todo funciona:

### Opción A: Cambiar el Binding en IIS

1. En IIS Manager, edita el sitio del subdominio
2. Cambia el **Host name** en el binding de `nuevo.tudominio.com` a `tudominio.com`
3. O crea un nuevo binding para el dominio principal

### Opción B: Reemplazar Archivos del Dominio Principal

1. **Hacer backup** del sitio actual (el viejo)
2. Subir los archivos nuevos al directorio del dominio principal
3. Actualizar la base de datos si es necesario
4. Verificar que todo funcione

### Opción C: Cambiar DNS (Más Seguro)

1. Mantener ambos sitios funcionando
2. Cambiar el DNS del dominio principal para que apunte al nuevo servidor/carpeta
3. Tiempo de propagación: 24-48 horas
4. Durante este tiempo, ambos sitios pueden funcionar

---

## 🔧 Configuración de Web.config para Producción

Ejemplo de configuración para producción:

```xml
<configuration>
  <connectionStrings>
    <add name="Contexto" 
         connectionString="Data Source=TU_SERVIDOR_SQL;Initial Catalog=HotelPrado;User ID=usuario;Password=contraseña;Connection Timeout=30;MultipleActiveResultSets=True;Pooling=true;Min Pool Size=5;Max Pool Size=100" 
         providerName="System.Data.SqlClient" />
  </connectionStrings>
  
  <system.web>
    <compilation debug="false" targetFramework="4.8.1" />
    <httpRuntime targetFramework="4.8.1" maxRequestLength="1048576" executionTimeout="600" />
    <!-- ... resto de configuración ... -->
  </system.web>
</configuration>
```

---

## 🆘 Solución de Problemas Comunes

### Error 500 - Internal Server Error
- Verifica los permisos de las carpetas
- Revisa el Application Pool (debe ser .NET Framework 4.8.1)
- Revisa los logs de IIS

### Error de Base de Datos
- Verifica la cadena de conexión
- Asegúrate de que SQL Server permite conexiones remotas
- Verifica firewall y puertos (1433 para SQL Server)

### Archivos DLL no encontrados
- Asegúrate de que todas las DLLs están en la carpeta `bin`
- Verifica que las referencias de NuGet están incluidas

### Error de Rutas (404)
- Verifica que el módulo de URL Rewrite está instalado en IIS
- Verifica la configuración de rutas en `RouteConfig.cs`

---

## 📞 Contacto con el Proveedor de Hosting

Antes de comenzar, contacta a tu proveedor de hosting y pregunta:

1. ¿Ofrecen hosting Windows con cPanel?
2. ¿Qué versión de .NET Framework está disponible?
3. ¿Tienen SQL Server disponible?
4. ¿Pueden crear un subdominio para testing?
5. ¿Tienen acceso a configuración de IIS o lo hacen ellos?

---

## 📝 Notas Finales

- **Siempre haz backup** antes de hacer cambios
- **Prueba primero en el subdominio** antes de cambiar el dominio principal
- **Mantén el sitio viejo funcionando** hasta estar 100% seguro
- **Documenta todos los cambios** que hagas

---

## 🔗 Recursos Adicionales

- [Documentación de IIS](https://docs.microsoft.com/en-us/iis/)
- [Despliegue de ASP.NET MVC](https://docs.microsoft.com/en-us/aspnet/mvc/overview/deployment/)
- [Configuración de Web.config](https://docs.microsoft.com/en-us/aspnet/identity/overview/getting-started/aspnet-identity-using-mysql-storage-with-an-entityframework-mysql-provider)









