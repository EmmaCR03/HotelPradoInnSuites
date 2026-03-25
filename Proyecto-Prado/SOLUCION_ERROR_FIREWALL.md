# 🔧 Solución: Error "Acceso Denegado" en Firewall

## ❌ Problema

El error `Acceso denegado` significa que PowerShell **NO está ejecutándose como Administrador**.

## ✅ Solución: Ejecutar PowerShell como Administrador

### Método 1: Desde el Menú Inicio (Más Fácil)

1. **Presiona la tecla Windows** (o haz clic en el menú inicio)
2. **Escribe:** `PowerShell`
3. **Clic derecho** en "Windows PowerShell"
4. **Selecciona:** "Ejecutar como administrador"
5. **Confirma** cuando Windows pregunte (clic en "Sí")

### Método 2: Desde el Explorador de Archivos

1. **Navega a:** `C:\Windows\System32\WindowsPowerShell\v1.0\`
2. **Busca:** `powershell.exe`
3. **Clic derecho** → "Ejecutar como administrador"

### Método 3: Usar el Script que Creé

1. **Clic derecho** en `Configurar_Acceso_Telefono.ps1`
2. **Selecciona:** "Ejecutar con PowerShell"
3. Si aparece un mensaje de seguridad, haz clic en "Sí" o "Ejecutar"

---

## ✅ Verificar que es Administrador

Cuando PowerShell se abre como Administrador, verás:
- **"Administrador"** en la barra de título
- O la ruta empieza con algo como: `PS C:\WINDOWS\system32>`

---

## 🔄 Ahora Ejecuta el Comando

Una vez que PowerShell esté abierto **como Administrador**, ejecuta:

```powershell
New-NetFirewallRule -DisplayName "IIS Express - Hotel Prado" -Direction Inbound -LocalPort 44380 -Protocol TCP -Action Allow
```

Deberías ver un mensaje de éxito o simplemente volver al prompt sin errores.

---

## 🎯 Alternativa: Usar la Interfaz Gráfica (Sin PowerShell)

Si prefieres no usar PowerShell, puedes configurar el firewall manualmente:

### Pasos:

1. **Abre Windows Defender Firewall:**
   - Presiona `Windows + R`
   - Escribe: `wf.msc`
   - Presiona Enter

2. **Clic en "Reglas de entrada"** (lado izquierdo)

3. **Clic en "Nueva regla..."** (lado derecho)

4. **Selecciona "Puerto"** → Siguiente

5. **Selecciona "TCP"** y escribe: `44380` → Siguiente

6. **Selecciona "Permitir la conexión"** → Siguiente

7. **Marca todas las casillas** (Dominio, Privada, Pública) → Siguiente

8. **Nombre:** `IIS Express - Hotel Prado` → Finalizar

**¡Listo!** El firewall está configurado.

---

## ✅ Verificar que Funcionó

Para verificar que la regla se creó correctamente:

```powershell
Get-NetFirewallRule -DisplayName "IIS Express - Hotel Prado"
```

Deberías ver información sobre la regla creada.

---

## 📝 Resumen

**Problema:** PowerShell no tiene permisos de administrador

**Solución:**
1. Cierra PowerShell actual
2. Abre PowerShell **como Administrador** (clic derecho → "Ejecutar como administrador")
3. Ejecuta el comando de nuevo

O usa la interfaz gráfica de Windows Defender Firewall.

