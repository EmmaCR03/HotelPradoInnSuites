# Análisis de Base de Datos - Migración Visual FoxPro a SQL Server

## 📊 Resumen Ejecutivo

Este documento compara la estructura de la base de datos **HotelPrado** (sistema antiguo) con la base de datos **DB** (sistema nuevo) para identificar diferencias y ajustes necesarios para la migración de datos.

---

## 🔍 Comparación de Tablas Principales

### 1. **CLIENTE** ✅ SIMILAR

| Campo | DB (Nuevo) | HotelPrado (Antiguo) | Estado |
|-------|------------|----------------------|--------|
| IdCliente | INT IDENTITY | INT IDENTITY | ✅ Igual |
| NombreCliente | VARCHAR(50) | VARCHAR(50) | ✅ Igual |
| PrimerApellidoCliente | VARCHAR(50) | VARCHAR(50) | ✅ Igual |
| SegundoApellidoCliente | VARCHAR(50) | VARCHAR(50) | ✅ Igual |
| EmailCliente | NVARCHAR(50) | NVARCHAR(50) | ✅ Igual |
| TelefonoCliente | INT | INT | ✅ Igual |
| DireccionCliente | NVARCHAR(100) | NVARCHAR(100) | ✅ Igual |

**✅ Esta tabla está bien - No requiere cambios**

---

### 2. **RESERVAS** ⚠️ DIFERENCIAS IMPORTANTES

| Campo | DB (Nuevo) | HotelPrado (Antiguo) | Estado |
|-------|------------|----------------------|--------|
| IdReserva | INT IDENTITY | INT IDENTITY | ✅ Igual |
| **IdCliente** | **NVARCHAR(128)** → AspNetUsers | **INT** → Cliente | ⚠️ **DIFERENTE** |
| IdHabitacion | INT | INT | ✅ Igual |
| FechaInicio | DATETIME | DATETIME | ✅ Igual |
| FechaFinal | DATETIME | DATETIME | ✅ Igual |
| EstadoReserva | NVARCHAR(50) | VARCHAR(15) | ⚠️ Tamaño diferente |
| MontoTotal | DECIMAL(18,2) | DECIMAL(10,2) | ⚠️ Precisión diferente |
| NombreCliente | NVARCHAR(128) | ❌ No existe | ⚠️ **FALTA EN ANTIGUO** |
| cantidadPersonas | INT | ❌ No existe | ⚠️ **FALTA EN ANTIGUO** |
| NumeroEmpresa | VARCHAR(50) | ❌ No existe | ⚠️ **FALTA EN ANTIGUO** |
| CorreoEmpresa | VARCHAR(50) | ❌ No existe | ⚠️ **FALTA EN ANTIGUO** |
| NumeroHabitacion | ❌ No existe | ❌ No existe | ✅ Igual |

**⚠️ PROBLEMA CRÍTICO:**
- En **DB**: `IdCliente` es `NVARCHAR(128)` y referencia `AspNetUsers` (sistema de autenticación)
- En **HotelPrado**: `IdCliente` es `INT` y referencia `Cliente`
- **Esto significa que el sistema nuevo usa autenticación ASP.NET Identity, pero el antiguo no**

---

### 3. **HABITACIONES** ⚠️ DIFERENCIAS IMPORTANTES

| Campo | DB (Nuevo) | HotelPrado (Antiguo) | Estado |
|-------|------------|----------------------|--------|
| IdHabitacion | INT IDENTITY | INT IDENTITY | ✅ Igual |
| NumeroHabitacion | VARCHAR(5) | VARCHAR(5) | ✅ Igual |
| Estado | VARCHAR(15) | VARCHAR(15) | ✅ Igual |
| **PrecioPorNoche** | ❌ No existe | **VARCHAR(50)** | ⚠️ **FALTA EN NUEVO** |
| **IdTipoHabitacion** | ❌ No existe | **INT** → TipoHabitacion | ⚠️ **FALTA EN NUEVO** |
| CapacidadMin | INT | ❌ No existe | ⚠️ **FALTA EN ANTIGUO** |
| CapacidadMax | INT | ❌ No existe | ⚠️ **FALTA EN ANTIGUO** |
| PrecioPorNoche1P | DECIMAL(18) | ❌ No existe | ⚠️ **FALTA EN ANTIGUO** |
| PrecioPorNoche2P | DECIMAL(18) | ❌ No existe | ⚠️ **FALTA EN ANTIGUO** |
| PrecioPorNoche3P | DECIMAL(18) | ❌ No existe | ⚠️ **FALTA EN ANTIGUO** |
| PrecioPorNoche4P | DECIMAL(18) | ❌ No existe | ⚠️ **FALTA EN ANTIGUO** |
| UrlImagenes | VARCHAR(MAX) | ❌ No existe | ⚠️ **FALTA EN ANTIGUO** |

**⚠️ PROBLEMA CRÍTICO:**
- El sistema **nuevo (DB)** tiene precios por número de personas (1P, 2P, 3P, 4P)
- El sistema **antiguo (HotelPrado)** tiene un solo precio y referencia a `TipoHabitacion`
- **Necesitas decidir qué estructura usar**

---

### 4. **DEPARTAMENTO** ✅ SIMILAR (con pequeñas diferencias)

| Campo | DB (Nuevo) | HotelPrado (Antiguo) | Estado |
|-------|------------|----------------------|--------|
| IdDepartamento | INT IDENTITY | INT IDENTITY | ✅ Igual |
| Nombre | VARCHAR(50) | VARCHAR(50) | ✅ Igual |
| IdTipoDepartamento | INT | INT | ✅ Igual |
| Precio | DECIMAL(10,2) | DECIMAL(10,2) | ✅ Igual |
| Estado | NVARCHAR(50) | NVARCHAR(50) | ✅ Igual |
| IdCliente | INT | INT | ✅ Igual |
| Descripcion | VARCHAR(255) | VARCHAR(255) | ✅ Igual |
| UrlImagenes | VARCHAR(MAX) | VARCHAR(MAX) | ✅ Igual |
| NumeroDepartamento | INT | INT | ✅ Igual |
| NumeroEmpresa | VARCHAR(50) | VARCHAR(50) | ✅ Igual |
| CorreoEmpresa | VARCHAR(50) | VARCHAR(50) | ✅ Igual |

**✅ Esta tabla está bien - No requiere cambios**

---

### 5. **TIPO HABITACION** ⚠️ FALTA EN DB

| Campo | HotelPrado (Antiguo) | DB (Nuevo) | Estado |
|-------|----------------------|------------|--------|
| IdTipoHabitacion | INT IDENTITY | ❌ **NO EXISTE** | ⚠️ **FALTA** |
| Descripcion | VARCHAR(25) | ❌ No existe | ⚠️ **FALTA** |
| CapacidadDePersonas | INT | ❌ No existe | ⚠️ **FALTA** |

**⚠️ PROBLEMA:**
- El sistema antiguo tiene una tabla `TipoHabitacion` que no existe en el sistema nuevo
- Las habitaciones en el sistema antiguo referencian esta tabla

---

### 6. **COLABORADOR** ✅ IDÉNTICA

| Campo | DB (Nuevo) | HotelPrado (Antiguo) | Estado |
|-------|------------|----------------------|--------|
| IdColaborador | INT IDENTITY | INT IDENTITY | ✅ Igual |
| NombreColaborador | VARCHAR(100) | VARCHAR(100) | ✅ Igual |
| PrimerApellidoColaborador | VARCHAR(100) | VARCHAR(100) | ✅ Igual |
| SegundoApellidoColaborador | VARCHAR(100) | VARCHAR(100) | ✅ Igual |
| CedulaColaborador | INT | INT | ✅ Igual |
| PuestoColaborador | VARCHAR(100) | VARCHAR(100) | ✅ Igual |
| EstadoLaboral | VARCHAR(50) | VARCHAR(50) | ✅ Igual |
| IngresoColaborador | INT | INT | ✅ Igual |

**✅ Esta tabla está bien - No requiere cambios**

---

### 7. **CITAS** ✅ SIMILAR

| Campo | DB (Nuevo) | HotelPrado (Antiguo) | Estado |
|-------|------------|----------------------|--------|
| IdCita | INT IDENTITY | INT IDENTITY | ✅ Igual |
| Nombre | VARCHAR(100) | VARCHAR(100) | ✅ Igual |
| PrimerApellido | VARCHAR(100) | VARCHAR(100) | ✅ Igual |
| SegundoApellido | VARCHAR(100) | VARCHAR(100) | ✅ Igual |
| Telefono | INT | INT | ✅ Igual |
| Correo | VARCHAR(150) | VARCHAR(150) | ✅ Igual |
| MedioContacto | VARCHAR(50) | VARCHAR(50) | ✅ Igual |
| IdDepartamento | INT | INT | ✅ Igual |
| FechaHoraInicio | DATETIME | DATETIME | ✅ Igual |
| FechaHoraFin | DATETIME | DATETIME | ✅ Igual |
| Estado | VARCHAR(50) | VARCHAR(50) | ✅ Igual |
| Observaciones | TEXT | TEXT | ✅ Igual |
| FechaCreacion | DATETIME | DATETIME | ✅ Igual |
| IdColaborador | INT | INT | ✅ Igual |
| EnlaceWhatsApp | VARCHAR(255) | VARCHAR(255) | ✅ Igual |
| EnlaceCorreo | VARCHAR(255) | VARCHAR(255) | ✅ Igual |

**✅ Esta tabla está bien - No requiere cambios**

---

## 🚨 PROBLEMAS CRÍTICOS IDENTIFICADOS

### 1. **Sistema de Autenticación**
- **DB (Nuevo)**: Usa `AspNetUsers` (ASP.NET Identity) - `IdCliente` es `NVARCHAR(128)`
- **HotelPrado (Antiguo)**: Usa tabla `Cliente` simple - `IdCliente` es `INT`
- **Solución necesaria**: Decidir si mantener autenticación o simplificar

### 2. **Estructura de Habitaciones**
- **DB (Nuevo)**: Precios por número de personas (1P, 2P, 3P, 4P) + CapacidadMin/Max
- **HotelPrado (Antiguo)**: Un solo precio + referencia a `TipoHabitacion`
- **Solución necesaria**: Unificar la estructura

### 3. **Tabla TipoHabitacion**
- Existe en **HotelPrado** pero no en **DB**
- Necesitas decidir si agregarla o adaptar los datos

---

## ✅ RECOMENDACIONES

### Opción A: Adaptar DB para usar HotelPrado (Recomendado si quieres mantener datos antiguos)

1. **Modificar Reservas**:
   - Cambiar `IdCliente` de `NVARCHAR(128)` a `INT`
   - Cambiar referencia de `AspNetUsers` a `Cliente`
   - Agregar campos faltantes si son necesarios

2. **Modificar Habitaciones**:
   - Agregar tabla `TipoHabitacion`
   - Agregar campo `IdTipoHabitacion` a `Habitaciones`
   - Decidir si mantener precios por persona o usar precio único

3. **Migrar datos de Visual FoxPro**:
   - Exportar datos desde Visual FoxPro
   - Mapear campos correctamente
   - Importar a SQL Server

### Opción B: Adaptar HotelPrado para usar DB (Si quieres mantener funcionalidades nuevas)

1. Agregar campos faltantes a HotelPrado
2. Modificar estructura de Habitaciones
3. Adaptar código de aplicación

---

## 📝 PRÓXIMOS PASOS

1. **Confirmar qué datos tienes del sistema antiguo (Visual FoxPro)**
2. **Decidir qué estructura final usar** (DB o HotelPrado)
3. **Crear script de migración de datos**
4. **Validar que todos los datos se migren correctamente**

---

## ❓ PREGUNTAS PARA TI

1. ¿Tienes acceso a los datos del sistema Visual FoxPro? ¿En qué formato?
2. ¿Prefieres mantener la estructura de DB (nuevo) o HotelPrado (antiguo)?
3. ¿Necesitas mantener el sistema de autenticación ASP.NET Identity o puedes simplificarlo?
4. ¿Cómo manejas los precios de habitaciones en el sistema antiguo? ¿Por tipo o por número de personas?






