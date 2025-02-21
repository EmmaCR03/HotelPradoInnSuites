CREATE TABLE [dbo].[TipoHabitacion] (
    [IdTipoHabitacion] INT           IDENTITY (1, 1) NOT NULL,
    [Nombre]           VARCHAR (25)  NULL,
    [Equipamiento]     VARCHAR (100) NULL,
    CONSTRAINT [PK_TipoHabitacion] PRIMARY KEY CLUSTERED ([IdTipoHabitacion] ASC)
);

