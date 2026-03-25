-- Agregar columna Deposito a CheckIn si no existe (para tarjeta de registro)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.CheckIn') AND name = 'Deposito')
BEGIN
    ALTER TABLE [dbo].[CheckIn] ADD [Deposito] DECIMAL(18,2) NULL;
    PRINT 'Columna Deposito agregada a CheckIn';
END
ELSE
    PRINT 'Columna Deposito ya existe en CheckIn';
