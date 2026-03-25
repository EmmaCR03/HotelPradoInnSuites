-- Script para agregar columna Usuario a la tabla bitacoraEventos
-- Ejecutar este script en SQL Server Management Studio

USE [HotelPrado]
GO

-- Verificar si la columna ya existe antes de agregarla
IF NOT EXISTS (
    SELECT * 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'bitacoraEventos' 
    AND COLUMN_NAME = 'Usuario'
)
BEGIN
    ALTER TABLE [dbo].[bitacoraEventos]
    ADD [Usuario] VARCHAR(100) NULL;
    
    PRINT 'Columna Usuario agregada exitosamente a bitacoraEventos';
END
ELSE
BEGIN
    PRINT 'La columna Usuario ya existe en bitacoraEventos';
END
GO











