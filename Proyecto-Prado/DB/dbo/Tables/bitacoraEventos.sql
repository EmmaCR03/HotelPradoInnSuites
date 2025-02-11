CREATE TABLE [dbo].[bitacoraEventos] (
    [IdEvento]            INT           IDENTITY (1, 1) NOT NULL,
    [TablaDeEvento]       VARCHAR (20)  NULL,
    [TipoDeEvento]        VARCHAR (20)  NOT NULL,
    [FechaDeEvento]       DATETIME      NOT NULL,
    [DescripcionDeEvento] VARCHAR (MAX) NOT NULL,
    [StackTrace]          VARCHAR (MAX) NULL,
    [DatosAnteriores]     VARCHAR (MAX) NULL,
    [DatosPosteriores]    VARCHAR (MAX) NULL,
    PRIMARY KEY CLUSTERED ([IdEvento] ASC)
);

