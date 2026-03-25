# 🌐 Guía: Acceder al Proyecto desde la Red Local

## 📋 Objetivo
Permitir que otros dispositivos en tu red local puedan acceder a la aplicación web del hotel.

## 🔍 Paso 1: Obtener tu IP Local

Ya tienes dos direcciones IP:
- **192.168.56.1** (probablemente red virtual)
- **192.168.0.2** (red local principal - **USA ESTA**)

## 🔧 Paso 2: Configurar IIS Express para Acceso de Red

### Opción A: Modificar desde Visual Studio (Recomendado)

1. **Clic derecho en el proyecto `HotelPrado.UI`** → **Properties**
2. Ve a la pestaña **Web**
3. En **Servers**, cambia:
   - **Project Url**: de `https://localhost:44380/` a `https://192.168.0.2:44380/`
   - O simplemente cambia `localhost` por `192.168.0.2`
4. Guarda los cambios

### Opción B: Modificar el archivo .csproj

Edita `HotelPrado.UI/HotelPrado.UI.csproj` y busca la línea:
```xml
<IISUrl>https://localhost:44380/</IISUrl>
```

Cámbiala por:
```xml
<IISUrl>https://192.168.0.2:44380/</IISUrl>
```

### Opción C: Modificar applicationhost.config

1. Busca el archivo `applicationhost.config` en:
   - `.vs\config\applicationhost.config` (dentro de tu proyecto)
   - O en: `C:\Users\[TuUsuario]\Documents\IISExpress\config\applicationhost.config`

2. Busca la sección `<site>` que corresponde a tu proyecto y modifica:
```xml
<bindings>
    <binding protocol="http" bindingInformation="*:44380:localhost" />
    <binding protocol="https" bindingInformation="*:44380:localhost" />
</bindings>
```

Por:
```xml
<bindings>
    <binding protocol="http" bindingInformation="*:44380:*" />
    <binding protocol="https" bindingInformation="*:44380:*" />
</bindings>
```

El `*` permite conexiones desde cualquier IP.

## 🔥 Paso 3: Configurar el Firewall de Windows

### Método 1: Desde la Interfaz Gráfica

1. Abre **Windows Defender Firewall** (busca "Firewall" en el menú inicio)
2. Clic en **Configuración avanzada**
3. Clic en **Reglas de entrada** → **Nueva regla**
4. Selecciona **Puerto** → **Siguiente**
5. Selecciona **TCP** y escribe el puerto: **44380**
6. Selecciona **Permitir la conexión** → **Siguiente**
7. Marca todas las casillas (Dominio, Privada, Pública) → **Siguiente**
8. Nombre: "IIS Express - Hotel Prado" → **Finalizar**

### Método 2: Desde PowerShell (Como Administrador)

```powershell
New-NetFirewallRule -DisplayName "IIS Express - Hotel Prado" -Direction Inbound -LocalPort 44380 -Protocol TCP -Action Allow
```

## ✅ Paso 4: Probar el Acceso

1. **Ejecuta el proyecto** desde Visual Studio (F5)
2. **Desde tu computadora**, accede a: `https://192.168.0.2:44380`
3. **Desde otro dispositivo en la misma red**:
   - Abre un navegador
   - Ve a: `https://192.168.0.2:44380`
   - ⚠️ **Nota**: Puede aparecer una advertencia de certificado SSL (es normal en desarrollo). Haz clic en "Avanzado" → "Continuar al sitio"

## 📱 Paso 5: Acceder desde Dispositivos Móviles

### Android/iPhone:
1. Asegúrate de estar en la misma red WiFi
2. Abre el navegador
3. Ve a: `https://192.168.0.2:44380`
4. Acepta la advertencia de certificado si aparece

## ⚠️ Notas Importantes

1. **Certificado SSL**: IIS Express usa un certificado autofirmado. Los navegadores mostrarán una advertencia. Es normal en desarrollo.

2. **IP Dinámica**: Si tu IP cambia (DHCP), tendrás que actualizar la configuración.

3. **Seguridad**: Esto solo funciona en tu red local. No expongas el puerto a Internet sin protección adecuada.

4. **Puerto HTTP**: Si también quieres acceso HTTP (sin SSL), agrega el puerto 44380 para HTTP en el firewall.

## 🔄 Si la IP Cambia

Si tu IP cambia, simplemente:
1. Ejecuta: `ipconfig` en CMD
2. Busca tu nueva IP en "Dirección IPv4"
3. Actualiza la configuración en Visual Studio o el archivo .csproj

## 🚀 Alternativa: Usar ngrok (Para Acceso Externo)

Si necesitas acceso desde Internet (no solo red local), puedes usar **ngrok**:

```bash
ngrok http 44380
```

Esto te dará una URL pública temporal como: `https://abc123.ngrok.io`

---

**¿Problemas?**
- Verifica que el firewall permita el puerto
- Asegúrate de estar en la misma red WiFi
- Prueba primero desde tu propia máquina con la IP
- Revisa que IIS Express esté corriendo


