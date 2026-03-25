-- =============================================
-- Ajustar campos de Cargos a BIGINT para valores grandes
-- =============================================

PRINT 'Ajustando campos de Cargos a BIGINT...';

-- Cambiar campos que pueden tener valores grandes
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Cargos')
BEGIN
    -- NumeroDocumento
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Cargos') AND name = 'NumeroDocumento' AND system_type_id = 56) -- INT
    BEGIN
        ALTER TABLE [dbo].[Cargos]
        ALTER COLUMN [NumeroDocumento] BIGINT NULL;
        PRINT 'NumeroDocumento cambiado a BIGINT';
    END
    
    -- QuienPaga
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Cargos') AND name = 'QuienPaga' AND system_type_id = 56) -- INT
    BEGIN
        ALTER TABLE [dbo].[Cargos]
        ALTER COLUMN [QuienPaga] BIGINT NULL;
        PRINT 'QuienPaga cambiado a BIGINT';
    END
    
    -- NumeroCierre
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Cargos') AND name = 'NumeroCierre' AND system_type_id = 56) -- INT
    BEGIN
        ALTER TABLE [dbo].[Cargos]
        ALTER COLUMN [NumeroCierre] BIGINT NULL;
        PRINT 'NumeroCierre cambiado a BIGINT';
    END
    
    -- NumeroFolioOriginal
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Cargos') AND name = 'NumeroFolioOriginal' AND system_type_id = 56) -- INT
    BEGIN
        ALTER TABLE [dbo].[Cargos]
        ALTER COLUMN [NumeroFolioOriginal] BIGINT NULL;
        PRINT 'NumeroFolioOriginal cambiado a BIGINT';
    END
    
    PRINT 'Ajustes completados';
END
ELSE
    PRINT 'Tabla Cargos no existe';


