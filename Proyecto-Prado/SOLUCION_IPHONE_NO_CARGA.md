# 📱 Solución: iPhone No Carga la Aplicación

## 🔍 Diagnóstico Rápido

### Verificar 1: ¿Estás en la misma red WiFi?

1. **En tu iPhone:**
   - Ve a **Configuración** → **Wi-Fi**
   - Verifica el nombre de la red WiFi
   
2. **En tu PC:**
   - Verifica que estés conectado a la **misma red WiFi**

**✅ Deben ser EXACTAMENTE la misma red.**

---

### Verificar 2: ¿La IP es correcta?

Ejecuta en CMD:
```
ipconfig
```

Busca tu IP actual (probablemente `192.168.0.12` o similar).

**En tu iPhone, usa:** `https://TU_IP:44380`

---

### Verificar 3: ¿El proyecto está ejecutándose?

- ✅ Debe estar corriendo en Visual Studio (F5)
- ✅ Debe abrirse en tu navegador de PC sin problemas

---

## 🔧 Soluciones para iPhone

### Solución 1: Usar HTTP en lugar de HTTPS (Más Fácil)

Los iPhones a veces tienen problemas con certificados autofirmados. Prueba con HTTP:

1. **Modifica applicationhost.config** (si no lo hiciste):
   - Busca también la línea de HTTP:
   ```xml
   <binding protocol="http" bindingInformation="*:44380:localhost" />
   ```
   - Cámbiala por:
   ```xml
   <binding protocol="http" bindingInformation="*:44380:*" />
   ```

2. **En tu iPhone:**
   - Usa: `http://192.168.0.12:44380` (sin la 's' de https)

---

### Solución 2: Aceptar Certificado en iPhone

Si usas HTTPS:

1. **Abre Safari** (no Chrome) en iPhone
2. **Escribe:** `https://192.168.0.12:44380`
3. **Si aparece advertencia:**
   - Toca **"Avanzado"** o **"Advanced"**
   - Toca **"Continuar"** o **"Proceed"**

**Nota:** Chrome en iPhone a veces no permite aceptar certificados autofirmados. Usa Safari.

---

### Solución 3: Verificar Puerto en Visual Studio

Cuando ejecutas el proyecto (F5), verifica en la barra de estado de Visual Studio o en la ventana del navegador:

- ¿Qué URL se abre? (ej: `https://localhost:44380` o `https://localhost:56582`)
- **Usa ese mismo puerto** en tu iPhone

---

### Solución 4: Probar desde PC Primero

Antes de probar en iPhone, prueba desde tu PC:

1. **En tu PC**, abre el navegador
2. **Escribe:** `https://192.168.0.12:44380` (o `http://...`)
3. **¿Funciona?** Si funciona en PC pero no en iPhone, es problema del iPhone/certificado

---

### Solución 5: Verificar Firewall

Aunque ya configuraste el firewall, verifica:

1. **Abre Windows Defender Firewall**
2. **Ve a "Reglas de entrada"**
3. **Busca:** "IIS Express - Hotel Prado"
4. **Verifica que esté "Habilitada"**

---

## 🎯 Pasos Recomendados (En Orden)

### Paso 1: Verificar IP Actual
```cmd
ipconfig
```
Anota tu IP actual.

### Paso 2: Probar desde PC
En tu navegador de PC: `https://TU_IP:44380`
- ✅ Si funciona → Continúa al paso 3
- ❌ Si no funciona → Revisa applicationhost.config

### Paso 3: Probar HTTP en iPhone
En Safari del iPhone: `http://TU_IP:44380` (sin 's')
- ✅ Si funciona → ¡Listo!
- ❌ Si no funciona → Continúa

### Paso 4: Probar HTTPS en Safari
En Safari del iPhone: `https://TU_IP:44380`
- Acepta el certificado si aparece advertencia
- ✅ Si funciona → ¡Listo!

---

## ⚠️ Problemas Comunes

### "No se puede conectar al servidor"
- ✅ Verifica que estés en la misma red WiFi
- ✅ Verifica que el proyecto esté ejecutándose
- ✅ Verifica la IP (puede haber cambiado)

### "Certificado no válido"
- ✅ Usa Safari (no Chrome)
- ✅ Acepta el certificado manualmente
- ✅ O usa HTTP en lugar de HTTPS

### "Página en blanco"
- ✅ Espera unos segundos (puede tardar en cargar)
- ✅ Verifica que el proyecto esté completamente iniciado
- ✅ Prueba primero desde PC

---

## 🔄 Si Nada Funciona

### Opción A: Usar Puerto HTTP Diferente

1. En Visual Studio, cambia el puerto HTTP a uno más común (ej: 8080)
2. Configura el firewall para ese puerto
3. Usa ese puerto en iPhone

### Opción B: Verificar Logs

1. En Visual Studio, revisa la **Ventana de Salida**
2. Busca errores cuando intentas acceder desde iPhone
3. Los errores te dirán qué está fallando

---

## ✅ Checklist Final

- [ ] Misma red WiFi (PC e iPhone)
- [ ] Proyecto ejecutándose (F5 en Visual Studio)
- [ ] IP correcta (verificar con ipconfig)
- [ ] Firewall configurado
- [ ] applicationhost.config modificado (localhost → *)
- [ ] Probado desde PC primero
- [ ] Usando Safari en iPhone (no Chrome)
- [ ] Probado HTTP y HTTPS

---

## 💡 Tip Rápido

**La forma más fácil:**
1. Usa **HTTP** en lugar de HTTPS: `http://192.168.0.12:44380`
2. Usa **Safari** en iPhone (no Chrome)
3. Asegúrate de estar en la **misma red WiFi**

¡Prueba esto primero!


