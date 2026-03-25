-- =============================================
-- Verificación Rápida Antes de Migrar
-- =============================================

PRINT 'Verificando estado de las tablas...';
PRINT '';

-- Verificar que IdCliente en Reservas es INT
DECLARE @TipoIdCliente VARCHAR(50);
SELECT @TipoIdCliente = t.name
FROM sys.columns c
INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
WHERE c.object_id = OBJECT_ID('dbo.Reservas') AND c.name = 'IdCliente';

IF @TipoIdCliente = 'int'
    PRINT '✓ IdCliente en Reservas es INT';
ELSE
    PRINT '✗ PROBLEMA: IdCliente en Reservas es ' + ISNULL(@TipoIdCliente, 'NO EXISTE') + ' (debe ser INT)';

-- Verificar Foreign Key
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Reservas_Cliente')
    PRINT '✓ Foreign Key FK_Reservas_Cliente existe';
ELSE
    PRINT '✗ Foreign Key FK_Reservas_Cliente NO existe';

-- Verificar que las tablas están vacías (o casi vacías)
DECLARE @CountCliente INT, @CountReservas INT;
SELECT @CountCliente = COUNT(*) FROM Cliente;
SELECT @CountReservas = COUNT(*) FROM Reservas;

PRINT '';
PRINT 'Registros actuales:';
PRINT '  - Cliente: ' + CAST(@CountCliente AS VARCHAR(10));
PRINT '  - Reservas: ' + CAST(@CountReservas AS VARCHAR(10));

PRINT '';
IF @TipoIdCliente = 'int'
    PRINT '✓ LISTO PARA MIGRAR';
ELSE
    PRINT '✗ NO LISTO - Corregir IdCliente primero';


