-- Script para eliminar la foreign key constraint que bloquea la asignación de clientes
-- Esta constraint requiere que IdCliente exista en la tabla Cliente,
-- pero estamos usando ApplicationUser (AspNetUsers) en lugar de la tabla Cliente

-- Verificar si la constraint existe antes de eliminarla
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Departamento_Cliente')
BEGIN
    ALTER TABLE [dbo].[Departamento]
    DROP CONSTRAINT [FK_Departamento_Cliente];
    
    PRINT 'Constraint FK_Departamento_Cliente eliminada exitosamente.';
END
ELSE
BEGIN
    PRINT 'La constraint FK_Departamento_Cliente no existe.';
END
GO




