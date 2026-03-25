USE [db_ac52da_pradoinn]
GO

-- Insertar o actualizar solo las páginas que agregamos/corregimos (Departamentos, Login, Registro)
MERGE [dbo].[ConfiguracionHeroBanner] AS t
USING (VALUES
    ('Departamentos', '/Img/images/Apartamentos/WhatsApp Image 2025-12-17 at 10.33.41 AM.jpeg'),
    ('Login', '/Img/images/login/WhatsApp Image 2025-12-11 at 3.27.01 PM.jpeg'),
    ('Registro', '/Img/images/login/WhatsApp Image 2025-12-11 at 3.27.01 PM.jpeg')
) AS s ([Pagina], [UrlImagen])
ON t.[Pagina] = s.[Pagina]
WHEN MATCHED THEN
    UPDATE SET [UrlImagen] = s.[UrlImagen], [FechaActualizacion] = GETDATE()
WHEN NOT MATCHED BY TARGET THEN
    INSERT ([Pagina], [UrlImagen], [FechaActualizacion], [ActualizadoPor])
    VALUES (s.[Pagina], s.[UrlImagen], GETDATE(), NULL);
GO
