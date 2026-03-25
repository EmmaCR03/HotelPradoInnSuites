-- =============================================
-- Script SEPARADO para cambiar TelefonoCliente de INT a VARCHAR
-- Ejecutar este script si el anterior falla en la parte de TelefonoCliente
-- =============================================

-- Paso 1: Verificar estado actual
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Cliente') AND name = 'TelefonoCliente')
BEGIN
    DECLARE @EsVarchar BIT = 0;
    DECLARE @MaxLen INT = 0;
    DECLARE @RowCount INT;
    
    SELECT @EsVarchar = CASE WHEN t.name = 'varchar' THEN 1 ELSE 0 END,
           @MaxLen = c.max_length
    FROM sys.columns c
    INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
    WHERE c.object_id = OBJECT_ID('dbo.Cliente') 
    AND c.name = 'TelefonoCliente';
    
    SELECT @RowCount = COUNT(*) FROM [dbo].[Cliente];
    
    PRINT 'Estado actual:';
    PRINT '  - Es VARCHAR: ' + CAST(@EsVarchar AS VARCHAR(1));
    PRINT '  - Longitud máxima: ' + CAST(@MaxLen AS VARCHAR(10));
    PRINT '  - Registros en tabla: ' + CAST(@RowCount AS VARCHAR(10));
    
    IF @EsVarchar = 0 AND @RowCount = 0
    BEGIN
        -- Tabla vacía, cambiar directamente
        ALTER TABLE [dbo].[Cliente]
        ALTER COLUMN [TelefonoCliente] VARCHAR(15) NULL;
        PRINT 'TelefonoCliente cambiado de INT a VARCHAR(15)';
    END
    ELSE IF @EsVarchar = 0 AND @RowCount > 0
    BEGIN
        -- Tabla tiene datos, usar proceso con columna temporal
        PRINT 'Iniciando proceso de migración...';
        
        -- Crear columna temporal
        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Cliente') AND name = 'TelefonoClienteTemp')
        BEGIN
            ALTER TABLE [dbo].[Cliente]
            ADD [TelefonoClienteTemp] VARCHAR(15) NULL;
            PRINT 'Paso 1: Columna temporal creada';
        END
        ELSE
            PRINT 'Paso 1: Columna temporal ya existe';
    END
    ELSE
    BEGIN
        -- Ya es VARCHAR
        IF @MaxLen < 15
        BEGIN
            ALTER TABLE [dbo].[Cliente]
            ALTER COLUMN [TelefonoCliente] VARCHAR(15) NULL;
            PRINT 'Tamaño de TelefonoCliente aumentado a 15';
        END
        ELSE
            PRINT 'TelefonoCliente ya es VARCHAR(15) o mayor - No se requiere cambio';
    END
END
ELSE
    PRINT 'ERROR: La columna TelefonoCliente no existe';

-- =============================================
-- Si necesitas completar la migración, ejecuta estos pasos manualmente:
-- =============================================
-- Paso 2: Copiar datos (ejecutar solo si la columna temporal existe)
/*
UPDATE [dbo].[Cliente]
SET [TelefonoClienteTemp] = CAST([TelefonoCliente] AS VARCHAR(15))
WHERE [TelefonoCliente] IS NOT NULL;
PRINT 'Paso 2: Datos copiados';
*/

-- Paso 3: Eliminar columna vieja (ejecutar solo después del paso 2)
/*
ALTER TABLE [dbo].[Cliente]
DROP COLUMN [TelefonoCliente];
PRINT 'Paso 3: Columna vieja eliminada';
*/

-- Paso 4: Renombrar columna temporal (ejecutar solo después del paso 3)
/*
EXEC sp_rename '[dbo].[Cliente].[TelefonoClienteTemp]', 'TelefonoCliente', 'COLUMN';
PRINT 'Paso 4: Migración completada';
*/


