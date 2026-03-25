# 🔧 Ajustes Necesarios en la Base de Datos para Migración

## 📊 Comparación de Estructuras

### 1. **TABLA CLIENTE**

#### Estructura Actual (SQL Server):
```sql
CREATE TABLE [dbo].[Cliente] (
    [IdCliente]              INT            IDENTITY (1, 1) NOT NULL,
    [NombreCliente]          VARCHAR (50)   NULL,
    [PrimerApellidoCliente]  VARCHAR (50)   NULL,
    [SegundoApellidoCliente] VARCHAR (50)   NULL,
    [EmailCliente]           NVARCHAR (50)  NULL,
    [TelefonoCliente]        INT            NULL,
    [DireccionCliente]       NVARCHAR (100) NULL,
    CONSTRAINT [PK_Cliente] PRIMARY KEY CLUSTERED ([IdCliente] ASC)
);
```

#### Datos de EXTRAS_AUT.dbf (DBF Viejo):
- `A_NOMBRE_D` (Texto, 50) - Nombre completo
- `TELEFONO` (Numérico, 11) - Teléfono
- `DIRECCION` (Texto, 90) - Dirección
- `CEDULA` (Texto, 16) - **⚠️ FALTA en SQL**
- `EMAIL` (Texto, 100) - Email
- `COD_EMPR` (Numérico, 4) - **⚠️ FALTA en SQL** (Código de empresa)

#### ⚠️ PROBLEMAS IDENTIFICADOS:

1. **Falta campo CEDULA** - Los datos viejos tienen cédula, la nueva BD no
2. **Falta relación con EMPRESAS** - Los datos viejos tienen COD_EMPR, la nueva BD no
3. **Nombre completo vs separado** - DBF tiene nombre completo, SQL tiene nombre y apellidos separados
4. **Tamaño de Email** - DBF permite 100 caracteres, SQL solo 50
5. **Tipo de Teléfono** - DBF es numérico de 11 dígitos, SQL es INT (puede ser insuficiente para números con código de país)

---

### 2. **TABLA RESERVAS**

#### Estructura Actual (SQL Server):
```sql
CREATE TABLE [dbo].[Reservas] (
    [IdReserva]        INT             IDENTITY (1, 1) NOT NULL,
    [IdCliente]        NVARCHAR (128)  NOT NULL,  -- ⚠️ PROBLEMA: referencia AspNetUsers
    [cantidadPersonas] INT             NULL,
    [NombreCliente]    NVARCHAR (128)  NULL,
    [IdHabitacion]     INT             NOT NULL,
    [FechaInicio]      DATETIME        NULL,
    [FechaFinal]       DATETIME        NULL,
    [EstadoReserva]    NVARCHAR (50)   NOT NULL,
    [MontoTotal]       DECIMAL (18, 2) NOT NULL,
    [NumeroEmpresa]    VARCHAR (50)    NULL,
    [CorreoEmpresa]    VARCHAR (50)    NULL,
    PRIMARY KEY CLUSTERED ([IdReserva] ASC),
    CONSTRAINT [FK_Reservas_Habitacion] FOREIGN KEY ([IdHabitacion]) REFERENCES [dbo].[Habitaciones] ([IdHabitacion]),
    CONSTRAINT [FK_Reservas_Usuarios] FOREIGN KEY ([IdCliente]) REFERENCES [dbo].[AspNetUsers] ([Id])  -- ⚠️ PROBLEMA
);
```

#### Datos de RESERVAS.dbf (DBF Viejo):
- `NO_RESERVA` (Numérico, 10) - Número de reserva
- `CLIENTE` (Texto, 40) - Nombre del cliente
- `TELEFONO` (Numérico, 11)
- `DIRECCION` (Texto, 90)
- `EMAIL` (Texto, 100)
- `CEDULA` (Texto, 16)
- `COD_EMPR` (Numérico, 4) - Código de empresa
- `FECHA_IN` (Fecha) - Fecha entrada
- `FECHA_OUT` (Fecha) - Fecha salida
- `N_ADULTOS` (Numérico, 3) - Número de adultos
- `N_NINOS` (Numérico, 3) - Número de niños
- `TOTAL` (Numérico, 10) - Monto total
- `ESTATUS` (Numérico, 1) - Estado

#### ⚠️ PROBLEMAS CRÍTICOS IDENTIFICADOS:

1. **IdCliente referencia AspNetUsers** - La BD nueva usa autenticación, pero los datos viejos no tienen usuarios. Necesitamos cambiar a referencia INT a Cliente
2. **Falta campo CEDULA** - Los datos viejos tienen cédula del cliente
3. **Falta campo TELEFONO** - Los datos viejos tienen teléfono
4. **Falta campo DIRECCION** - Los datos viejos tienen dirección
5. **Falta campo EMAIL** - Los datos viejos tienen email
6. **cantidadPersonas vs N_ADULTOS + N_NINOS** - DBF tiene adultos y niños separados, SQL tiene cantidad total
7. **EstadoReserva vs ESTATUS** - DBF usa numérico (1, 2, 3...), SQL usa texto

---

## ✅ SOLUCIONES PROPUESTAS

### Script SQL para Ajustar Tabla CLIENTE

```sql
-- 1. Agregar campo CEDULA
ALTER TABLE [dbo].[Cliente]
ADD [CedulaCliente] VARCHAR(16) NULL;

-- 2. Agregar campo para relación con Empresas
ALTER TABLE [dbo].[Cliente]
ADD [IdEmpresa] INT NULL;

-- 3. Aumentar tamaño de EmailCliente
ALTER TABLE [dbo].[Cliente]
ALTER COLUMN [EmailCliente] NVARCHAR(100) NULL;

-- 4. Cambiar TelefonoCliente a VARCHAR para soportar números con código de país
ALTER TABLE [dbo].[Cliente]
ALTER COLUMN [TelefonoCliente] VARCHAR(15) NULL;

-- 5. Aumentar tamaño de DireccionCliente
ALTER TABLE [dbo].[Cliente]
ALTER COLUMN [DireccionCliente] NVARCHAR(200) NULL;

-- 6. Aumentar tamaño de NombreCliente (por si el nombre completo no se puede separar)
ALTER TABLE [dbo].[Cliente]
ALTER COLUMN [NombreCliente] VARCHAR(100) NULL;
```

### Script SQL para Ajustar Tabla RESERVAS

```sql
-- 1. ELIMINAR la foreign key a AspNetUsers (si existe)
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Reservas_Usuarios')
BEGIN
    ALTER TABLE [dbo].[Reservas] DROP CONSTRAINT [FK_Reservas_Usuarios];
END

-- 2. Cambiar IdCliente de NVARCHAR(128) a INT
-- Primero, crear una columna temporal
ALTER TABLE [dbo].[Reservas]
ADD [IdClienteTemp] INT NULL;

-- 3. Agregar campos faltantes
ALTER TABLE [dbo].[Reservas]
ADD [CedulaCliente] VARCHAR(16) NULL,
    [TelefonoCliente] VARCHAR(15) NULL,
    [DireccionCliente] NVARCHAR(200) NULL,
    [EmailCliente] NVARCHAR(100) NULL,
    [NumeroAdultos] INT NULL,
    [NumeroNinos] INT NULL,
    [IdEmpresa] INT NULL,
    [Observaciones] NVARCHAR(500) NULL;

-- 4. Si hay datos, migrarlos primero (esto se hará en el script de migración)
-- Por ahora, solo preparamos la estructura

-- 5. Después de migrar datos, eliminar la columna vieja y renombrar
-- ALTER TABLE [dbo].[Reservas] DROP COLUMN [IdCliente];
-- EXEC sp_rename '[dbo].[Reservas].[IdClienteTemp]', 'IdCliente', 'COLUMN';

-- 6. Agregar la foreign key correcta a Cliente
-- ALTER TABLE [dbo].[Reservas]
-- ADD CONSTRAINT [FK_Reservas_Cliente] FOREIGN KEY ([IdCliente]) REFERENCES [dbo].[Cliente] ([IdCliente]);
```

### Script SQL para Crear Tabla EMPRESAS (si no existe)

```sql
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Empresas')
BEGIN
    CREATE TABLE [dbo].[Empresas] (
        [IdEmpresa] INT IDENTITY(1,1) NOT NULL,
        [CodigoEmpresa] INT NULL,  -- Para mantener el código original del DBF
        [NombreEmpresa] VARCHAR(100) NULL,
        [Telefono1] VARCHAR(15) NULL,
        [Telefono2] VARCHAR(15) NULL,
        [Fax] VARCHAR(15) NULL,
        [Contacto] VARCHAR(100) NULL,
        [Direccion] NVARCHAR(300) NULL,
        [Observaciones] NVARCHAR(500) NULL,
        [LimiteCredito] DECIMAL(18,2) NULL,
        [CorreoElectronico] VARCHAR(100) NULL,
        CONSTRAINT [PK_Empresas] PRIMARY KEY CLUSTERED ([IdEmpresa] ASC)
    );
    
    -- Crear índice único en CodigoEmpresa para búsquedas rápidas
    CREATE UNIQUE NONCLUSTERED INDEX [IX_Empresas_CodigoEmpresa] 
    ON [dbo].[Empresas] ([CodigoEmpresa]);
END
```

---

## 📋 RESUMEN DE CAMBIOS NECESARIOS

### Tabla CLIENTE:
- ✅ Agregar campo `CedulaCliente` (VARCHAR(16))
- ✅ Agregar campo `IdEmpresa` (INT) - Foreign Key a Empresas
- ✅ Aumentar `EmailCliente` de 50 a 100 caracteres
- ✅ Cambiar `TelefonoCliente` de INT a VARCHAR(15)
- ✅ Aumentar `DireccionCliente` de 100 a 200 caracteres
- ✅ Aumentar `NombreCliente` de 50 a 100 caracteres (por si no se puede separar)

### Tabla RESERVAS:
- ✅ Cambiar `IdCliente` de NVARCHAR(128) a INT
- ✅ Cambiar Foreign Key de AspNetUsers a Cliente
- ✅ Agregar campo `CedulaCliente` (VARCHAR(16))
- ✅ Agregar campo `TelefonoCliente` (VARCHAR(15))
- ✅ Agregar campo `DireccionCliente` (NVARCHAR(200))
- ✅ Agregar campo `EmailCliente` (NVARCHAR(100))
- ✅ Agregar campo `NumeroAdultos` (INT)
- ✅ Agregar campo `NumeroNinos` (INT)
- ✅ Agregar campo `IdEmpresa` (INT) - Foreign Key a Empresas
- ✅ Agregar campo `Observaciones` (NVARCHAR(500))

### Nueva Tabla EMPRESAS:
- ✅ Crear tabla `Empresas` con campos principales

---

## 🚀 ORDEN DE EJECUCIÓN

1. **Crear tabla Empresas** (si no existe)
2. **Ajustar tabla Cliente** (agregar campos faltantes)
3. **Ajustar tabla Reservas** (cambiar estructura y agregar campos)
4. **Migrar datos de EMPRESAS.dbf** → `Empresas`
5. **Migrar datos de EXTRAS_AUT.dbf** → `Cliente`
6. **Migrar datos de RESERVAS.dbf** → `Reservas`

---

## ⚠️ NOTAS IMPORTANTES

1. **Separación de nombres**: El campo `A_NOMBRE_D` contiene el nombre completo. Necesitaremos un algoritmo para separarlo en nombre y apellidos, o guardarlo todo en `NombreCliente`.

2. **Duplicados**: Puede haber clientes duplicados en EXTRAS_AUT.dbf. Necesitaremos una estrategia para manejar duplicados (usar CEDULA como identificador único, o crear un cliente por cada registro).

3. **Relación Reservas-Cliente**: Las reservas en RESERVAS.dbf tienen el nombre del cliente pero no un ID. Necesitaremos buscar el cliente por nombre/cedula para establecer la relación.

4. **Backup**: **SIEMPRE hacer backup de la base de datos antes de ejecutar estos cambios.**

---

¿Quieres que cree los scripts SQL completos para aplicar estos cambios?


