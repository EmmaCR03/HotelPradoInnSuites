-- ============================================================
-- Limpiar bitácora antigua para liberar espacio en la BD
-- La tabla bitacoraEventos crece con cada acción y no se limpia sola.
-- Ejecutar en el servidor (site4now / SQL Server) cuando la BD esté llena.
-- ============================================================

-- Ver cuántos registros hay y desde cuándo (ejecutar primero para revisar)
-- SELECT COUNT(*) AS TotalRegistros, MIN(FechaDeEvento) AS MasAntiguo, MAX(FechaDeEvento) AS MasReciente FROM bitacoraEventos;

-- Borrar registros con más de 90 días (ajustar el número de días si quieres conservar más o menos)
DECLARE @DiasConservar INT = 90;
DECLARE @FechaCorte DATETIME = DATEADD(DAY, -@DiasConservar, GETDATE());
DECLARE @FilasBorradas INT;

DELETE FROM bitacoraEventos
WHERE FechaDeEvento < @FechaCorte;

SET @FilasBorradas = @@ROWCOUNT;

-- Opcional: recuperar espacio (en MSSQL reduce el tamaño del archivo de datos después de borrar)
-- DBCC SHRINKDATABASE (db_ac52da_pradoinn);  -- Usar solo si site4now lo permite

PRINT 'Se eliminaron ' + CAST(@FilasBorradas AS VARCHAR(20)) + ' registros de bitácora anteriores a ' + CONVERT(VARCHAR(20), @FechaCorte, 120);
