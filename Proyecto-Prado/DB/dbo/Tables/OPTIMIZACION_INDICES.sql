-- ============================================
-- SCRIPT DE OPTIMIZACIÓN DE BASE DE DATOS
-- Índices para mejorar el rendimiento
-- ============================================

USE [HotelPrado]
GO

PRINT 'Iniciando optimización de índices...'
GO

-- ============================================
-- ÍNDICES PARA TABLA RESERVAS
-- ============================================

-- Índice para búsquedas por cliente
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Reservas_IdCliente' AND object_id = OBJECT_ID('Reservas'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Reservas_IdCliente] 
    ON [dbo].[Reservas] ([IdCliente])
    INCLUDE ([IdReserva], [FechaInicio], [FechaFinal], [EstadoReserva])
    PRINT 'Índice IX_Reservas_IdCliente creado'
END
GO

-- Índice para búsquedas por fechas (muy importante para calendarios)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Reservas_Fechas' AND object_id = OBJECT_ID('Reservas'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Reservas_Fechas] 
    ON [dbo].[Reservas] ([FechaInicio], [FechaFinal])
    INCLUDE ([IdReserva], [IdHabitacion], [EstadoReserva], [IdCliente])
    PRINT 'Índice IX_Reservas_Fechas creado'
END
GO

-- Índice para búsquedas por habitación y fechas
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Reservas_Habitacion_Fechas' AND object_id = OBJECT_ID('Reservas'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Reservas_Habitacion_Fechas] 
    ON [dbo].[Reservas] ([IdHabitacion], [FechaInicio], [FechaFinal])
    INCLUDE ([IdReserva], [EstadoReserva])
    PRINT 'Índice IX_Reservas_Habitacion_Fechas creado'
END
GO

-- Índice para búsquedas por estado
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Reservas_Estado' AND object_id = OBJECT_ID('Reservas'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Reservas_Estado] 
    ON [dbo].[Reservas] ([EstadoReserva])
    INCLUDE ([IdReserva], [FechaInicio], [FechaFinal])
    PRINT 'Índice IX_Reservas_Estado creado'
END
GO

-- ============================================
-- ÍNDICES PARA TABLA HABITACIONES
-- ============================================

-- Índice para búsquedas por estado de habitación
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Habitaciones_Estado' AND object_id = OBJECT_ID('Habitaciones'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Habitaciones_Estado] 
    ON [dbo].[Habitaciones] ([Estado])
    INCLUDE ([IdHabitacion], [NumeroHabitacion])
    PRINT 'Índice IX_Habitaciones_Estado creado'
END
GO

-- Índice para búsquedas por número de habitación
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Habitaciones_Numero' AND object_id = OBJECT_ID('Habitaciones'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Habitaciones_Numero] 
    ON [dbo].[Habitaciones] ([NumeroHabitacion])
    PRINT 'Índice IX_Habitaciones_Numero creado'
END
GO

-- ============================================
-- ÍNDICES PARA TABLA BITACORA EVENTOS
-- ============================================

-- Índice para búsquedas por fecha (muy usado en listados)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_bitacoraEventos_Fecha' AND object_id = OBJECT_ID('bitacoraEventos'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_bitacoraEventos_Fecha] 
    ON [dbo].[bitacoraEventos] ([FechaDeEvento] DESC)
    INCLUDE ([IdEvento], [TipoDeEvento], [TablaDeEvento])
    PRINT 'Índice IX_bitacoraEventos_Fecha creado'
END
GO

-- Índice para búsquedas por tipo de evento
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_bitacoraEventos_Tipo' AND object_id = OBJECT_ID('bitacoraEventos'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_bitacoraEventos_Tipo] 
    ON [dbo].[bitacoraEventos] ([TipoDeEvento], [FechaDeEvento] DESC)
    PRINT 'Índice IX_bitacoraEventos_Tipo creado'
END
GO

-- Índice para búsquedas por usuario
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_bitacoraEventos_Usuario' AND object_id = OBJECT_ID('bitacoraEventos'))
    AND EXISTS (SELECT * FROM sys.columns WHERE name = 'Usuario' AND object_id = OBJECT_ID('bitacoraEventos'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_bitacoraEventos_Usuario] 
    ON [dbo].[bitacoraEventos] ([Usuario], [FechaDeEvento] DESC)
    PRINT 'Índice IX_bitacoraEventos_Usuario creado'
END
GO

-- ============================================
-- ÍNDICES PARA TABLA CLIENTE
-- ============================================

-- Índice para búsquedas por email
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Cliente_Email' AND object_id = OBJECT_ID('Cliente'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Cliente_Email] 
    ON [dbo].[Cliente] ([EmailCliente])
    WHERE [EmailCliente] IS NOT NULL
    PRINT 'Índice IX_Cliente_Email creado'
END
GO

-- ============================================
-- ÍNDICES PARA TABLA FACTURAS
-- ============================================

-- Índice para búsquedas por fecha de factura
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Facturas_Fecha' AND object_id = OBJECT_ID('Facturas'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Facturas_Fecha] 
    ON [dbo].[Facturas] ([FechaEmision] DESC)
    INCLUDE ([IdFactura], [TotalComprobante])
    PRINT 'Índice IX_Facturas_Fecha creado'
END
GO

-- ============================================
-- ESTADÍSTICAS
-- ============================================

-- Actualizar estadísticas para optimizar el plan de ejecución
UPDATE STATISTICS [dbo].[Reservas] WITH FULLSCAN
UPDATE STATISTICS [dbo].[Habitaciones] WITH FULLSCAN
UPDATE STATISTICS [dbo].[bitacoraEventos] WITH FULLSCAN
PRINT 'Estadísticas actualizadas'

GO

PRINT 'Optimización de índices completada'
GO

-- ============================================
-- VERIFICACIÓN DE ÍNDICES CREADOS
-- ============================================

SELECT 
    OBJECT_NAME(object_id) AS Tabla,
    name AS NombreIndice,
    type_desc AS TipoIndice
FROM sys.indexes
WHERE object_id IN (
    OBJECT_ID('Reservas'),
    OBJECT_ID('Habitaciones'),
    OBJECT_ID('bitacoraEventos'),
    OBJECT_ID('Cliente'),
    OBJECT_ID('Facturas')
)
AND name LIKE 'IX_%'
ORDER BY OBJECT_NAME(object_id), name

GO











