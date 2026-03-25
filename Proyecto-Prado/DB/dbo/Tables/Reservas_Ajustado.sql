-- =============================================
-- Script para ajustar tabla Reservas para migración
-- =============================================

-- 1. ELIMINAR la foreign key a AspNetUsers (si existe)
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Reservas_Usuarios')
BEGIN
    ALTER TABLE [dbo].[Reservas] DROP CONSTRAINT [FK_Reservas_Usuarios];
    PRINT 'Foreign Key FK_Reservas_Usuarios eliminada';
END

-- 2. Agregar campos faltantes
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Reservas') AND name = 'CedulaCliente')
BEGIN
    ALTER TABLE [dbo].[Reservas]
    ADD [CedulaCliente] VARCHAR(16) NULL;
    PRINT 'Campo CedulaCliente agregado';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Reservas') AND name = 'TelefonoCliente')
BEGIN
    ALTER TABLE [dbo].[Reservas]
    ADD [TelefonoCliente] VARCHAR(15) NULL;
    PRINT 'Campo TelefonoCliente agregado';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Reservas') AND name = 'DireccionCliente')
BEGIN
    ALTER TABLE [dbo].[Reservas]
    ADD [DireccionCliente] NVARCHAR(200) NULL;
    PRINT 'Campo DireccionCliente agregado';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Reservas') AND name = 'EmailCliente')
BEGIN
    ALTER TABLE [dbo].[Reservas]
    ADD [EmailCliente] NVARCHAR(100) NULL;
    PRINT 'Campo EmailCliente agregado';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Reservas') AND name = 'NumeroAdultos')
BEGIN
    ALTER TABLE [dbo].[Reservas]
    ADD [NumeroAdultos] INT NULL;
    PRINT 'Campo NumeroAdultos agregado';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Reservas') AND name = 'NumeroNinos')
BEGIN
    ALTER TABLE [dbo].[Reservas]
    ADD [NumeroNinos] INT NULL;
    PRINT 'Campo NumeroNinos agregado';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Reservas') AND name = 'IdEmpresa')
BEGIN
    ALTER TABLE [dbo].[Reservas]
    ADD [IdEmpresa] INT NULL;
    PRINT 'Campo IdEmpresa agregado';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Reservas') AND name = 'Observaciones')
BEGIN
    ALTER TABLE [dbo].[Reservas]
    ADD [Observaciones] NVARCHAR(500) NULL;
    PRINT 'Campo Observaciones agregado';
END

-- 3. Cambiar IdCliente de NVARCHAR(128) a INT
-- NOTA: Esto requiere migrar los datos primero. Este script solo prepara la estructura.
-- Después de migrar datos, ejecutar la parte comentada abajo.

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Reservas') AND name = 'IdCliente' AND system_type_id = 231) -- NVARCHAR
BEGIN
    -- Verificar si hay datos
    IF NOT EXISTS (SELECT TOP 1 * FROM [dbo].[Reservas])
    BEGIN
        -- Si no hay datos, podemos cambiar directamente
        ALTER TABLE [dbo].[Reservas]
        ALTER COLUMN [IdCliente] INT NULL;
        PRINT 'IdCliente cambiado a INT (tabla estaba vacía)';
    END
    ELSE
    BEGIN
        -- Si hay datos, necesitamos migrarlos primero
        PRINT 'ADVERTENCIA: La tabla Reservas tiene datos.';
        PRINT 'Necesitas migrar los datos primero antes de cambiar IdCliente a INT.';
        PRINT 'Pasos:';
        PRINT '1. Crear columna temporal IdClienteTemp INT';
        PRINT '2. Migrar datos relacionando por nombre/cedula';
        PRINT '3. Eliminar columna vieja y renombrar';
    END
END

-- 4. Agregar Foreign Key a Cliente (después de cambiar IdCliente a INT)
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Reservas') AND name = 'IdCliente' AND system_type_id = 56) -- INT
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Reservas_Cliente')
    BEGIN
        IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Cliente')
        BEGIN
            ALTER TABLE [dbo].[Reservas]
            ADD CONSTRAINT [FK_Reservas_Cliente] 
            FOREIGN KEY ([IdCliente]) REFERENCES [dbo].[Cliente] ([IdCliente]);
            PRINT 'Foreign Key FK_Reservas_Cliente agregada';
        END
    END
    ELSE
        PRINT 'Foreign Key FK_Reservas_Cliente ya existe';
END

-- 5. Agregar Foreign Key a Empresas
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Empresas')
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Reservas_Empresas')
    BEGIN
        ALTER TABLE [dbo].[Reservas]
        ADD CONSTRAINT [FK_Reservas_Empresas] 
        FOREIGN KEY ([IdEmpresa]) REFERENCES [dbo].[Empresas] ([IdEmpresa]);
        PRINT 'Foreign Key FK_Reservas_Empresas agregada';
    END
    ELSE
        PRINT 'Foreign Key FK_Reservas_Empresas ya existe';
END

PRINT 'Ajustes a tabla Reservas completados';


