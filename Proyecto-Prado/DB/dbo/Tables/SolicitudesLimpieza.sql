CREATE TABLE [dbo].[SolicitudesLimpieza] (
    [IdSolicitudLimpieza]    INT            IDENTITY (1, 1) NOT NULL,
    [Descripcion]            VARCHAR (MAX)  NOT NULL,
    [Estado]                 VARCHAR (15)   NOT NULL DEFAULT 'Pendiente',
    [idHabitacion]           INT            NULL,
    [idDepartamento]         INT            NULL,
    [DepartamentoNombre]      NVARCHAR (100) NULL,
    [FechaSolicitud]         DATETIME       NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_SolicitudesLimpieza] PRIMARY KEY CLUSTERED ([IdSolicitudLimpieza] ASC),
    CONSTRAINT [fk_solicitud_limpieza_departamento] FOREIGN KEY ([idDepartamento]) REFERENCES [dbo].[Departamento] ([IdDepartamento]),
    CONSTRAINT [fk_solicitud_limpieza_habitacion] FOREIGN KEY ([idHabitacion]) REFERENCES [dbo].[Habitaciones] ([IdHabitacion])
);




