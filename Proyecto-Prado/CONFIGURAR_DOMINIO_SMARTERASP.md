# 🌐 Configurar tu Dominio Propio en SmarterASP.NET

## ✅ Respuesta Rápida

**SÍ, puedes usar tu dominio propio** (`pradoinn.com` o el que tengas) con SmarterASP.NET.

Todos los proveedores de hosting permiten usar dominios propios. Es una función estándar.

---

## 🎯 Opciones para Usar tu Dominio

### Opción 1: Usar Dominio que Ya Tienes ⭐ RECOMENDADO

**Si ya tienes el dominio registrado:**
- ✅ Puedes usarlo directamente
- ✅ Solo necesitas cambiar los DNS
- ✅ No necesitas comprar dominio nuevo

**Pasos:**
1. Contrata SmarterASP.NET
2. En el panel de control, agrega tu dominio
3. SmarterASP.NET te dará los DNS a configurar
4. Ve a donde compraste tu dominio y cambia los DNS
5. Espera 24-48 horas para que se propague
6. ¡Listo!

---

### Opción 2: Comprar Dominio Nuevo

**Si no tienes dominio:**
- SmarterASP.NET puede ayudarte a comprarlo
- O puedes comprarlo en otro proveedor (Namecheap, Cloudflare, GoDaddy)
- Luego lo configuras igual

---

## 📋 Pasos Detallados para Configurar

### Paso 1: Contratar SmarterASP.NET

1. Ve a: https://www.smarterasp.net/
2. Elige el plan Professional ($7/mes)
3. Durante el registro, te preguntarán sobre el dominio

### Paso 2: Agregar tu Dominio en SmarterASP.NET

1. Inicia sesión en el panel de control de SmarterASP.NET
2. Busca la sección "Domains" o "Add Domain"
3. Ingresa tu dominio: `pradoinn.com` (o el que tengas)
4. SmarterASP.NET te dará información de DNS

### Paso 3: Configurar DNS en tu Proveedor de Dominio

**SmarterASP.NET te dará algo como:**
```
DNS 1: ns1.smarterasp.net
DNS 2: ns2.smarterasp.net
```

**O te dará registros A/CNAME:**
```
Tipo A: @ → IP del servidor (ej: 123.45.67.89)
Tipo CNAME: www → tu-dominio.smarterasp.net
```

**Pasos:**
1. Ve a donde compraste tu dominio (GoDaddy, Namecheap, etc.)
2. Busca la sección "DNS" o "Name Servers"
3. Cambia los DNS a los que te dio SmarterASP.NET
4. O agrega los registros A/CNAME que te indicaron

### Paso 4: Esperar Propagación

- **Tiempo:** 24-48 horas normalmente
- **Puede ser más rápido:** 2-4 horas a veces
- **Verifica:** Usa herramientas como https://www.whatsmydns.net/

### Paso 5: Verificar que Funciona

1. Espera 24-48 horas
2. Abre tu navegador
3. Ve a `http://pradoinn.com` (o tu dominio)
4. Deberías ver tu aplicación

---

## 🔧 Configuración DNS Específica

### Si SmarterASP.NET Usa Name Servers (Más Fácil):

**En tu proveedor de dominio:**
1. Ve a configuración de DNS
2. Cambia "Name Servers" a:
   ```
   ns1.smarterasp.net
   ns2.smarterasp.net
   ```
3. Guarda cambios
4. Espera propagación

**Ventaja:** SmarterASP.NET maneja todo el DNS por ti

---

### Si SmarterASP.NET Usa Registros A/CNAME:

**En tu proveedor de dominio, agrega:**

**Registro A (para dominio principal):**
```
Tipo: A
Nombre: @ (o pradoinn.com)
Valor: [IP que te da SmarterASP.NET]
TTL: 3600 (o el que recomienden)
```

**Registro CNAME (para www):**
```
Tipo: CNAME
Nombre: www
Valor: tu-dominio.smarterasp.net (o lo que te indiquen)
TTL: 3600
```

---

## ⚠️ Cosas Importantes

### 1. **No Pierdes tu Dominio**
- Solo cambias dónde apunta
- Tu dominio sigue siendo tuyo
- Puedes cambiarlo de vuelta cuando quieras

### 2. **Puedes Mantener Email en Otro Lugar**
- Si tienes email configurado en otro lugar
- Puedes mantenerlo ahí
- Solo cambias los DNS para el sitio web

### 3. **Subdominios**
- Puedes crear subdominios también
- Ejemplo: `app.pradoinn.com`, `test.pradoinn.com`
- Se configuran igual

### 4. **SSL/HTTPS**
- SmarterASP.NET incluye SSL gratis (Let's Encrypt)
- Se configura automáticamente después de DNS
- Tu sitio tendrá `https://pradoinn.com`

---

## 🎯 Estrategia Recomendada

### Opción A: Migración Gradual ⭐ RECOMENDADO

1. **Fase 1:** Despliega en subdominio temporal
   - Ejemplo: `test.pradoinn.com` o `nuevo.pradoinn.com`
   - Prueba todo durante 1-2 semanas

2. **Fase 2:** Si todo funciona bien
   - Cambia DNS del dominio principal
   - Apunta `pradoinn.com` a SmarterASP.NET

3. **Ventaja:** No afectas tu sitio actual hasta estar seguro

---

### Opción B: Cambio Directo

1. Cambia DNS directamente
2. Espera propagación
3. Tu sitio estará en SmarterASP.NET

**Ventaja:** Más rápido  
**Desventaja:** Si algo sale mal, afecta sitio actual

---

## 📞 Preguntas para SmarterASP.NET

Antes de configurar, pregunta:

1. **¿Puedo usar mi dominio propio?** (debe ser SÍ)
2. **¿Qué DNS debo configurar?**
3. **¿Ofrecen ayuda para configurar DNS?**
4. **¿Cuánto tarda la propagación?**
5. **¿Puedo usar subdominios también?**
6. **¿SSL se configura automáticamente?**
7. **¿Puedo mantener email en otro lugar?**

---

## 🔍 Verificar Configuración

### Herramientas Útiles:

1. **WhatsMyDNS:** https://www.whatsmydns.net/
   - Verifica si DNS se propagó correctamente

2. **DNS Checker:** https://dnschecker.org/
   - Verifica propagación en diferentes lugares

3. **SSL Checker:** https://www.sslshopper.com/ssl-checker.html
   - Verifica certificado SSL después de configurar

---

## ⚠️ Problemas Comunes

### Problema 1: DNS No Se Propaga
**Solución:**
- Espera más tiempo (puede tardar hasta 48 horas)
- Verifica que configuraste DNS correctamente
- Contacta soporte de SmarterASP.NET

### Problema 2: Sitio No Carga
**Solución:**
- Verifica que DNS se propagó (usa herramientas arriba)
- Verifica que configuraste dominio en panel de SmarterASP.NET
- Verifica que aplicación está desplegada correctamente

### Problema 3: SSL No Funciona
**Solución:**
- Espera después de configurar DNS (SSL se configura después)
- Verifica en panel de SmarterASP.NET si SSL está activo
- Contacta soporte si no se activa automáticamente

---

## ✅ Checklist de Configuración

### Antes de Configurar:
- [ ] Tienes acceso a donde compraste tu dominio
- [ ] Sabes cómo cambiar DNS en tu proveedor
- [ ] Tienes cuenta en SmarterASP.NET
- [ ] Tienes aplicación lista para desplegar

### Durante Configuración:
- [ ] Agregaste dominio en panel de SmarterASP.NET
- [ ] Obtuviste información de DNS
- [ ] Configuraste DNS en tu proveedor de dominio
- [ ] Guardaste cambios

### Después de Configurar:
- [ ] Esperaste 24-48 horas
- [ ] Verificaste propagación DNS
- [ ] Probaste acceder a tu dominio
- [ ] Verificaste que SSL funciona (https://)

---

## 🎯 Resumen

### ✅ SÍ, Puedes Usar tu Dominio Propio

**Pasos Básicos:**
1. Contrata SmarterASP.NET
2. Agrega tu dominio en su panel
3. Configura DNS en tu proveedor de dominio
4. Espera propagación (24-48 horas)
5. ¡Listo!

**No necesitas:**
- ❌ Comprar dominio nuevo
- ❌ Transferir dominio
- ❌ Cancelar dominio actual

**Solo necesitas:**
- ✅ Cambiar DNS para que apunte a SmarterASP.NET
- ✅ Tu dominio sigue siendo tuyo
- ✅ Puedes cambiarlo de vuelta cuando quieras

---

## 💡 Consejos

1. **Haz migración gradual** - Usa subdominio primero para probar
2. **Mantén hosting anterior** - No lo canceles hasta estar seguro
3. **Haz backup** - Antes de cambiar cualquier cosa
4. **Documenta configuración** - Por si necesitas volver atrás
5. **Contacta soporte** - Si tienes dudas, pregúntales

---

## 🆘 Si Necesitas Ayuda

**SmarterASP.NET tiene soporte:**
- Chat en vivo
- Email
- Teléfono
- Pueden ayudarte a configurar DNS

**No tengas miedo de preguntar.** Es su trabajo ayudarte.

---

**Conclusión:** SÍ, puedes usar tu dominio propio sin problemas. Es una función estándar de todos los proveedores de hosting. 🌐
