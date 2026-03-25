# CSS y archivos no cargan en el hosting – qué revisar

Se hicieron cambios en el código para que las rutas de CSS y scripts usen siempre `Url.Content()` y no dependan de bundles. Aun así, si en el hosting no cargan los estilos, revisa lo siguiente.

---

## 1. Carpeta a la que apunta tu sitio (lo más importante)

La URL `pradoinn-001-site1.stempurl.com` debe apuntar a la **carpeta que contiene** `bin`, `Content`, `Img`, `Scripts`, `Views` y `Web.config` directamente dentro.

- **Correcto:** La “raíz del sitio” o “physical path” es, por ejemplo, `...\www\site1`. Dentro de `site1` están: `bin`, `Content`, `Img`, `Scripts`, `Views`, `Web.config`.
- **Incorrecto:** La raíz es `...\www`. Ahí solo hay carpetas `site1` y `PradoPubli`; no hay `Content` ni `Scripts` directamente en `www`. Entonces el navegador pide `/Content/site.css` y el servidor busca `www\Content\site.css`, que no existe → 404 y sin CSS.

**Qué hacer en el panel (SmarterASP / site4now):**

1. Entra a **WEBSITES** (o Dominios / Sitios).
2. Localiza el sitio **site1** o la URL `pradoinn-001-site1.stempurl.com`.
3. Revisa la **ruta física** o **document root**:
   - Debe ser la carpeta **site1** (algo como `h:\root\home\pradoinn-001\www\site1`), **no** solo `www`.
4. Si está apuntando a `www`, cámbiala a `www\site1` (o la ruta equivalente que te muestre el panel) y guarda.

Así las peticiones a `/Content/...`, `/Scripts/...`, `/Img/...` irán a `site1\Content`, `site1\Scripts`, `site1\Img`, donde están los archivos.

---

## 2. Que los archivos estén dentro de esa carpeta

Dentro de la carpeta que configuraste como raíz del sitio (por ejemplo `site1`) debe haber:

- **Content** (con todos los .css: `site.css`, `layout-styles.css`, `navbar-new.css`, etc.)
- **Scripts** (jquery, bootstrap-datepicker, flatpickr, modernizr, etc.)
- **Img** (images, logo, galería)
- **bin** (DLLs)
- **Views**
- **Web.config**

Si subiste todo en **PradoPubli**, entonces o bien:

- Copias **el contenido** de PradoPubli (no la carpeta) dentro de **site1**, o  
- Cambias la raíz del sitio para que apunte a **PradoPubli** en lugar de **site1**.

No puede ser que la raíz sea `www` y los archivos estén solo en `www\site1` o `www\PradoPubli` sin que el sitio apunte a una de esas dos carpetas.

---

## 3. Ver en el navegador qué falla

1. Abre la página del sitio.
2. **F12** → pestaña **Red** (Network).
3. Recarga la página (F5).
4. Mira qué peticiones salen en **rojo** (404 o error):
   - Si es algo como `https://pradoinn-001-site1.stempurl.com/Content/site.css` → 404: la ruta está bien pero **no existe ese archivo** en la carpeta del sitio (falta subir/copiar **Content**).
   - Si la URL tiene otra ruta rara o apunta a otra carpeta, entonces el **document root** del sitio está mal (vuelve al punto 1).

---

## 4. Cambios que ya tiene el proyecto

En `_Layout.cshtml`:

- Los CSS ya no usan solo el bundle; se cargan con enlaces directos usando `Url.Content("~/Content/...")`.
- Los scripts (jQuery, datepicker, flatpickr, modernizr) también usan `Url.Content("~/Scripts/...")`.
- Las imágenes del layout usan `Url.Content("~/Img/...")`.

Con eso las URLs que se generan son correctas siempre que la **raíz del sitio** en el hosting sea la carpeta que contiene `Content`, `Scripts`, `Img`, etc.

---

Resumen: **Ajusta en el panel que la raíz del sitio sea la carpeta que tiene `Content`, `Scripts`, `Img` (site1 o PradoPubli), y asegúrate de que ahí estén todos los archivos.**
