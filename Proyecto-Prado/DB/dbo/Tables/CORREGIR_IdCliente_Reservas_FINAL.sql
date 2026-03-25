-- =============================================
-- Script FINAL para corregir IdCliente en Reservas
-- Este script verifica y corrige todo lo necesario
-- =============================================

PRINT '========================================';
PRINT 'CORRIGIENDO IdCliente EN RESERVAS';
PRINT '========================================';
PRINT '';

-- Verificar estado actual
DECLARE @TipoActual VARCHAR(50);
DECLARE @RowCount INT;
DECLARE @TieneFKUsuarios BIT = 0;
DECLARE @TieneFKCliente BIT = 0;

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

-- Si ya es INT, verificar que todo esté bien
IF @TipoActual = 'int'
BEGIN
    PRINT '✓ IdCliente ya es INT';
    
    IF @TieneFKUsuarios = 1
    BEGIN
        PRINT '⚠ Eliminando FK incorrecta a AspNetUsers...';
        ALTER TABLE [dbo].[Reservas] DROP CONSTRAINT [FK_Reservas_Usuarios];
        PRINT '✓ FK a AspNetUsers eliminada';
    END
    
    IF @TieneFKCliente = 0
    BEGIN
        PRINT 'Agregando FK a Cliente...';
        IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Cliente')
        BEGIN
            ALTER TABLE [dbo].[Reservas]
            ADD CONSTRAINT [FK_Reservas_Cliente] 
            FOREIGN KEY ([IdCliente]) REFERENCES [dbo].[Cliente] ([IdCliente]);
            PRINT '✓ FK a Cliente agregada';
        END
        ELSE
            PRINT '✗ ERROR: Tabla Cliente no existe';
    END
    ELSE
        PRINT '✓ FK a Cliente ya existe';
END
ELSE IF @TipoActual IN ('nvarchar', 'varchar')
BEGIN
    PRINT 'IdCliente es ' + @TipoActual + ' - Necesita cambio a INT';
    PRINT '';
    
    -- Paso 1: Eliminar Foreign Key a AspNetUsers
    IF @TieneFKUsuarios = 1
    BEGIN
        PRINT 'Paso 1: Eliminando FK a AspNetUsers...';
        ALTER TABLE [dbo].[Reservas] DROP CONSTRAINT [FK_Reservas_Usuarios];
        PRINT '✓ FK eliminada';
    END
    ELSE
        PRINT 'Paso 1: No hay FK a AspNetUsers';
    
    -- Paso 2: Eliminar datos si existen
    IF @RowCount > 0
    BEGIN
        PRINT 'Paso 2: Eliminando ' + CAST(@RowCount AS VARCHAR(10)) + ' registros...';
        DELETE FROM [dbo].[Reservas];
        PRINT '✓ Registros eliminados';
    END
    ELSE
        PRINT 'Paso 2: Tabla ya está vacía';
    
    -- Paso 3: Cambiar tipo a INT
    PRINT 'Paso 3: Cambiando IdCliente a INT...';
    ALTER TABLE [dbo].[Reservas]
    ALTER COLUMN [IdCliente] INT NULL;
    PRINT '✓ IdCliente cambiado a INT';
    
    -- Paso 4: Agregar Foreign Key a Cliente
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Cliente')
    BEGIN
        IF @TieneFKCliente = 0
        BEGIN
            PRINT 'Paso 4: Agregando FK a Cliente...';
            ALTER TABLE [dbo].[Reservas]
            ADD CONSTRAINT [FK_Reservas_Cliente] 
            FOREIGN KEY ([IdCliente]) REFERENCES [dbo].[Cliente] ([IdCliente]);
            PRINT '✓ FK a Cliente agregada';
        END
        ELSE
            PRINT 'Paso 4: FK a Cliente ya existe';
    END
    ELSE
        PRINT 'Paso 4: ✗ ERROR - Tabla Cliente no existe';
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

IF @TipoFinal = 'int'
    PRINT '✓ IdCliente es INT (CORRECTO)';
ELSE
    PRINT '✗ IdCliente es ' + ISNULL(@TipoFinal, 'NO EXISTE') + ' (INCORRECTO)';

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Reservas_Cliente')
    PRINT '✓ FK a Cliente existe';
ELSE
    PRINT '✗ FK a Cliente NO existe';

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Reservas_Usuarios')
    PRINT '✗ FK a AspNetUsers AÚN existe (debe eliminarse)';
ELSE
    PRINT '✓ FK a AspNetUsers NO existe (correcto)';

PRINT '';
PRINT '========================================';
PRINT 'PROCESO COMPLETADO';
PRINT '========================================';


