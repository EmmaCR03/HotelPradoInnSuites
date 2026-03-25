# ✅ SOLUCIÓN RÁPIDA - Acceso desde Red Local

## ⚠️ IMPORTANTE: Estás ejecutando el proyecto correcto?

Tu proyecto principal es **HotelPrado.UI** (puerto 44380), NO HotelPrado.Servicio (puerto 44336).

## 🚀 Pasos para Acceso desde Red Local (SIN ROMPER NADA)

### Paso 1: Ejecutar el Proyecto Correcto
1. En Visual Studio, asegúrate de que **HotelPrado.UI** esté configurado como proyecto de inicio
2. Clic derecho en **HotelPrado.UI** → **Set as StartUp Project**
3. Presiona **F5** para ejecutar

### Paso 2: Modificar applicationhost.config (SIN TOCAR EL .csproj)

**NO modifiques el .csproj**, solo modifica el archivo `applicationhost.config`:

1. Busca el archivo en:
   - `.vs\config\applicationhost.config` (dentro de tu proyecto)
   - O ejecuta este comando para encontrarlo:
     ```
     dir /s applicationhost.config
     ```

2. Abre el archivo y busca la sección de tu sitio (busca "HotelPrado.UI" o el puerto 44380)

3. Cambia de:
   ```xml
   <binding protocol="https" bindingInformation="*:44380:localhost" />
   ```
   
   A:
   ```xml
   <binding protocol="https" bindingInformation="*:44380:*" />
   ```

4. Guarda el archivo

### Paso 3: Configurar Firewall (Una sola vez)

Abre PowerShell **como Administrador** y ejecuta:
```powershell
New-NetFirewallRule -DisplayName "IIS Express - Hotel Prado" -Direction Inbound -LocalPort 44380 -Protocol TCP -Action Allow
```

### Paso 4: Obtener tu IP

Ejecuta en CMD:
```
ipconfig
```

Busca tu IP en "Dirección IPv4" (probablemente 192.168.0.2)

### Paso 5: Acceder desde Otros Dispositivos

1. **Desde tu PC**: `https://localhost:44380`
2. **Desde otros dispositivos en la red**: `https://192.168.0.2:44380`

⚠️ **Nota**: Puede aparecer advertencia de certificado SSL (es normal). Haz clic en "Avanzado" → "Continuar".

## 🔄 Si Algo Sale Mal

1. **Cierra Visual Studio completamente**
2. **Elimina la carpeta `.vs`** dentro de tu proyecto (esto regenerará applicationhost.config)
3. **Vuelve a abrir Visual Studio**
4. **Ejecuta el proyecto normalmente**

## ✅ Verificación

- ✅ El proyecto debe seguir funcionando en `localhost:44380`
- ✅ Otros dispositivos pueden acceder con `https://192.168.0.2:44380`
- ✅ NO modifiques el .csproj (ya está revertido a localhost)


