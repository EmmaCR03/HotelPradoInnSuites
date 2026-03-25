-- =============================================
-- Script para Verificar el Tamaño de la Base de Datos
-- Hotel Prado - Verificar Almacenamiento
-- =============================================

-- =============================================
-- 1. TAMAÑO TOTAL DE LA BASE DE DATOS
-- =============================================
-- Muestra el tamaño total de la base de datos en MB y GB
SELECT 
    DB_NAME() AS NombreBaseDatos,
    SUM(CAST(FILEPROPERTY(name, 'SpaceUsed') AS bigint) * 8192.) / 1024 / 1024 AS TamañoUsado_MB,
    SUM(CAST(FILEPROPERTY(name, 'SpaceUsed') AS bigint) * 8192.) / 1024 / 1024 / 1024 AS TamañoUsado_GB,
    SUM(size * 8192.) / 1024 / 1024 AS TamañoReservado_MB,
    SUM(size * 8192.) / 1024 / 1024 / 1024 AS TamañoReservado_GB
FROM sys.database_files
WHERE type_desc = 'ROWS';

-- =============================================
-- 2. TAMAÑO POR ARCHIVO DE BASE DE DATOS
-- =============================================
-- Muestra el tamaño de cada archivo de la base de datos
SELECT 
    name AS NombreArchivo,
    type_desc AS TipoArchivo,
    physical_name AS RutaFisica,
    CAST(size * 8.0 / 1024 AS DECIMAL(10, 2)) AS TamañoReservado_MB,
    CAST(FILEPROPERTY(name, 'SpaceUsed') * 8.0 / 1024 AS DECIMAL(10, 2)) AS TamañoUsado_MB,
    CAST((size - FILEPROPERTY(name, 'SpaceUsed')) * 8.0 / 1024 AS DECIMAL(10, 2)) AS EspacioLibre_MB
FROM sys.database_files
ORDER BY type_desc, name;

-- =============================================
-- 3. TAMAÑO POR TABLA (TOP 20 MÁS GRANDES)
-- =============================================
-- Muestra las tablas más grandes y cuánto espacio ocupan
SELECT TOP 20
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
-- 4. RESUMEN POR ESQUEMA
-- =============================================
-- Muestra el tamaño agrupado por esquema (dbo, etc.)
SELECT 
    s.Name AS NombreEsquema,
    COUNT(DISTINCT t.NAME) AS NumeroTablas,
    CAST(ROUND(((SUM(a.total_pages) * 8) / 1024.00), 2) AS DECIMAL(10, 2)) AS TamañoTotal_MB,
    CAST(ROUND(((SUM(a.total_pages) * 8) / 1024.00 / 1024.00), 2) AS DECIMAL(10, 2)) AS TamañoTotal_GB
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
    s.Name
ORDER BY 
    TamañoTotal_MB DESC;

-- =============================================
-- 5. TAMAÑO DE ÍNDICES
-- =============================================
-- Muestra el tamaño de los índices (pueden ocupar mucho espacio)
SELECT 
    OBJECT_NAME(i.object_id) AS NombreTabla,
    i.name AS NombreIndice,
    i.type_desc AS TipoIndice,
    CAST(ROUND(((SUM(a.total_pages) * 8) / 1024.00), 2) AS DECIMAL(10, 2)) AS Tamaño_MB
FROM 
    sys.indexes i
INNER JOIN 
    sys.partitions p ON i.object_id = p.OBJECT_ID AND i.index_id = p.index_id
INNER JOIN 
    sys.allocation_units a ON p.partition_id = a.container_id
WHERE 
    i.object_id > 255
GROUP BY 
    i.object_id, i.name, i.type_desc
HAVING 
    SUM(a.total_pages) > 0
ORDER BY 
    Tamaño_MB DESC;

-- =============================================
-- 6. INFORMACIÓN DETALLADA DE LA BASE DE DATOS
-- =============================================
-- Información completa sobre el tamaño y configuración
EXEC sp_spaceused;

-- =============================================
-- 7. TAMAÑO DE LOGS (TRANSACTION LOG)
-- =============================================
-- Muestra el tamaño del archivo de log de transacciones
SELECT 
    name AS NombreArchivoLog,
    type_desc AS TipoArchivo,
    physical_name AS RutaFisica,
    CAST(size * 8.0 / 1024 AS DECIMAL(10, 2)) AS TamañoReservado_MB,
    CAST(FILEPROPERTY(name, 'SpaceUsed') * 8.0 / 1024 AS DECIMAL(10, 2)) AS TamañoUsado_MB
FROM sys.database_files
WHERE type_desc = 'LOG';

-- =============================================
-- 8. TABLAS CON MÁS FILAS (Pueden ocupar más espacio)
-- =============================================
-- Muestra las tablas con más registros
SELECT TOP 20
    t.NAME AS NombreTabla,
    s.Name AS NombreEsquema,
    p.rows AS NumeroFilas,
    CAST(ROUND(((SUM(a.total_pages) * 8) / 1024.00), 2) AS DECIMAL(10, 2)) AS Tamaño_MB
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
    NumeroFilas DESC;
