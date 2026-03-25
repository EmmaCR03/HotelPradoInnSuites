-- =============================================
-- Script para eliminar datos y cambiar IdCliente a INT
-- ADVERTENCIA: Este script ELIMINARÁ todos los datos de la tabla Reservas
-- =============================================

PRINT '========================================';
PRINT 'ELIMINANDO DATOS Y CAMBIANDO IdCliente';
PRINT '========================================';
PRINT '';

-- Verificar estado actual
DECLARE @RowCount INT;
SELECT @RowCount = COUNT(*) FROM [dbo].[Reservas];
PRINT 'Registros a eliminar: ' + CAST(@RowCount AS VARCHAR(10));
PRINT '';

-- Paso 1: Eliminar Foreign Key a AspNetUsers si existe
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Reservas_Usuarios')
BEGIN
    ALTER TABLE [dbo].[Reservas] DROP CONSTRAINT [FK_Reservas_Usuarios];
    PRINT 'Paso 1: Foreign Key FK_Reservas_Usuarios eliminada';
END
ELSE
    PRINT 'Paso 1: No hay Foreign Key a AspNetUsers';

-- Paso 2: Eliminar todos los datos
DELETE FROM [dbo].[Reservas];
PRINT 'Paso 2: ' + CAST(@RowCount AS VARCHAR(10)) + ' registros eliminados';

-- Paso 3: Cambiar IdCliente de NVARCHAR a INT
IF EXISTS (
    SELECT 1 
    FROM sys.columns c
    INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
    WHERE c.object_id = OBJECT_ID('dbo.Reservas') 
    AND c.name = 'IdCliente'
    AND (t.name = 'nvarchar' OR t.name = 'varchar')
)
BEGIN
    ALTER TABLE [dbo].[Reservas]
    ALTER COLUMN [IdCliente] INT NULL;
    PRINT 'Paso 3: IdCliente cambiado de NVARCHAR a INT';
END
ELSE
    PRINT 'Paso 3: IdCliente ya es INT o no existe';

-- Paso 4: Agregar Foreign Key a Cliente
IF EXISTS (
    SELECT 1 
    FROM sys.columns c
    INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
    WHERE c.object_id = OBJECT_ID('dbo.Reservas') 
    AND c.name = 'IdCliente'
    AND t.name = 'int'
)
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Reservas_Cliente')
    BEGIN
        IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Cliente')
        BEGIN
            ALTER TABLE [dbo].[Reservas]
            ADD CONSTRAINT [FK_Reservas_Cliente] 
            FOREIGN KEY ([IdCliente]) REFERENCES [dbo].[Cliente] ([IdCliente]);
            PRINT 'Paso 4: Foreign Key FK_Reservas_Cliente agregada';
        END
        ELSE
            PRINT 'Paso 4: ERROR - Tabla Cliente no existe';
    END
    ELSE
        PRINT 'Paso 4: Foreign Key FK_Reservas_Cliente ya existe';
END
ELSE
    PRINT 'Paso 4: No se puede agregar Foreign Key - IdCliente no es INT';

-- Paso 5: Agregar Foreign Key a Empresas (si existe la tabla)
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Empresas')
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Reservas_Empresas')
    BEGIN
        ALTER TABLE [dbo].[Reservas]
        ADD CONSTRAINT [FK_Reservas_Empresas] 
        FOREIGN KEY ([IdEmpresa]) REFERENCES [dbo].[Empresas] ([IdEmpresa]);
        PRINT 'Paso 5: Foreign Key FK_Reservas_Empresas agregada';
    END
    ELSE
        PRINT 'Paso 5: Foreign Key FK_Reservas_Empresas ya existe';
END
ELSE
    PRINT 'Paso 5: Tabla Empresas no existe (opcional)';

PRINT '';
PRINT '========================================';
PRINT 'PROCESO COMPLETADO';
PRINT '========================================';
PRINT '';
PRINT 'La tabla Reservas ahora está lista para recibir datos migrados.';
PRINT 'IdCliente es INT y está relacionado con la tabla Cliente.';


