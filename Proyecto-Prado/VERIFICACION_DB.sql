-- =============================================
-- SCRIPT DE VERIFICACIÓN DE COMPATIBILIDAD
-- Base de Datos: HotelPrado vs DB
-- =============================================

-- 1. VERIFICAR ESTRUCTURA DE TABLAS PRINCIPALES
-- =============================================

-- Verificar si existe la tabla TipoHabitacion en DB
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TipoHabitacion')
BEGIN
    PRINT '⚠️ ADVERTENCIA: La tabla TipoHabitacion NO existe en DB'
    PRINT '   El sistema antiguo (HotelPrado) tiene esta tabla'
    PRINT '   Necesitas decidir si agregarla o adaptar los datos'
END
ELSE
BEGIN
    PRINT '✅ TipoHabitacion existe en DB'
END
GO

-- Verificar estructura de Reservas
PRINT ''
PRINT '=== VERIFICACIÓN DE TABLA RESERVAS ==='
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Reservas')
BEGIN
    -- Verificar tipo de IdCliente
    IF EXISTS (
        SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
        WHERE TABLE_NAME = 'Reservas' 
        AND COLUMN_NAME = 'IdCliente' 
        AND DATA_TYPE = 'nvarchar'
    )
    BEGIN
        PRINT '⚠️ ADVERTENCIA: IdCliente en Reservas es NVARCHAR (sistema nuevo)'
        PRINT '   El sistema antiguo usa INT'
        PRINT '   Necesitas decidir qué estructura mantener'
    END
    ELSE IF EXISTS (
        SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
        WHERE TABLE_NAME = 'Reservas' 
        AND COLUMN_NAME = 'IdCliente' 
        AND DATA_TYPE = 'int'
    )
    BEGIN
        PRINT '✅ IdCliente en Reservas es INT (compatible con sistema antiguo)'
    END
    
    -- Verificar campos adicionales
    IF NOT EXISTS (
        SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
        WHERE TABLE_NAME = 'Reservas' 
        AND COLUMN_NAME = 'NombreCliente'
    )
    BEGIN
        PRINT 'ℹ️ INFO: Campo NombreCliente no existe (puede ser necesario)'
    END
    
    IF NOT EXISTS (
        SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
        WHERE TABLE_NAME = 'Reservas' 
        AND COLUMN_NAME = 'cantidadPersonas'
    )
    BEGIN
        PRINT 'ℹ️ INFO: Campo cantidadPersonas no existe (puede ser necesario)'
    END
END
GO

-- Verificar estructura de Habitaciones
PRINT ''
PRINT '=== VERIFICACIÓN DE TABLA HABITACIONES ==='
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Habitaciones')
BEGIN
    -- Verificar si tiene IdTipoHabitacion
    IF NOT EXISTS (
        SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
        WHERE TABLE_NAME = 'Habitaciones' 
        AND COLUMN_NAME = 'IdTipoHabitacion'
    )
    BEGIN
        PRINT '⚠️ ADVERTENCIA: Campo IdTipoHabitacion NO existe en Habitaciones'
        PRINT '   El sistema antiguo tiene este campo'
    END
    ELSE
    BEGIN
        PRINT '✅ Campo IdTipoHabitacion existe'
    END
    
    -- Verificar precios
    IF EXISTS (
        SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
        WHERE TABLE_NAME = 'Habitaciones' 
        AND COLUMN_NAME = 'PrecioPorNoche1P'
    )
    BEGIN
        PRINT 'ℹ️ INFO: Sistema nuevo usa precios por número de personas (1P, 2P, 3P, 4P)'
    END
    
    IF EXISTS (
        SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
        WHERE TABLE_NAME = 'Habitaciones' 
        AND COLUMN_NAME = 'PrecioPorNoche'
    )
    BEGIN
        PRINT 'ℹ️ INFO: Sistema antiguo usa un solo precio (PrecioPorNoche)'
    END
END
GO

-- 2. VERIFICAR RELACIONES Y FOREIGN KEYS
-- =============================================
PRINT ''
PRINT '=== VERIFICACIÓN DE RELACIONES ==='

-- Verificar relación Reservas -> Cliente
IF EXISTS (
    SELECT * FROM sys.foreign_keys 
    WHERE name LIKE '%FK_Reservas%Cliente%' 
    OR name LIKE '%FK_Reservas%Usuarios%'
)
BEGIN
    DECLARE @FKName NVARCHAR(128)
    SELECT @FKName = name FROM sys.foreign_keys 
    WHERE name LIKE '%FK_Reservas%Cliente%' OR name LIKE '%FK_Reservas%Usuarios%'
    
    PRINT 'ℹ️ INFO: Existe Foreign Key: ' + @FKName
    
    -- Verificar a qué tabla referencia
    IF @FKName LIKE '%Usuarios%'
    BEGIN
        PRINT '⚠️ ADVERTENCIA: Reservas referencia AspNetUsers (sistema nuevo)'
        PRINT '   El sistema antiguo referencia Cliente'
    END
    ELSE IF @FKName LIKE '%Cliente%'
    BEGIN
        PRINT '✅ Reservas referencia Cliente (compatible con sistema antiguo)'
    END
END
GO

-- 3. VERIFICAR DATOS EXISTENTES
-- =============================================
PRINT ''
PRINT '=== VERIFICACIÓN DE DATOS ==='

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Cliente')
BEGIN
    DECLARE @CountClientes INT
    SELECT @CountClientes = COUNT(*) FROM Cliente
    PRINT '📊 Clientes en base de datos: ' + CAST(@CountClientes AS VARCHAR(10))
END

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Reservas')
BEGIN
    DECLARE @CountReservas INT
    SELECT @CountReservas = COUNT(*) FROM Reservas
    PRINT '📊 Reservas en base de datos: ' + CAST(@CountReservas AS VARCHAR(10))
END

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Habitaciones')
BEGIN
    DECLARE @CountHabitaciones INT
    SELECT @CountHabitaciones = COUNT(*) FROM Habitaciones
    PRINT '📊 Habitaciones en base de datos: ' + CAST(@CountHabitaciones AS VARCHAR(10))
END

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Departamento')
BEGIN
    DECLARE @CountDepartamentos INT
    SELECT @CountDepartamentos = COUNT(*) FROM Departamento
    PRINT '📊 Departamentos en base de datos: ' + CAST(@CountDepartamentos AS VARCHAR(10))
END
GO

-- 4. COMPARAR ESTRUCTURAS (si ambas bases de datos están disponibles)
-- =============================================
PRINT ''
PRINT '=== RESUMEN DE VERIFICACIÓN ==='
PRINT ''
PRINT 'Para migrar datos del sistema antiguo (Visual FoxPro) necesitas:'
PRINT ''
PRINT '1. ✅ Verificar que la estructura de tablas sea compatible'
PRINT '2. ⚠️  Ajustar Reservas.IdCliente (INT vs NVARCHAR)'
PRINT '3. ⚠️  Decidir estructura de Habitaciones (precios)'
PRINT '4. ⚠️  Agregar TipoHabitacion si es necesario'
PRINT '5. ✅ Crear script de migración de datos'
PRINT ''
PRINT 'Ejecuta este script en tu base de datos para ver los detalles.'






