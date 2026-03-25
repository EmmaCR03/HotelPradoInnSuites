# 🔍 Guía de Verificación de cPanel - Hotel Prado

## ⚠️ VERIFICACIÓN CRÍTICA: ¿Windows o Linux?

Tu aplicación ASP.NET MVC **SOLO funciona en Windows Server con IIS**. Necesitas verificar esto PRIMERO.

### Paso 1: Verificar el Tipo de Servidor

1. En tu cPanel, busca en la sección **"Información general"** (General Information) en el panel derecho
2. Busca información sobre el sistema operativo del servidor
3. **Alternativamente**, busca en el menú izquierdo:
   - Busca "**Select PHP Version**" o "**PHP Version**"
   - Si encuentras esto, probablemente es un servidor **Linux** (no compatible con .NET Framework)
   - Si NO encuentras PHP pero encuentras herramientas de IIS o .NET, es **Windows**

### Paso 2: Buscar Herramientas de .NET/IIS

En el menú izquierdo o en la búsqueda de herramientas, busca:

**Si encuentras estas opciones, es Windows (✅ Compatible):**
- "IIS" o "Internet Information Services"
- ".NET" o ".NET Framework"
- "Application Pools"
- "Websites" o "Sites"

**Si encuentras estas opciones, es Linux (❌ NO Compatible):**
- "Select PHP Version"
- "Apache Modules"
- "Ruby Version Manager"
- "Python Version Manager"

---

## 📋 Checklist de Verificación en cPanel

### ✅ 1. Verificar Información del Servidor

**Ubicación:** Panel derecho "Información general"

Verifica:
- [ ] **Usuario Actual:** `pradoinn` ✅ (ya lo tienes)
- [ ] **Dominio Principal:** `pradoinn.com` ✅ (ya lo tienes)
- [ ] **Directorio Principal:** `/home/pradoinn` ✅ (ya lo tienes)
- [ ] **IP Compartida:** `212.56.34.147` ✅ (ya lo tienes)

**Busca también:**
- [ ] Información sobre el sistema operativo del servidor
- [ ] Versión de .NET Framework disponible (si es Windows)
- [ ] Versión de IIS (si es Windows)

---

### ✅ 2. Verificar Herramientas Disponibles

**Ubicación:** Menú izquierdo o búsqueda de herramientas

#### Herramientas Necesarias para .NET:

**Busca estas secciones en el menú:**

1. **Subdominios (Subdomains)**
   - [ ] ¿Existe la opción "Subdominios" o "Subdomains"?
   - **Ubicación probable:** "Dominios" (Domains) > "Subdominios"
   - **Necesario para:** Crear `nuevo.pradoinn.com` o `v2.pradoinn.com`

2. **Administrador de Archivos (File Manager)**
   - [ ] ¿Existe "File Manager" o "Administrador de archivos"?
   - **Necesario para:** Subir archivos del proyecto

3. **Bases de Datos (Databases)**
   - [ ] ¿Existe "MySQL Databases" o "SQL Server"?
   - **Necesario para:** Crear/configurar la base de datos
   - **IMPORTANTE:** Si solo ves "MySQL", probablemente es Linux
   - **Necesitas:** "SQL Server" o "Microsoft SQL Server"

4. **IIS o Configuración de Sitios Web**
   - [ ] ¿Existe alguna opción relacionada con IIS?
   - [ ] ¿Existe "Application Pools"?
   - [ ] ¿Existe "Websites" o "Sites"?

---

### ✅ 3. Verificar Acceso a Subdominios

**Pasos para verificar:**

1. En el menú izquierdo, busca **"Dominios"** (Domains) o **"Subdominios"** (Subdomains)
2. Haz clic en esa opción
3. Verifica si puedes:
   - [ ] Ver una lista de subdominios existentes
   - [ ] Crear un nuevo subdominio
   - [ ] Configurar la carpeta de destino del subdominio

**Si puedes crear subdominios:**
- ✅ Puedes crear `nuevo.pradoinn.com` para testing
- ✅ Puedes crear `v2.pradoinn.com` para la nueva versión

---

### ✅ 4. Verificar Acceso a Base de Datos

**Pasos para verificar:**

1. En el menú izquierdo, busca **"Bases de datos"** (Databases)
2. Haz clic en esa opción
3. Verifica qué tipos de bases de datos están disponibles:

**Opciones posibles:**
- [ ] **MySQL Databases** - ❌ NO compatible con tu proyecto (.NET Framework usa SQL Server)
- [ ] **SQL Server** o **Microsoft SQL Server** - ✅ Compatible
- [ ] **PostgreSQL** - ❌ NO compatible con tu proyecto
- [ ] **Remote MySQL** - ❌ NO compatible

**Si solo ves MySQL:**
- ⚠️ Tu servidor probablemente es Linux
- ⚠️ Necesitarás un servidor Windows con SQL Server
- ⚠️ Contacta a tu proveedor de hosting

---

### ✅ 5. Verificar Estructura de Directorios

**Pasos para verificar:**

1. Abre **"File Manager"** o **"Administrador de archivos"**
2. Navega a la carpeta principal: `/home/pradoinn`
3. Verifica la estructura:

**Estructura típica en cPanel:**
```
/home/pradoinn/
├── public_html/          ← Archivos del sitio principal (pradoinn.com)
├── public_html/nuevo/   ← Aquí subirías el subdominio (si lo creas)
├── mail/
├── tmp/
└── ...
```

**Verifica:**
- [ ] ¿Existe la carpeta `public_html`?
- [ ] ¿Puedes crear carpetas nuevas?
- [ ] ¿Puedes subir archivos?

---

### ✅ 6. Verificar Permisos y Configuración

**En File Manager, verifica:**

1. Haz clic derecho en una carpeta (ej: `public_html`)
2. Busca la opción **"Cambiar permisos"** o **"Change Permissions"**
3. Verifica que puedes:
   - [ ] Ver permisos de archivos/carpetas
   - [ ] Modificar permisos (si es necesario)

**Permisos típicos necesarios:**
- Carpetas: `755` o `775`
- Archivos: `644` o `664`
- Carpeta `App_Data` (si existe): Necesita escritura

---

## 🔍 Cómo Verificar el Tipo de Servidor (Método Alternativo)

### Método 1: Contactar al Proveedor

**Pregunta directa a tu proveedor de hosting:**

```
"¿Mi cuenta de hosting está en un servidor Windows o Linux?
¿Tienen soporte para aplicaciones ASP.NET (.NET Framework 4.8.1)?
¿Tienen SQL Server disponible?
¿Puedo desplegar aplicaciones que requieren IIS?"
```

### Método 2: Revisar Documentación del Hosting

1. Busca en el sitio web de tu proveedor de hosting
2. Revisa las características de tu plan
3. Busca información sobre:
   - Sistema operativo del servidor
   - Soporte para .NET
   - Bases de datos disponibles

### Método 3: Probar Crear un Archivo de Prueba

1. En File Manager, crea un archivo llamado `test.aspx` en `public_html`
2. Contenido del archivo:
```aspx
<%@ Page Language="C#" %>
<!DOCTYPE html>
<html>
<head>
    <title>Test .NET</title>
</head>
<body>
    <h1>Si ves esto, .NET funciona!</h1>
    <p>Versión: <%= System.Environment.Version %></p>
</body>
</html>
```
3. Intenta acceder a: `https://pradoinn.com/test.aspx`
4. **Si funciona:** ✅ Tienes soporte para .NET
5. **Si no funciona o da error 404/500:** ❌ Probablemente no tienes soporte para .NET

---

## 📞 Información para Contactar a tu Proveedor

Basado en tu cPanel, tu información es:

- **Dominio:** `pradoinn.com`
- **Usuario:** `pradoinn`
- **IP Compartida:** `212.56.34.147`
- **Directorio Principal:** `/home/pradoinn`

**Preguntas específicas para hacer:**

1. "¿Mi servidor es Windows o Linux?"
2. "¿Tienen soporte para ASP.NET MVC con .NET Framework 4.8.1?"
3. "¿Tienen SQL Server disponible o solo MySQL?"
4. "¿Puedo crear subdominios para testing?"
5. "¿Tienen acceso a configuración de IIS o Application Pools?"
6. "¿Qué versión de .NET Framework está disponible?"

---

## ⚠️ Si tu Servidor es Linux

**Opciones disponibles:**

### Opción 1: Migrar a Hosting Windows
- Buscar un proveedor que ofrezca hosting Windows
- Migrar el dominio
- Desplegar la aplicación ASP.NET

### Opción 2: Usar Servicio de Base de Datos Remota
- Mantener el hosting actual para archivos estáticos
- Usar SQL Server en la nube (Azure SQL, AWS RDS, etc.)
- **PERO:** Aún necesitarías un servidor Windows para la aplicación

### Opción 3: Migrar a .NET Core (Largo Plazo)
- .NET Core funciona en Linux
- Requiere reescribir/adaptar el proyecto
- No es una solución rápida

---

## ✅ Próximos Pasos Según el Resultado

### Si tienes Windows Server con IIS:
1. ✅ Procede con la guía de despliegue
2. ✅ Crea el subdominio para testing
3. ✅ Configura la base de datos SQL Server
4. ✅ Despliega la aplicación

### Si tienes Linux:
1. ⚠️ Contacta a tu proveedor para opciones
2. ⚠️ Considera migrar a hosting Windows
3. ⚠️ O evalúa migrar el proyecto a .NET Core (proyecto grande)

---

## 📝 Notas de tu cPanel Actual

Basado en la captura de pantalla que compartiste:

- ✅ Tienes acceso a cPanel
- ✅ Dominio principal: `pradoinn.com`
- ✅ SSL activo
- ✅ Usuario: `pradoinn`
- ❓ **Falta verificar:** Tipo de servidor (Windows/Linux)
- ❓ **Falta verificar:** Soporte para .NET Framework
- ❓ **Falta verificar:** Disponibilidad de SQL Server

**Acción inmediata:** Busca en el menú izquierdo de tu cPanel las opciones mencionadas arriba y verifica qué herramientas tienes disponibles.









