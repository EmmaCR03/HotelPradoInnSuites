-- =============================================
-- Script para crear registros dummy en Emisores y Receptores
-- Necesario para poder insertar facturas
-- =============================================

-- Crear emisor dummy si no existe
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Emisores')
BEGIN
    IF NOT EXISTS (SELECT * FROM Emisores)
    BEGIN
        -- Intentar insertar con solo Id (si es la única columna requerida)
        BEGIN TRY
            INSERT INTO Emisores (Id)
            VALUES (NEWID());
            PRINT 'Registro dummy creado en Emisores';
        END TRY
        BEGIN CATCH
            -- Si falla, mostrar las columnas requeridas
            PRINT 'ERROR al crear emisor dummy. Verifica las columnas requeridas en Emisores.';
            PRINT ERROR_MESSAGE();
        END CATCH
    END
    ELSE
    BEGIN
        PRINT 'Emisores ya tiene registros';
    END
END
ELSE
BEGIN
    PRINT 'La tabla Emisores no existe';
END
GO

-- Crear receptor dummy si no existe
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Receptores')
BEGIN
    IF NOT EXISTS (SELECT * FROM Receptores)
    BEGIN
        BEGIN TRY
            INSERT INTO Receptores (Id)
            VALUES (NEWID());
            PRINT 'Registro dummy creado en Receptores';
        END TRY
        BEGIN CATCH
            PRINT 'ERROR al crear receptor dummy. Verifica las columnas requeridas en Receptores.';
            PRINT ERROR_MESSAGE();
        END CATCH
    END
    ELSE
    BEGIN
        PRINT 'Receptores ya tiene registros';
    END
END
ELSE
BEGIN
    PRINT 'La tabla Receptores no existe';
END
GO

