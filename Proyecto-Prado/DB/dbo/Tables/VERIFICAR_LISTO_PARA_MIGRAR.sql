-- =============================================
-- Verificación FINAL antes de migrar datos
-- =============================================

PRINT '========================================';
PRINT 'VERIFICACION FINAL - LISTO PARA MIGRAR';
PRINT '========================================';
PRINT '';

-- 1. Verificar tabla Empresas
PRINT '1. TABLA EMPRESAS:';
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Empresas')
BEGIN
    DECLARE @CountEmpresas INT;
    SELECT @CountEmpresas = COUNT(*) FROM [dbo].[Empresas];
    PRINT '   ✓ Tabla existe';
    PRINT '   - Registros: ' + CAST(@CountEmpresas AS VARCHAR(10));
END
ELSE
    PRINT '   ✗ Tabla NO existe';
PRINT '';

-- 2. Verificar tabla Cliente
PRINT '2. TABLA CLIENTE:';
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Cliente')
BEGIN
    DECLARE @CountCliente INT;
    SELECT @CountCliente = COUNT(*) FROM [dbo].[Cliente];
    
    PRINT '   ✓ Tabla existe';
    PRINT '   - Registros: ' + CAST(@CountCliente AS VARCHAR(10));
    
    -- Verificar campos importantes
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Cliente') AND name = 'CedulaCliente')
        PRINT '   ✓ Campo CedulaCliente existe';
    ELSE
        PRINT '   ✗ Campo CedulaCliente NO existe';
    
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Cliente') AND name = 'IdEmpresa')
        PRINT '   ✓ Campo IdEmpresa existe';
    ELSE
        PRINT '   ✗ Campo IdEmpresa NO existe';
    
    -- Verificar tipo de TelefonoCliente
    DECLARE @TelTipo VARCHAR(50);
    SELECT @TelTipo = t.name
    FROM sys.columns c
    INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
    WHERE c.object_id = OBJECT_ID('dbo.Cliente') AND c.name = 'TelefonoCliente';
    
    IF @TelTipo = 'varchar'
        PRINT '   ✓ TelefonoCliente es VARCHAR (correcto)';
    ELSE
        PRINT '   ✗ TelefonoCliente es ' + ISNULL(@TelTipo, 'NO EXISTE') + ' (debe ser VARCHAR)';
END
ELSE
    PRINT '   ✗ Tabla NO existe';
PRINT '';

-- 3. Verificar tabla Reservas
PRINT '3. TABLA RESERVAS:';
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Reservas')
BEGIN
    DECLARE @CountReservas INT;
    SELECT @CountReservas = COUNT(*) FROM [dbo].[Reservas];
    
    PRINT '   ✓ Tabla existe';
    PRINT '   - Registros: ' + CAST(@CountReservas AS VARCHAR(10));
    
    -- Verificar tipo de IdCliente
    DECLARE @IdClienteTipo VARCHAR(50);
    SELECT @IdClienteTipo = t.name
    FROM sys.columns c
    INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
    WHERE c.object_id = OBJECT_ID('dbo.Reservas') AND c.name = 'IdCliente';
    
    IF @IdClienteTipo = 'int'
        PRINT '   ✓ IdCliente es INT (correcto)';
    ELSE
        PRINT '   ✗ IdCliente es ' + ISNULL(@IdClienteTipo, 'NO EXISTE') + ' (debe ser INT)';
    
    -- Verificar Foreign Keys
    IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Reservas_Cliente')
        PRINT '   ✓ FK a Cliente existe';
    ELSE
        PRINT '   ✗ FK a Cliente NO existe';
    
    IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Reservas_Usuarios')
        PRINT '   ✗ FK a AspNetUsers AÚN existe (debe eliminarse)';
    ELSE
        PRINT '   ✓ FK a AspNetUsers NO existe (correcto)';
    
    -- Verificar campos importantes
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Reservas') AND name = 'CedulaCliente')
        PRINT '   ✓ Campo CedulaCliente existe';
    ELSE
        PRINT '   ✗ Campo CedulaCliente NO existe';
    
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Reservas') AND name = 'IdEmpresa')
        PRINT '   ✓ Campo IdEmpresa existe';
    ELSE
        PRINT '   ✗ Campo IdEmpresa NO existe';
END
ELSE
    PRINT '   ✗ Tabla NO existe';
PRINT '';

-- Resumen final
PRINT '========================================';
PRINT 'RESUMEN';
PRINT '========================================';

DECLARE @TodoListo BIT = 1;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Empresas')
    SET @TodoListo = 0;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Cliente')
    SET @TodoListo = 0;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Reservas')
    SET @TodoListo = 0;

IF NOT EXISTS (
    SELECT 1 
    FROM sys.columns c
    INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
    WHERE c.object_id = OBJECT_ID('dbo.Cliente') 
    AND c.name = 'TelefonoCliente'
    AND t.name = 'varchar'
)
    SET @TodoListo = 0;

IF NOT EXISTS (
    SELECT 1 
    FROM sys.columns c
    INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
    WHERE c.object_id = OBJECT_ID('dbo.Reservas') 
    AND c.name = 'IdCliente'
    AND t.name = 'int'
)
    SET @TodoListo = 0;

IF @TodoListo = 1
BEGIN
    PRINT '';
    PRINT '✓✓✓ TODO ESTÁ LISTO PARA MIGRAR ✓✓✓';
    PRINT '';
    PRINT 'Puedes ejecutar: python MigrarDatosDBF.py';
END
ELSE
BEGIN
    PRINT '';
    PRINT '✗✗✗ HAY PROBLEMAS QUE RESOLVER ANTES DE MIGRAR ✗✗✗';
    PRINT '';
    PRINT 'Revisa los errores marcados con ✗ arriba.';
END


