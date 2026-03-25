# Instrucciones para Configurar Hero Banners y Precios

## ⚠️ IMPORTANTE: Ejecutar Scripts SQL Primero

Antes de usar las funcionalidades de configuración, debes ejecutar el script SQL para crear las tablas necesarias.

### Paso 1: Ejecutar Script SQL

1. Abre **SQL Server Management Studio (SSMS)** o tu herramienta de gestión de base de datos
2. Conéctate a tu base de datos del proyecto
3. Abre el archivo: `DB/dbo/Tables/CREAR_TABLAS_CONFIGURACION.sql`
4. Ejecuta el script completo (F5 o botón "Execute")

Este script creará:
- Tabla `ConfiguracionHeroBanner` con valores por defecto
- Tabla `ConfiguracionPreciosDepartamentos` con valores por defecto

### Paso 2: Verificar que las tablas se crearon

Ejecuta esta consulta para verificar:

```sql
SELECT * FROM ConfiguracionHeroBanner;
SELECT * FROM ConfiguracionPreciosDepartamentos;
```

Deberías ver:
- 6 registros en `ConfiguracionHeroBanner` (Home, Habitaciones, Contacto, About, Services, Departamentos)
- 1 registro en `ConfiguracionPreciosDepartamentos` (con precio 275000)

## 🎨 Cómo Usar las Funcionalidades

### Cambiar Imágenes de Hero Banners

1. Inicia sesión como **Administrador**
2. Ve a **Configuraciones** → **Hero Banners**
3. Para cada página, puedes:
   - **Subir una imagen desde tu computadora**: Haz clic en "Elegir archivo" y selecciona una imagen
   - **O ingresar una URL manual**: Escribe la ruta de la imagen en el campo de texto
4. Haz clic en **"Guardar Cambios"**
5. La imagen se actualizará inmediatamente

**Nota**: Las imágenes subidas se guardan en `/Img/hero-banners/`

### Configurar Precios de Departamentos

1. Inicia sesión como **Administrador**
2. Ve a **Configuraciones** → **Precios Departamentos**
3. Edita:
   - **Precio Base**: El precio que se mostrará
   - **Texto del Precio**: Ej: "Por mes", "Por semana", etc.
   - **Mostrar precio**: Marca/desmarca para mostrar u ocultar el precio
4. Haz clic en **"Guardar Cambios"**

## 🔧 Solución de Problemas

### Error: "El nombre de objeto 'dbo.ConfiguracionPreciosDepartamentos' no es válido"

**Solución**: Ejecuta el script SQL `CREAR_TABLAS_CONFIGURACION.sql` en tu base de datos.

### Los cambios no se ven reflejados

1. Verifica que las tablas existan en la base de datos
2. Limpia la caché del navegador (Ctrl + F5)
3. Verifica que los archivos de imagen existan en las rutas especificadas

### No puedo subir imágenes

1. Verifica que la carpeta `/Img/hero-banners/` tenga permisos de escritura
2. Verifica que el formato de imagen sea válido (JPG, PNG, GIF, WEBP)
3. Verifica que el tamaño del archivo no sea demasiado grande














