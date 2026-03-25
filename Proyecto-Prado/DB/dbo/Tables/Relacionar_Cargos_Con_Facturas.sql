-- =============================================
-- Script para Relacionar Cargos con Facturas
-- =============================================
-- Este script relaciona los Cargos existentes con las Facturas
-- usando campos comunes como QuienPaga o agrupando cargos para crear facturas
-- =============================================

PRINT '========================================';
PRINT 'RELACIONANDO CARGOS CON FACTURAS';
PRINT '========================================';
PRINT '';

-- =============================================
-- Paso 1: Relacionar Cargos con Facturas existentes por QuienPaga
-- =============================================

PRINT 'Paso 1: Relacionando cargos con facturas existentes por QuienPaga...';
PRINT '';

DECLARE @CargosRelacionados INT = 0;
DECLARE @CargosSinFactura INT = 0;

-- Contar cargos que pueden relacionarse
SELECT @CargosSinFactura = COUNT(*)
FROM Cargos c
WHERE c.IdFactura IS NULL
  AND c.QuienPaga IS NOT NULL
  AND EXISTS (
      SELECT 1 FROM Facturas f 
      WHERE f.QuienPaga = c.QuienPaga
  );

PRINT CONCAT('Cargos que pueden relacionarse con facturas existentes: ', @CargosSinFactura);
PRINT '';

-- Relacionar cargos con facturas por QuienPaga
-- Tomamos la factura más reciente para cada QuienPaga
UPDATE c
SET c.IdFactura = f.IdFactura
FROM Cargos c
INNER JOIN (
    SELECT 
        f.IdFactura,
        f.QuienPaga,
        ROW_NUMBER() OVER (PARTITION BY f.QuienPaga ORDER BY f.FechaHoraFactura DESC) AS rn
    FROM Facturas f
    WHERE f.QuienPaga IS NOT NULL
) f ON c.QuienPaga = f.QuienPaga AND f.rn = 1
WHERE c.IdFactura IS NULL
  AND c.QuienPaga IS NOT NULL;

SET @CargosRelacionados = @@ROWCOUNT;

PRINT CONCAT('Cargos relacionados con facturas existentes: ', @CargosRelacionados);
PRINT '';

-- =============================================
-- Paso 2: Crear Facturas a partir de Cargos agrupados (OPCIONAL)
-- =============================================

PRINT 'Paso 2: Crear facturas a partir de cargos agrupados (opcional)...';
PRINT '';

-- Verificar si hay cargos sin factura que puedan agruparse
DECLARE @CargosParaAgrupar INT = 0;

SELECT @CargosParaAgrupar = COUNT(DISTINCT c.QuienPaga)
FROM Cargos c
WHERE c.IdFactura IS NULL
  AND c.QuienPaga IS NOT NULL
  AND NOT EXISTS (
      SELECT 1 FROM Facturas f 
      WHERE f.QuienPaga = c.QuienPaga
  );

PRINT CONCAT('Grupos de cargos (por QuienPaga) que pueden crear nuevas facturas: ', @CargosParaAgrupar);
PRINT '';

IF @CargosParaAgrupar > 0
BEGIN
    PRINT '¿Deseas crear facturas automáticamente?';
    PRINT 'Descomenta la siguiente sección si quieres crear facturas a partir de cargos agrupados.';
    PRINT '';
    
    /*
    -- Crear facturas a partir de cargos agrupados por QuienPaga
    INSERT INTO Facturas (
        NumeroFactura,
        IdEmpleado,
        FechaHoraFactura,
        TotalConsumos,
        ImpuestoICT,
        ImpuestoServicio,
        ImpuestoVentas,
        TotalGeneral,
        QuienPaga,
        Cerrado,
        EnFacturaExtras,
        FechaCreacion,
        FechaModificacion
    )
    SELECT 
        -- Generar un número de factura único (puedes ajustar la lógica)
        (SELECT ISNULL(MAX(NumeroFactura), 0) FROM Facturas) + ROW_NUMBER() OVER (ORDER BY grupo.QuienPaga) AS NumeroFactura,
        MAX(CAST(grupo.NumeroEmpleado AS INT)) AS IdEmpleado,
        MAX(grupo.FechaHora) AS FechaHoraFactura,
        SUM(grupo.MontoCargo) AS TotalConsumos,
        SUM(grupo.ImpuestoHotel) AS ImpuestoICT,  -- Ajustar según corresponda
        SUM(grupo.ImpuestoServicio) AS ImpuestoServicio,
        SUM(grupo.ImpuestoVenta) AS ImpuestoVentas,
        SUM(grupo.MontoTotal) AS TotalGeneral,
        grupo.QuienPaga,
        0 AS Cerrado,
        0 AS EnFacturaExtras,
        GETDATE() AS FechaCreacion,
        GETDATE() AS FechaModificacion
    FROM (
        SELECT 
            c.QuienPaga,
            c.NumeroEmpleado,
            c.FechaHora,
            c.MontoCargo,
            c.ImpuestoHotel,
            c.ImpuestoServicio,
            c.ImpuestoVenta,
            c.MontoTotal
        FROM Cargos c
        WHERE c.IdFactura IS NULL
          AND c.QuienPaga IS NOT NULL
          AND NOT EXISTS (
              SELECT 1 FROM Facturas f 
              WHERE f.QuienPaga = c.QuienPaga
          )
    ) grupo
    GROUP BY grupo.QuienPaga;
    
    -- Relacionar los cargos con las facturas recién creadas
    UPDATE c
    SET c.IdFactura = f.IdFactura
    FROM Cargos c
    INNER JOIN Facturas f ON c.QuienPaga = f.QuienPaga
    WHERE c.IdFactura IS NULL
      AND f.FechaCreacion >= DATEADD(MINUTE, -1, GETDATE());  -- Facturas recién creadas
    
    PRINT CONCAT('Facturas creadas: ', @@ROWCOUNT);
    */
END

-- =============================================
-- Paso 3: Relacionar por IdCheckIn (si ambos tienen ese campo)
-- =============================================

PRINT '';
PRINT 'Paso 3: Relacionando cargos con facturas por IdCheckIn...';
PRINT '';

DECLARE @CargosRelacionadosPorCheckIn INT = 0;

-- Relacionar cargos con facturas por IdCheckIn
UPDATE c
SET c.IdFactura = f.IdFactura
FROM Cargos c
INNER JOIN Facturas f ON c.IdCheckIn = f.IdCheckIn
WHERE c.IdFactura IS NULL
  AND c.IdCheckIn IS NOT NULL
  AND f.IdCheckIn IS NOT NULL;

SET @CargosRelacionadosPorCheckIn = @@ROWCOUNT;

PRINT CONCAT('Cargos relacionados por IdCheckIn: ', @CargosRelacionadosPorCheckIn);
PRINT '';

-- =============================================
-- Resumen Final
-- =============================================

PRINT '';
PRINT '========================================';
PRINT 'RESUMEN';
PRINT '========================================';

DECLARE @TotalCargos INT;
DECLARE @CargosConFactura INT;
DECLARE @CargosSinRelacion INT;

SELECT @TotalCargos = COUNT(*) FROM Cargos;
SELECT @CargosConFactura = COUNT(*) FROM Cargos WHERE IdFactura IS NOT NULL;
SELECT @CargosSinRelacion = COUNT(*) FROM Cargos WHERE IdFactura IS NULL;

PRINT CONCAT('Total de cargos: ', @TotalCargos);
PRINT CONCAT('Cargos relacionados con facturas: ', @CargosConFactura);
PRINT CONCAT('Cargos sin relación: ', @CargosSinRelacion);
PRINT CONCAT('Porcentaje relacionado: ', CAST((@CargosConFactura * 100.0 / NULLIF(@TotalCargos, 0)) AS DECIMAL(5,2)), '%');
PRINT '';

-- Mostrar algunos ejemplos de cargos sin relación
IF @CargosSinRelacion > 0
BEGIN
    PRINT 'Ejemplos de cargos sin relación (primeros 10):';
    SELECT TOP 10
        IdCargo,
        QuienPaga,
        NumeroFolioOriginal,
        IdCheckIn,
        FechaHora,
        MontoTotal
    FROM Cargos
    WHERE IdFactura IS NULL
    ORDER BY FechaHora DESC;
END

PRINT '';
PRINT '========================================';
PRINT 'PROCESO COMPLETADO';
PRINT '========================================';
GO

