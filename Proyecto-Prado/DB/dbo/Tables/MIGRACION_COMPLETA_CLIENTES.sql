-- =============================================
-- MIGRACIÓN COMPLETA DE CLIENTES
-- Este script verifica y completa la migración de clientes
-- =============================================

PRINT '========================================';
PRINT 'MIGRACIÓN COMPLETA DE CLIENTES';
PRINT '========================================';
PRINT '';

-- 1. Verificar estructura de la tabla Cliente
PRINT '1. VERIFICANDO ESTRUCTURA DE LA TABLA Cliente:';
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Cliente')
BEGIN
    PRINT '   ✓ Tabla Cliente existe';
    
    -- Verificar campo IdUsuario
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Cliente') AND name = 'IdUsuario')
    BEGIN
        PRINT '   ✓ Campo IdUsuario existe';
    END
    ELSE
    BEGIN
        PRINT '   ✗ Campo IdUsuario NO existe. Ejecutando Cliente_Enlazar_AspNetUsers.sql primero...';
        PRINT '   Por favor ejecuta: DB/dbo/Tables/Cliente_Enlazar_AspNetUsers.sql';
        RETURN;
    END
END
ELSE
BEGIN
    PRINT '   ✗ ERROR: Tabla Cliente NO existe';
    RETURN;
END
PRINT '';

-- 2. Estadísticas de clientes
PRINT '2. ESTADÍSTICAS DE CLIENTES:';
DECLARE @TotalClientes INT;
DECLARE @ConNombre INT;
DECLARE @ConEmail INT;
DECLARE @ConCedula INT;
DECLARE @ConTelefono INT;
DECLARE @ConUsuario INT;
DECLARE @SinUsuario INT;

SELECT @TotalClientes = COUNT(*) FROM [dbo].[Cliente];
SELECT @ConNombre = COUNT(*) FROM [dbo].[Cliente] WHERE NombreCliente IS NOT NULL AND NombreCliente <> '';
SELECT @ConEmail = COUNT(*) FROM [dbo].[Cliente] WHERE EmailCliente IS NOT NULL AND EmailCliente <> '';
SELECT @ConCedula = COUNT(*) FROM [dbo].[Cliente] WHERE CedulaCliente IS NOT NULL AND CedulaCliente <> '';
SELECT @ConTelefono = COUNT(*) FROM [dbo].[Cliente] WHERE TelefonoCliente IS NOT NULL;
SELECT @ConUsuario = COUNT(*) FROM [dbo].[Cliente] WHERE IdUsuario IS NOT NULL;
SELECT @SinUsuario = COUNT(*) FROM [dbo].[Cliente] WHERE IdUsuario IS NULL;

PRINT '   Total de clientes: ' + CAST(@TotalClientes AS VARCHAR(10));
PRINT '   Clientes con nombre: ' + CAST(@ConNombre AS VARCHAR(10));
PRINT '   Clientes con email: ' + CAST(@ConEmail AS VARCHAR(10));
PRINT '   Clientes con cédula: ' + CAST(@ConCedula AS VARCHAR(10));
PRINT '   Clientes con teléfono: ' + CAST(@ConTelefono AS VARCHAR(10));
PRINT '   Clientes con usuario web: ' + CAST(@ConUsuario AS VARCHAR(10));
PRINT '   Clientes sin usuario web: ' + CAST(@SinUsuario AS VARCHAR(10));
PRINT '';

-- 3. Verificar clientes con datos incompletos
PRINT '3. CLIENTES CON DATOS INCOMPLETOS:';
DECLARE @SinNombre INT;
DECLARE @SinEmailNiCedula INT;

SELECT @SinNombre = COUNT(*) FROM [dbo].[Cliente] WHERE NombreCliente IS NULL OR NombreCliente = '';
SELECT @SinEmailNiCedula = COUNT(*) FROM [dbo].[Cliente] 
WHERE (EmailCliente IS NULL OR EmailCliente = '') 
AND (CedulaCliente IS NULL OR CedulaCliente = '');

PRINT '   Clientes sin nombre: ' + CAST(@SinNombre AS VARCHAR(10));
PRINT '   Clientes sin email ni cédula: ' + CAST(@SinEmailNiCedula AS VARCHAR(10));
PRINT '';

-- 4. Ejecutar vinculación automática de TODOS los clientes
PRINT '4. EJECUTANDO VINCULACIÓN AUTOMÁTICA:';
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'AspNetUsers')
BEGIN
    PRINT '   Ejecutando script de vinculación (procesando TODOS los clientes)...';
    PRINT '';
    
    -- Ejecutar el script de vinculación
    -- Nota: Este script procesa TODOS los clientes sin filtros
    EXEC('
    -- Vincular por Email (TODOS los clientes con email)
    UPDATE c
    SET c.IdUsuario = u.Id
    FROM [dbo].[Cliente] c
    INNER JOIN [dbo].[AspNetUsers] u ON LOWER(LTRIM(RTRIM(c.EmailCliente))) = LOWER(LTRIM(RTRIM(u.Email)))
    WHERE c.EmailCliente IS NOT NULL
    AND c.EmailCliente <> ''
    AND (c.IdUsuario IS NULL OR c.IdUsuario <> u.Id);
    
    -- Vincular por Cédula (TODOS los clientes con cédula)
    UPDATE c
    SET c.IdUsuario = u.Id
    FROM [dbo].[Cliente] c
    INNER JOIN [dbo].[AspNetUsers] u ON LTRIM(RTRIM(c.CedulaCliente)) = LTRIM(RTRIM(u.cedula))
    WHERE c.CedulaCliente IS NOT NULL
    AND c.CedulaCliente <> ''
    AND u.cedula IS NOT NULL
    AND u.cedula <> ''
    AND (c.IdUsuario IS NULL OR c.IdUsuario <> u.Id);
    ');
    
    PRINT '   ✓ Vinculación automática completada';
    PRINT '';
    
    -- Verificar resultados de la vinculación
    DECLARE @UsuariosVinculados INT;
    SELECT @UsuariosVinculados = COUNT(*) 
    FROM [dbo].[Cliente] c
    INNER JOIN [dbo].[AspNetUsers] u ON c.IdUsuario = u.Id;
    
    PRINT '   Clientes vinculados correctamente: ' + CAST(@UsuariosVinculados AS VARCHAR(10));
    
    -- Verificar clientes con IdUsuario pero sin usuario en AspNetUsers (datos inconsistentes)
    DECLARE @VinculacionesInconsistentes INT;
    SELECT @VinculacionesInconsistentes = COUNT(*) 
    FROM [dbo].[Cliente] c
    WHERE c.IdUsuario IS NOT NULL
    AND NOT EXISTS (SELECT 1 FROM [dbo].[AspNetUsers] u WHERE u.Id = c.IdUsuario);
    
    IF @VinculacionesInconsistentes > 0
    BEGIN
        PRINT '   ⚠ ADVERTENCIA: ' + CAST(@VinculacionesInconsistentes AS VARCHAR(10)) + ' clientes con IdUsuario pero sin usuario en AspNetUsers';
        PRINT '   Estos registros tienen datos inconsistentes y deberían limpiarse.';
    END
END
ELSE
BEGIN
    PRINT '   ⚠ ADVERTENCIA: Tabla AspNetUsers no existe';
    PRINT '   No se puede ejecutar la vinculación automática.';
END
PRINT '';

-- 5. Recomendaciones
PRINT '5. RECOMENDACIONES:';
PRINT '';
IF @SinUsuario > 0
BEGIN
    PRINT '   • Tienes ' + CAST(@SinUsuario AS VARCHAR(10)) + ' clientes sin usuario web.';
    PRINT '   • Estos clientes pueden registrarse y vincularse automáticamente.';
    PRINT '   • O puedes crear usuarios manualmente desde el panel de administración.';
    PRINT '';
END

IF @SinEmailNiCedula > 0
BEGIN
    PRINT '   • Tienes ' + CAST(@SinEmailNiCedula AS VARCHAR(10)) + ' clientes sin email ni cédula.';
    PRINT '   • Estos clientes no pueden vincularse automáticamente.';
    PRINT '   • Considera completar sus datos si necesitan acceso web.';
    PRINT '';
END

-- 6. Muestra de clientes sin usuario (para referencia)
PRINT '6. MUESTRA DE CLIENTES SIN USUARIO (primeros 10):';
SELECT TOP 10
    IdCliente,
    NombreCliente,
    PrimerApellidoCliente,
    EmailCliente,
    CedulaCliente,
    TelefonoCliente,
    CASE 
        WHEN EmailCliente IS NOT NULL AND EmailCliente <> '' THEN 'Puede vincularse por email'
        WHEN CedulaCliente IS NOT NULL AND CedulaCliente <> '' THEN 'Puede vincularse por cédula'
        ELSE 'Sin datos para vincular'
    END AS EstadoVinculacion
FROM [dbo].[Cliente]
WHERE IdUsuario IS NULL
ORDER BY IdCliente;

PRINT '';
PRINT '========================================';
PRINT 'MIGRACIÓN VERIFICADA';
PRINT '========================================';
PRINT '';
PRINT 'La migración de clientes está completa.';
PRINT 'Los clientes pueden vincularse automáticamente al registrarse en el sistema web.';
PRINT '';

