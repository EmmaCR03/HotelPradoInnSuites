-- =============================================
-- Script para Analizar Datos Antiguos y Tamaño por Tabla
-- Hotel Prado - Identificar qué ocupa más espacio
-- =============================================

-- =============================================
-- 1. TAMAÑO POR TABLA (Ordenado por tamaño)
-- =============================================
-- Muestra todas las tablas y cuánto espacio ocupan
SELECT 
    t.NAME AS NombreTabla,
    s.Name AS NombreEsquema,
    p.rows AS NumeroFilas,
    CAST(ROUND(((SUM(a.total_pages) * 8) / 1024.00), 2) AS DECIMAL(10, 2)) AS TamañoTotal_MB,
    CAST(ROUND(((SUM(a.used_pages) * 8) / 1024.00), 2) AS DECIMAL(10, 2)) AS TamañoUsado_MB,
    CAST(ROUND(((SUM(a.total_pages) - SUM(a.used_pages)) * 8) / 1024.00, 2) AS DECIMAL(10, 2)) AS EspacioSinUsar_MB
FROM 
    sys.tables t
INNER JOIN      
    sys.indexes i ON t.OBJECT_ID = i.object_id
INNER JOIN 
    sys.partitions p ON i.object_id = p.OBJECT_ID AND i.index_id = p.index_id
INNER JOIN 
    sys.allocation_units a ON p.partition_id = a.container_id
LEFT OUTER JOIN 
    sys.schemas s ON t.schema_id = s.schema_id
WHERE 
    t.NAME NOT LIKE 'dt%' 
    AND t.is_ms_shipped = 0
    AND i.OBJECT_ID > 255 
GROUP BY 
    t.Name, s.Name, p.Rows
ORDER BY 
    TamañoTotal_MB DESC;

-- =============================================
-- 2. ANÁLISIS DE CLIENTES (Datos antiguos)
-- =============================================
-- Ver cuántos clientes hay y cuándo fueron creados
SELECT 
    COUNT(*) AS TotalClientes,
    MIN(CONVERT(DATE, FechaCreacion)) AS ClienteMasAntiguo,
    MAX(CONVERT(DATE, FechaCreacion)) AS ClienteMasReciente,
    COUNT(CASE WHEN CONVERT(DATE, FechaCreacion) < DATEADD(YEAR, -2, GETDATE()) THEN 1 END) AS ClientesMas2Anos,
    COUNT(CASE WHEN CONVERT(DATE, FechaCreacion) < DATEADD(YEAR, -1, GETDATE()) THEN 1 END) AS ClientesMas1Ano
FROM Cliente;

-- Ver clientes sin reservas (pueden ser candidatos para limpieza)
SELECT 
    COUNT(*) AS ClientesSinReservas,
    MIN(CONVERT(DATE, FechaCreacion)) AS ClienteMasAntiguoSinReserva,
    MAX(CONVERT(DATE, FechaCreacion)) AS ClienteMasRecienteSinReserva
FROM Cliente c
LEFT JOIN Reservas r ON c.IdCliente = r.IdCliente
WHERE r.IdReserva IS NULL;

-- =============================================
-- 3. ANÁLISIS DE RESERVAS ANTIGUAS
-- =============================================
-- Ver distribución de reservas por año
SELECT 
    YEAR(FechaInicio) AS Anio,
    COUNT(*) AS NumeroReservas,
    MIN(FechaInicio) AS ReservaMasAntigua,
    MAX(FechaInicio) AS ReservaMasReciente
FROM Reservas
GROUP BY YEAR(FechaInicio)
ORDER BY Anio DESC;

-- Reservas muy antiguas (más de 2 años)
SELECT 
    COUNT(*) AS ReservasMas2Anos,
    MIN(FechaInicio) AS ReservaMasAntigua,
    MAX(FechaInicio) AS ReservaMasReciente
FROM Reservas
WHERE FechaInicio < DATEADD(YEAR, -2, GETDATE());

-- =============================================
-- 4. ANÁLISIS DE FACTURAS ANTIGUAS
-- =============================================
-- Ver distribución de facturas por año
SELECT 
    YEAR(FechaFactura) AS Anio,
    COUNT(*) AS NumeroFacturas,
    MIN(FechaFactura) AS FacturaMasAntigua,
    MAX(FechaFactura) AS FacturaMasReciente
FROM Facturas
GROUP BY YEAR(FechaFactura)
ORDER BY Anio DESC;

-- Facturas muy antiguas (más de 2 años)
SELECT 
    COUNT(*) AS FacturasMas2Anos,
    MIN(FechaFactura) AS FacturaMasAntigua,
    MAX(FechaFactura) AS FacturaMasReciente
FROM Facturas
WHERE FechaFactura < DATEADD(YEAR, -2, GETDATE());

-- =============================================
-- 5. ANÁLISIS DE BITÁCORA DE EVENTOS (Puede ocupar mucho)
-- =============================================
-- Ver cuántos eventos hay y distribución por fecha
SELECT 
    COUNT(*) AS TotalEventos,
    MIN(CONVERT(DATE, FechaDeEvento)) AS EventoMasAntiguo,
    MAX(CONVERT(DATE, FechaDeEvento)) AS EventoMasReciente,
    COUNT(CASE WHEN CONVERT(DATE, FechaDeEvento) < DATEADD(MONTH, -6, GETDATE()) THEN 1 END) AS EventosMas6Meses,
    COUNT(CASE WHEN CONVERT(DATE, FechaDeEvento) < DATEADD(MONTH, -12, GETDATE()) THEN 1 END) AS EventosMas1Ano
FROM BitacoraEventos;

-- Eventos por mes (últimos 12 meses)
SELECT 
    FORMAT(CONVERT(DATE, FechaDeEvento), 'yyyy-MM') AS Mes,
    COUNT(*) AS NumeroEventos
FROM BitacoraEventos
WHERE FechaDeEvento >= DATEADD(MONTH, -12, GETDATE())
GROUP BY FORMAT(CONVERT(DATE, FechaDeEvento), 'yyyy-MM')
ORDER BY Mes DESC;

-- =============================================
-- 6. RESUMEN DE DATOS ANTIGUOS
-- =============================================
-- Resumen general de qué datos antiguos tienes
SELECT 
    'Clientes' AS TipoDato,
    COUNT(*) AS TotalRegistros,
    COUNT(CASE WHEN CONVERT(DATE, FechaCreacion) < DATEADD(YEAR, -2, GETDATE()) THEN 1 END) AS RegistrosMas2Anos,
    COUNT(CASE WHEN CONVERT(DATE, FechaCreacion) < DATEADD(YEAR, -1, GETDATE()) THEN 1 END) AS RegistrosMas1Ano
FROM Cliente

UNION ALL

SELECT 
    'Reservas' AS TipoDato,
    COUNT(*) AS TotalRegistros,
    COUNT(CASE WHEN FechaInicio < DATEADD(YEAR, -2, GETDATE()) THEN 1 END) AS RegistrosMas2Anos,
    COUNT(CASE WHEN FechaInicio < DATEADD(YEAR, -1, GETDATE()) THEN 1 END) AS RegistrosMas1Ano
FROM Reservas

UNION ALL

SELECT 
    'Facturas' AS TipoDato,
    COUNT(*) AS TotalRegistros,
    COUNT(CASE WHEN FechaFactura < DATEADD(YEAR, -2, GETDATE()) THEN 1 END) AS RegistrosMas2Anos,
    COUNT(CASE WHEN FechaFactura < DATEADD(YEAR, -1, GETDATE()) THEN 1 END) AS RegistrosMas1Ano
FROM Facturas

UNION ALL

SELECT 
    'BitacoraEventos' AS TipoDato,
    COUNT(*) AS TotalRegistros,
    COUNT(CASE WHEN CONVERT(DATE, FechaDeEvento) < DATEADD(MONTH, -24, GETDATE()) THEN 1 END) AS RegistrosMas2Anos,
    COUNT(CASE WHEN CONVERT(DATE, FechaDeEvento) < DATEADD(MONTH, -12, GETDATE()) THEN 1 END) AS RegistrosMas1Ano
FROM BitacoraEventos;

-- =============================================
-- 7. ESTIMACIÓN DE ESPACIO QUE SE LIBERARÍA
-- =============================================
-- Esto es una estimación aproximada
-- El espacio real puede variar debido a índices y fragmentación

SELECT 
    'Clientes sin reservas (estimado)' AS TipoLimpieza,
    COUNT(*) AS RegistrosAEliminar,
    CAST(COUNT(*) * 0.001 AS DECIMAL(10, 2)) AS EspacioLiberado_MB_Estimado
FROM Cliente c
LEFT JOIN Reservas r ON c.IdCliente = r.IdCliente
WHERE r.IdReserva IS NULL

UNION ALL

SELECT 
    'Bitacora eventos > 6 meses (estimado)' AS TipoLimpieza,
    COUNT(*) AS RegistrosAEliminar,
    CAST(COUNT(*) * 0.0005 AS DECIMAL(10, 2)) AS EspacioLiberado_MB_Estimado
FROM BitacoraEventos
WHERE CONVERT(DATE, FechaDeEvento) < DATEADD(MONTH, -6, GETDATE());
