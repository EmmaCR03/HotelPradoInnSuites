CREATE TABLE [dbo].[ImagenesDepartamento] (
    [IdImagen]       INT           IDENTITY (1, 1) NOT NULL,
    [IdDepartamento] INT           NOT NULL,
    [UrlImagen]      VARCHAR (500) NOT NULL,
    PRIMARY KEY CLUSTERED ([IdImagen] ASC),
    FOREIGN KEY ([IdDepartamento]) REFERENCES [dbo].[Departamento] ([IdDepartamento]) ON DELETE CASCADE
);

