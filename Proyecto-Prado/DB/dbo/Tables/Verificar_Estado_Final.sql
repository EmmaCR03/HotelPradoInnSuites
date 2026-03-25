-- =============================================
-- Verificación final del estado de las tablas
-- =============================================

PRINT '========================================';
PRINT 'VERIFICACION FINAL - ESTADO DE TABLAS';
PRINT '========================================';
PRINT '';

-- Verificar tabla Cliente
PRINT 'TABLA CLIENTE:';
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Cliente')
BEGIN
    PRINT '  ✓ Tabla existe';
    
    -- Verificar campos importantes
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Cliente') AND name = 'CedulaCliente')
        PRINT '  ✓ CedulaCliente existe';
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Cliente') AND name = 'IdEmpresa')
        PRINT '  ✓ IdEmpresa existe';
    
    -- Verificar tipo de TelefonoCliente
    DECLARE @TelCliente VARCHAR(50);
    SELECT @TelCliente = t.name + '(' + CAST(c.max_length AS VARCHAR(10)) + ')'
    FROM sys.columns c
    INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
    WHERE c.object_id = OBJECT_ID('dbo.Cliente') AND c.name = 'TelefonoCliente';
    PRINT '  - TelefonoCliente: ' + ISNULL(@TelCliente, 'NO EXISTE');
END
ELSE
    PRINT '  ✗ Tabla NO existe';

PRINT '';

-- Verificar tabla Reservas
PRINT 'TABLA RESERVAS:';
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Reservas')
BEGIN
    PRINT '  ✓ Tabla existe';
    
    -- Verificar tipo de IdCliente
    DECLARE @IdClienteReservas VARCHAR(50);
    SELECT @IdClienteReservas = t.name + '(' + CAST(c.max_length AS VARCHAR(10)) + ')'
    FROM sys.columns c
    INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
    WHERE c.object_id = OBJECT_ID('dbo.Reservas') AND c.name = 'IdCliente';
    PRINT '  - IdCliente: ' + ISNULL(@IdClienteReservas, 'NO EXISTE');
    
    IF @IdClienteReservas LIKE 'int%'
        PRINT '  ✓ IdCliente es INT (correcto)';
    ELSE
        PRINT '  ✗ IdCliente NO es INT';
    
    -- Verificar Foreign Keys
    IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Reservas_Cliente')
        PRINT '  ✓ FK_Reservas_Cliente existe';
    ELSE
        PRINT '  ✗ FK_Reservas_Cliente NO existe';
    
    -- Verificar campos nuevos
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Reservas') AND name = 'CedulaCliente')
        PRINT '  ✓ CedulaCliente existe';
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Reservas') AND name = 'NumeroAdultos')
        PRINT '  ✓ NumeroAdultos existe';
END
ELSE
    PRINT '  ✗ Tabla NO existe';

PRINT '';

-- Verificar tabla Empresas
PRINT 'TABLA EMPRESAS:';
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Empresas')
BEGIN
    PRINT '  ✓ Tabla existe';
    DECLARE @CountEmpresas INT;
    SELECT @CountEmpresas = COUNT(*) FROM [dbo].[Empresas];
    PRINT '  - Registros: ' + CAST(@CountEmpresas AS VARCHAR(10));
END
ELSE
    PRINT '  ✗ Tabla NO existe (necesaria para relación con empresas)';

PRINT '';
PRINT '========================================';
PRINT 'LISTO PARA MIGRACION';
PRINT '========================================';


