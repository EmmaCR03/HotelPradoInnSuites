CREATE TABLE [dbo].[ConfiguracionHeroBanner] (
    [IdConfiguracion] INT IDENTITY (1, 1) NOT NULL,
    [Pagina]          VARCHAR (50)   NOT NULL,
    [UrlImagen]       VARCHAR (MAX)  NOT NULL,
    [FechaActualizacion] DATETIME DEFAULT GETDATE() NOT NULL,
    [ActualizadoPor]  NVARCHAR (128) NULL,
    PRIMARY KEY CLUSTERED ([IdConfiguracion] ASC),
    CONSTRAINT [UQ_ConfiguracionHeroBanner_Pagina] UNIQUE ([Pagina])
);

-- Insertar valores por defecto
INSERT INTO [dbo].[ConfiguracionHeroBanner] ([Pagina], [UrlImagen])
VALUES 
    ('Home', '/Img/images/img_3.jpg'),
    ('Habitaciones', '/Img/images/IMG_2.JPG'),
    ('Contacto', '/Img/images/Contactenos/WhatsApp Image 2025-12-11 at 3.27.01 PM.jpeg'),
    ('About', '/Img/images/IMG_3.JPG'),
    ('Services', '/Img/images/Servicios/WhatsApp SIPEPE2025-12-11 at 3.27.00 PM.jpeg'),
    ('Departamentos', '/Img/images/Apartamentos/WhatsApp Imasssssssssssssss.41 AM.jpeg');

