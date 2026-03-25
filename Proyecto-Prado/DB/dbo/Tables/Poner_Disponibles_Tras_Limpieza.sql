-- ============================================================
-- Poner habitaciones y departamentos en Disponible tras limpieza
-- Ejecutar cuando ya terminaron la limpieza pero el plano sigue mostrando "Limpieza".
-- Luego podrá probar que al marcar Completado desde Editar/CambiarEstado ya los deja disponibles.
-- ============================================================

-- 1) CONSULTAS: ver qué está en Limpieza
SELECT IdHabitacion, NumeroHabitacion, Estado 
FROM Habitaciones 
WHERE LOWER(LTRIM(RTRIM(Estado))) = 'limpieza';

SELECT IdDepartamento, NumeroDepartamento, Estado 
FROM Departamento 
WHERE LOWER(LTRIM(RTRIM(Estado))) = 'limpieza';

-- 2) ACTUALIZAR: poner como Disponible (ejecute solo los UPDATE que necesite)
-- Habitaciones en Limpieza -> Disponible
UPDATE Habitaciones 
SET Estado = 'Disponible'
WHERE LOWER(LTRIM(RTRIM(Estado))) = 'limpieza';

-- Departamentos en Limpieza -> Disponible
UPDATE Departamento 
SET Estado = 'Disponible'
WHERE LOWER(LTRIM(RTRIM(Estado))) = 'limpieza';

-- 3) OPCIONAL: solo las que tienen solicitud de limpieza ya "Completado" (más conservador)
-- Descomente si prefiere tocar solo esas:
/*
UPDATE h
SET h.Estado = 'Disponible'
FROM Habitaciones h
WHERE LOWER(LTRIM(RTRIM(h.Estado))) = 'limpieza'
  AND EXISTS (
    SELECT 1 FROM SolicitudesLimpieza s 
    WHERE s.idHabitacion = h.IdHabitacion 
      AND LOWER(LTRIM(RTRIM(s.Estado))) = 'completado'
  );

UPDATE d
SET d.Estado = 'Disponible'
FROM Departamento d
WHERE LOWER(LTRIM(RTRIM(d.Estado))) = 'limpieza'
  AND EXISTS (
    SELECT 1 FROM SolicitudesLimpieza s 
    WHERE s.idDepartamento = d.IdDepartamento 
      AND LOWER(LTRIM(RTRIM(s.Estado))) = 'completado'
  );
*/

-- 4) Comprobar: no debería quedar ninguna en limpieza
SELECT IdHabitacion, NumeroHabitacion, Estado FROM Habitaciones WHERE LOWER(LTRIM(RTRIM(Estado))) = 'limpieza';
SELECT IdDepartamento, NumeroDepartamento, Estado FROM Departamento WHERE LOWER(LTRIM(RTRIM(Estado))) = 'limpieza';
