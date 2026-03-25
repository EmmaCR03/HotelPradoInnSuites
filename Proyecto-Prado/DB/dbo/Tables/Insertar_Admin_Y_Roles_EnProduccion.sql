-- Ejecutar en la base de datos del HOST (producción)
-- Crea los 4 roles, el usuario administrador y le asigna el rol Administrador.
-- Contraseña del admin: Admin123456!

SET NOCOUNT ON;

-- 1) Crear roles si no existen (Administrador, Colaborador, Cliente, Usuarios)
IF NOT EXISTS (SELECT 1 FROM [dbo].[AspNetRoles] WHERE [Name] = N'Administrador')
    INSERT INTO [dbo].[AspNetRoles] ([Id], [Name]) VALUES (NEWID(), N'Administrador');
IF NOT EXISTS (SELECT 1 FROM [dbo].[AspNetRoles] WHERE [Name] = N'Colaborador')
    INSERT INTO [dbo].[AspNetRoles] ([Id], [Name]) VALUES (NEWID(), N'Colaborador');
IF NOT EXISTS (SELECT 1 FROM [dbo].[AspNetRoles] WHERE [Name] = N'Cliente')
    INSERT INTO [dbo].[AspNetRoles] ([Id], [Name]) VALUES (NEWID(), N'Cliente');
IF NOT EXISTS (SELECT 1 FROM [dbo].[AspNetRoles] WHERE [Name] = N'Usuarios')
    INSERT INTO [dbo].[AspNetRoles] ([Id], [Name]) VALUES (NEWID(), N'Usuarios');

-- 2) Usuario administrador (contraseña: Admin123456! - hash compatible con ASP.NET Identity)
DECLARE @AdminUserId NVARCHAR(128) = N'39570811-3138-4507-bb75-bffdf38c87be';

IF NOT EXISTS (SELECT 1 FROM [dbo].[AspNetUsers] WHERE [Id] = @AdminUserId)
BEGIN
    INSERT INTO [dbo].[AspNetUsers] (
        [Id],
        [Email],
        [EmailConfirmed],
        [PasswordHash],
        [SecurityStamp],
        [PhoneNumber],
        [PhoneNumberConfirmed],
        [TwoFactorEnabled],
        [LockoutEndDateUtc],
        [LockoutEnabled],
        [AccessFailedCount],
        [UserName],
        [Telefono],
        [NombreCompleto],
        [cedula]
    ) VALUES (
        @AdminUserId,
        N'adminHotel@hotelPrado.com',
        0,
        N'AMfGD3Y+EiN7OreTP0FgkJIkoAQLHGJq6qNnI+8zZ4zAyLbNHvLPd0mCnMAT8xMqzQ==',
        N'3b6869c1-e792-4d79-bf23-9285971befcc',
        NULL,
        0,
        0,
        NULL,
        0,
        0,
        N'adminHotel@hotelPrado.com',
        N'87464858',
        N'Admin Hotel',
        N'001234569'
    );
    PRINT 'Usuario administrador creado.';
END
ELSE
    PRINT 'El usuario administrador ya existe.';

-- 3) Asignar rol Administrador al usuario admin
DECLARE @RolAdministradorId NVARCHAR(128);
SELECT @RolAdministradorId = [Id] FROM [dbo].[AspNetRoles] WHERE [Name] = N'Administrador';

IF @RolAdministradorId IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM [dbo].[AspNetUserRoles] WHERE [UserId] = @AdminUserId AND [RoleId] = @RolAdministradorId)
    BEGIN
        INSERT INTO [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (@AdminUserId, @RolAdministradorId);
        PRINT 'Rol Administrador asignado al usuario admin.';
    END
    ELSE
        PRINT 'El usuario admin ya tiene el rol Administrador.';
END
ELSE
    PRINT 'ERROR: No se encontró el rol Administrador.';

SET NOCOUNT OFF;
GO
