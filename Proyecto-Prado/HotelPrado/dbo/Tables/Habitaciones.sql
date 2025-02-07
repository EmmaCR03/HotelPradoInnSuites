CREATE TABLE [dbo].[Habitaciones] (
    [IdHabitacion]     INT          IDENTITY (1, 1) NOT NULL,
    [NumeroHabitacion] VARCHAR (5)  NULL,
    [Estado]           VARCHAR (15) NULL,
    [PrecioPorNoche]   VARCHAR (50) NULL,
    [IdTipoHabitacion] INT          NULL,
    CONSTRAINT [PK_Habitaciones] PRIMARY KEY CLUSTERED ([IdHabitacion] ASC),
    CONSTRAINT [FK_Habitaciones_TipoHabitacion] FOREIGN KEY ([IdTipoHabitacion]) REFERENCES [dbo].[TipoHabitacion] ([IdTipoHabitacion])
);

