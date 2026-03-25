-- =============================================
-- Script para ajustar tabla Cliente para migración (VERSIÓN FINAL)
-- =============================================

-- 1. Agregar campo CEDULA
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Cliente') AND name = 'CedulaCliente')
BEGIN
    ALTER TABLE [dbo].[Cliente]
    ADD [CedulaCliente] VARCHAR(16) NULL;
    PRINT 'Campo CedulaCliente agregado';
END
ELSE
    PRINT 'Campo CedulaCliente ya existe';

-- 2. Agregar campo para relación con Empresas
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Cliente') AND name = 'IdEmpresa')
BEGIN
    ALTER TABLE [dbo].[Cliente]
    ADD [IdEmpresa] INT NULL;
    PRINT 'Campo IdEmpresa agregado';
END
ELSE
    PRINT 'Campo IdEmpresa ya existe';

-- 3. Aumentar tamaño de EmailCliente
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Cliente') AND name = 'EmailCliente')
BEGIN
    ALTER TABLE [dbo].[Cliente]
    ALTER COLUMN [EmailCliente] NVARCHAR(100) NULL;
    PRINT 'Tamaño de EmailCliente aumentado a 100';
END

-- 4. Cambiar TelefonoCliente a VARCHAR para soportar números con código de país
-- Primero verificar si hay una columna temporal de una ejecución anterior
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Cliente') AND name = 'TelefonoClienteTemp')
BEGIN
    -- Limpiar ejecución anterior
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Cliente') AND name = 'TelefonoCliente')
    BEGIN
        ALTER TABLE [dbo].[Cliente] DROP COLUMN [TelefonoCliente];
    END
    EXEC sp_rename '[dbo].[Cliente].[TelefonoClienteTemp]', 'TelefonoCliente', 'COLUMN';
    PRINT 'TelefonoCliente cambiado a VARCHAR(15) (limpiando ejecución anterior)';
END
ELSE IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Cliente') AND name = 'TelefonoCliente')
BEGIN
    -- Verificar si ya es VARCHAR
    DECLARE @EsVarchar BIT = 0;
    DECLARE @MaxLen INT = 0;
    
    SELECT @EsVarchar = CASE WHEN t.name = 'varchar' THEN 1 ELSE 0 END,
           @MaxLen = c.max_length
    FROM sys.columns c
    INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
    WHERE c.object_id = OBJECT_ID('dbo.Cliente') 
    AND c.name = 'TelefonoCliente';
    
    IF @EsVarchar = 1
    BEGIN
        -- Ya es VARCHAR, solo verificar tamaño
        IF @MaxLen < 15
        BEGIN
            ALTER TABLE [dbo].[Cliente]
            ALTER COLUMN [TelefonoCliente] VARCHAR(15) NULL;
            PRINT 'Tamaño de TelefonoCliente aumentado a 15';
        END
        ELSE
            PRINT 'TelefonoCliente ya es VARCHAR(15) o mayor';
    END
    ELSE
    BEGIN
        -- Es INT, necesitamos cambiarlo usando un enfoque diferente
        -- Usar ALTER COLUMN directamente si la tabla está vacía, o crear script separado
        DECLARE @RowCount INT;
        SELECT @RowCount = COUNT(*) FROM [dbo].[Cliente];
        
        IF @RowCount = 0
        BEGIN
            -- Tabla vacía, podemos cambiar directamente
            ALTER TABLE [dbo].[Cliente]
            ALTER COLUMN [TelefonoCliente] VARCHAR(15) NULL;
            PRINT 'TelefonoCliente cambiado de INT a VARCHAR(15) (tabla vacía)';
        END
        ELSE
        BEGIN
            -- Tabla tiene datos, necesitamos usar columna temporal
            -- Crear columna temporal
            ALTER TABLE [dbo].[Cliente]
            ADD [TelefonoClienteTemp] VARCHAR(15) NULL;
            PRINT 'Columna temporal TelefonoClienteTemp creada';
            
            -- Usar un script dinámico para copiar datos
            DECLARE @SQL NVARCHAR(MAX);
            SET @SQL = N'UPDATE [dbo].[Cliente] SET [TelefonoClienteTemp] = CAST([TelefonoCliente] AS VARCHAR(15)) WHERE [TelefonoCliente] IS NOT NULL;';
            EXEC sp_executesql @SQL;
            PRINT 'Datos copiados a columna temporal';
            
            -- Eliminar columna vieja
            ALTER TABLE [dbo].[Cliente]
            DROP COLUMN [TelefonoCliente];
            PRINT 'Columna TelefonoCliente (INT) eliminada';
            
            -- Renombrar
            EXEC sp_rename '[dbo].[Cliente].[TelefonoClienteTemp]', 'TelefonoCliente', 'COLUMN';
            PRINT 'TelefonoCliente cambiado de INT a VARCHAR(15)';
        END
    END
END

-- 5. Aumentar tamaño de DireccionCliente
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Cliente') AND name = 'DireccionCliente')
BEGIN
    ALTER TABLE [dbo].[Cliente]
    ALTER COLUMN [DireccionCliente] NVARCHAR(200) NULL;
    PRINT 'Tamaño de DireccionCliente aumentado a 200';
END

-- 6. Aumentar tamaño de NombreCliente (por si el nombre completo no se puede separar)
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Cliente') AND name = 'NombreCliente')
BEGIN
    ALTER TABLE [dbo].[Cliente]
    ALTER COLUMN [NombreCliente] VARCHAR(100) NULL;
    PRINT 'Tamaño de NombreCliente aumentado a 100';
END

-- 7. Agregar Foreign Key a Empresas (si la tabla existe)
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Empresas')
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Cliente_Empresas')
    BEGIN
        ALTER TABLE [dbo].[Cliente]
        ADD CONSTRAINT [FK_Cliente_Empresas] 
        FOREIGN KEY ([IdEmpresa]) REFERENCES [dbo].[Empresas] ([IdEmpresa]);
        PRINT 'Foreign Key a Empresas agregada';
    END
    ELSE
        PRINT 'Foreign Key a Empresas ya existe';
END
ELSE
    PRINT 'Tabla Empresas no existe aún. Crear primero la tabla Empresas.';

PRINT 'Ajustes a tabla Cliente completados';


