-- ============================================================
-- Ver cuánto pesan las tablas (especialmente Cargos y bitácora)
-- Ejecutar en SQL Server (site4now) sobre la base db_ac52da_pradoinn
-- ============================================================

-- 1) Tamaño solo de la tabla Cargos
EXEC sp_spaceused 'Cargos';
-- Resultado: name, rows, reserved, data, index_size, unused (en KB)

-- 2) Tamaño de TODAS las tablas (ordenado por espacio reservado, las más pesadas primero)
SELECT 
    t.NAME AS Tabla,
    p.rows AS Filas,
    CAST(ROUND(((SUM(a.total_pages) * 8) / 1024.00), 2) AS DECIMAL(18,2)) AS Reservado_MB,
    CAST(ROUND(((SUM(a.used_pages) * 8) / 1024.00), 2) AS DECIMAL(18,2)) AS Usado_MB,
    CAST(ROUND(((SUM(a.total_pages) - SUM(a.used_pages)) * 8) / 1024.00, 2) AS DECIMAL(18,2)) AS SinUsar_MB
FROM sys.tables t
INNER JOIN sys.indexes i ON t.OBJECT_ID = i.object_id
INNER JOIN sys.partitions p ON i.object_id = p.OBJECT_ID AND i.index_id = p.index_id
INNER JOIN sys.allocation_units a ON p.partition_id = a.container_id
WHERE t.is_ms_shipped = 0
GROUP BY t.Name, p.Rows
ORDER BY Reservado_MB DESC;
