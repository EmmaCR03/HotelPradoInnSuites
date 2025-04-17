CREATE TABLE [dbo].[Mantenimiento] (
    [IdMantenimiento]    INT            NOT NULL,
    [Descripcion]        VARCHAR (MAX)  NOT NULL,
    [Estado]             VARCHAR (15)   NOT NULL,
    [idHabitacion]       INT            NULL,
    [idDepartamento]     INT            NULL,
    [DepartamentoNombre] NVARCHAR (100) NULL,
    CONSTRAINT [PK_Mantenimiento] PRIMARY KEY CLUSTERED ([IdMantenimiento] ASC),
    CONSTRAINT [fk_mantenimiento_departamento] FOREIGN KEY ([idDepartamento]) REFERENCES [dbo].[Departamento] ([IdDepartamento]),
    CONSTRAINT [fk_mantenimiento_habitacion] FOREIGN KEY ([idHabitacion]) REFERENCES [dbo].[Habitaciones] ([IdHabitacion])
);

