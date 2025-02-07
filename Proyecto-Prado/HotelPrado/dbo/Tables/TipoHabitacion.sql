CREATE TABLE [dbo].[TipoHabitacion] (
    [IdTipoHabitacion]    INT          IDENTITY (1, 1) NOT NULL,
    [Descripcion]         VARCHAR (25) NULL,
    [CapacidadDePersonas] INT          NULL,
    CONSTRAINT [PK_TipoHabitacion] PRIMARY KEY CLUSTERED ([IdTipoHabitacion] ASC)
);

