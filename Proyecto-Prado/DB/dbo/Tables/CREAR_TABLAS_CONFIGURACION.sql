-- =============================================
-- Script para crear tablas de configuración
-- Ejecutar este script en la base de datos
-- =============================================

-- 1. Crear tabla ConfiguracionHeroBanner
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ConfiguracionHeroBanner')
BEGIN
    CREATE TABLE [dbo].[ConfiguracionHeroBanner] (
        [IdConfiguracion] INT IDENTITY (1, 1) NOT NULL,
        [Pagina]          VARCHAR (50)   NOT NULL,
        [UrlImagen]       VARCHAR (MAX)  NOT NULL,
        [FechaActualizacion] DATETIME DEFAULT GETDATE() NOT NULL,
        [ActualizadoPor]  NVARCHAR (128) NULL,
        PRIMARY KEY CLUSTERED ([IdConfiguracion] ASC),
        CONSTRAINT [UQ_ConfiguracionHeroBanner_Pagina] UNIQUE ([Pagina])
    );
    
    PRINT 'Tabla ConfiguracionHeroBanner creada exitosamente';
END
ELSE
BEGIN
    PRINT 'Tabla ConfiguracionHeroBanner ya existe';
END
GO

-- 2. Insertar valores por defecto en ConfiguracionHeroBanner (solo si no existen)
IF NOT EXISTS (SELECT * FROM [dbo].[ConfiguracionHeroBanner] WHERE Pagina = 'Home')
BEGIN
    INSERT INTO [dbo].[ConfiguracionHeroBanner] ([Pagina], [UrlImagen])
    VALUES 
        ('Home', '/Img/images/img_3.jpg'),
        ('Habitaciones', '/Img/images/IMG_2.JPG'),
        ('Contacto', '/Img/images/Contactenos/WhatsApp Image 2025-12-11 at 3.27.01 PM.jpeg'),
        ('About', '/Img/images/IMG_3.JPG'),
        ('Services', '/Img/images/Servicios/WhatsApp SIPEPE2025-12-11 at 3.27.00 PM.jpeg'),
        ('Departamentos', '/Img/images/Apartamentos/WhatsApp Image 2025-12-17 at 10.33.41 AM.jpeg'),
        ('Login', '/Img/images/login/WhatsApp Image 2025-12-11 at 3.27.01 PM.jpeg'),
        ('Registro', '/Img/images/login/WhatsApp Image 2025-12-11 at 3.27.01 PM.jpeg');
    
    PRINT 'Valores por defecto insertados en ConfiguracionHeroBanner';
END
ELSE
BEGIN
    PRINT 'Valores por defecto ya existen en ConfiguracionHeroBanner';
END
GO

-- 3. Crear tabla ConfiguracionPreciosDepartamentos
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ConfiguracionPreciosDepartamentos')
BEGIN
    CREATE TABLE [dbo].[ConfiguracionPreciosDepartamentos] (
        [IdConfiguracion] INT IDENTITY (1, 1) NOT NULL,
        [PrecioBase]      DECIMAL (10, 2) NOT NULL,
        [TextoPrecio]     VARCHAR (100)   NULL,
        [MostrarPrecio]   BIT DEFAULT 1 NOT NULL,
        [FechaActualizacion] DATETIME DEFAULT GETDATE() NOT NULL,
        [ActualizadoPor]  NVARCHAR (128) NULL,
        PRIMARY KEY CLUSTERED ([IdConfiguracion] ASC)
    );
    
    PRINT 'Tabla ConfiguracionPreciosDepartamentos creada exitosamente';
END
ELSE
BEGIN
    PRINT 'Tabla ConfiguracionPreciosDepartamentos ya existe';
END
GO

-- 4. Insertar valor por defecto en ConfiguracionPreciosDepartamentos (solo si no existe)
IF NOT EXISTS (SELECT * FROM [dbo].[ConfiguracionPreciosDepartamentos])
BEGIN
    INSERT INTO [dbo].[ConfiguracionPreciosDepartamentos] ([PrecioBase], [TextoPrecio], [MostrarPrecio])
    VALUES (275000.00, 'Por mes', 1);
    
    PRINT 'Valor por defecto insertado en ConfiguracionPreciosDepartamentos';
END
ELSE
BEGIN
    PRINT 'Valor por defecto ya existe en ConfiguracionPreciosDepartamentos';
END
GO

PRINT '========================================';
PRINT 'Script completado exitosamente';
PRINT '========================================';














