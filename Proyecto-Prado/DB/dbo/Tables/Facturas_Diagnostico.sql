-- =============================================
-- Script de Diagnóstico: Estructura de la tabla Facturas
-- =============================================

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Facturas')
BEGIN
    PRINT '=== ESTRUCTURA ACTUAL DE LA TABLA Facturas ===';
    PRINT '';
    
    PRINT 'COLUMNAS:';
    SELECT 
        c.name AS NombreColumna,
        t.name AS TipoDato,
        CASE 
            WHEN t.name IN ('varchar', 'nvarchar', 'char', 'nchar') THEN CAST(c.max_length AS VARCHAR) + ' (' + CAST(c.max_length AS VARCHAR) + ')'
            WHEN t.name IN ('decimal', 'numeric') THEN t.name + '(' + CAST(c.precision AS VARCHAR) + ',' + CAST(c.scale AS VARCHAR) + ')'
            ELSE t.name
        END AS TipoCompleto,
        CASE WHEN c.is_nullable = 1 THEN 'NULL' ELSE 'NOT NULL' END AS PermiteNull,
        CASE WHEN c.is_identity = 1 THEN 'SÍ' ELSE 'NO' END AS EsIdentity
    FROM sys.columns c
    INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
    WHERE c.object_id = OBJECT_ID('dbo.Facturas')
    ORDER BY c.column_id;
    
    PRINT '';
    PRINT 'CLAVES PRIMARIAS:';
    SELECT 
        kc.name AS NombreConstraint,
        c.name AS NombreColumna
    FROM sys.key_constraints kc
    INNER JOIN sys.index_columns ic ON kc.parent_object_id = ic.object_id AND kc.unique_index_id = ic.index_id
    INNER JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
    WHERE kc.parent_object_id = OBJECT_ID('dbo.Facturas') AND kc.type = 'PK';
    
    PRINT '';
    PRINT 'CANTIDAD DE REGISTROS:';
    SELECT COUNT(*) AS TotalRegistros FROM [dbo].[Facturas];
END
ELSE
BEGIN
    PRINT 'La tabla Facturas NO existe';
END
GO

