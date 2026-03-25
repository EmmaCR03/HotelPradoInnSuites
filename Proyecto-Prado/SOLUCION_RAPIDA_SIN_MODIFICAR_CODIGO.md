# 🚀 Solución Rápida: Desplegar .NET Framework SIN Modificar Código

## ✅ BUENAS NOTICIAS

**Tu código .NET Framework funciona perfectamente tal cual está.** Solo necesitas un servidor Windows con IIS. No necesitas cambiar NADA de tu código.

---

## 🎯 OPCIONES (De Más Rápida a Más Lenta)

### Opción 1: Azure App Service (Windows) ⚡ **RECOMENDADO**

**Tiempo:** 2-4 horas (no semanas, HORAS)

**Ventajas:**
- ✅ Despliegue en horas, no semanas
- ✅ Tu código funciona tal cual (cero cambios)
- ✅ Plan gratuito disponible para empezar
- ✅ SQL Server incluido (Azure SQL)
- ✅ SSL gratuito
- ✅ Escalable
- ✅ Muy confiable

**Pasos rápidos:**
1. Crear cuenta Azure (gratis, $200 crédito por 30 días)
2. Crear App Service (Windows)
3. Crear SQL Database
4. Publicar desde Visual Studio (1 clic)
5. Listo

**Costo:**
- **Gratis:** Plan F1 (limitado, para testing)
- **Básico:** ~$13/mes (B1)
- **Estándar:** ~$50/mes (S1) - recomendado para producción

**Tiempo real:** 2-4 horas si sigues los pasos

---

### Opción 2: Hosting Windows Tradicional

**Tiempo:** 1-3 días (depende del proveedor)

**Ventajas:**
- ✅ Tu código funciona tal cual
- ✅ Soporte técnico incluido
- ✅ Más control

**Desventajas:**
- ⚠️ Puede ser más lento de configurar
- ⚠️ Menos flexible que Azure

**Proveedores sugeridos:**
- SmarterASP.NET (~$3-10/mes)
- DiscountASP.NET (~$10-20/mes)
- GoDaddy Windows Hosting (~$10-30/mes)

**Tiempo real:** 1-3 días

---

### Opción 3: VPS Windows

**Tiempo:** 1-2 días

**Ventajas:**
- ✅ Control total
- ✅ Tu código funciona tal cual

**Desventajas:**
- ⚠️ Necesitas configurar todo tú mismo
- ⚠️ Más trabajo inicial

**Proveedores:**
- Azure VM (Windows Server)
- AWS EC2 (Windows)
- DigitalOcean (Windows)

**Tiempo real:** 1-2 días (si sabes configurar)

---

## 🚀 GUÍA RÁPIDA: Azure App Service (La Más Rápida)

### Paso 1: Crear Cuenta Azure (5 minutos)

1. Ve a: https://azure.microsoft.com/es-es/free/
2. Crea cuenta gratuita (necesitas tarjeta, pero no te cobran si usas plan gratuito)
3. Obtienes $200 crédito por 30 días

### Paso 2: Crear App Service (10 minutos)

1. En Azure Portal, busca "App Services"
2. Click "Create" o "Crear"
3. Configuración:
   - **Subscription:** Tu suscripción
   - **Resource Group:** Crear nuevo (ej: "HotelPrado")
   - **Name:** `hotelprado` o `pradoinn-app` (debe ser único)
   - **Publish:** Code
   - **Runtime stack:** .NET Framework 4.8
   - **Operating System:** Windows ✅
   - **Region:** El más cercano a tus usuarios
   - **App Service Plan:** 
     - Crear nuevo
     - **Sku and size:** F1 (Free) para empezar, o B1 (Basic) para producción
4. Click "Review + Create" → "Create"

### Paso 3: Crear SQL Database (10 minutos)

1. En Azure Portal, busca "SQL databases"
2. Click "Create"
3. Configuración:
   - **Resource Group:** Mismo que App Service
   - **Database name:** `HotelPrado`
   - **Server:** Crear nuevo
     - **Server name:** `hotelprado-sql` (único)
     - **Location:** Mismo que App Service
     - **Admin login:** `sqladmin` (o el que quieras)
     - **Password:** (guárdala bien)
   - **Compute + storage:** Basic ($5/mes) o S0 ($15/mes)
4. Click "Review + Create" → "Create"

### Paso 4: Configurar Connection String (5 minutos)

1. Ve a tu App Service
2. En el menú izquierdo: "Configuration" → "Connection strings"
3. Click "New connection string"
4. **Name:** `Contexto` (igual que en tu Web.config)
5. **Value:** 
   ```
   Server=tcp:hotelprado-sql.database.windows.net,1433;Initial Catalog=HotelPrado;Persist Security Info=False;User ID=sqladmin;Password=TU_PASSWORD;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
   ```
   (Reemplaza `sqladmin` y `TU_PASSWORD` con tus valores)
6. Click "OK" → "Save"

### Paso 5: Publicar desde Visual Studio (15 minutos)

1. Abre tu proyecto en Visual Studio
2. Click derecho en `HotelPrado.UI` → "Publish"
3. Selecciona "Azure" → "Azure App Service (Windows)"
4. Selecciona tu App Service creado
5. Click "Publish"
6. Visual Studio publicará automáticamente

### Paso 6: Configurar Base de Datos (30 minutos)

1. En Azure Portal, ve a tu SQL Database
2. Click "Query editor" (o usa SQL Server Management Studio)
3. Ejecuta tus scripts de creación de base de datos
4. O usa Entity Framework Migrations si los tienes

### Paso 7: Configurar Dominio (Opcional, 10 minutos)

1. En App Service → "Custom domains"
2. Agrega tu dominio `pradoinn.com`
3. Configura DNS en tu proveedor actual
4. Azure te dará las instrucciones

**TOTAL: 2-4 horas** ⚡

---

## 💰 COSTOS REALES

### Plan Gratuito (Azure F1):
- **Costo:** $0/mes
- **Limitaciones:** 
  - 1 GB de almacenamiento
  - 60 minutos de CPU/día
  - Solo HTTP (no HTTPS personalizado)
  - **Perfecto para:** Testing

### Plan Básico (Azure B1):
- **Costo:** ~$13/mes
- **Incluye:**
  - 10 GB almacenamiento
  - CPU dedicado
  - HTTPS personalizado
  - **Perfecto para:** Producción pequeña-mediana

### SQL Database Basic:
- **Costo:** ~$5/mes
- **Incluye:** 2 GB de base de datos

### SQL Database S0:
- **Costo:** ~$15/mes
- **Incluye:** 10 GB de base de datos

**Total recomendado para producción:** ~$18-28/mes

---

## 🎯 CONFIGURACIÓN DE WEB.CONFIG PARA AZURE

Tu `Web.config` necesita un pequeño ajuste para la connection string. Azure la inyecta automáticamente si la nombras igual.

**Opción 1: Usar Connection String de Azure (Recomendado)**

En Azure Portal → App Service → Configuration → Connection strings:
- **Name:** `Contexto`
- **Value:** (la cadena de conexión a Azure SQL)

En tu `Web.config`, puedes dejar la connection string vacía o usar:

```xml
<connectionStrings>
  <add name="Contexto" 
       connectionString=""
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

Azure la reemplazará automáticamente.

**Opción 2: Hardcodear (No recomendado, pero funciona)**

```xml
<connectionStrings>
  <add name="Contexto" 
       connectionString="Server=tcp:TU_SERVIDOR.database.windows.net,1433;Initial Catalog=HotelPrado;User ID=usuario;Password=contraseña;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;" 
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

---

## ✅ CHECKLIST RÁPIDO

### Antes de Publicar:
- [ ] Proyecto compilado en modo **Release**
- [ ] `Web.config` con `debug="false"`
- [ ] Connection string preparada (o dejar que Azure la inyecte)
- [ ] Cuenta Azure creada

### Durante la Publicación:
- [ ] App Service creado (Windows, .NET Framework 4.8)
- [ ] SQL Database creada
- [ ] Connection string configurada en Azure
- [ ] Proyecto publicado desde Visual Studio

### Después de Publicar:
- [ ] Base de datos creada y migrada
- [ ] Sitio accesible
- [ ] Probar login y funcionalidades principales
- [ ] Configurar dominio personalizado (opcional)

---

## 🆘 SI ALGO SALE MAL

### Error: "Cannot connect to database"
- Verifica que el firewall de SQL Database permite conexiones de Azure
- En SQL Database → "Set server firewall" → Agrega regla para Azure services

### Error: "Application failed to start"
- Revisa los logs en App Service → "Log stream"
- Verifica que todas las DLLs se publicaron
- Verifica que el Application Pool está configurado para .NET Framework 4.8

### Error: "Connection string not found"
- Verifica que la connection string se llama exactamente `Contexto`
- Verifica que está configurada en App Service → Configuration

---

## 🎯 RECOMENDACIÓN FINAL

**Para tu situación:**
- ✅ Tienes código .NET Framework funcionando
- ✅ No quieres modificar código
- ✅ Quieres desplegar rápido
- ✅ Tienes sitio actual en PHP en Linux

**Solución:**
1. **Azure App Service (Windows)** - 2-4 horas
2. Mantener tu sitio actual en `pradoinn.com` (PHP)
3. Desplegar nueva app en subdominio: `app.pradoinn.com` o `nuevo.pradoinn.com`
4. Cuando esté probado, cambiar DNS del dominio principal

**Ventajas:**
- ✅ Cero cambios en tu código
- ✅ Despliegue en horas
- ✅ Puedes probar sin afectar el sitio actual
- ✅ Costo razonable ($18-28/mes)

---

## 📞 ¿NECESITAS AYUDA?

Si quieres, puedo:
1. Crear una guía paso a paso más detallada
2. Ayudarte a configurar el Web.config para Azure
3. Guiarte en la publicación desde Visual Studio
4. Ayudarte a configurar la base de datos

**¿Empezamos con Azure?** Es la opción más rápida y tu código funciona tal cual. 🚀







