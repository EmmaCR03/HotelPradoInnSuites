CREATE TABLE [dbo].[Reservas] (
    [IdReserva]     INT             IDENTITY (1, 1) NOT NULL,
    [IdCliente]     NVARCHAR (128)  NOT NULL,
    [NombreCliente] NVARCHAR (128)  NULL,
    [IdHabitacion]  INT             NOT NULL,
    [FechaInicio]   DATETIME        NULL,
    [FechaFinal]    DATETIME        NULL,
    [EstadoReserva] NVARCHAR (50)   NOT NULL,
    [MontoTotal]    DECIMAL (18, 2) NOT NULL,
    [cantidadPersonas] INT NULL, 
    PRIMARY KEY CLUSTERED ([IdReserva] ASC),
    CONSTRAINT [FK_Reservas_Habitacion] FOREIGN KEY ([IdHabitacion]) REFERENCES [dbo].[Habitaciones] ([IdHabitacion]),
    CONSTRAINT [FK_Reservas_Usuarios] FOREIGN KEY ([IdCliente]) REFERENCES [dbo].[AspNetUsers] ([Id])
);

