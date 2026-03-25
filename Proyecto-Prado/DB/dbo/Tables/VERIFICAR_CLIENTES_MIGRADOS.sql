-- =============================================
-- Verificar Clientes Migrados
-- Este script muestra cuántos clientes hay en la base de datos
-- =============================================

PRINT '========================================';
PRINT 'VERIFICACION DE CLIENTES MIGRADOS';
PRINT '========================================';
PRINT '';

-- 1. Contar total de clientes
PRINT '1. TOTAL DE CLIENTES EN LA TABLA Cliente:';
DECLARE @TotalClientes INT;
SELECT @TotalClientes = COUNT(*) FROM [dbo].[Cliente];
PRINT '   Total clientes: ' + CAST(@TotalClientes AS VARCHAR(10));
PRINT '';

-- 2. Verificar estructura de la tabla
PRINT '2. ESTRUCTURA DE LA TABLA Cliente:';
SELECT 
    COLUMN_NAME AS 'Campo',
    DATA_TYPE AS 'Tipo',
    CHARACTER_MAXIMUM_LENGTH AS 'Tamaño',
    IS_NULLABLE AS 'Permite NULL'
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Cliente'
ORDER BY ORDINAL_POSITION;
PRINT '';

-- 3. Estadísticas detalladas
PRINT '3. ESTADISTICAS DETALLADAS:';
DECLARE @ConNombre INT;
DECLARE @ConEmail INT;
DECLARE @ConCedula INT;
DECLARE @ConTelefono INT;
DECLARE @ConDireccion INT;
DECLARE @ConEmpresa INT;
DECLARE @ConUsuario INT;

SELECT @ConNombre = COUNT(*) FROM [dbo].[Cliente] WHERE NombreCliente IS NOT NULL AND NombreCliente <> '';
SELECT @ConEmail = COUNT(*) FROM [dbo].[Cliente] WHERE EmailCliente IS NOT NULL AND EmailCliente <> '';
SELECT @ConCedula = COUNT(*) FROM [dbo].[Cliente] WHERE CedulaCliente IS NOT NULL AND CedulaCliente <> '';
SELECT @ConTelefono = COUNT(*) FROM [dbo].[Cliente] WHERE TelefonoCliente IS NOT NULL;
SELECT @ConDireccion = COUNT(*) FROM [dbo].[Cliente] WHERE DireccionCliente IS NOT NULL AND DireccionCliente <> '';
SELECT @ConEmpresa = COUNT(*) FROM [dbo].[Cliente] WHERE IdEmpresa IS NOT NULL;
SELECT @ConUsuario = COUNT(*) FROM [dbo].[Cliente] WHERE IdUsuario IS NOT NULL;

PRINT '   Clientes con nombre: ' + CAST(@ConNombre AS VARCHAR(10));
PRINT '   Clientes con email: ' + CAST(@ConEmail AS VARCHAR(10));
PRINT '   Clientes con cédula: ' + CAST(@ConCedula AS VARCHAR(10));
PRINT '   Clientes con teléfono: ' + CAST(@ConTelefono AS VARCHAR(10));
PRINT '   Clientes con dirección: ' + CAST(@ConDireccion AS VARCHAR(10));
PRINT '   Clientes con empresa: ' + CAST(@ConEmpresa AS VARCHAR(10));
PRINT '   Clientes con usuario web: ' + CAST(@ConUsuario AS VARCHAR(10));
PRINT '';

-- 4. Rango de IDs
PRINT '4. RANGO DE IDs:';
DECLARE @MinId INT;
DECLARE @MaxId INT;
SELECT @MinId = MIN(IdCliente), @MaxId = MAX(IdCliente) FROM [dbo].[Cliente];
PRINT '   ID mínimo: ' + CAST(@MinId AS VARCHAR(10));
PRINT '   ID máximo: ' + CAST(@MaxId AS VARCHAR(10));
PRINT '';

-- 5. Muestra de clientes
PRINT '========================================';
PRINT 'MUESTRA DE CLIENTES (Primeros 10)';
PRINT '========================================';
PRINT '';

SELECT TOP 10
    IdCliente,
    NombreCliente,
    PrimerApellidoCliente,
    SegundoApellidoCliente,
    EmailCliente,
    CedulaCliente,
    TelefonoCliente,
    IdEmpresa,
    IdUsuario
FROM [dbo].[Cliente]
ORDER BY IdCliente;

PRINT '';
PRINT '========================================';
PRINT 'MUESTRA DE CLIENTES (Últimos 10)';
PRINT '========================================';
PRINT '';

SELECT TOP 10
    IdCliente,
    NombreCliente,
    PrimerApellidoCliente,
    SegundoApellidoCliente,
    EmailCliente,
    CedulaCliente,
    TelefonoCliente,
    IdEmpresa,
    IdUsuario
FROM [dbo].[Cliente]
ORDER BY IdCliente DESC;

PRINT '';
PRINT '========================================';
PRINT 'VERIFICACION COMPLETADA';
PRINT '========================================';
PRINT '';

-- 6. Verificar si hay datos en otras tablas relacionadas
PRINT '6. DATOS EN TABLAS RELACIONADAS:';
DECLARE @TotalReservas INT;
DECLARE @TotalCheckIn INT;

SELECT @TotalReservas = COUNT(*) FROM [dbo].[Reservas];
SELECT @TotalCheckIn = COUNT(*) FROM [dbo].[CheckIn];

PRINT '   Total reservas: ' + CAST(@TotalReservas AS VARCHAR(10));
PRINT '   Total check-ins: ' + CAST(@TotalCheckIn AS VARCHAR(10));
PRINT '';

-- 7. Verificar si hay clientes en otras bases de datos
PRINT '7. VERIFICAR OTRAS BASES DE DATOS:';
PRINT '   (Ejecuta esto manualmente si tienes múltiples bases de datos)';
PRINT '';
PRINT '   Para verificar en otra base de datos, ejecuta:';
PRINT '   USE [NombreBaseDatos];';
PRINT '   SELECT COUNT(*) FROM [dbo].[Cliente];';
PRINT '';

PRINT '========================================';
PRINT 'FIN DE VERIFICACION';
PRINT '========================================';















