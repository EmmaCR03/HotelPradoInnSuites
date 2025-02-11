CREATE TABLE [dbo].[TipoDepartamento] (
    [IdTipoDepartamento] INT IDENTITY (1, 1) NOT NULL,
    [NumeroHabitaciones] INT NOT NULL,
    [Amueblado]          BIT NULL,
    PRIMARY KEY CLUSTERED ([IdTipoDepartamento] ASC)
);

