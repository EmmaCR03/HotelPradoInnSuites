-- Agregar columnas de tarifas: general (la que ven clientes), gobierno, corporativa empresas.
-- Ejecutar una sola vez. Si la columna ya existe, ignorar el error o usar IF NOT EXISTS según tu versión de SQL Server.

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Habitaciones') AND name = 'PrecioGeneral')
BEGIN
    ALTER TABLE Habitaciones ADD PrecioGeneral DECIMAL(18,2) NOT NULL DEFAULT 0;
END
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Habitaciones') AND name = 'PrecioGobierno')
BEGIN
    ALTER TABLE Habitaciones ADD PrecioGobierno DECIMAL(18,2) NOT NULL DEFAULT 0;
END
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Habitaciones') AND name = 'PrecioCorporativo')
BEGIN
    ALTER TABLE Habitaciones ADD PrecioCorporativo DECIMAL(18,2) NOT NULL DEFAULT 0;
END
GO
