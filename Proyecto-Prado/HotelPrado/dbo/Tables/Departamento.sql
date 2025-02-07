CREATE TABLE [dbo].[Departamento] (
    [IdDepartamento]     INT             IDENTITY (1, 1) NOT NULL,
    [Nombre]             VARCHAR (50)    NULL,
    [IdTipoDepartamento] INT             NOT NULL,
    [Precio]             DECIMAL (10, 2) NOT NULL,
    [Estado]             NVARCHAR (50)   DEFAULT ('Disponible') NOT NULL,
    [IdCliente]          INT             NULL,
    [Descripcion]        VARCHAR (255)   NULL,
    [UrlImagenes]        VARCHAR (MAX)   NULL,
    [NumeroDepartamento] INT             NULL,
    [NumeroEmpresa]      VARCHAR (50)    NULL,
    [CorreoEmpresa]      VARCHAR (50)    NULL,
    PRIMARY KEY CLUSTERED ([IdDepartamento] ASC),
    FOREIGN KEY ([IdTipoDepartamento]) REFERENCES [dbo].[TipoDepartamento] ([IdTipoDepartamento]),
    CONSTRAINT [FK_Departamento_Cliente] FOREIGN KEY ([IdCliente]) REFERENCES [dbo].[Cliente] ([IdCliente])
);

