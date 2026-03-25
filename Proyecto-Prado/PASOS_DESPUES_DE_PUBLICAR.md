# Pasos después de publicar – Hotel Prado (SmarterASP.NET)

La publicación ya está en: **C:\Users\emmag\OneDrive\Documentos\PradoPubli**. Sigue estos pasos para dejarla en línea.

---

## 1. Subir los archivos al hosting

1. Entra al **panel de SmarterASP.NET** (cuenta pradoinn-001).
2. Abre **File Manager** (administrador de archivos) o conecta por **FTP** con los datos que te dio SmarterASP.
3. Ve a la carpeta del sitio (por ejemplo `httpdocs` o la que te indiquen para tu dominio/subdominio).
4. **Sube todo el contenido** de `C:\Users\emmag\OneDrive\Documentos\PradoPubli`:
   - Incluye carpetas: `bin`, `Content`, `Scripts`, `Views`, `Img`, etc.
   - Incluye archivos raíz: `Web.config`, `Global.asax`, etc.
   - La estructura debe quedar igual que en PradoPubli (no subas la carpeta “PradoPubli”, sino lo que hay dentro).

---

## 2. Configurar la base de datos en el servidor

1. En el panel de SmarterASP.NET, entra a **SQL Server** / **Databases**.
2. Si aún no creaste la base de datos:
   - Crea una nueva base (nombre tipo `db_xxxx_pradoinn`).
   - Anota: **servidor**, **nombre de la base de datos**, **usuario** y **contraseña**.
3. Restaura o migra tu base de datos actual (HotelPrado) a esa base en SmarterASP (backup/restore o scripts SQL, según lo que uses).

---

## 3. Poner la cadena de conexión en el servidor

En el **File Manager** de SmarterASP ve a la carpeta donde está el sitio: **www** o **site1** (la que uses para los archivos publicados). Ahí edita **Web.config** (botón "Edit") y ajusta la cadena de conexión **Contexto**.

Reemplaza la sección `<connectionStrings>` por esta, sustituyendo solo **SERVIDOR**, **BASE_DE_DATOS** y **USUARIO** por los valores que te dio SmarterASP en **DATABASES** (la contraseña ya está puesta):

```xml
<connectionStrings>
  <add name="Contexto"
       connectionString="Data Source=SERVIDOR;Initial Catalog=BASE_DE_DATOS;User ID=USUARIO;Password=Prado2026!;Connection Timeout=30;MultipleActiveResultSets=True;Pooling=true;Min Pool Size=5;Max Pool Size=100"
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

- **SERVIDOR**: host de SQL Server (ej. `sql123.smarterasp.net`).
- **BASE_DE_DATOS**: nombre de la base (ej. `db_xxxx_pradoinn`).
- **USUARIO**: usuario de la base de datos.
- **Contraseña**: `Prado2026!` (ya incluida arriba).

Guarda el archivo en el servidor con "Save" en el editor.

*Nota:* La contraseña solo debe estar en el **Web.config del servidor**. No subas ese archivo con la contraseña real a tu repositorio; en tu PC mantén el Web.config con la conexión local.

---

## 4. Probar el sitio

1. Abre la URL que te asigna SmarterASP (ej. `tu-sitio.smarterasp.net` o la IP que te den).
2. Comprueba que carga la página principal, el login y que no salgan errores de conexión a la base de datos.

---

## 5. (Opcional) Usar tu dominio (pradoinn.com)

Cuando todo funcione bien con la URL de SmarterASP:

1. En el panel de SmarterASP, en **Domains**, agrega **pradoinn.com**.
2. En el lugar donde gestionas el dominio (cPanel, GoDaddy, etc.), cambia los DNS a los que te indique SmarterASP (o los registros A/CNAME que te den).
3. Espera la propagación (24–48 h suele ser suficiente) y verifica con la URL de tu dominio.

---

## Resumen rápido

| Paso | Acción |
|------|--------|
| 1 | Subir todo el contenido de **PradoPubli** al directorio del sitio en SmarterASP (File Manager o FTP). |
| 2 | Tener la base de datos creada en SmarterASP y con los datos de Hotel Prado restaurados/migrados. |
| 3 | Editar **Web.config** en el servidor y poner la cadena de conexión **Contexto** con servidor, base, usuario y contraseña de SmarterASP. |
| 4 | Probar la URL del hosting. |
| 5 | (Opcional) Apuntar **pradoinn.com** con los DNS que te indique SmarterASP. |

Si en algún paso te sale un error concreto (mensaje en pantalla o en el panel), copia el mensaje y lo revisamos.
