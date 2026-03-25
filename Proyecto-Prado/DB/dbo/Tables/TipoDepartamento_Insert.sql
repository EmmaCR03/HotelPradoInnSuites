-- INSERT para [dbo].[TipoDepartamento]
-- IdTipoDepartamento es IDENTITY, no se incluye en el INSERT.
-- Uso: ejecutar en la base de datos [HotelPrado].

USE [HotelPrado]
GO

-- Opción 1: Insertar los 4 tipos estándar (si la tabla está vacía o en otro ambiente)
INSERT INTO [dbo].[TipoDepartamento]
           ([NumeroHabitaciones]
           ,[Amueblado])
     VALUES
           (1, 1),   -- 1 habitación, amueblado
           (1, 0),   -- 1 habitación, no amueblado
           (2, 1),   -- 2 habitaciones, amueblado
           (2, 0);   -- 2 habitaciones, no amueblado
GO

-- Opción 2: Insertar una sola fila (ejemplo con valores)
-- INSERT INTO [dbo].[TipoDepartamento]
--            ([NumeroHabitaciones]
--            ,[Amueblado])
--      VALUES
--            (1, 1);   -- NumeroHabitaciones = 1, Amueblado = 1 (true)
-- GO

-- Para más filas, repetir en VALUES: (2, 0), (3, 1), ...
