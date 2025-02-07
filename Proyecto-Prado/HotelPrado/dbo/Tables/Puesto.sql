CREATE TABLE [dbo].[Puesto] (
    [IdPuesto]    INT            IDENTITY (1, 1) NOT NULL,
    [Nombre]      NVARCHAR (50)  NOT NULL,
    [Descripcion] NVARCHAR (255) NULL,
    PRIMARY KEY CLUSTERED ([IdPuesto] ASC)
);

