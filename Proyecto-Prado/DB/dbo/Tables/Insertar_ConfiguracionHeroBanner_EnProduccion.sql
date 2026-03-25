-- Ejecutar este script en la base de datos del HOST (producción)
-- para que las páginas usen las mismas imágenes de hero que en local.
-- La tabla ConfiguracionHeroBanner debe existir (creada por tu proyecto/migración).

-- Crea la tabla si no existe (por si no la tienes en producción)
IF OBJECT_ID('[dbo].[ConfiguracionHeroBanner]', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[ConfiguracionHeroBanner] (
        [IdConfiguracion]    INT IDENTITY(1,1) NOT NULL,
        [Pagina]             VARCHAR(50) NOT NULL,
        [UrlImagen]          VARCHAR(MAX) NOT NULL,
        [FechaActualizacion] DATETIME NOT NULL DEFAULT GETDATE(),
        [ActualizadoPor]     NVARCHAR(128) NULL,
        PRIMARY KEY ([IdConfiguracion]),
        CONSTRAINT [UQ_ConfiguracionHeroBanner_Pagina] UNIQUE ([Pagina])
    );
END
GO

-- Insertar o actualizar la configuración de cada página
MERGE [dbo].[ConfiguracionHeroBanner] AS t
USING (VALUES
    ('Home', '/Img/images/img_1.JPG'),
    ('Habitaciones', '/Img/images/IMG_2.JPG'),
    ('Contacto', '/Img/images/Contactenos/WhatsApp Image 2025-12-11 at 3.27.01 PM.jpeg'),
    ('About', '/Img/images/IMG_3.JPG'),
    ('Services', '/Img/images/Servicios/WhatsApp SIPEPE2025-12-11 at 3.27.00 PM.jpeg'),
    ('Departamentos', '/Img/images/Apartamentos/WhatsApp Image 2025-12-17 at 10.33.41 AM.jpeg'),
    ('Login', '/Img/images/login/WhatsApp Image 2025-12-11 at 3.27.01 PM.jpeg'),
    ('Registro', '/Img/images/login/WhatsApp Image 2025-12-11 at 3.27.01 PM.jpeg')
) AS s ([Pagina], [UrlImagen])
ON t.[Pagina] = s.[Pagina]
WHEN MATCHED THEN
    UPDATE SET [UrlImagen] = s.[UrlImagen], [FechaActualizacion] = GETDATE()
WHEN NOT MATCHED BY TARGET THEN
    INSERT ([Pagina], [UrlImagen], [FechaActualizacion])
    VALUES (s.[Pagina], s.[UrlImagen], GETDATE());
GO
