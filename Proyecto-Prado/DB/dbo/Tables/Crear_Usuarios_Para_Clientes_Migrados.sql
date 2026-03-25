-- =============================================
-- Crear usuarios en AspNetUsers para clientes migrados
-- Este script crea usuarios con contraseña temporal
-- Los clientes deberán cambiar su contraseña al primer acceso
-- =============================================

PRINT '========================================';
PRINT 'CREAR USUARIOS PARA CLIENTES MIGRADOS';
PRINT '========================================';
PRINT '';
PRINT 'ADVERTENCIA: Este script crea usuarios con contraseña temporal.';
PRINT 'Los clientes deberan cambiar su contraseña al primer acceso.';
PRINT '';
PRINT '¿Deseas continuar? (Comentar las líneas de INSERT si no)';
PRINT '';

-- Contraseña temporal por defecto: "Cliente123!" (debe ser hasheada en producción)
-- NOTA: En producción, debes usar el UserManager de ASP.NET Identity para crear usuarios
-- Este script es solo para referencia. NO ejecutes los INSERT directamente.

-- Ver clientes sin usuario
DECLARE @ClientesSinUsuario INT;
SELECT @ClientesSinUsuario = COUNT(*) 
FROM [dbo].[Cliente] 
WHERE IdUsuario IS NULL 
AND EmailCliente IS NOT NULL 
AND EmailCliente <> '';

PRINT 'Clientes sin usuario (con email): ' + CAST(@ClientesSinUsuario AS VARCHAR(10));
PRINT '';

-- Mostrar algunos ejemplos
PRINT 'Ejemplos de clientes que recibirian usuario:';
SELECT TOP 10
    IdCliente,
    NombreCliente,
    EmailCliente,
    CedulaCliente
FROM [dbo].[Cliente]
WHERE IdUsuario IS NULL
AND EmailCliente IS NOT NULL
AND EmailCliente <> ''
ORDER BY IdCliente;

PRINT '';
PRINT '========================================';
PRINT 'INSTRUCCIONES';
PRINT '========================================';
PRINT '';
PRINT 'NO ejecutes INSERT directo en AspNetUsers.';
PRINT 'En su lugar, usa el proceso de registro en la aplicación web.';
PRINT '';
PRINT 'Opciones:';
PRINT '1. Proceso de Registro Automático (recomendado)';
PRINT '   - Cliente ingresa email/cédula en registro';
PRINT '   - Sistema busca si existe cliente';
PRINT '   - Si existe, vincula automáticamente';
PRINT '';
PRINT '2. Admin crea usuarios manualmente';
PRINT '   - Admin crea usuario en sistema';
PRINT '   - Sistema busca y vincula cliente automáticamente';
PRINT '';
PRINT '3. Script C# para crear usuarios masivamente (ver CrearUsuariosParaClientes.cs)';


