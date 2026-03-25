-- =============================================
-- Script para crear tabla Temporadas
-- =============================================

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Temporadas')
BEGIN
    CREATE TABLE [dbo].[Temporadas] (
        [IdTemporada] INT IDENTITY(1,1) NOT NULL,
        [NumeroTemporada] INT NULL,  -- M_NUMTEMP (para mantener código original)
        [Descripcion] NVARCHAR(60) NULL,  -- DESCRIP_TE
        [CodigoCuenta] BIGINT NULL,  -- COD_CTA
        [AumentaAl] INT NULL,  -- AUMENTA_AL
        CONSTRAINT [PK_Temporadas] PRIMARY KEY CLUSTERED ([IdTemporada] ASC)
    );
    
    -- Crear índice único en NumeroTemporada para búsquedas rápidas
    CREATE UNIQUE NONCLUSTERED INDEX [IX_Temporadas_NumeroTemporada] 
    ON [dbo].[Temporadas] ([NumeroTemporada])
    WHERE [NumeroTemporada] IS NOT NULL;
    
    PRINT 'Tabla Temporadas creada exitosamente';
END
ELSE
    PRINT 'Tabla Temporadas ya existe';

