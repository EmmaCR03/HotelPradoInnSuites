-- =============================================
-- Corregir TelefonoCliente en Cliente (de INT a VARCHAR)
-- VERSIÓN CON GO - Ejecutar todo junto
-- =============================================

PRINT 'Corrigiendo TelefonoCliente en tabla Cliente...';
PRINT '';

-- Verificar tipo actual y cantidad de datos
DECLARE @TipoActual VARCHAR(50);
DECLARE @RowCount INT;

SELECT @TipoActual = t.name
FROM sys.columns c
INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
WHERE c.object_id = OBJECT_ID('dbo.Cliente') AND c.name = 'TelefonoCliente';

SELECT @RowCount = COUNT(*) FROM [dbo].[Cliente];

PRINT 'Tipo actual: ' + ISNULL(@TipoActual, 'NO EXISTE');
PRINT 'Registros en tabla: ' + CAST(@RowCount AS VARCHAR(10));
PRINT '';

IF @TipoActual = 'int'
BEGIN
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
        PRINT 'Tabla tiene datos - Usando proceso con columna temporal';
    END
END
ELSE IF @TipoActual = 'varchar'
BEGIN
    PRINT 'TelefonoCliente ya es VARCHAR - No se requiere cambio';
END
GO

-- Si hay datos, crear columna temporal
IF EXISTS (
    SELECT 1 
    FROM sys.columns c
    INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
    WHERE c.object_id = OBJECT_ID('dbo.Cliente') 
    AND c.name = 'TelefonoCliente'
    AND t.name = 'int'
)
AND EXISTS (SELECT 1 FROM [dbo].[Cliente])
AND NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Cliente') AND name = 'TelefonoClienteTemp')
BEGIN
    ALTER TABLE [dbo].[Cliente]
    ADD [TelefonoClienteTemp] VARCHAR(15) NULL;
    PRINT 'Columna temporal TelefonoClienteTemp creada';
END
GO

-- Copiar datos a columna temporal
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Cliente') AND name = 'TelefonoClienteTemp')
BEGIN
    UPDATE [dbo].[Cliente]
    SET [TelefonoClienteTemp] = CAST([TelefonoCliente] AS VARCHAR(15))
    WHERE [TelefonoCliente] IS NOT NULL;
    PRINT 'Datos copiados a columna temporal';
END
GO

-- Eliminar columna vieja
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Cliente') AND name = 'TelefonoClienteTemp')
BEGIN
    IF EXISTS (
        SELECT 1 
        FROM sys.columns c
        INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
        WHERE c.object_id = OBJECT_ID('dbo.Cliente') 
        AND c.name = 'TelefonoCliente'
        AND t.name = 'int'
    )
    BEGIN
        ALTER TABLE [dbo].[Cliente]
        DROP COLUMN [TelefonoCliente];
        PRINT 'Columna TelefonoCliente (INT) eliminada';
    END
END
GO

-- Renombrar columna temporal
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Cliente') AND name = 'TelefonoClienteTemp')
BEGIN
    EXEC sp_rename '[dbo].[Cliente].[TelefonoClienteTemp]', 'TelefonoCliente', 'COLUMN';
    PRINT 'TelefonoCliente cambiado de INT a VARCHAR(15)';
END
GO

-- Verificación final
PRINT '';
PRINT 'Verificación final:';
SELECT 
    c.name AS Columna,
    t.name AS Tipo,
    c.max_length AS Longitud
FROM sys.columns c
INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
WHERE c.object_id = OBJECT_ID('dbo.Cliente') AND c.name = 'TelefonoCliente';
GO


