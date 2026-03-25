-- =============================================
-- Verificar datos migrados
-- =============================================

PRINT '========================================';
PRINT 'VERIFICACION DE DATOS MIGRADOS';
PRINT '========================================';
PRINT '';

-- 1. Clientes
PRINT '1. TABLA CLIENTE:';
DECLARE @CountCliente INT;
SELECT @CountCliente = COUNT(*) FROM [dbo].[Cliente];
PRINT '   Total clientes: ' + CAST(@CountCliente AS VARCHAR(10));

DECLARE @ClientesConCedula INT;
SELECT @ClientesConCedula = COUNT(*) FROM [dbo].[Cliente] WHERE CedulaCliente IS NOT NULL AND CedulaCliente <> '';
PRINT '   Clientes con cédula: ' + CAST(@ClientesConCedula AS VARCHAR(10));

DECLARE @ClientesConEmpresa INT;
SELECT @ClientesConEmpresa = COUNT(*) FROM [dbo].[Cliente] WHERE IdEmpresa IS NOT NULL;
PRINT '   Clientes con empresa: ' + CAST(@ClientesConEmpresa AS VARCHAR(10));
PRINT '';

-- 2. Reservas
PRINT '2. TABLA RESERVAS:';
DECLARE @CountReservas INT;
SELECT @CountReservas = COUNT(*) FROM [dbo].[Reservas];
PRINT '   Total reservas: ' + CAST(@CountReservas AS VARCHAR(10));

DECLARE @ReservasConCliente INT;
SELECT @ReservasConCliente = COUNT(*) FROM [dbo].[Reservas] WHERE IdCliente IS NOT NULL;
PRINT '   Reservas con cliente: ' + CAST(@ReservasConCliente AS VARCHAR(10));

DECLARE @ReservasSinCliente INT;
SELECT @ReservasSinCliente = COUNT(*) FROM [dbo].[Reservas] WHERE IdCliente IS NULL;
PRINT '   Reservas sin cliente: ' + CAST(@ReservasSinCliente AS VARCHAR(10));

DECLARE @ReservasConEmpresa INT;
SELECT @ReservasConEmpresa = COUNT(*) FROM [dbo].[Reservas] WHERE IdEmpresa IS NOT NULL;
PRINT '   Reservas con empresa: ' + CAST(@ReservasConEmpresa AS VARCHAR(10));
PRINT '';

-- 3. Empresas
PRINT '3. TABLA EMPRESAS:';
DECLARE @CountEmpresas INT;
SELECT @CountEmpresas = COUNT(*) FROM [dbo].[Empresas];
PRINT '   Total empresas: ' + CAST(@CountEmpresas AS VARCHAR(10));
PRINT '';

-- 4. Muestra de datos
PRINT '========================================';
PRINT 'MUESTRA DE DATOS';
PRINT '========================================';
PRINT '';

PRINT 'Primeros 5 clientes:';
SELECT TOP 5 
    IdCliente,
    NombreCliente,
    PrimerApellidoCliente,
    CedulaCliente,
    TelefonoCliente,
    EmailCliente
FROM [dbo].[Cliente]
ORDER BY IdCliente;

PRINT '';
PRINT 'Primeras 5 reservas:';
SELECT TOP 5 
    IdReserva,
    IdCliente,
    NombreCliente,
    FechaInicio,
    FechaFinal,
    EstadoReserva,
    MontoTotal
FROM [dbo].[Reservas]
ORDER BY IdReserva;


