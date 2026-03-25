-- Script para agregar la columna idHabitacion a la tabla Mantenimiento
-- Ejecutar este script en SQL Server Management Studio

USE [HotelPrado]
GO

-- Verificar si la columna ya existe
IF NOT EXISTS (
    SELECT 1 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'Mantenimiento' 
    AND COLUMN_NAME = 'idHabitacion'
)
BEGIN
    -- Agregar la columna idHabitacion
    ALTER TABLE [dbo].[Mantenimiento]
    ADD [idHabitacion] INT NULL;
    
    PRINT 'Columna idHabitacion agregada exitosamente a la tabla Mantenimiento';
END
ELSE
BEGIN
    PRINT 'La columna idHabitacion ya existe en la tabla Mantenimiento';
END
GO

-- Verificar si la foreign key ya existe
IF NOT EXISTS (
    SELECT 1 
    FROM sys.foreign_keys 
    WHERE name = 'fk_mantenimiento_habitacion'
)
BEGIN
    -- Agregar la foreign key constraint
    ALTER TABLE [dbo].[Mantenimiento]
    ADD CONSTRAINT [fk_mantenimiento_habitacion] 
    FOREIGN KEY ([idHabitacion]) 
    REFERENCES [dbo].[Habitaciones] ([IdHabitacion]);
    
    PRINT 'Foreign key fk_mantenimiento_habitacion agregada exitosamente';
END
ELSE
BEGIN
    PRINT 'La foreign key fk_mantenimiento_habitacion ya existe';
END
GO

PRINT 'Script completado exitosamente';
GO

