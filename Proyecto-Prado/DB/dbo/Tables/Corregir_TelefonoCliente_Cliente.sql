-- =============================================
-- Corregir TelefonoCliente en Cliente (de INT a VARCHAR)
-- =============================================

PRINT 'Corrigiendo TelefonoCliente en tabla Cliente...';
PRINT '';

-- Verificar tipo actual
DECLARE @TipoActual VARCHAR(50);
SELECT @TipoActual = t.name
FROM sys.columns c
INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
WHERE c.object_id = OBJECT_ID('dbo.Cliente') AND c.name = 'TelefonoCliente';

PRINT 'Tipo actual: ' + ISNULL(@TipoActual, 'NO EXISTE');
PRINT '';

IF @TipoActual = 'int'
BEGIN
    DECLARE @RowCount INT;
    SELECT @RowCount = COUNT(*) FROM [dbo].[Cliente];
    
    IF @RowCount = 0
    BEGIN
        -- Tabla vacía, cambiar directamente
        ALTER TABLE [dbo].[Cliente]
        ALTER COLUMN [TelefonoCliente] VARCHAR(15) NULL;
        PRINT 'TelefonoCliente cambiado de INT a VARCHAR(15)';
    END
    ELSE
    BEGIN
        -- Tabla tiene datos, usar columna temporal
        PRINT 'Tabla tiene ' + CAST(@RowCount AS VARCHAR(10)) + ' registros';
        PRINT 'Creando columna temporal...';
        
        ALTER TABLE [dbo].[Cliente]
        ADD [TelefonoClienteTemp] VARCHAR(15) NULL;
        
        PRINT 'Copiando datos...';
        UPDATE [dbo].[Cliente]
        SET [TelefonoClienteTemp] = CAST([TelefonoCliente] AS VARCHAR(15))
        WHERE [TelefonoCliente] IS NOT NULL;
        
        PRINT 'Eliminando columna vieja...';
        ALTER TABLE [dbo].[Cliente]
        DROP COLUMN [TelefonoCliente];
        
        PRINT 'Renombrando columna...';
        EXEC sp_rename '[dbo].[Cliente].[TelefonoClienteTemp]', 'TelefonoCliente', 'COLUMN';
        
        PRINT 'TelefonoCliente cambiado de INT a VARCHAR(15)';
    END
END
ELSE IF @TipoActual = 'varchar'
BEGIN
    PRINT 'TelefonoCliente ya es VARCHAR - No se requiere cambio';
END
ELSE
    PRINT 'ERROR: No se pudo determinar el tipo de TelefonoCliente';

PRINT '';
PRINT 'Verificación final:';
SELECT 
    c.name AS Columna,
    t.name AS Tipo,
    c.max_length AS Longitud
FROM sys.columns c
INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
WHERE c.object_id = OBJECT_ID('dbo.Cliente') AND c.name = 'TelefonoCliente';


