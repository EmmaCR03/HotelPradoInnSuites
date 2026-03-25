-- =============================================
-- Script para verificar que los ajustes de Reservas se aplicaron correctamente
-- =============================================

PRINT 'Verificando estructura de tabla Reservas...';
PRINT '';

-- Verificar campos agregados
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Reservas') AND name = 'CedulaCliente')
    PRINT '✓ Campo CedulaCliente existe';
ELSE
    PRINT '✗ Campo CedulaCliente NO existe';

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Reservas') AND name = 'TelefonoCliente')
    PRINT '✓ Campo TelefonoCliente existe';
ELSE
    PRINT '✗ Campo TelefonoCliente NO existe';

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Reservas') AND name = 'DireccionCliente')
    PRINT '✓ Campo DireccionCliente existe';
ELSE
    PRINT '✗ Campo DireccionCliente NO existe';

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Reservas') AND name = 'EmailCliente')
    PRINT '✓ Campo EmailCliente existe';
ELSE
    PRINT '✗ Campo EmailCliente NO existe';

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Reservas') AND name = 'NumeroAdultos')
    PRINT '✓ Campo NumeroAdultos existe';
ELSE
    PRINT '✗ Campo NumeroAdultos NO existe';

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Reservas') AND name = 'NumeroNinos')
    PRINT '✓ Campo NumeroNinos existe';
ELSE
    PRINT '✗ Campo NumeroNinos NO existe';

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Reservas') AND name = 'IdEmpresa')
    PRINT '✓ Campo IdEmpresa existe';
ELSE
    PRINT '✗ Campo IdEmpresa NO existe';

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Reservas') AND name = 'Observaciones')
    PRINT '✓ Campo Observaciones existe';
ELSE
    PRINT '✗ Campo Observaciones NO existe';

-- Verificar tipo de IdCliente (debe ser INT, no NVARCHAR)
PRINT '';
PRINT 'Verificando tipo de IdCliente:';
DECLARE @TipoIdCliente VARCHAR(50);
SELECT @TipoIdCliente = t.name + '(' + CAST(c.max_length AS VARCHAR(10)) + ')'
FROM sys.columns c
INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
WHERE c.object_id = OBJECT_ID('dbo.Reservas') 
AND c.name = 'IdCliente';

IF @TipoIdCliente IS NOT NULL
BEGIN
    IF @TipoIdCliente LIKE 'int%'
        PRINT '✓ IdCliente es INT (correcto)';
    ELSE
        PRINT '✗ IdCliente es ' + @TipoIdCliente + ' (debe ser INT)';
END
ELSE
    PRINT '✗ Campo IdCliente NO existe';

-- Verificar tipos y tamaños de todos los campos importantes
PRINT '';
PRINT 'Estructura de campos importantes:';
SELECT 
    c.name AS NombreColumna,
    t.name AS TipoDato,
    c.max_length AS LongitudMaxima,
    c.is_nullable AS PermiteNull
FROM sys.columns c
INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
WHERE c.object_id = OBJECT_ID('dbo.Reservas')
AND c.name IN (
    'IdCliente', 'CedulaCliente', 'TelefonoCliente', 'DireccionCliente', 
    'EmailCliente', 'NumeroAdultos', 'NumeroNinos', 'IdEmpresa', 'Observaciones',
    'FechaInicio', 'FechaFinal', 'EstadoReserva', 'MontoTotal'
)
ORDER BY c.name;

-- Verificar Foreign Keys
PRINT '';
PRINT 'Verificando Foreign Keys:';

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Reservas_Cliente')
    PRINT '✓ Foreign Key FK_Reservas_Cliente existe';
ELSE
    PRINT '✗ Foreign Key FK_Reservas_Cliente NO existe';

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Reservas_Empresas')
    PRINT '✓ Foreign Key FK_Reservas_Empresas existe';
ELSE
    PRINT '⚠ Foreign Key FK_Reservas_Empresas NO existe (opcional)';

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Reservas_Usuarios')
    PRINT '⚠ Foreign Key FK_Reservas_Usuarios existe (debe eliminarse si IdCliente es INT)';
ELSE
    PRINT '✓ Foreign Key FK_Reservas_Usuarios NO existe (correcto)';

PRINT '';
PRINT 'Verificación completada';


