-- Actualizar solo la imagen de hero de Departamentos
UPDATE [dbo].[ConfiguracionHeroBanner]
SET [UrlImagen] = '/Img/images/Apartamentos/WhatsApp Imasssssssssssssss.41 AM.jpeg',
    [FechaActualizacion] = GETDATE()
WHERE [Pagina] = 'Departamentos';
