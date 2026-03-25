# Optimizaciones de rendimiento aplicadas

## Cambios realizados

### 1. HomeController – Hero Banner
- **Caché en memoria (5 min):** La URL del hero banner se guarda en `HttpRuntime.Cache` por página (Home, About, Contacto, Services). Se evitan consultas a la BD en cada visita.
- **Consulta más ligera:** `AsNoTracking()`, `AutoDetectChangesEnabled = false` y `Select(c => c.UrlImagen)` para leer solo el campo necesario.

### 2. Página de inicio (Index)
- **Output cache (90 s):** La respuesta de la portada se cachea en el servidor 90 segundos. Las visitas repetidas no vuelven a ejecutar la acción ni a tocar la BD en ese tiempo.

### 3. Global.asax – Arranque
- **RolInitialize retrasado 5 s:** Se ejecuta 5 segundos después del inicio para no competir con la primera petición.
- **Limpieza automática:** Ya no se ejecuta al iniciar; solo cada 24 h. El primer arranque es más rápido.

### 4. Web.Release.config – Conexión a BD
- **Connection Timeout:** 30 → 15 s (fallos antes si la BD no responde).
- **Min Pool Size:** 5 → 0 (no se mantienen conexiones abiertas sin uso en hosting compartido).
- **Max Pool Size:** 100 → 50 (límite más razonable para el plan).

## Cómo aplicar los cambios en el servidor

1. Compilar en **Release** y **Publicar** a PradoPubli.
2. Subir al menos:
   - `bin/HotelPrado.UI.dll`
   - `Web.config` (o que se aplique la transformación de Release al publicar).

## Recomendaciones adicionales (opcional)

### En el servidor / hosting
- **customErrors:** Cuando todo funcione bien, poner `mode="RemoteOnly"` para no mostrar detalles de error al público.
- **Compresión:** En `Web.config` ya está `urlCompression` (estática y dinámica). Comprobar que el hosting no la desactive.

### Front-end (si sigue lento)
- **_Layout.cshtml** carga muchos CSS (Bootstrap, varios propios, Google Fonts, Font Awesome). Valorar:
  - Unificar o reducir hojas de estilo.
  - Cargar Font Awesome o fuentes solo donde haga falta.
- **Imágenes:** Usar tamaños adecuados y formato WebP donde sea posible; evitar subir imágenes muy grandes.
- **CDN:** Bootstrap y Font Awesome ya van por CDN; está bien para caché del navegador.

### Base de datos
- Revisar que existan índices en tablas muy usadas (por ejemplo en `ConfiguracionHeroBanner`: `Pagina`, `FechaActualizacion`).
- En otras pantallas que listen muchos datos, considerar paginación o límites.

## Resumen

Con los cambios actuales:
- La portada hace muchas menos consultas a la BD (caché de hero + output cache).
- El arranque de la aplicación es más ligero.
- La conexión a la BD está mejor ajustada para hosting compartido.

Si después de publicar sigues notando lentitud, el siguiente paso es ver qué página o acción concreta tarda (por ejemplo con las herramientas de red del navegador o con logs en el servidor) y optimizar esa parte (consultas, caché o tamaño de respuestas).
