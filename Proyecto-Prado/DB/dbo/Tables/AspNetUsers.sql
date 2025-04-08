CREATE TABLE [dbo].[AspNetUsers] (
    [Id]                   NVARCHAR (128) NOT NULL,
    [Email]                NVARCHAR (256) NULL,
    [EmailConfirmed]       BIT            NULL,
    [PasswordHash]         NVARCHAR (MAX) NULL,
    [SecurityStamp]        NVARCHAR (MAX) NULL,
    [PhoneNumber]          NVARCHAR (MAX) NULL,
    [PhoneNumberConfirmed] BIT            NULL,
    [TwoFactorEnabled]     BIT            NULL,
    [LockoutEndDateUtc]    DATETIME       NULL,
    [LockoutEnabled]       BIT            NULL,
    [AccessFailedCount]    INT            NULL,
    [UserName]             NVARCHAR (256) NULL,
    [Telefono] NCHAR(10) NULL, 
    [NombreCompleto] NCHAR(50) NULL, 
    [cedula] NCHAR(10) NULL, 
    CONSTRAINT [PK_dbo.AspNetUsers] PRIMARY KEY CLUSTERED ([Id] ASC)
);

