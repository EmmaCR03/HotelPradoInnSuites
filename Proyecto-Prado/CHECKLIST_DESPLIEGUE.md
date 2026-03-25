# ✅ Checklist de Despliegue - Hotel Prado

Usa esta lista para asegurarte de que todo esté listo antes y después del despliegue.

---

## 🚨 Para que funcionen Login, Registrarse y Mis reservas (obligatorio)

Si al entrar a **Iniciar sesión**, **Registrarme** o **Mis reservas** ves error o te redirige a una página en blanco/404, revisa esto:

1. **Publicar siempre en modo Release**
   - En Visual Studio: arriba elige **Release** (no Debug).
   - Build → Clean Solution → Publish.
   - Así el `Web.config` publicado llevará la cadena de conexión de **producción** (SQL5106.site4now.net). Si publicas en Debug, en el host seguirá la conexión local y fallará.

2. **Subir TODO lo que genera la publicación**
   - Incluye la carpeta **bin** (con todas las DLL), **Views**, **Img**, **Content**, **Scripts** y el **Web.config**.
   - Si usas el script `SubirAlHosting.ps1`, sube desde la misma carpeta donde publicas (ej. `PradoPubli`).

3. **Base de datos en el host**
   - La base de datos de producción debe tener las tablas de **Identity** (AspNetUsers, AspNetRoles, AspNetUserRoles, etc.). Si creaste la BD desde cero en el host, ejecuta ahí los mismos scripts o migraciones que usaste en local para crear esas tablas.
   - Opcional: ejecutar `DB\dbo\Tables\Insertar_ConfiguracionHeroBanner_EnProduccion.sql` en la BD del host para los banners.

4. **Comprobar el Web.config en el host**
   - En la carpeta publicada (o ya en el servidor), abre `Web.config` y revisa que en `<connectionStrings>` el valor de `connectionString` sea el del servidor (ej. `Data Source=SQL5106.site4now.net;...`). Si sigue `Data Source=.;` o `Initial Catalog=HotelPrado`, la transformación no se aplicó: vuelve a publicar en **Release** y a subir el `Web.config`.

Cuando todo esto esté bien, Iniciar sesión, Registrarme y Mis reservas deberían cargar y funcionar.

---

## 📦 Preparación del Proyecto

### Antes de Compilar
- [ ] Revisar y actualizar todas las cadenas de conexión
- [ ] Verificar que no hay credenciales hardcodeadas en el código
- [ ] Revisar configuraciones de email, APIs externas, etc.
- [ ] Verificar que todas las dependencias NuGet están actualizadas
- [ ] Probar la aplicación localmente en modo Release

### Compilación
- [ ] Cambiar configuración a **Release** en Visual Studio
- [ ] Limpiar la solución (Build > Clean Solution)
- [ ] Compilar la solución (Build > Build Solution)
- [ ] Verificar que no hay errores de compilación
- [ ] Publicar el proyecto (Build > Publish)
  - [ ] Seleccionar "Folder" como método de publicación
  - [ ] Elegir una carpeta de destino
  - [ ] Verificar que todos los archivos se generaron correctamente

### Archivos a Verificar
- [ ] Carpeta `bin` contiene todas las DLLs necesarias
- [ ] `Web.config` está configurado para producción
- [ ] `Global.asax` y `Global.asax.cs` están incluidos
- [ ] Todas las carpetas de contenido están incluidas:
  - [ ] `Content/` (CSS, imágenes)
  - [ ] `Scripts/` (JavaScript)
  - [ ] `Views/` (Vistas Razor)
  - [ ] `Img/` (Imágenes del sitio)
  - [ ] Cualquier otra carpeta de recursos

---

## 🗄️ Base de Datos

### Preparación
- [ ] Backup de la base de datos actual (si existe)
- [ ] Crear nueva base de datos en el servidor de producción
- [ ] Ejecutar scripts de creación de tablas
- [ ] Ejecutar scripts de migración de datos (si aplica)
- [ ] Verificar que todas las tablas se crearon correctamente
- [ ] Probar conexión a la base de datos desde el servidor

### Configuración
- [ ] Obtener cadena de conexión del servidor
- [ ] Verificar permisos del usuario de base de datos
- [ ] Actualizar `Web.config` con la cadena de conexión correcta
- [ ] Probar conexión con la nueva cadena de conexión

---

## 🌐 Configuración del Servidor

### Requisitos del Servidor
- [ ] Verificar que el servidor tiene Windows Server instalado
- [ ] Verificar que IIS está instalado y funcionando
- [ ] Verificar que .NET Framework 4.8.1 está instalado
- [ ] Verificar que SQL Server está disponible y accesible
- [ ] Verificar que el firewall permite conexiones necesarias

### IIS - Application Pool
- [ ] Crear nuevo Application Pool (o usar existente)
- [ ] Configurar .NET CLR Version: **v4.0**
- [ ] Configurar Managed Pipeline Mode: **Integrated**
- [ ] Configurar Identity (usuario que ejecuta el pool)
- [ ] Verificar que el pool está iniciado

### IIS - Website/Application
- [ ] Crear nuevo Website o Application
- [ ] Configurar Physical Path (apuntar a la carpeta del proyecto)
- [ ] Configurar Binding:
  - [ ] Tipo: HTTP o HTTPS
  - [ ] Puerto: 80 (HTTP) o 443 (HTTPS)
  - [ ] Host name: `nuevo.tudominio.com` (para subdominio) o `tudominio.com` (para dominio principal)
- [ ] Asignar Application Pool creado anteriormente
- [ ] Verificar que el sitio está iniciado

### Permisos
- [ ] Verificar permisos de lectura en la carpeta del proyecto
- [ ] Verificar permisos de escritura en `App_Data` (si existe)
- [ ] Verificar permisos del Application Pool Identity

---

## 📤 Subida de Archivos

### Método de Subida
- [ ] Elegir método: File Manager de cPanel o FTP
- [ ] Conectar al servidor
- [ ] Navegar a la carpeta correcta (subdominio o dominio principal)

### Archivos a Subir
- [ ] Todos los archivos de la carpeta de publicación
- [ ] Verificar que la estructura de carpetas se mantiene
- [ ] Verificar que no se subieron archivos innecesarios (`.cs`, `.csproj`, etc.)

### Verificación Post-Subida
- [ ] Verificar que todos los archivos se subieron correctamente
- [ ] Verificar tamaños de archivos (no deben ser 0 bytes)
- [ ] Verificar estructura de carpetas

---

## 🔧 Configuración Post-Despliegue

### Web.config
- [ ] Verificar que `debug="false"`
- [ ] Verificar cadena de conexión correcta
- [ ] Verificar configuraciones de producción
- [ ] Verificar que `customErrors` está configurado apropiadamente

### Pruebas Iniciales
- [ ] Acceder al sitio desde el navegador
- [ ] Verificar que la página principal carga
- [ ] Probar login (si aplica)
- [ ] Verificar que las imágenes cargan
- [ ] Verificar que los estilos CSS se aplican
- [ ] Verificar que JavaScript funciona
- [ ] Probar funcionalidades principales

### Pruebas de Base de Datos
- [ ] Verificar que las consultas a la base de datos funcionan
- [ ] Probar creación de registros
- [ ] Probar lectura de datos
- [ ] Probar actualización de datos
- [ ] Verificar que los logs se generan correctamente (si aplica)

---

## 🔒 Seguridad

### Configuración de Seguridad
- [ ] Verificar que HTTPS está configurado (si aplica)
- [ ] Verificar que las contraseñas no están en texto plano
- [ ] Verificar que los archivos sensibles no son accesibles públicamente
- [ ] Verificar configuración de autenticación/autorización
- [ ] Revisar permisos de archivos y carpetas

### SSL/HTTPS (Recomendado)
- [ ] Certificado SSL instalado
- [ ] Redirección HTTP a HTTPS configurada
- [ ] Verificar que el certificado es válido

---

## 📊 Monitoreo

### Logs
- [ ] Verificar que los logs se generan correctamente
- [ ] Configurar monitoreo de errores
- [ ] Revisar logs de IIS
- [ ] Revisar Event Viewer de Windows (si es necesario)

### Rendimiento
- [ ] Verificar tiempos de carga
- [ ] Verificar uso de memoria
- [ ] Verificar uso de CPU
- [ ] Configurar alertas si es necesario

---

## 🔄 Migración al Dominio Principal

### Antes de Migrar
- [ ] Probar exhaustivamente en el subdominio
- [ ] Obtener aprobación de stakeholders
- [ ] Hacer backup completo del sitio actual
- [ ] Hacer backup de la base de datos actual
- [ ] Planificar ventana de mantenimiento (si es necesario)

### Proceso de Migración
- [ ] Subir archivos al directorio del dominio principal
- [ ] Actualizar configuración si es necesario
- [ ] Probar en el dominio principal
- [ ] Verificar que todo funciona correctamente

### Post-Migración
- [ ] Monitorear el sitio durante las primeras 24-48 horas
- [ ] Revisar logs de errores
- [ ] Verificar que los usuarios pueden acceder
- [ ] Verificar que todas las funcionalidades trabajan
- [ ] Mantener el subdominio activo por un tiempo (por si acaso)

---

## 📝 Documentación

### Documentar
- [ ] Credenciales de acceso (guardar de forma segura)
- [ ] Configuraciones específicas del servidor
- [ ] Cambios realizados durante el despliegue
- [ ] Problemas encontrados y soluciones aplicadas
- [ ] Contactos del proveedor de hosting

---

## 🆘 Plan de Rollback

### Si algo sale mal
- [ ] Tener backup del sitio anterior listo
- [ ] Tener backup de la base de datos anterior
- [ ] Saber cómo restaurar rápidamente
- [ ] Tener contacto del proveedor de hosting disponible
- [ ] Documentar el proceso de rollback

---

## ✅ Verificación Final

Antes de considerar el despliegue completo:

- [ ] El sitio carga correctamente
- [ ] Todas las funcionalidades principales trabajan
- [ ] La base de datos funciona correctamente
- [ ] No hay errores en los logs
- [ ] El rendimiento es aceptable
- [ ] Los usuarios pueden acceder y usar el sistema
- [ ] El sitio es seguro (HTTPS, permisos, etc.)

---

## 📞 Contactos de Emergencia

- **Proveedor de Hosting**: _______________________
- **Teléfono**: _______________________
- **Email**: _______________________
- **Desarrollador**: _______________________
- **Administrador de Base de Datos**: _______________________

---

**Fecha de Despliegue**: _______________________
**Responsable**: _______________________
**Versión Desplegada**: _______________________









