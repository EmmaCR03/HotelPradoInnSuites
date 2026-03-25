# Subir toda la carpeta por FTP (para que el diseño y CSS carguen bien)

Si el sitio hosteado se ve sin estilos, suele ser porque no se subieron todas las carpetas o archivos. La solución es **subir todo** el contenido de la publicación.

---

## En FileZilla: arrastrar TODO

### 1. Conectar
- Servidor: `win8232.site4now.net`
- Usuario: `pradoinn-001`
- Contraseña: (tu contraseña FTP)
- **Conexión rápida**

### 2. Panel izquierdo (tu PC)
- Ve a: **`C:\Users\emmag\OneDrive\Documentos\PradoPubli`**
- En la **lista de abajo** (archivos y carpetas), selecciona **todo**:
  - **Ctrl + A** (selecciona todo lo que hay dentro de PradoPubli)
  - Debes tener seleccionado: `bin`, `Content`, `Img`, `Properties`, `Scripts`, `Views`, `favicon.ico`, `Global.asax`, `Web.config`, etc.

### 3. Panel derecho (servidor)
- Entra en **`/site1`** (que sea la carpeta donde está tu sitio).
- Asegúrate de que la ruta superior diga **/site1**.

### 4. Arrastrar
- **Arrastra** la selección del panel izquierdo hacia el panel derecho (hacia `/site1`).
- Cuando pregunte **"¿Reemplazar?"** o **"¿Sobrescribir archivos existentes?"**, elige **"Sí a todos"** o **"Overwrite all"** para que se actualicen todos los archivos.

### 5. Esperar
- En la parte de abajo de FileZilla verás la cola de transferencias.
- Espera a que terminen **todas** (sobre todo la carpeta **Content** y **Scripts**, que tienen muchos archivos).

---

## Qué debe quedar en /site1 en el servidor

Después de subir, dentro de **/site1** debe haber:

| Carpeta/archivo | Para qué |
|-----------------|----------|
| **bin**         | DLLs de la aplicación |
| **Content**     | **CSS** (bootstrap, site.css, layout-styles.css, etc.) – si falta, no hay diseño |
| **Scripts**     | JavaScript (jQuery, Bootstrap, etc.) |
| **Img**         | Imágenes (logo, galería, hero) |
| **Views**       | Vistas .cshtml |
| **Web.config**   | Configuración y cadena de conexión |
| **Global.asax** | Inicio de la aplicación |
| **favicon.ico** | Icono del sitio |

Si **Content** está vacía o incompleta, el CSS no cargará y se verá “raro”.

---

## Si ya habías subido antes

- Puedes volver a arrastrar todo y elegir **Sí a todos** al reemplazar. Así te aseguras de que **Content**, **Scripts** e **Img** estén completos.
- No borres la carpeta del servidor antes: arrastrar y reemplazar es suficiente.

---

## Después de subir

1. Prueba la URL: **http://pradoinn-001-site1.stempurl.com**
2. Si sigue sin estilos, en el navegador: **F12** → pestaña **Red/Network** → recarga la página y mira si algún archivo `.css` sale en rojo (error 404). Esa ruta es la que falta o está mal en el servidor.
