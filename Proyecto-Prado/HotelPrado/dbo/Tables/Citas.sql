CREATE TABLE [dbo].[Citas] (
    [IdCita]          INT           IDENTITY (1, 1) NOT NULL,
    [Nombre]          VARCHAR (100) NOT NULL,
    [PrimerApellido]  VARCHAR (100) NOT NULL,
    [SegundoApellido] VARCHAR (100) NULL,
    [Telefono]        INT           NULL,
    [Correo]          VARCHAR (150) NOT NULL,
    [MedioContacto]   VARCHAR (50)  NOT NULL,
    [IdDepartamento]  INT           NOT NULL,
    [FechaHoraInicio] DATETIME      NULL,
    [FechaHoraFin]    DATETIME      NULL,
    [Estado]          VARCHAR (50)  CONSTRAINT [DF_Citas_Estado] DEFAULT ('CitaPendiente') NOT NULL,
    [Observaciones]   TEXT          NULL,
    [FechaCreacion]   DATETIME      DEFAULT (getdate()) NOT NULL,
    [IdColaborador]   INT           NULL,
    [EnlaceWhatsApp]  VARCHAR (255) NULL,
    [EnlaceCorreo]    VARCHAR (255) NULL,
    PRIMARY KEY CLUSTERED ([IdCita] ASC),
    FOREIGN KEY ([IdColaborador]) REFERENCES [dbo].[Colaborador] ([IdColaborador]),
    FOREIGN KEY ([IdDepartamento]) REFERENCES [dbo].[Departamento] ([IdDepartamento])
);

