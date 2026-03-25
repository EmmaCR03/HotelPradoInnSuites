-- =============================================
-- Script para crear tabla Cargos
-- =============================================

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Cargos')
BEGIN
    CREATE TABLE [dbo].[Cargos] (
        [IdCargo] INT IDENTITY(1,1) NOT NULL,
        [IdCheckIn] INT NULL,  -- Relación con CheckIn (N_FOLIO)
        [CodigoExtra] INT NULL,  -- COD_EXTRA
        [DescripcionExtra] NVARCHAR(60) NULL,  -- DESCRIP_EX
        [NumeroDocumento] BIGINT NULL,  -- NO_DOCU (puede ser muy grande)
        [MontoCargo] DECIMAL(18,2) NULL,  -- MONTO_CARG
        [MontoServicio] DECIMAL(18,2) NULL,  -- M_R_SERVIC
        [ImpuestoVenta] DECIMAL(18,2) NULL,  -- M_IMP_VENT
        [ImpuestoHotel] DECIMAL(18,2) NULL,  -- M_IMP_HOTE
        [ImpuestoServicio] DECIMAL(18,2) NULL,  -- M_IMP_SERV
        [MontoTotal] DECIMAL(18,2) NULL,  -- MONTO_TOTA
        [QuienPaga] BIGINT NULL,  -- Q_PAGA (puede ser muy grande)
        [FechaHora] DATETIME NULL,  -- FECHA_HORA
        [NumeroEmpleado] BIGINT NULL,  -- NO_EMPLEA
        [Cancelado] BIT NULL,  -- CANCELADO
        [Notas] NVARCHAR(200) NULL,  -- NOTAS
        [EnFacturaExtras] BIT NULL,  -- EN_FEXTRAS
        [CuentaError] BIT NULL,  -- CTA_ERROR
        [NumeroCierre] BIGINT NULL,  -- NO_CIERRE (puede ser muy grande)
        [PagoImpuesto] BIT NULL,  -- PAG_IMP
        [TipoCambio] DECIMAL(6,2) NULL,  -- TIPO_CAMBV
        [FechaTraslado] DATETIME NULL,  -- F_TRASLADO
        [Facturar] BIT NULL,  -- FACTURAR
        [Secuencia] INT NULL,  -- SECUENCIA
        [NoContable] BIT NULL,  -- NO_CONTABL
        [NumeroFolioOriginal] BIGINT NULL,  -- N_FOLIO (puede ser muy grande)
        CONSTRAINT [PK_Cargos] PRIMARY KEY CLUSTERED ([IdCargo] ASC)
    );
    
    -- Crear índices para búsquedas rápidas
    CREATE NONCLUSTERED INDEX [IX_Cargos_IdCheckIn] 
    ON [dbo].[Cargos] ([IdCheckIn])
    WHERE [IdCheckIn] IS NOT NULL;
    
    CREATE NONCLUSTERED INDEX [IX_Cargos_NumeroFolio] 
    ON [dbo].[Cargos] ([NumeroFolioOriginal])
    WHERE [NumeroFolioOriginal] IS NOT NULL;
    
    CREATE NONCLUSTERED INDEX [IX_Cargos_FechaHora] 
    ON [dbo].[Cargos] ([FechaHora])
    WHERE [FechaHora] IS NOT NULL;
    
    -- Foreign Key a CheckIn (si existe la tabla)
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'CheckIn')
    BEGIN
        ALTER TABLE [dbo].[Cargos]
        ADD CONSTRAINT [FK_Cargos_CheckIn] 
        FOREIGN KEY ([IdCheckIn]) REFERENCES [dbo].[CheckIn] ([IdCheckIn]);
    END
    
    PRINT 'Tabla Cargos creada exitosamente';
END
ELSE
    PRINT 'Tabla Cargos ya existe';

