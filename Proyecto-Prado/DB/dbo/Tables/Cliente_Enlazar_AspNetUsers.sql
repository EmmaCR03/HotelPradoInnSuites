-- =============================================
-- Enlazar tabla Cliente con AspNetUsers
-- Permite que los clientes tengan acceso al sistema web
-- =============================================

PRINT '========================================';
PRINT 'ENLAZANDO CLIENTE CON ASPNETUSERS';
PRINT '========================================';
PRINT '';

-- 1. Agregar campo IdUsuario en Cliente
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Cliente') AND name = 'IdUsuario')
BEGIN
    ALTER TABLE [dbo].[Cliente]
    ADD [IdUsuario] NVARCHAR(128) NULL;
    PRINT 'Campo IdUsuario agregado a Cliente';
END
ELSE
    PRINT 'Campo IdUsuario ya existe en Cliente';
GO

-- 2. Crear índice único para que un usuario solo pueda estar vinculado a un cliente
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Cliente_IdUsuario' AND object_id = OBJECT_ID('dbo.Cliente'))
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX [IX_Cliente_IdUsuario] 
    ON [dbo].[Cliente] ([IdUsuario])
    WHERE [IdUsuario] IS NOT NULL;
    PRINT 'Indice único IX_Cliente_IdUsuario creado';
END
ELSE
    PRINT 'Indice IX_Cliente_IdUsuario ya existe';
GO

-- 3. Agregar Foreign Key a AspNetUsers
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'AspNetUsers')
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Cliente_AspNetUsers')
    BEGIN
        ALTER TABLE [dbo].[Cliente]
        ADD CONSTRAINT [FK_Cliente_AspNetUsers] 
        FOREIGN KEY ([IdUsuario]) REFERENCES [dbo].[AspNetUsers] ([Id])
        ON DELETE SET NULL;  -- Si se elimina el usuario, se desvincula el cliente
        PRINT 'Foreign Key FK_Cliente_AspNetUsers agregada';
    END
    ELSE
        PRINT 'Foreign Key FK_Cliente_AspNetUsers ya existe';
END
ELSE
    PRINT 'ADVERTENCIA: Tabla AspNetUsers no existe';
GO

PRINT '';
PRINT '========================================';
PRINT 'ESTRUCTURA COMPLETADA';
PRINT '========================================';
PRINT '';
PRINT 'Ahora puedes:';
PRINT '1. Vincular clientes existentes con usuarios (usar script Vincular_Clientes_Usuarios.sql)';
PRINT '2. Crear usuarios nuevos para clientes que se registren';
PRINT '3. Los clientes podran acceder al sistema y hacer reservas';


