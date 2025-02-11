CREATE TABLE [dbo].[Reservas] (
    [IdReserva]     INT             IDENTITY (1, 1) NOT NULL,
    [IdCliente]     INT             NOT NULL,
    [IdHabitacion]  INT             NOT NULL,
    [FechaInicio]   DATETIME        NULL,
    [FechaFinal]    DATETIME        NULL,
    [EstadoReserva] VARCHAR (15)    NULL,
    [MontoTotal]    DECIMAL (10, 2) NULL,
    CONSTRAINT [PK_Reservas] PRIMARY KEY CLUSTERED ([IdReserva] ASC),
    CONSTRAINT [FK_Reservas_Cliente] FOREIGN KEY ([IdCliente]) REFERENCES [dbo].[Cliente] ([IdCliente]),
    CONSTRAINT [FK_Reservas_Habitaciones] FOREIGN KEY ([IdHabitacion]) REFERENCES [dbo].[Habitaciones] ([IdHabitacion])
);

