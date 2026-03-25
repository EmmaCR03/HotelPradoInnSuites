# 🔍 Dónde Buscar en tu cPanel - Guía Específica

## ✅ Lo que YA SABEMOS de tu cPanel:

- ✅ **Subdominios:** Puedes crear subdominios ilimitados (0 / ∞)
- ✅ **Bases de datos:** Puedes crear 2 bases de datos (0 / 2)
- ✅ **File Manager:** Debe estar disponible
- ❓ **Tipo de servidor:** Necesitamos verificar (Windows/Linux)
- ❓ **Tipo de base de datos:** Necesitamos verificar (MySQL/SQL Server)

---

## 🎯 PASOS ESPECÍFICOS PARA TU CPANEL

### Paso 1: Encontrar "Subdominios" (Para crear el subdominio de prueba)

**Busca en el menú izquierdo:**

1. Busca la sección **"Dominios"** (Domains)
   - Puede estar agrupada con otras opciones
   - Puede tener un icono de globo o dominio

2. **O usa la barra de búsqueda:**
   - Escribe: `subdomain` o `subdominio`
   - Debería aparecer "Subdominios" o "Subdomains"

3. **O busca en "Herramientas" (Tools):**
   - En la sección principal donde estás ahora
   - Busca cualquier opción relacionada con dominios

**Cuando lo encuentres:**
- Haz clic en "Subdominios" o "Subdomains"
- Deberías ver un botón para "Crear subdominio" o "Create Subdomain"
- Esto te permitirá crear `nuevo.pradoinn.com` o `v2.pradoinn.com`

---

### Paso 2: Verificar Tipo de Base de Datos (CRÍTICO)

**Busca en el menú izquierdo:**

1. Busca la sección **"Bases de datos"** (Databases) o **"MySQL Databases"**
   - Puede estar en el menú principal
   - Puede tener un icono de base de datos

2. **O usa la barra de búsqueda:**
   - Escribe: `database` o `mysql` o `sql`
   - Debería aparecer algo relacionado con bases de datos

3. **Cuando encuentres la sección de bases de datos:**
   - Haz clic en ella
   - **Mira el TÍTULO de la página:**
     - Si dice "**MySQL Databases**" → ❌ NO compatible (necesitas SQL Server)
     - Si dice "**SQL Server**" o "**Microsoft SQL Server**" → ✅ Compatible
     - Si dice solo "**Databases**" → Necesitas ver más detalles

4. **Si ves opciones para crear base de datos:**
   - ¿Qué tipo de base de datos puedes crear?
   - ¿Dice "MySQL" o "SQL Server"?
   - Toma una captura de pantalla si no estás seguro

---

### Paso 3: Encontrar File Manager

**Busca en el menú izquierdo:**

1. Busca **"File Manager"** o **"Administrador de archivos"**
   - Suele estar en la sección principal
   - Puede tener un icono de carpeta

2. **O usa la barra de búsqueda:**
   - Escribe: `file` o `manager`
   - Debería aparecer "File Manager"

**Cuando lo encuentres:**
- Haz clic para abrirlo
- Deberías ver la carpeta `/home/pradoinn`
- Deberías ver la carpeta `public_html` (esta es donde van los archivos del sitio)

---

### Paso 4: Verificar Tipo de Servidor (Métodos Alternativos)

#### Método A: Buscar en "Información del servidor"

1. En el panel derecho, busca **"Información del servidor"** (Server Information)
2. Haz clic en ese enlace
3. Busca información sobre:
   - Sistema operativo
   - Versión del servidor
   - Software instalado

#### Método B: Buscar herramientas específicas

**Busca estas palabras en la barra de búsqueda:**

1. Escribe: `PHP`
   - Si aparece "Select PHP Version" → Probablemente Linux
   - Si NO aparece nada → Puede ser Windows

2. Escribe: `IIS`
   - Si aparece algo → Es Windows ✅
   - Si NO aparece nada → Puede ser Linux

3. Escribe: `.NET` o `dotnet`
   - Si aparece algo → Es Windows con .NET ✅
   - Si NO aparece nada → No hay .NET disponible

#### Método C: Revisar todas las secciones del menú

**En el menú izquierdo, revisa TODAS las secciones visibles:**

- **Files** (Archivos)
- **Databases** (Bases de datos)
- **Domains** (Dominios)
- **Email** (Correo)
- **Metrics** (Métricas)
- **Security** (Seguridad)
- **Software** (Software)
- **Advanced** (Avanzado)
- **Preferences** (Preferencias)

**Busca en cada sección:**
- ¿Ves algo sobre "PHP"?
- ¿Ves algo sobre "IIS" o ".NET"?
- ¿Ves algo sobre "SQL Server"?

---

## 📸 Qué Hacer Si No Encuentras Nada

### Opción 1: Contactar al Proveedor Directamente

**Información que tienes:**
- Dominio: `pradoinn.com`
- Usuario: `pradoinn`
- IP: `212.56.34.147`

**Preguntas específicas para hacer:**

```
"Buenos días,

Tengo una cuenta de hosting en pradoinn.com (usuario: pradoinn) 
y necesito verificar lo siguiente:

1. ¿Mi servidor es Windows o Linux?
2. ¿Tienen soporte para aplicaciones ASP.NET MVC con .NET Framework 4.8.1?
3. ¿Qué tipo de bases de datos están disponibles? (¿Solo MySQL o también SQL Server?)
4. ¿Puedo desplegar aplicaciones que requieren IIS (Internet Information Services)?

Necesito esta información para desplegar una aplicación web desarrollada en ASP.NET MVC.

Gracias."
```

### Opción 2: Probar Crear un Archivo de Prueba

1. Abre **File Manager**
2. Ve a la carpeta `public_html`
3. Crea un archivo nuevo llamado `test.aspx`
4. Pega este contenido:

```aspx
<%@ Page Language="C#" %>
<!DOCTYPE html>
<html>
<head>
    <title>Test .NET</title>
</head>
<body>
    <h1>Test de .NET</h1>
    <p>Si ves esto funcionando, tienes soporte para .NET</p>
    <p>Versión: <%= System.Environment.Version %></p>
</body>
</html>
```

5. Guarda el archivo
6. Intenta acceder desde el navegador: `https://pradoinn.com/test.aspx`
7. **Resultados:**
   - Si funciona y muestra la versión → ✅ Tienes .NET
   - Si da error 404/500 o no carga → ❌ Probablemente no tienes .NET

---

## 🎯 PRIORIDAD: Encuentra Esto Primero

**Orden de prioridad para buscar:**

1. **🔥 CRÍTICO:** Tipo de base de datos (MySQL vs SQL Server)
   - Ve a "Bases de datos" o "Databases"
   - Mira qué tipo puedes crear

2. **🔥 CRÍTICO:** Tipo de servidor (Windows vs Linux)
   - Busca "PHP" o "IIS" en la búsqueda
   - O contacta al proveedor

3. **✅ IMPORTANTE:** Subdominios (ya sabemos que puedes crear)
   - Encuentra la opción para crearlos

4. **✅ IMPORTANTE:** File Manager
   - Para subir los archivos

---

## 💡 Tip: Explora el Menú Completo

**No te limites a la sección "Tools":**

1. Mira **TODAS las secciones** del menú izquierdo
2. Haz clic en cada sección para ver qué opciones tiene
3. Usa la **barra de búsqueda** para buscar palabras clave
4. Toma capturas de pantalla de lo que encuentres

---

## 📋 Checklist Rápido

Marca lo que encuentres:

- [ ] Encontré la sección "Subdominios" o "Subdomains"
- [ ] Encontré la sección "Bases de datos" o "Databases"
- [ ] Vi qué tipo de base de datos puedo crear (MySQL/SQL Server)
- [ ] Encontré "File Manager"
- [ ] Busqué "PHP" en la búsqueda → ¿Qué apareció?
- [ ] Busqué "IIS" en la búsqueda → ¿Qué apareció?
- [ ] Busqué ".NET" en la búsqueda → ¿Qué apareció?
- [ ] Revisé "Información del servidor" → ¿Qué información vi?

---

## 🚨 Si Todo Falla

**Contacta a tu proveedor de hosting con esta información:**

- **Dominio:** pradoinn.com
- **Usuario cPanel:** pradoinn
- **IP:** 212.56.34.147

**Pregunta directa:**
"¿Mi servidor soporta aplicaciones ASP.NET MVC que requieren .NET Framework 4.8.1, IIS y SQL Server?"

**Si la respuesta es NO:**
- Necesitarás migrar a un hosting Windows
- O considerar otras opciones (migrar a .NET Core, usar servicios en la nube, etc.)









