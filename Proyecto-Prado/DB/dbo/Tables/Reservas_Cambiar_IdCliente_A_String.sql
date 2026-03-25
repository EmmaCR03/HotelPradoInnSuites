-- =============================================
-- Script para cambiar IdCliente de INT a NVARCHAR(128) en Reservas
-- Esto es necesario porque el modelo usa ApplicationUser (AspNetUsers)
-- que tiene Id como string (NVARCHAR(128))
-- =============================================

PRINT '========================================';
PRINT 'CAMBIANDO IdCliente A NVARCHAR(128)';
PRINT '========================================';
PRINT '';

-- Verificar estado actual
DECLARE @TipoActual VARCHAR(50);
DECLARE @RowCount INT;
DECLARE @TieneFKUsuarios BIT = 0;
DECLARE @TieneFKCliente BIT = 0;''

SELECT @TipoActual = t.name
FROM sys.columns c
INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
WHERE c.object_id = OBJECT_ID('dbo.Reservas') 
AND c.name = 'IdCliente';

SELECT @RowCount = COUNT(*) FROM [dbo].[Reservas];

SELECT @TieneFKUsuarios = CASE WHEN EXISTS (
    SELECT * FROM sys.foreign_keys WHERE name = 'FK_Reservas_Usuarios'
) THEN 1 ELSE 0 END;

SELECT @TieneFKCliente = CASE WHEN EXISTS (
    SELECT * FROM sys.foreign_keys WHERE name = 'FK_Reservas_Cliente'
) THEN 1 ELSE 0 END;

PRINT 'Estado actual:';
PRINT '  - Tipo de IdCliente: ' + ISNULL(@TipoActual, 'NO EXISTE');
PRINT '  - Registros en tabla: ' + CAST(@RowCount AS VARCHAR(10));
PRINT '  - Tiene FK a AspNetUsers: ' + CAST(@TieneFKUsuarios AS VARCHAR(1));
PRINT '  - Tiene FK a Cliente: ' + CAST(@TieneFKCliente AS VARCHAR(1));
PRINT '';

-- Si ya es NVARCHAR, verificar que todo esté bien
IF @TipoActual = 'nvarchar'
BEGIN
    PRINT '✓ IdCliente ya es NVARCHAR';
    
    IF @TieneFKCliente = 1
    BEGIN
        PRINT '⚠ Eliminando FK incorrecta a Cliente...';
        ALTER TABLE [dbo].[Reservas] DROP CONSTRAINT [FK_Reservas_Cliente];
        PRINT '✓ FK a Cliente eliminada';
    END
    
    IF @TieneFKUsuarios = 0
    BEGIN
        PRINT 'Agregando FK a AspNetUsers...';
        IF EXISTS (SELECT * FROM sys.tables WHERE name = 'AspNetUsers')
        BEGIN
            ALTER TABLE [dbo].[Reservas]
            ADD CONSTRAINT [FK_Reservas_Usuarios] 
            FOREIGN KEY ([IdCliente]) REFERENCES [dbo].[AspNetUsers] ([Id]);
            PRINT '✓ FK a AspNetUsers agregada';
        END
        ELSE
            PRINT '✗ ERROR: Tabla AspNetUsers no existe';
    END
    ELSE
        PRINT '✓ FK a AspNetUsers ya existe';
END
ELSE IF @TipoActual = 'int'
BEGIN
    PRINT 'IdCliente es INT - Necesita cambio a NVARCHAR(128)';
    PRINT '';
    
    -- Paso 1: Eliminar Foreign Key a Cliente si existe
    IF @TieneFKCliente = 1
    BEGIN
        PRINT 'Paso 1: Eliminando FK a Cliente...';
        ALTER TABLE [dbo].[Reservas] DROP CONSTRAINT [FK_Reservas_Cliente];
        PRINT '✓ FK eliminada';
    END
    ELSE
        PRINT 'Paso 1: No hay FK a Cliente';
    
    -- Paso 2: Eliminar datos si existen (porque no podemos convertir INT a string automáticamente)
    IF @RowCount > 0
    BEGIN
        PRINT 'Paso 2: ADVERTENCIA - La tabla tiene ' + CAST(@RowCount AS VARCHAR(10)) + ' registros';
        PRINT '        Los datos deben eliminarse o migrarse manualmente antes de cambiar el tipo.';
        PRINT '        ¿Deseas eliminar los datos? (Descomenta las líneas abajo)';
        PRINT '';
        /*
        -- Descomentar estas líneas si quieres eliminar los datos:
        DELETE FROM [dbo].[Reservas];
        PRINT '✓ Registros eliminados';
        */
    END
    ELSE
        PRINT 'Paso 2: Tabla ya está vacía';
    
    -- Paso 3: Cambiar tipo a NVARCHAR(128) (solo si la tabla está vacía)
    IF @RowCount = 0
    BEGIN
        PRINT 'Paso 3: Cambiando IdCliente a NVARCHAR(128)...';
        ALTER TABLE [dbo].[Reservas]
        ALTER COLUMN [IdCliente] NVARCHAR(128) NOT NULL;
        PRINT '✓ IdCliente cambiado a NVARCHAR(128)';
        
        -- Paso 4: Agregar Foreign Key a AspNetUsers
        IF EXISTS (SELECT * FROM sys.tables WHERE name = 'AspNetUsers')
        BEGIN
            IF @TieneFKUsuarios = 0
            BEGIN
                PRINT 'Paso 4: Agregando FK a AspNetUsers...';
                ALTER TABLE [dbo].[Reservas]
                ADD CONSTRAINT [FK_Reservas_Usuarios] 
                FOREIGN KEY ([IdCliente]) REFERENCES [dbo].[AspNetUsers] ([Id]);
                PRINT '✓ FK a AspNetUsers agregada';
            END
            ELSE
                PRINT 'Paso 4: FK a AspNetUsers ya existe';
        END
        ELSE
            PRINT 'Paso 4: ✗ ERROR - Tabla AspNetUsers no existe';
    END
    ELSE
    BEGIN
        PRINT 'Paso 3: ✗ ERROR - No se puede cambiar el tipo porque hay datos en la tabla';
        PRINT '        Por favor, elimina o migra los datos primero';
    END
END
ELSE
BEGIN
    PRINT '✗ ERROR: No se pudo determinar el tipo de IdCliente';
    PRINT 'Tipo encontrado: ' + ISNULL(@TipoActual, 'NULL');
END

PRINT '';
PRINT '========================================';
PRINT 'VERIFICACION FINAL';
PRINT '========================================';

-- Verificar resultado final
DECLARE @TipoFinal VARCHAR(50);
SELECT @TipoFinal = t.name
FROM sys.columns c
INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
WHERE c.object_id = OBJECT_ID('dbo.Reservas') 
AND c.name = 'IdCliente';

IF @TipoFinal = 'nvarchar'
    PRINT '✓ IdCliente es NVARCHAR(128) (CORRECTO)';
ELSE
    PRINT '✗ IdCliente es ' + ISNULL(@TipoFinal, 'NO EXISTE') + ' (INCORRECTO)';

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Reservas_Usuarios')
    PRINT '✓ FK a AspNetUsers existe';
ELSE
    PRINT '✗ FK a AspNetUsers NO existe';

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Reservas_Cliente')
    PRINT '✗ FK a Cliente AÚN existe (debe eliminarse)';
ELSE
    PRINT '✓ FK a Cliente NO existe (correcto)';

PRINT '';
PRINT '========================================';
PRINT 'PROCESO COMPLETADO';
PRINT '========================================';

