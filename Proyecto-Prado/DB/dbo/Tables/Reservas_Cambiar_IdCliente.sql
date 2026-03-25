-- =============================================
-- Script para cambiar IdCliente de NVARCHAR a INT en Reservas
-- =============================================

PRINT 'Verificando estado actual de IdCliente...';
PRINT '';

-- Verificar tipo actual y cantidad de datos
DECLARE @TipoActual VARCHAR(50);
DECLARE @RowCount INT;
DECLARE @TieneFKUsuarios BIT = 0;

SELECT @TipoActual = t.name + '(' + CAST(c.max_length AS VARCHAR(10)) + ')'
FROM sys.columns c
INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
WHERE c.object_id = OBJECT_ID('dbo.Reservas') 
AND c.name = 'IdCliente';

SELECT @RowCount = COUNT(*) FROM [dbo].[Reservas];

SELECT @TieneFKUsuarios = CASE WHEN EXISTS (
    SELECT * FROM sys.foreign_keys WHERE name = 'FK_Reservas_Usuarios'
) THEN 1 ELSE 0 END;

PRINT 'Estado actual:';
PRINT '  - Tipo de IdCliente: ' + ISNULL(@TipoActual, 'NO EXISTE');
PRINT '  - Registros en tabla: ' + CAST(@RowCount AS VARCHAR(10));
PRINT '  - Tiene FK a AspNetUsers: ' + CAST(@TieneFKUsuarios AS VARCHAR(1));
PRINT '';

-- Si es NVARCHAR, proceder con el cambio
IF @TipoActual LIKE 'nvarchar%' OR @TipoActual LIKE 'varchar%'
BEGIN
    -- Paso 1: Eliminar Foreign Key a AspNetUsers si existe
    IF @TieneFKUsuarios = 1
    BEGIN
        ALTER TABLE [dbo].[Reservas] DROP CONSTRAINT [FK_Reservas_Usuarios];
        PRINT 'Paso 1: Foreign Key FK_Reservas_Usuarios eliminada';
    END
    ELSE
        PRINT 'Paso 1: No hay Foreign Key a AspNetUsers';
    
    -- Paso 2: Verificar si hay datos
    IF @RowCount = 0
    BEGIN
        -- Tabla vacía, cambiar directamente
        ALTER TABLE [dbo].[Reservas]
        ALTER COLUMN [IdCliente] INT NULL;
        PRINT 'Paso 2: IdCliente cambiado a INT (tabla vacía)';
    END
    ELSE
    BEGIN
        -- Tabla tiene datos - ADVERTENCIA
        PRINT 'Paso 2: ADVERTENCIA - La tabla tiene ' + CAST(@RowCount AS VARCHAR(10)) + ' registros';
        PRINT '        No se puede cambiar directamente. Necesitas:';
        PRINT '        1. Migrar los datos primero (relacionar por nombre/cedula)';
        PRINT '        2. O eliminar los datos existentes';
        PRINT '';
        PRINT '        ¿Deseas eliminar los datos existentes? (Descomenta las líneas abajo)';
        /*
        -- Descomentar estas líneas si quieres eliminar los datos y cambiar el tipo:
        DELETE FROM [dbo].[Reservas];
        ALTER TABLE [dbo].[Reservas]
        ALTER COLUMN [IdCliente] INT NULL;
        PRINT 'Datos eliminados e IdCliente cambiado a INT';
        */
    END
    
    -- Paso 3: Agregar Foreign Key a Cliente (solo si IdCliente es INT)
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
                PRINT 'Paso 3: Foreign Key FK_Reservas_Cliente agregada';
            END
            ELSE
                PRINT 'Paso 3: Tabla Cliente no existe';
        END
        ELSE
            PRINT 'Paso 3: Foreign Key FK_Reservas_Cliente ya existe';
    END
    ELSE
        PRINT 'Paso 3: IdCliente aún no es INT, no se puede agregar Foreign Key';
END
ELSE IF @TipoActual LIKE 'int%'
BEGIN
    PRINT 'IdCliente ya es INT - No se requiere cambio';
    
    -- Solo verificar Foreign Key
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
ELSE
    PRINT 'ERROR: No se pudo determinar el tipo de IdCliente';

PRINT '';
PRINT 'Verificación completada';


