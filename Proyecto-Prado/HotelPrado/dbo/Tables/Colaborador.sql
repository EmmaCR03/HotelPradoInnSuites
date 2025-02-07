CREATE TABLE [dbo].[Colaborador] (
    [IdColaborador]              INT           IDENTITY (1, 1) NOT NULL,
    [NombreColaborador]          VARCHAR (100) NOT NULL,
    [PrimerApellidoColaborador]  VARCHAR (100) NOT NULL,
    [SegundoApellidoColaborador] VARCHAR (100) NOT NULL,
    [CedulaColaborador]          INT           NOT NULL,
    [PuestoColaborador]          VARCHAR (100) NOT NULL,
    [EstadoLaboral]              VARCHAR (50)  NOT NULL,
    [IngresoColaborador]         INT           NULL,
    PRIMARY KEY CLUSTERED ([IdColaborador] ASC),
    UNIQUE NONCLUSTERED ([CedulaColaborador] ASC)
);

