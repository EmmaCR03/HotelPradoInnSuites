CREATE TABLE [dbo].[ImagenesHabitacion] (
    [IdImagen]     INT            IDENTITY (1, 1) NOT NULL,
    [IdHabitacion] INT            NOT NULL,
    [UrlImagen]    NVARCHAR (255) NOT NULL,
    PRIMARY KEY CLUSTERED ([IdImagen] ASC)
);

