-- =============================================
-- Script para verificar que los ajustes se aplicaron correctamente
-- =============================================

PRINT 'Verificando estructura de tabla Cliente...';
PRINT '';

-- Verificar campos agregados
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Cliente') AND name = 'CedulaCliente')
    PRINT '✓ Campo CedulaCliente existe';
ELSE
    PRINT '✗ Campo CedulaCliente NO existe';

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Cliente') AND name = 'IdEmpresa')
    PRINT '✓ Campo IdEmpresa existe';
ELSE
    PRINT '✗ Campo IdEmpresa NO existe';

-- Verificar tipos y tamaños
SELECT 
    c.name AS NombreColumna,
    t.name AS TipoDato,
    c.max_length AS LongitudMaxima,
    c.is_nullable AS PermiteNull
FROM sys.columns c
INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
WHERE c.object_id = OBJECT_ID('dbo.Cliente')
AND c.name IN ('EmailCliente', 'TelefonoCliente', 'DireccionCliente', 'NombreCliente', 'CedulaCliente', 'IdEmpresa')
ORDER BY c.name;

-- Verificar Foreign Key
PRINT '';
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Cliente_Empresas')
    PRINT '✓ Foreign Key FK_Cliente_Empresas existe';
ELSE
    PRINT '✗ Foreign Key FK_Cliente_Empresas NO existe';

PRINT '';
PRINT 'Verificación completada';

