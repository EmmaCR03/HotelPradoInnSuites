CREATE TABLE [dbo].[Habitaciones] (
    [IdHabitacion]     INT           IDENTITY (1, 1) NOT NULL,
    [NumeroHabitacion] VARCHAR (5)   NULL,
    [Estado]           VARCHAR (15)  NULL,
    [CapacidadMin]     INT           NULL,
    [PrecioPorNoche2P] DECIMAL (18)  NULL,
    [PrecioPorNoche1P] DECIMAL (18)  NULL,
    [PrecioPorNoche3P] DECIMAL (18)  NULL,
    [PrecioPorNoche4P] DECIMAL (18)  NULL,
    [UrlImagenes]      VARCHAR (MAX) NULL,
    [CapacidadMax]     INT           NULL,
    CONSTRAINT [PK_Habitaciones] PRIMARY KEY CLUSTERED ([IdHabitacion] ASC)
);

