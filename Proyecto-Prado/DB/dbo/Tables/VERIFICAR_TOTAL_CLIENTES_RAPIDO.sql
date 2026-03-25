-- =============================================
-- VERIFICACION RAPIDA DE CLIENTES MIGRADOS
-- =============================================

PRINT '========================================';
PRINT 'VERIFICACION RAPIDA';
PRINT '========================================';
PRINT '';

-- 1. Total de clientes
SELECT COUNT(*) AS TotalClientes FROM [dbo].[Cliente];

-- 2. Rango de IDs
SELECT 
    MIN(IdCliente) AS IdMinimo,
    MAX(IdCliente) AS IdMaximo,
    MAX(IdCliente) - MIN(IdCliente) + 1 AS RangoEsperado,
    COUNT(*) AS TotalReal
FROM [dbo].[Cliente];

-- 3. Distribución por rangos
SELECT 
    CASE 
        WHEN IdCliente BETWEEN 1 AND 1000 THEN '1-1000'
        WHEN IdCliente BETWEEN 1001 AND 5000 THEN '1001-5000'
        WHEN IdCliente BETWEEN 5001 AND 10000 THEN '5001-10000'
        WHEN IdCliente BETWEEN 10001 AND 50000 THEN '10001-50000'
        WHEN IdCliente BETWEEN 50001 AND 100000 THEN '50001-100000'
        WHEN IdCliente > 100000 THEN '>100000'
    END AS Rango,
    COUNT(*) AS Cantidad
FROM [dbo].[Cliente]
GROUP BY 
    CASE 
        WHEN IdCliente BETWEEN 1 AND 1000 THEN '1-1000'
        WHEN IdCliente BETWEEN 1001 AND 5000 THEN '1001-5000'
        WHEN IdCliente BETWEEN 5001 AND 10000 THEN '5001-10000'
        WHEN IdCliente BETWEEN 10001 AND 50000 THEN '10001-50000'
        WHEN IdCliente BETWEEN 50001 AND 100000 THEN '50001-100000'
        WHEN IdCliente > 100000 THEN '>100000'
    END
ORDER BY MIN(IdCliente);

PRINT '';
PRINT '========================================';
PRINT 'IMPORTANTE: Verificar base de datos';
PRINT '========================================';
PRINT 'El script de migracion se conecta a: HotelPrado';
PRINT 'Asegurate de estar consultando la base de datos correcta';
PRINT '';

-- Verificar en qué base de datos estamos
SELECT DB_NAME() AS BaseDeDatosActual;
