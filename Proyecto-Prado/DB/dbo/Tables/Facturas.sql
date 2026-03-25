-- =============================================
-- Script para crear tabla Facturas
-- Basado en la estructura de FACTURAS.dbf (sicre_d) - 2023
-- =============================================

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Facturas')
BEGIN
    CREATE TABLE [dbo].[Facturas] (
        [IdFactura] INT IDENTITY(1,1) NOT NULL,
        [NumeroFactura] INT NOT NULL,              -- NO_FACT (Número de factura)
        [IdCheckIn] INT NULL,                      -- Relación con CheckIn (N_FOLIO)
        [IdEmpleado] INT NULL,                     -- NO_EMPLEA (Número de empleado)
        [FechaHoraFactura] DATETIME NOT NULL,      -- FECH_HO_FA (Fecha y hora de factura)
        [TotalConsumos] DECIMAL(18,2) NULL,        -- TOT_CONS (Total consumos)
        [ImpuestoICT] DECIMAL(18,2) NULL,          -- AIMP_ICT (Impuesto ICT)
        [ImpuestoServicio] DECIMAL(18,2) NULL,     -- AIMP_SERV (Impuesto servicio)
        [ImpuestoVentas] DECIMAL(18,2) NULL,      -- AIMP_VEN (Impuesto ventas)
        [TotalGeneral] DECIMAL(18,2) NOT NULL,     -- TOT1 (Total general)
        [QuienPaga] BIGINT NULL,                   -- Q_PAGA (Quien paga)
        [Cerrado] BIT DEFAULT 0,                   -- CERRADO (Factura cerrada)
        [EnFacturaExtras] BIT DEFAULT 0,          -- EN_FEXTRAS (En factura extras)
        [FechaCreacion] DATETIME DEFAULT GETDATE(),
        [FechaModificacion] DATETIME DEFAULT GETDATE(),
        CONSTRAINT [PK_Facturas] PRIMARY KEY CLUSTERED ([IdFactura] ASC)
    );
    
    PRINT 'Tabla Facturas creada exitosamente';
END
ELSE
BEGIN
    PRINT 'La tabla Facturas ya existe';
END
GO

-- =============================================
-- Agregar columnas faltantes si la tabla ya existe
-- =============================================

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Facturas')
BEGIN
    -- NOTA: La tabla Facturas ya existe con una estructura de facturación electrónica
    -- La clave primaria es 'Id' (uniqueidentifier), no 'IdFactura'
    -- Agregamos IdFactura como columna adicional para compatibilidad con migración DBF
    -- y para la relación con Cargos
    
    -- Agregar NumeroFactura si no existe
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Facturas') AND name = 'NumeroFactura')
    BEGIN
        ALTER TABLE [dbo].[Facturas]
        ADD [NumeroFactura] INT NULL;
        PRINT 'Columna NumeroFactura agregada a Facturas';
    END
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Facturas') AND name = 'IdFactura')
    BEGIN
        -- Agregar IdFactura como columna adicional (no como PK, ya que Id es la PK)
        ALTER TABLE [dbo].[Facturas]
        ADD [IdFactura] INT IDENTITY(1,1) NOT NULL;
        
        -- Crear índice único en IdFactura para que pueda ser referenciado
        CREATE UNIQUE NONCLUSTERED INDEX [IX_Facturas_IdFactura] 
        ON [dbo].[Facturas] ([IdFactura]);
        
        PRINT 'Columna IdFactura agregada a Facturas (IDENTITY, índice único)';
        PRINT 'NOTA: La clave primaria sigue siendo Id (uniqueidentifier)';
    END
    ELSE
    BEGIN
        PRINT 'IdFactura ya existe en Facturas';
    END
    
    -- Agregar IdCheckIn si no existe
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Facturas') AND name = 'IdCheckIn')
    BEGIN
        ALTER TABLE [dbo].[Facturas]
        ADD [IdCheckIn] INT NULL;
        PRINT 'Columna IdCheckIn agregada a Facturas';
    END
    
    -- Agregar IdEmpleado si no existe
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Facturas') AND name = 'IdEmpleado')
    BEGIN
        ALTER TABLE [dbo].[Facturas]
        ADD [IdEmpleado] INT NULL;
        PRINT 'Columna IdEmpleado agregada a Facturas';
    END
    
    -- Agregar FechaHoraFactura si no existe
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Facturas') AND name = 'FechaHoraFactura')
    BEGIN
        ALTER TABLE [dbo].[Facturas]
        ADD [FechaHoraFactura] DATETIME NULL;
        PRINT 'Columna FechaHoraFactura agregada a Facturas';
    END
    
    -- Agregar TotalConsumos si no existe
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Facturas') AND name = 'TotalConsumos')
    BEGIN
        ALTER TABLE [dbo].[Facturas]
        ADD [TotalConsumos] DECIMAL(18,2) NULL;
        PRINT 'Columna TotalConsumos agregada a Facturas';
    END
    
    -- Agregar ImpuestoICT si no existe
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Facturas') AND name = 'ImpuestoICT')
    BEGIN
        ALTER TABLE [dbo].[Facturas]
        ADD [ImpuestoICT] DECIMAL(18,2) NULL;
        PRINT 'Columna ImpuestoICT agregada a Facturas';
    END
    
    -- Agregar ImpuestoServicio si no existe
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Facturas') AND name = 'ImpuestoServicio')
    BEGIN
        ALTER TABLE [dbo].[Facturas]
        ADD [ImpuestoServicio] DECIMAL(18,2) NULL;
        PRINT 'Columna ImpuestoServicio agregada a Facturas';
    END
    
    -- Agregar ImpuestoVentas si no existe
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Facturas') AND name = 'ImpuestoVentas')
    BEGIN
        ALTER TABLE [dbo].[Facturas]
        ADD [ImpuestoVentas] DECIMAL(18,2) NULL;
        PRINT 'Columna ImpuestoVentas agregada a Facturas';
    END
    
    -- Agregar TotalGeneral si no existe
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Facturas') AND name = 'TotalGeneral')
    BEGIN
        ALTER TABLE [dbo].[Facturas]
        ADD [TotalGeneral] DECIMAL(18,2) NULL;
        PRINT 'Columna TotalGeneral agregada a Facturas';
    END
    
    -- Agregar QuienPaga si no existe
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Facturas') AND name = 'QuienPaga')
    BEGIN
        ALTER TABLE [dbo].[Facturas]
        ADD [QuienPaga] BIGINT NULL;
        PRINT 'Columna QuienPaga agregada a Facturas';
    END
    
    -- Agregar Cerrado si no existe
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Facturas') AND name = 'Cerrado')
    BEGIN
        ALTER TABLE [dbo].[Facturas]
        ADD [Cerrado] BIT DEFAULT 0;
        PRINT 'Columna Cerrado agregada a Facturas';
    END
    
    -- Agregar EnFacturaExtras si no existe
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Facturas') AND name = 'EnFacturaExtras')
    BEGIN
        ALTER TABLE [dbo].[Facturas]
        ADD [EnFacturaExtras] BIT DEFAULT 0;
        PRINT 'Columna EnFacturaExtras agregada a Facturas';
    END
    
    -- Agregar FechaCreacion si no existe
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Facturas') AND name = 'FechaCreacion')
    BEGIN
        ALTER TABLE [dbo].[Facturas]
        ADD [FechaCreacion] DATETIME DEFAULT GETDATE();
        PRINT 'Columna FechaCreacion agregada a Facturas';
    END
    
    -- Agregar FechaModificacion si no existe
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Facturas') AND name = 'FechaModificacion')
    BEGIN
        ALTER TABLE [dbo].[Facturas]
        ADD [FechaModificacion] DATETIME DEFAULT GETDATE();
        PRINT 'Columna FechaModificacion agregada a Facturas';
    END
END
GO

-- =============================================
-- Crear índices para búsquedas rápidas
-- =============================================

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Facturas')
BEGIN
    -- Índice en NumeroFactura (solo si la columna existe)
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Facturas') AND name = 'NumeroFactura')
        AND NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Facturas_NumeroFactura' AND object_id = OBJECT_ID('dbo.Facturas'))
    BEGIN
        CREATE NONCLUSTERED INDEX [IX_Facturas_NumeroFactura] 
        ON [dbo].[Facturas] ([NumeroFactura]);
    END
    
    -- Índice filtrado en IdCheckIn (solo si la columna existe)
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Facturas') AND name = 'IdCheckIn')
        AND NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Facturas_IdCheckIn' AND object_id = OBJECT_ID('dbo.Facturas'))
    BEGIN
        CREATE NONCLUSTERED INDEX [IX_Facturas_IdCheckIn] 
        ON [dbo].[Facturas] ([IdCheckIn])
        WHERE [IdCheckIn] IS NOT NULL;
    END
    
    -- Índice en FechaHoraFactura (solo si la columna existe)
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Facturas') AND name = 'FechaHoraFactura')
        AND NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Facturas_FechaHoraFactura' AND object_id = OBJECT_ID('dbo.Facturas'))
    BEGIN
        CREATE NONCLUSTERED INDEX [IX_Facturas_FechaHoraFactura] 
        ON [dbo].[Facturas] ([FechaHoraFactura]);
    END
    
    -- Índice filtrado en QuienPaga (solo si la columna existe)
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Facturas') AND name = 'QuienPaga')
        AND NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Facturas_QuienPaga' AND object_id = OBJECT_ID('dbo.Facturas'))
    BEGIN
        CREATE NONCLUSTERED INDEX [IX_Facturas_QuienPaga] 
        ON [dbo].[Facturas] ([QuienPaga])
        WHERE [QuienPaga] IS NOT NULL;
    END
END
GO

-- =============================================
-- Agregar relaciones (Foreign Keys)
-- =============================================

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Facturas')
BEGIN
    -- Relación con CheckIn si existe y la columna IdCheckIn existe
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'CheckIn')
        AND EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Facturas') AND name = 'IdCheckIn')
        AND NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Facturas_CheckIn')
    BEGIN
        ALTER TABLE [dbo].[Facturas]
        ADD CONSTRAINT [FK_Facturas_CheckIn] 
        FOREIGN KEY ([IdCheckIn]) REFERENCES [dbo].[CheckIn] ([IdCheckIn]);
        PRINT 'Relación FK_Facturas_CheckIn creada';
    END
    
    -- Relación con Colaborador (empleado) si existe y la columna IdEmpleado existe
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Colaborador')
        AND EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Facturas') AND name = 'IdEmpleado')
        AND NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Facturas_Colaborador')
    BEGIN
        ALTER TABLE [dbo].[Facturas]
        ADD CONSTRAINT [FK_Facturas_Colaborador] 
        FOREIGN KEY ([IdEmpleado]) REFERENCES [dbo].[Colaborador] ([IdColaborador]);
        PRINT 'Relación FK_Facturas_Colaborador creada';
    END
END
GO

-- =============================================
-- Agregar relación con Cargos
-- =============================================

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Cargos')
    AND EXISTS (SELECT * FROM sys.tables WHERE name = 'Facturas')
BEGIN
    -- Agregar columna IdFactura a Cargos si no existe
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Cargos') AND name = 'IdFactura')
    BEGIN
        ALTER TABLE [dbo].[Cargos]
        ADD [IdFactura] INT NULL;
        
        PRINT 'Columna IdFactura agregada a Cargos';
    END
END
GO

-- =============================================
-- Crear índice y relación en Cargos (después de agregar la columna)
-- =============================================

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Cargos')
    AND EXISTS (SELECT * FROM sys.tables WHERE name = 'Facturas')
BEGIN
    -- Crear índice filtrado en IdFactura (solo si la columna existe)
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Cargos') AND name = 'IdFactura')
        AND NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Cargos_IdFactura' AND object_id = OBJECT_ID('dbo.Cargos'))
    BEGIN
        CREATE NONCLUSTERED INDEX [IX_Cargos_IdFactura] 
        ON [dbo].[Cargos] ([IdFactura])
        WHERE [IdFactura] IS NOT NULL;
        
        PRINT 'Índice IX_Cargos_IdFactura creado';
    END
    
    -- Agregar foreign key (IdFactura tiene índice único, puede ser referenciado)
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Cargos') AND name = 'IdFactura')
        AND EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Facturas') AND name = 'IdFactura')
        AND EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('dbo.Facturas') AND name = 'IX_Facturas_IdFactura')
        AND NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Cargos_Facturas')
    BEGIN
        ALTER TABLE [dbo].[Cargos]
        ADD CONSTRAINT [FK_Cargos_Facturas] 
        FOREIGN KEY ([IdFactura]) REFERENCES [dbo].[Facturas] ([IdFactura]);
        
        PRINT 'Relación entre Cargos y Facturas creada exitosamente (usando IdFactura)';
    END
    ELSE IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Cargos_Facturas')
    BEGIN
        PRINT 'La relación entre Cargos y Facturas ya existe';
    END
    ELSE IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Facturas') AND name = 'IdFactura')
    BEGIN
        PRINT 'ERROR: La tabla Facturas no tiene la columna IdFactura.';
        PRINT 'Ejecuta primero la sección de agregar columnas.';
    END
    ELSE IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('dbo.Facturas') AND name = 'IX_Facturas_IdFactura')
    BEGIN
        PRINT 'ERROR: La columna IdFactura no tiene índice único. Creando índice...';
        CREATE UNIQUE NONCLUSTERED INDEX [IX_Facturas_IdFactura] 
        ON [dbo].[Facturas] ([IdFactura]);
        
        -- Intentar crear la foreign key nuevamente
        IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Cargos_Facturas')
        BEGIN
            ALTER TABLE [dbo].[Cargos]
            ADD CONSTRAINT [FK_Cargos_Facturas] 
            FOREIGN KEY ([IdFactura]) REFERENCES [dbo].[Facturas] ([IdFactura]);
            
            PRINT 'Relación entre Cargos y Facturas creada exitosamente';
        END
    END
END
GO

