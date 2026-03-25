CREATE TABLE [dbo].[ConfiguracionPreciosDepartamentos] (
    [IdConfiguracion] INT IDENTITY (1, 1) NOT NULL,
    [PrecioBase]      DECIMAL (10, 2) NOT NULL,
    [TextoPrecio]     VARCHAR (100)   NULL,
    [MostrarPrecio]   BIT DEFAULT 1 NOT NULL,
    [FechaActualizacion] DATETIME DEFAULT GETDATE() NOT NULL,
    [ActualizadoPor]  NVARCHAR (128) NULL,
    PRIMARY KEY CLUSTERED ([IdConfiguracion] ASC)
);

-- Insertar valor por defecto
INSERT INTO [dbo].[ConfiguracionPreciosDepartamentos] ([PrecioBase], [TextoPrecio], [MostrarPrecio])
VALUES (275000.00, 'Por mes', 1);

