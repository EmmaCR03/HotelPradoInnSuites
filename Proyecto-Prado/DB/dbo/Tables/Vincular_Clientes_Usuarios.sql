-- =============================================
-- Vincular clientes existentes con usuarios de AspNetUsers
-- Este script busca coincidencias por email o cédula
-- PROCESA TODOS LOS CLIENTES SIN FILTROS
-- =============================================

PRINT '========================================';
PRINT 'VINCULANDO CLIENTES CON USUARIOS';
PRINT 'PROCESANDO TODOS LOS CLIENTES';
PRINT '========================================';
PRINT '';

-- Estadísticas iniciales
DECLARE @TotalClientes INT;
DECLARE @ClientesConEmail INT;
DECLARE @ClientesConCedula INT;
DECLARE @UsuariosDisponibles INT;

SELECT @TotalClientes = COUNT(*) FROM [dbo].[Cliente];
SELECT @ClientesConEmail = COUNT(*) FROM [dbo].[Cliente] 
WHERE EmailCliente IS NOT NULL AND EmailCliente <> '';
SELECT @ClientesConCedula = COUNT(*) FROM [dbo].[Cliente] 
WHERE CedulaCliente IS NOT NULL AND CedulaCliente <> '';
SELECT @UsuariosDisponibles = COUNT(*) FROM [dbo].[AspNetUsers];

PRINT 'ESTADÍSTICAS INICIALES:';
PRINT '   Total de clientes: ' + CAST(@TotalClientes AS VARCHAR(10));
PRINT '   Clientes con email: ' + CAST(@ClientesConEmail AS VARCHAR(10));
PRINT '   Clientes con cédula: ' + CAST(@ClientesConCedula AS VARCHAR(10));
PRINT '   Usuarios disponibles en AspNetUsers: ' + CAST(@UsuariosDisponibles AS VARCHAR(10));
PRINT '';

-- Vincular por Email (TODOS los clientes con email, sin importar si ya tienen IdUsuario)
PRINT 'Vincular por Email (procesando TODOS los clientes con email)...';
UPDATE c
SET c.IdUsuario = u.Id
FROM [dbo].[Cliente] c
INNER JOIN [dbo].[AspNetUsers] u ON LOWER(LTRIM(RTRIM(c.EmailCliente))) = LOWER(LTRIM(RTRIM(u.Email)))
WHERE c.EmailCliente IS NOT NULL
AND c.EmailCliente <> ''
AND (c.IdUsuario IS NULL OR c.IdUsuario <> u.Id); -- Actualizar solo si no está vinculado o si la vinculación es diferente

DECLARE @VinculadosEmail INT = @@ROWCOUNT;
PRINT 'Clientes vinculados/actualizados por Email: ' + CAST(@VinculadosEmail AS VARCHAR(10));
GO

-- Vincular por Cédula (TODOS los clientes con cédula, sin importar si ya tienen IdUsuario)
PRINT 'Vincular por Cedula (procesando TODOS los clientes con cédula)...';
UPDATE c
SET c.IdUsuario = u.Id
FROM [dbo].[Cliente] c
INNER JOIN [dbo].[AspNetUsers] u ON LTRIM(RTRIM(c.CedulaCliente)) = LTRIM(RTRIM(u.cedula))
WHERE c.CedulaCliente IS NOT NULL
AND c.CedulaCliente <> ''
AND u.cedula IS NOT NULL
AND u.cedula <> ''
AND (c.IdUsuario IS NULL OR c.IdUsuario <> u.Id); -- Actualizar solo si no está vinculado o si la vinculación es diferente

DECLARE @VinculadosCedula INT = @@ROWCOUNT;
PRINT 'Clientes vinculados/actualizados por Cedula: ' + CAST(@VinculadosCedula AS VARCHAR(10));
GO

-- Resumen completo
PRINT '';
PRINT '========================================';
PRINT 'RESUMEN COMPLETO DE MIGRACIÓN';
PRINT '========================================';

DECLARE @TotalClientesFinal INT;
DECLARE @ClientesVinculadosFinal INT;
DECLARE @ClientesSinVincularFinal INT;
DECLARE @ClientesConEmailSinVincular INT;
DECLARE @ClientesConCedulaSinVincular INT;
DECLARE @ClientesSinEmailNiCedula INT;

SELECT @TotalClientesFinal = COUNT(*) FROM [dbo].[Cliente];
SELECT @ClientesVinculadosFinal = COUNT(*) FROM [dbo].[Cliente] WHERE IdUsuario IS NOT NULL;
SELECT @ClientesSinVincularFinal = COUNT(*) FROM [dbo].[Cliente] WHERE IdUsuario IS NULL;
SELECT @ClientesConEmailSinVincular = COUNT(*) FROM [dbo].[Cliente] 
WHERE IdUsuario IS NULL 
AND EmailCliente IS NOT NULL 
AND EmailCliente <> '';
SELECT @ClientesConCedulaSinVincular = COUNT(*) FROM [dbo].[Cliente] 
WHERE IdUsuario IS NULL 
AND CedulaCliente IS NOT NULL 
AND CedulaCliente <> '';
SELECT @ClientesSinEmailNiCedula = COUNT(*) FROM [dbo].[Cliente] 
WHERE (EmailCliente IS NULL OR EmailCliente = '') 
AND (CedulaCliente IS NULL OR CedulaCliente = '');

PRINT 'RESULTADOS FINALES:';
PRINT '   Total de clientes procesados: ' + CAST(@TotalClientesFinal AS VARCHAR(10));
PRINT '   Clientes vinculados: ' + CAST(@ClientesVinculadosFinal AS VARCHAR(10));
PRINT '   Clientes sin vincular: ' + CAST(@ClientesSinVincularFinal AS VARCHAR(10));
PRINT '';
PRINT 'DETALLE DE CLIENTES SIN VINCULAR:';
PRINT '   Con email pero sin vincular: ' + CAST(@ClientesConEmailSinVincular AS VARCHAR(10));
PRINT '   Con cédula pero sin vincular: ' + CAST(@ClientesConCedulaSinVincular AS VARCHAR(10));
PRINT '   Sin email ni cédula: ' + CAST(@ClientesSinEmailNiCedula AS VARCHAR(10));
PRINT '';

-- Mostrar muestra de TODOS los clientes sin vincular (no solo los primeros 10)
IF @ClientesSinVincularFinal > 0
BEGIN
    PRINT 'MUESTRA DE CLIENTES SIN VINCULAR (primeros 20):';
    SELECT TOP 20 
        IdCliente,
        NombreCliente,
        PrimerApellidoCliente,
        EmailCliente,
        CedulaCliente,
        TelefonoCliente,
        CASE 
            WHEN EmailCliente IS NOT NULL AND EmailCliente <> '' THEN 'Tiene email - puede vincularse'
            WHEN CedulaCliente IS NOT NULL AND CedulaCliente <> '' THEN 'Tiene cédula - puede vincularse'
            ELSE 'Sin datos para vincular automáticamente'
        END AS EstadoVinculacion
    FROM [dbo].[Cliente]
    WHERE IdUsuario IS NULL
    ORDER BY IdCliente;
    PRINT '';
    PRINT '   (Mostrando 20 de ' + CAST(@ClientesSinVincularFinal AS VARCHAR(10)) + ' clientes sin vincular)';
END
ELSE
BEGIN
    PRINT '✓ ¡Todos los clientes con datos disponibles están vinculados!';
END

PRINT '';
PRINT '========================================';
PRINT 'MIGRACIÓN COMPLETADA';
PRINT '========================================';
PRINT '';
PRINT 'NOTAS:';
PRINT '• Se procesaron TODOS los clientes sin filtros';
PRINT '• Los clientes con email o cédula fueron vinculados automáticamente';
PRINT '• Los clientes sin email ni cédula no pueden vincularse automáticamente';
PRINT '';
PRINT 'Para crear usuarios para clientes sin vincular:';
PRINT '1. Crear usuarios manualmente en el sistema';
PRINT '2. Los clientes pueden registrarse y vincularse automáticamente';
PRINT '3. Usar el script Crear_Usuarios_Para_Clientes_Migrados.sql (si existe)';
PRINT '';


