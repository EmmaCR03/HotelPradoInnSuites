-- =============================================
-- Script para crear tabla Empresas
-- =============================================

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Empresas')
BEGIN
    CREATE TABLE [dbo].[Empresas] (
        [IdEmpresa] INT IDENTITY(1,1) NOT NULL,
        [CodigoEmpresa] INT NULL,  -- Para mantener el código original del DBF
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
    
    -- Crear índice único en CodigoEmpresa para búsquedas rápidas
    CREATE UNIQUE NONCLUSTERED INDEX [IX_Empresas_CodigoEmpresa] 
    ON [dbo].[Empresas] ([CodigoEmpresa])
    WHERE [CodigoEmpresa] IS NOT NULL;
    
    PRINT 'Tabla Empresas creada exitosamente';
END
ELSE
    PRINT 'Tabla Empresas ya existe';

