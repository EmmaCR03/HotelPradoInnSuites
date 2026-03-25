-- =============================================
-- Script COMPLETO para preparar todo antes de migrar
-- Ejecutar este script para asegurar que todo está listo
-- =============================================

PRINT '========================================';
PRINT 'PREPARANDO BASE DE DATOS PARA MIGRACION';
PRINT '========================================';
PRINT '';

-- 1. Crear tabla Empresas
PRINT '1. Creando tabla Empresas...';
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Empresas')
BEGIN
    CREATE TABLE [dbo].[Empresas] (
        [IdEmpresa] INT IDENTITY(1,1) NOT NULL,
        [CodigoEmpresa] INT NULL,
        [NombreEmpresa] VARCHAR(100) NULL,
        [Telefono1] VARCHAR(15) NULL,
        [Telefono2] VARCHAR(15) NULL,
        [Fax] VARCHAR(15) NULL,
        [Contacto] VARCHAR(100) NULL,
        [Direccion] NVARCHAR(300) NULL,
        [Observaciones] NVARCHAR(500) NULL,
        [LimiteCredito] DECIMAL(18,2) NULL,
        [CorreoElectronico] VARCHAR(100) NULL,
        CONSTRAINT [PK_Empresas] PRIMARY KEY CLUSTERED ([IdEmpresa] ASC)
    );
    
    CREATE UNIQUE NONCLUSTERED INDEX [IX_Empresas_CodigoEmpresa] 
    ON [dbo].[Empresas] ([CodigoEmpresa])
    WHERE [CodigoEmpresa] IS NOT NULL;
    
    PRINT '   ✓ Tabla Empresas creada';
END
ELSE
    PRINT '   ✓ Tabla Empresas ya existe';
GO

-- 2. Corregir TelefonoCliente en Cliente (si es necesario)
PRINT '';
PRINT '2. Verificando TelefonoCliente en Cliente...';
DECLARE @TelTipo VARCHAR(50);
SELECT @TelTipo = t.name
FROM sys.columns c
INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
WHERE c.object_id = OBJECT_ID('dbo.Cliente') AND c.name = 'TelefonoCliente';

IF @TelTipo = 'int'
BEGIN
    DECLARE @CountCliente INT;
    SELECT @CountCliente = COUNT(*) FROM [dbo].[Cliente];
    
    IF @CountCliente = 0
    BEGIN
        ALTER TABLE [dbo].[Cliente]
        ALTER COLUMN [TelefonoCliente] VARCHAR(15) NULL;
        PRINT '   ✓ TelefonoCliente cambiado a VARCHAR(15)';
    END
    ELSE
    BEGIN
        PRINT '   ⚠ Tabla tiene datos - Se requiere proceso manual';
        PRINT '   Ejecutar: Corregir_TelefonoCliente_Cliente_Con_GO.sql';
    END
END
ELSE
    PRINT '   ✓ TelefonoCliente ya es VARCHAR';
GO

PRINT '';
PRINT '========================================';
PRINT 'VERIFICACION FINAL';
PRINT '========================================';

-- Verificar todo
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Empresas')
    PRINT '✓ Tabla Empresas existe';
ELSE
    PRINT '✗ Tabla Empresas NO existe';

IF EXISTS (
    SELECT 1 
    FROM sys.columns c
    INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
    WHERE c.object_id = OBJECT_ID('dbo.Cliente') 
    AND c.name = 'TelefonoCliente'
    AND t.name = 'varchar'
)
    PRINT '✓ TelefonoCliente es VARCHAR';
ELSE
    PRINT '✗ TelefonoCliente NO es VARCHAR';

IF EXISTS (
    SELECT 1 
    FROM sys.columns c
    INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
    WHERE c.object_id = OBJECT_ID('dbo.Reservas') 
    AND c.name = 'IdCliente'
    AND t.name = 'int'
)
    PRINT '✓ IdCliente en Reservas es INT';
ELSE
    PRINT '✗ IdCliente en Reservas NO es INT';

PRINT '';
PRINT 'Si todo está ✓, puedes proceder con la migración.';


