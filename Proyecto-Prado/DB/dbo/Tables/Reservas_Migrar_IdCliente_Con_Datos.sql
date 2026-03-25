-- =============================================
-- Script para migrar IdCliente de INT a NVARCHAR(128) CON DATOS
-- Migra los datos usando Cliente.IdUsuario
-- =============================================

PRINT '========================================';
PRINT 'MIGRANDO IdCliente CON DATOS';
PRINT '========================================';
PRINT '';

-- Verificar estado actual
DECLARE @TipoActual VARCHAR(50);
DECLARE @RowCount INT;
DECLARE @ClientesConUsuario INT;
DECLARE @ClientesSinUsuario INT;
DECLARE @ReservasConClienteValido INT;
DECLARE @ReservasSinClienteValido INT;

SELECT @TipoActual = t.name
FROM sys.columns c
INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
WHERE c.object_id = OBJECT_ID('dbo.Reservas') 
AND c.name = 'IdCliente';

SELECT @RowCount = COUNT(*) FROM [dbo].[Reservas];

-- Verificar si Cliente tiene IdUsuario
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Cliente') AND name = 'IdUsuario')
BEGIN
    PRINT '✗ ERROR: La tabla Cliente no tiene el campo IdUsuario';
    PRINT 'Por favor, ejecuta primero el script Cliente_Enlazar_AspNetUsers.sql';
    RETURN;
END

-- Contar cuántos clientes tienen IdUsuario
SELECT @ClientesConUsuario = COUNT(DISTINCT c.IdCliente)
FROM [dbo].[Cliente] c
INNER JOIN [dbo].[Reservas] r ON r.IdCliente = c.IdCliente
WHERE c.IdUsuario IS NOT NULL;

SELECT @ClientesSinUsuario = COUNT(DISTINCT c.IdCliente)
FROM [dbo].[Cliente] c
INNER JOIN [dbo].[Reservas] r ON r.IdCliente = c.IdCliente
WHERE c.IdUsuario IS NULL;

-- Contar reservas que pueden migrarse
SELECT @ReservasConClienteValido = COUNT(*)
FROM [dbo].[Reservas] r
INNER JOIN [dbo].[Cliente] c ON r.IdCliente = c.IdCliente
WHERE c.IdUsuario IS NOT NULL;

SELECT @ReservasSinClienteValido = COUNT(*)
FROM [dbo].[Reservas] r
INNER JOIN [dbo].[Cliente] c ON r.IdCliente = c.IdCliente
WHERE c.IdUsuario IS NULL;

PRINT 'Estado actual:';
PRINT '  - Tipo de IdCliente: ' + ISNULL(@TipoActual, 'NO EXISTE');
PRINT '  - Total de reservas: ' + CAST(@RowCount AS VARCHAR(10));
PRINT '  - Reservas con cliente vinculado a usuario: ' + CAST(@ReservasConClienteValido AS VARCHAR(10));
PRINT '  - Reservas con cliente SIN usuario: ' + CAST(@ReservasSinClienteValido AS VARCHAR(10));
PRINT '  - Clientes únicos con usuario: ' + CAST(@ClientesConUsuario AS VARCHAR(10));
PRINT '  - Clientes únicos sin usuario: ' + CAST(@ClientesSinUsuario AS VARCHAR(10));
PRINT '';

IF @TipoActual != 'int'
BEGIN
    PRINT '✗ ERROR: IdCliente no es INT. Tipo actual: ' + ISNULL(@TipoActual, 'NULL');
    PRINT 'Este script solo funciona cuando IdCliente es INT';
    RETURN;
END

IF @ReservasSinClienteValido > 0
BEGIN
    PRINT '⚠ ADVERTENCIA: Hay ' + CAST(@ReservasSinClienteValido AS VARCHAR(10)) + ' reservas con clientes que NO tienen usuario vinculado.';
    PRINT 'Estas reservas se marcarán con un valor especial o se eliminarán.';
    PRINT '';
    PRINT 'Opciones:';
    PRINT '1. Crear usuarios para estos clientes primero';
    PRINT '2. Eliminar estas reservas';
    PRINT '3. Continuar y dejar IdCliente como NULL para estas reservas';
    PRINT '';
END

-- Paso 1: Crear columna temporal
PRINT 'Paso 1: Creando columna temporal IdCliente_Nuevo...';

-- Eliminar columna temporal si existe (para empezar limpio)
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Reservas') AND name = 'IdCliente_Nuevo')
BEGIN
    PRINT 'Eliminando columna temporal existente...';
    ALTER TABLE [dbo].[Reservas] DROP COLUMN [IdCliente_Nuevo];
END

-- Crear la columna temporal
ALTER TABLE [dbo].[Reservas]
ADD [IdCliente_Nuevo] NVARCHAR(128) NULL;

PRINT '✓ Columna temporal creada';
GO

-- Paso 2: Migrar datos
PRINT '';
PRINT 'Paso 2: Migrando datos de IdCliente (INT) a IdCliente_Nuevo (NVARCHAR)...';
PRINT 'Esto puede tardar varios minutos si hay muchos registros...';

DECLARE @RegistrosMigrados INT;

UPDATE r
SET r.[IdCliente_Nuevo] = c.[IdUsuario]
FROM [dbo].[Reservas] r
INNER JOIN [dbo].[Cliente] c ON r.IdCliente = c.IdCliente
WHERE c.IdUsuario IS NOT NULL;

SET @RegistrosMigrados = @@ROWCOUNT;
PRINT '✓ ' + CAST(@RegistrosMigrados AS VARCHAR(10)) + ' registros migrados exitosamente';

-- Verificar cuántos quedaron sin migrar
DECLARE @SinMigrar INT;
SELECT @SinMigrar = COUNT(*) 
FROM [dbo].[Reservas] 
WHERE [IdCliente_Nuevo] IS NULL;

IF @SinMigrar > 0
BEGIN
    PRINT '⚠ ' + CAST(@SinMigrar AS VARCHAR(10)) + ' registros NO pudieron migrarse (cliente sin usuario)';
    PRINT 'Estos registros tendrán IdCliente_Nuevo = NULL';
END

-- Paso 3: Verificar integridad de datos
PRINT '';
PRINT 'Paso 3: Verificando integridad de datos...';
DECLARE @RegistrosInvalidos INT;
SELECT @RegistrosInvalidos = COUNT(*)
FROM [dbo].[Reservas] r
WHERE r.[IdCliente_Nuevo] IS NOT NULL
AND NOT EXISTS (
    SELECT 1 FROM [dbo].[AspNetUsers] u 
    WHERE u.Id = r.[IdCliente_Nuevo]
);

IF @RegistrosInvalidos > 0
BEGIN
    PRINT '✗ ERROR: ' + CAST(@RegistrosInvalidos AS VARCHAR(10)) + ' registros tienen IdCliente_Nuevo que no existe en AspNetUsers';
    PRINT 'Por favor, revisa los datos antes de continuar';
    RETURN;
END
ELSE
    PRINT '✓ Todos los IdCliente_Nuevo válidos existen en AspNetUsers';
GO

-- Paso 4: Eliminar Foreign Key antigua si existe
PRINT '';
PRINT 'Paso 4: Eliminando Foreign Key antigua...';
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Reservas_Cliente')
BEGIN
    ALTER TABLE [dbo].[Reservas] DROP CONSTRAINT [FK_Reservas_Cliente];
    PRINT '✓ Foreign Key FK_Reservas_Cliente eliminada';
END
ELSE
    PRINT 'No hay Foreign Key antigua que eliminar';

-- Paso 5: Eliminar columna antigua y renombrar nueva
PRINT '';
PRINT 'Paso 5: Reemplazando columna antigua por la nueva...';
PRINT 'Esto puede tardar varios minutos...';

-- Eliminar columna antigua
ALTER TABLE [dbo].[Reservas] DROP COLUMN [IdCliente];
GO

-- Renombrar columna nueva
PRINT 'Renombrando columna temporal...';
EXEC sp_rename '[dbo].[Reservas].[IdCliente_Nuevo]', 'IdCliente', 'COLUMN';
PRINT '✓ Columna reemplazada exitosamente';
GO

-- Paso 6: Agregar Foreign Key nueva
PRINT '';
PRINT 'Paso 6: Agregando Foreign Key a AspNetUsers...';
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'AspNetUsers')
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Reservas_Usuarios')
    BEGIN
        -- Primero, hacer la columna NOT NULL si todos los registros tienen valor
        IF NOT EXISTS (SELECT 1 FROM [dbo].[Reservas] WHERE [IdCliente] IS NULL)
        BEGIN
            ALTER TABLE [dbo].[Reservas]
            ALTER COLUMN [IdCliente] NVARCHAR(128) NOT NULL;
            PRINT '  - Columna IdCliente establecida como NOT NULL';
        END
        
        ALTER TABLE [dbo].[Reservas]
        ADD CONSTRAINT [FK_Reservas_Usuarios] 
        FOREIGN KEY ([IdCliente]) REFERENCES [dbo].[AspNetUsers] ([Id]);
        PRINT '✓ Foreign Key FK_Reservas_Usuarios agregada';
    END
    ELSE
        PRINT 'Foreign Key FK_Reservas_Usuarios ya existe';
END
ELSE
    PRINT '✗ ERROR: Tabla AspNetUsers no existe';

-- Verificación final
PRINT '';
PRINT '========================================';
PRINT 'VERIFICACION FINAL';
PRINT '========================================';

DECLARE @TipoFinal VARCHAR(50);
DECLARE @ReservasConNull INT;
DECLARE @RowCountFinal INT;

SELECT @TipoFinal = t.name
FROM sys.columns c
INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
WHERE c.object_id = OBJECT_ID('dbo.Reservas') 
AND c.name = 'IdCliente';

SELECT @RowCountFinal = COUNT(*) FROM [dbo].[Reservas];
SELECT @ReservasConNull = COUNT(*) FROM [dbo].[Reservas] WHERE [IdCliente] IS NULL;

IF @TipoFinal = 'nvarchar'
    PRINT '✓ IdCliente es NVARCHAR(128) (CORRECTO)';
ELSE
    PRINT '✗ IdCliente es ' + ISNULL(@TipoFinal, 'NO EXISTE') + ' (INCORRECTO)';

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Reservas_Usuarios')
    PRINT '✓ FK a AspNetUsers existe';
ELSE
    PRINT '✗ FK a AspNetUsers NO existe';

IF @ReservasConNull > 0
    PRINT '⚠ ADVERTENCIA: ' + CAST(@ReservasConNull AS VARCHAR(10)) + ' reservas tienen IdCliente = NULL';
ELSE
    PRINT '✓ Todas las reservas tienen IdCliente válido';

PRINT '';
PRINT '========================================';
PRINT 'MIGRACION COMPLETADA';
PRINT '========================================';
PRINT '';
PRINT 'Resumen:';
PRINT '  - Total de reservas: ' + CAST(@RowCountFinal AS VARCHAR(10));
IF @ReservasConNull > 0
    PRINT '  - Reservas con IdCliente NULL: ' + CAST(@ReservasConNull AS VARCHAR(10));

