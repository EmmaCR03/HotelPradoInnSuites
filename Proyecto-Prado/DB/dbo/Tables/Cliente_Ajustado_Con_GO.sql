-- =============================================
-- Script para ajustar tabla Cliente (VERSIÓN CON GO - Ejecutar todo junto)
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
GO

-- 2. Agregar campo para relación con Empresas
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Cliente') AND name = 'IdEmpresa')
BEGIN
    ALTER TABLE [dbo].[Cliente]
    ADD [IdEmpresa] INT NULL;
    PRINT 'Campo IdEmpresa agregado';
END
ELSE
    PRINT 'Campo IdEmpresa ya existe';
GO

-- 3. Aumentar tamaño de EmailCliente
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Cliente') AND name = 'EmailCliente')
BEGIN
    ALTER TABLE [dbo].[Cliente]
    ALTER COLUMN [EmailCliente] NVARCHAR(100) NULL;
    PRINT 'Tamaño de EmailCliente aumentado a 100';
END
GO

-- 4. Cambiar TelefonoCliente a VARCHAR
-- Primero verificar si hay una columna temporal de una ejecución anterior
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Cliente') AND name = 'TelefonoClienteTemp')
BEGIN
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Cliente') AND name = 'TelefonoCliente')
    BEGIN
        ALTER TABLE [dbo].[Cliente] DROP COLUMN [TelefonoCliente];
    END
    EXEC sp_rename '[dbo].[Cliente].[TelefonoClienteTemp]', 'TelefonoCliente', 'COLUMN';
    PRINT 'TelefonoCliente cambiado a VARCHAR(15) (limpiando ejecución anterior)';
END
ELSE IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Cliente') AND name = 'TelefonoCliente')
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
    
    IF @EsVarchar = 0 AND @RowCount = 0
    BEGIN
        ALTER TABLE [dbo].[Cliente]
        ALTER COLUMN [TelefonoCliente] VARCHAR(15) NULL;
        PRINT 'TelefonoCliente cambiado de INT a VARCHAR(15) (tabla vacía)';
    END
    ELSE IF @EsVarchar = 0 AND @RowCount > 0
    BEGIN
        ALTER TABLE [dbo].[Cliente]
        ADD [TelefonoClienteTemp] VARCHAR(15) NULL;
        PRINT 'Columna temporal creada';
    END
    ELSE IF @EsVarchar = 1 AND @MaxLen < 15
    BEGIN
        ALTER TABLE [dbo].[Cliente]
        ALTER COLUMN [TelefonoCliente] VARCHAR(15) NULL;
        PRINT 'Tamaño de TelefonoCliente aumentado a 15';
    END
END
GO

-- 4b. Copiar datos a columna temporal (solo si existe la temporal Y la columna original es INT)
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Cliente') AND name = 'TelefonoClienteTemp')
BEGIN
    -- Verificar que la columna original todavía existe y es INT
    IF EXISTS (
        SELECT 1 
        FROM sys.columns c
        INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
        WHERE c.object_id = OBJECT_ID('dbo.Cliente') 
        AND c.name = 'TelefonoCliente'
        AND t.name != 'varchar'
    )
    BEGIN
        UPDATE [dbo].[Cliente]
        SET [TelefonoClienteTemp] = CAST([TelefonoCliente] AS VARCHAR(15))
        WHERE [TelefonoCliente] IS NOT NULL;
        PRINT 'Datos copiados a columna temporal';
    END
    ELSE
        PRINT 'TelefonoCliente ya fue cambiado a VARCHAR, no se requiere copia de datos';
END
ELSE
    PRINT 'No se requiere migración de TelefonoCliente (ya es VARCHAR o tabla estaba vacía)';
GO

-- 4c. Eliminar columna vieja y renombrar (solo si existe la temporal Y la columna original todavía es INT)
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Cliente') AND name = 'TelefonoClienteTemp')
BEGIN
    -- Verificar que la columna original todavía existe y NO es VARCHAR
    IF EXISTS (
        SELECT 1 
        FROM sys.columns c
        INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
        WHERE c.object_id = OBJECT_ID('dbo.Cliente') 
        AND c.name = 'TelefonoCliente'
        AND t.name != 'varchar'
    )
    BEGIN
        ALTER TABLE [dbo].[Cliente]
        DROP COLUMN [TelefonoCliente];
        PRINT 'Columna TelefonoCliente (INT) eliminada';
        
        EXEC sp_rename '[dbo].[Cliente].[TelefonoClienteTemp]', 'TelefonoCliente', 'COLUMN';
        PRINT 'TelefonoCliente cambiado de INT a VARCHAR(15)';
    END
    ELSE
    BEGIN
        -- La columna original ya fue cambiada, eliminar la temporal
        ALTER TABLE [dbo].[Cliente]
        DROP COLUMN [TelefonoClienteTemp];
        PRINT 'Columna temporal eliminada (no se requiere)';
    END
END
GO

-- 5. Aumentar tamaño de DireccionCliente
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Cliente') AND name = 'DireccionCliente')
BEGIN
    ALTER TABLE [dbo].[Cliente]
    ALTER COLUMN [DireccionCliente] NVARCHAR(200) NULL;
    PRINT 'Tamaño de DireccionCliente aumentado a 200';
END
GO

-- 6. Aumentar tamaño de NombreCliente
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Cliente') AND name = 'NombreCliente')
BEGIN
    ALTER TABLE [dbo].[Cliente]
    ALTER COLUMN [NombreCliente] VARCHAR(100) NULL;
    PRINT 'Tamaño de NombreCliente aumentado a 100';
END
GO

-- 7. Agregar Foreign Key a Empresas
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
    PRINT 'Tabla Empresas no existe aún.';
GO

PRINT 'Ajustes a tabla Cliente completados';

