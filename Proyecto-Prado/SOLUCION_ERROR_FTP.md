# 🔧 Solución: Error 530 User cannot log in (FTP)

## Problema
El servidor FTP rechaza las credenciales. Esto puede ser por:
- Contraseña incorrecta
- Cuenta FTP no activa
- Usuario incorrecto

---

## ✅ Solución 1: Cambiar/Verificar Contraseña FTP en SmarterASP.NET

1. **Entra al panel de SmarterASP.NET** (donde viste el Server Overview)
2. **Busca la sección "FTP Accounts"** o **"FTP Access"** o **"FTP Settings"**
   - Puede estar en el menú lateral o en la página principal
3. **Busca la cuenta FTP** `pradoinn-001`
4. **Cambia la contraseña**:
   - Haz clic en "Change Password" o "Reset Password"
   - Genera una nueva contraseña o pon una que recuerdes
   - **Anota la nueva contraseña**
5. **Vuelve a FileZilla** y prueba con la nueva contraseña

---

## ✅ Solución 2: Usar File Manager del Panel (MÁS FÁCIL)

Si no encuentras cómo cambiar la contraseña FTP o prefieres no usar FTP:

1. **Entra al panel de SmarterASP.NET**
2. **Busca "File Manager"** o **"Files"** en el menú
3. **Navega a la carpeta de tu sitio**:
   - Busca la carpeta de **site1** o donde esté `pradoinn-001-site1.stempurl.com`
   - Puede llamarse `site1`, `httpdocs`, `wwwroot`, o similar
4. **Sube los archivos**:
   - Busca el botón **"Upload"** o **"Subir"**
   - Selecciona **todos los archivos y carpetas** de `C:\Users\emmag\OneDrive\Documentos\PradoPubli`
   - O arrastra y suelta si el File Manager lo permite
   - Sube: `bin`, `Content`, `Scripts`, `Views`, `Web.config`, `Global.asax`, `Img`, etc.

**Ventaja**: No necesitas credenciales FTP, solo tu usuario y contraseña del panel.

---

## ✅ Solución 3: Verificar Usuario FTP

A veces el usuario FTP puede ser diferente. Prueba estas variaciones:

- `pradoinn-001` (el que estás usando)
- `pradoinn-001@win8232.site4now.net`
- `pradoinn-001@site4now.net`
- Solo el número de cuenta si te lo dieron diferente

---

## 📋 Pasos Recomendados

1. **Primero intenta**: Usar el **File Manager** del panel (Solución 2) - es más fácil y no requiere FTP
2. **Si prefieres FTP**: Cambia la contraseña FTP desde el panel (Solución 1)
3. **Después de subir**: Edita `Web.config` en el servidor para poner la cadena de conexión correcta

---

## 🔍 Dónde encontrar FTP Settings en SmarterASP.NET

El panel de SmarterASP.NET puede tener diferentes nombres según la versión. Busca en:
- Menú lateral: **"FTP"**, **"FTP Accounts"**, **"FTP Access"**
- O en la página principal busca un botón o enlace relacionado con FTP
- Si no lo encuentras, usa el **File Manager** directamente

---

## ⚠️ Nota Importante

Después de subir los archivos (por File Manager o FTP), **NO OLVIDES**:
1. Editar `Web.config` en el servidor para actualizar la cadena de conexión a la base de datos
2. Verificar que la carpeta `bin` se subió completa con todas las DLLs
