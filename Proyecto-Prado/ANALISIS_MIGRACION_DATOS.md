# 📊 Análisis de Migración de Datos - Base de Datos Vieja del Hotel

## 🎯 Resumen Ejecutivo

Se ha identificado y analizado **20 tablas importantes** de la base de datos vieja del hotel (Visual FoxPro). Este documento resume qué datos están disponibles y qué se puede migrar a la nueva base de datos SQL Server.

---

## 📋 Tablas Identificadas por Categoría

### 👥 **CLIENTES/HUÉSPEDES**

#### 1. **EXTRAS_AUT.dbf** ⭐ (RECOMENDADO PARA MIGRAR)
- **Registros:** 135,865
- **Campos principales:**
  - `A_NOMBRE_D` (Texto, 50) - Nombre del cliente
  - `TELEFONO` (Numérico, 11) - Teléfono
  - `DIRECCION` (Texto, 90) - Dirección
  - `CEDULA` (Texto, 16) - Cédula
  - `EMAIL` (Texto, 100) - Email
  - `COD_EMPR` (Numérico, 4) - Código de empresa
  - `NOM_EMPR` (Texto, 50) - Nombre de empresa
  - `FECHA` (Fecha) - Fecha de entrada
  - `FECHA_OUT` (Fecha) - Fecha de salida
  - `N_FOLIO` (Numérico, 10) - Número de folio

**✅ Esta tabla contiene información completa de clientes y es la más recomendada para migrar.**

#### 2. **CHEKIN.dbf**
- **Registros:** 21,279
- **Campos principales:**
  - `CLIENTE` (Texto) - Nombre del cliente
  - `TELEFONO` (Numérico) - Teléfono
  - `DIRECCION` (Texto) - Dirección
  - `CEDULA` (Texto) - Cédula
  - `COD_EMPR` (Numérico) - Código de empresa
  - `NO_HABITA` (Numérico) - Número de habitación
  - `N_FOLIO` (Numérico) - Número de folio

**⚠️ Esta tabla tiene 137 campos, muchos pueden ser redundantes. Revisar estructura completa antes de migrar.**

---

### 🏨 **RESERVAS**

#### 3. **RESERVAS.dbf** ⭐ (RECOMENDADO PARA MIGRAR)
- **Registros:** 22,555
- **Campos principales:**
  - `NO_RESERVA` (Numérico, 10) - Número de reserva
  - `CLIENTE` (Texto) - Nombre del cliente
  - `TELEFONO` (Numérico) - Teléfono
  - `DIRECCION` (Texto) - Dirección
  - `EMAIL` (Texto) - Email
  - `CEDULA` (Texto) - Cédula
  - `COD_EMPR` (Numérico) - Código de empresa
  - `FECHA_IN` (Fecha) - Fecha de entrada
  - `FECHA_OUT` (Fecha) - Fecha de salida
  - `ESTADO` (Texto) - Estado de la reserva

**✅ Esta tabla contiene información de reservas y es importante migrarla.**

#### 4. **DT_RESER.dbf**
- **Registros:** 156,210
- **Campos principales:**
  - `NO_RESERVA` (Numérico, 10) - Número de reserva
  - `DESC_HAB` (Texto, 15) - Descripción de habitación
  - `TIPO_HAB` (Entero, 4) - Tipo de habitación
  - `CANTIDAD` (Entero, 4) - Cantidad
  - `TARIFA` (Numérico, 14) - Tarifa

**📝 Tabla de detalle de reservas (relación con RESERVAS.dbf).**

#### 5. **DT_RESALLOT.dbf**
- **Registros:** 903,120
- **Campos principales:**
  - `NO_RESERVA` (Numérico, 10) - Número de reserva
  - `TIPO_HAB` (Entero, 4) - Tipo de habitación
  - `DESC_HAB` (Texto, 15) - Descripción de habitación
  - `CANTIDAD` (Entero, 4) - Cantidad

**📝 Tabla de detalle de reservas por allotment.**

---

### 🛏️ **HABITACIONES**

#### 6. **HABITAC.dbf** ⭐ (RECOMENDADO PARA MIGRAR)
- **Registros:** 42
- **Campos principales:**
  - `NO_HABITAC` (Numérico) - Número de habitación
  - `DESCRIP_HA` (Texto) - Descripción de habitación
  - `DISPONIBLE` (Lógico) - Disponible
  - `BLOQUEADA` (Lógico) - Bloqueada
  - `SUCIA` (Lógico) - Sucia
  - `EN_MANTE` (Lógico) - En mantenimiento
  - `OCUPADO` (Lógico) - Ocupado
  - `NOTAS` (Texto) - Notas

**✅ Esta tabla contiene información de las habitaciones del hotel.**

#### 7. **TIPOS_HAB.dbf** ⭐ (RECOMENDADO PARA MIGRAR)
- **Registros:** 9
- **Campos principales:**
  - `CODIGO` (Numérico) - Código
  - `DESC_HAB` (Texto) - Descripción de habitación
  - `DESC_CORTA` (Texto) - Descripción corta
  - `CANT_HAB` (Numérico) - Cantidad de habitaciones
  - `CAP_HAB` (Numérico) - Capacidad de habitación
  - `TIPO_HAB` (Entero) - Tipo de habitación

**✅ Esta tabla contiene los tipos de habitaciones disponibles.**

#### 8. **HABITACIONESSEPARADAS.dbf**
- **Registros:** 243,694
- **Campos principales:**
  - `NO_RESERVA` (Numérico, 10) - Número de reserva
  - `NO_RES_IND` (Numérico, 3) - Número de reserva individual
  - `NO_HABITA` (Numérico, 4) - Número de habitación
  - `FECHA_R` (Fecha) - Fecha de reserva

**📝 Tabla de relación entre reservas y habitaciones específicas.**

---

### 💰 **CARGOS/FACTURACIÓN**

#### 9. **CARGOS.dbf**
- **Registros:** 116,620
- **Campos principales:**
  - `N_FOLIO` (Numérico, 10) - Número de folio
  - `COD_EXTRA` (Numérico, 5) - Código de extra
  - `DESCRIP_EX` (Texto, 60) - Descripción del extra
  - `MONTO_CARG` (Numérico, 14) - Monto del cargo
  - `MONTO_TOTA` (Numérico, 14) - Monto total
  - `FECHA_HORA` (Fecha/Hora) - Fecha y hora
  - `CANCELADO` (Lógico) - Cancelado

**📝 Tabla de cargos realizados a los huéspedes.**

#### 10. **CARGOSREGISTRADOS.dbf**
- **Registros:** 88,869
- **Campos principales:**
  - `USUARIO` (Texto) - Usuario
  - `FECHA_HORA` (Fecha/Hora) - Fecha y hora
  - `NO_DOCU` (Numérico) - Número de documento
  - `MONTO_CARG` (Numérico) - Monto del cargo
  - `MONTO_TOTA` (Numérico) - Monto total

**📝 Tabla de cargos registrados (historial).**

#### 11. **ACUM_CARGOS.dbf**
- **Registros:** 87,590
- **Campos principales:**
  - `Q_PAGA` (Numérico) - Quien paga
  - `NO_FOLIO` (Numérico) - Número de folio
  - `FECHA_HORA` (Fecha/Hora) - Fecha y hora
  - `MONTO` (Numérico) - Monto
  - `TOTAL` (Numérico) - Total

**📝 Tabla de acumulación de cargos.**

---

### ⚙️ **CONFIGURACIÓN**

#### 12. **TARIFAS.dbf** ⭐ (RECOMENDADO PARA MIGRAR)
- **Registros:** 14
- **Campos principales:**
  - `CODIGO` (Numérico) - Código
  - `NOMBRE_TAR` (Texto) - Nombre de tarifa

**✅ Tabla de configuración de tarifas.**

#### 13. **TEMPORADAS.dbf** ⭐ (RECOMENDADO PARA MIGRAR)
- **Registros:** 36
- **Campos principales:**
  - `M_NUMTEMP` (Numérico) - Número de temporada
  - `DESCRIP_TE` (Texto) - Descripción de temporada
  - `COD_CTA` (Numérico) - Código de cuenta
  - `AUMENTA_AL` (Numérico) - Aumento adicional

**✅ Tabla de configuración de temporadas.**

#### 14. **FORM_PAG.dbf** ⭐ (RECOMENDADO PARA MIGRAR)
- **Registros:** 25
- **Campos principales:**
  - `COD_PAGO` (Numérico) - Código de pago
  - `TIPO_FPAGO` (Texto) - Tipo de forma de pago
  - `DESC_F_PAG` (Texto) - Descripción de forma de pago

**✅ Tabla de configuración de formas de pago.**

---

### 👨‍💼 **EMPLEADOS**

#### 15. **EMPLEADOS.dbf** ⭐ (RECOMENDADO PARA MIGRAR)
- **Registros:** 45
- **Campos principales:**
  - `NO_EMPLEA` (Numérico) - Número de empleado
  - `NOMBRE_EMP` (Texto) - Nombre del empleado
  - `NOMBRE_COR` (Texto) - Nombre corto
  - `TELEFONO` (Numérico) - Teléfono
  - `DIRECCION` (Texto) - Dirección
  - `TIPO_ACCES` (Texto) - Tipo de acceso

**✅ Esta tabla contiene información de empleados/colaboradores.**

---

### 🏢 **EMPRESAS**

#### 16. **EMPRESAS.dbf** ⭐ (RECOMENDADO PARA MIGRAR)
- **Registros:** 591
- **Campos principales:**
  - `COD_EMPR` (Numérico) - Código de empresa
  - `NOM_EMPR` (Texto) - Nombre de empresa
  - `COD_RAZ` (Numérico) - Código de razón social
  - `TELEFONO1` (Numérico) - Teléfono 1
  - `TELEFONO2` (Numérico) - Teléfono 2
  - `FAX` (Numérico) - Fax
  - `CONTACTO` (Texto) - Contacto
  - `DIRECCION` (Texto) - Dirección
  - `LIMITE_CRE` (Numérico) - Límite de crédito

**✅ Esta tabla contiene información de empresas clientes.**

---

### 📍 **UBICACIONES**

#### 17. **PROVINCIAS.dbf** ⭐ (RECOMENDADO PARA MIGRAR)
- **Registros:** 14
- **Campos principales:**
  - `NUMEROPROV` (Numérico) - Número de provincia
  - `NOMBRE` (Texto) - Nombre de provincia

**✅ Tabla de provincias de Costa Rica.**

#### 18. **CANTONES.dbf** ⭐ (RECOMENDADO PARA MIGRAR)
- **Registros:** 160
- **Campos principales:**
  - `NUMEROCANT` (Numérico) - Número de cantón
  - `NUMEROPROV` (Numérico) - Número de provincia
  - `NOMBRE` (Texto) - Nombre de cantón

**✅ Tabla de cantones de Costa Rica.**

#### 19. **DISTRITOS.dbf** ⭐ (RECOMENDADO PARA MIGRAR)
- **Registros:** 934
- **Campos principales:**
  - `NUMERODIST` (Numérico) - Número de distrito
  - `NUMEROCANT` (Numérico) - Número de cantón
  - `NUMEROPROV` (Numérico) - Número de provincia
  - `NOMBRE` (Texto) - Nombre de distrito

**✅ Tabla de distritos de Costa Rica.**

#### 20. **NACIONAL.dbf** ⭐ (RECOMENDADO PARA MIGRAR)
- **Registros:** 57
- **Campos principales:**
  - `COD_NAC` (Numérico) - Código de nacionalidad
  - `NACIONALI` (Texto) - Nacionalidad

**✅ Tabla de nacionalidades.**

---

## ✅ Recomendaciones de Migración

### 🔴 **PRIORIDAD ALTA** (Migrar primero)

1. **EXTRAS_AUT.dbf** → **Cliente**
   - Contiene información completa de clientes
   - 135,865 registros
   - Campos: nombre, teléfono, dirección, cédula, email

2. **RESERVAS.dbf** → **Reservas**
   - Información de reservas
   - 22,555 registros
   - Relacionada con clientes y habitaciones

3. **HABITAC.dbf** → **Habitaciones**
   - Información de habitaciones
   - 42 registros (número fijo de habitaciones)

4. **TIPOS_HAB.dbf** → **TipoHabitacion**
   - Tipos de habitaciones
   - 9 registros

5. **EMPLEADOS.dbf** → **Colaborador**
   - Información de empleados
   - 45 registros

6. **EMPRESAS.dbf** → (Nueva tabla o campo en Cliente)
   - Información de empresas
   - 591 registros

### 🟡 **PRIORIDAD MEDIA** (Migrar después)

7. **TARIFAS.dbf** → (Tabla de configuración)
8. **TEMPORADAS.dbf** → (Tabla de configuración)
9. **FORM_PAG.dbf** → (Tabla de configuración)
10. **PROVINCIAS.dbf**, **CANTONES.dbf**, **DISTRITOS.dbf** → (Tablas de referencia)
11. **NACIONAL.dbf** → (Tabla de referencia)

### 🟢 **PRIORIDAD BAJA** (Opcional, según necesidad)

12. **DT_RESER.dbf** → (Detalle de reservas)
13. **DT_RESALLOT.dbf** → (Detalle de reservas allotment)
14. **HABITACIONESSEPARADAS.dbf** → (Relación reserva-habitación)
15. **CARGOS.dbf** → (Historial de cargos - si se necesita)

---

## 🔄 Mapeo de Campos - Clientes

### EXTRAS_AUT.dbf → Cliente (SQL Server)

| Campo DBF | Campo SQL Server | Notas |
|-----------|------------------|-------|
| `A_NOMBRE_D` | `NombreCliente` | Separar nombre y apellidos si es posible |
| `TELEFONO` | `TelefonoCliente` | Convertir a INT |
| `DIRECCION` | `DireccionCliente` | |
| `CEDULA` | (Nuevo campo) | Agregar campo Cédula si no existe |
| `EMAIL` | `EmailCliente` | |
| `COD_EMPR` | (Relación) | Relacionar con tabla Empresas |
| `NOM_EMPR` | (No migrar) | Se obtiene de relación con Empresas |

**⚠️ Nota:** El campo `A_NOMBRE_D` contiene el nombre completo. Puede ser necesario separarlo en:
- `NombreCliente`
- `PrimerApellidoCliente`
- `SegundoApellidoCliente`

---

## 📝 Próximos Pasos

1. ✅ **Completado:** Identificar todas las tablas disponibles
2. ⏳ **Pendiente:** Revisar estructura completa de tablas clave (CHEKIN.dbf, RESERVAS.dbf)
3. ⏳ **Pendiente:** Crear script de migración para tabla Cliente
4. ⏳ **Pendiente:** Probar migración con muestra de datos
5. ⏳ **Pendiente:** Migrar datos completos

---

## 📄 Archivos Generados

- `INVENTARIO_TABLAS_IMPORTANTES.txt` - Inventario completo con estructura y muestras de datos
- `EscanearDBF_Importantes.py` - Script para escanear tablas importantes
- `EscanearDBF.py` - Script completo para escanear todos los DBF

---

## ❓ Preguntas para Decidir

1. ¿Necesitas migrar el historial completo de cargos (CARGOS.dbf)?
2. ¿Necesitas migrar todas las reservas históricas o solo las activas?
3. ¿Cómo manejar duplicados de clientes? (puede haber clientes repetidos en EXTRAS_AUT.dbf)
4. ¿Necesitas mantener la relación con empresas (EMPRESAS.dbf)?

---

**Última actualización:** Generado automáticamente después del escaneo de la base de datos vieja.


