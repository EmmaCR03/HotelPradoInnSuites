# 🌐 Opciones de Dominio con Negox

## 📋 Respuesta Rápida

**Negox NO vende dominios directamente**, pero puedes:
1. ✅ **Usar tu dominio existente** (sin costo adicional)
2. ✅ **Comprar dominio en otro proveedor** y conectarlo a Negox
3. ✅ **Comprar dominio y hosting juntos** en otro proveedor

---

## ✅ Opción 1: Usar Dominio Existente (Recomendado si ya tienes uno)

### Si ya tienes un dominio:
- ✅ **Puedes usarlo con Negox sin costo adicional**
- ✅ Solo necesitas configurar los DNS para que apunten a Negox
- ✅ Negox te dará los nameservers (servidores DNS) para configurar

### Pasos:
1. Contratas el hosting en Negox
2. Negox te da los **nameservers** (ej: `ns1.negox.com`, `ns2.negox.com`)
3. Vas a donde compraste tu dominio (GoDaddy, Namecheap, etc.)
4. Cambias los DNS para que apunten a los nameservers de Negox
5. ¡Listo! Tu dominio funcionará con Negox

**Costo:** $0 USD adicional (solo pagas el hosting)

---

## 🛒 Opción 2: Comprar Dominio Nuevo en Otro Proveedor

### Si NO tienes dominio y necesitas comprar uno:

Negox no vende dominios, pero puedes comprarlo en:

### Proveedores Recomendados:

#### 1. **Namecheap** ⭐ (Recomendado)
- **Precio:** ~$10-15 USD/año
- **Ventajas:**
  - Precios competitivos
  - Fácil de usar
  - Buena atención al cliente
  - Incluye protección de privacidad gratis
- **Sitio:** [www.namecheap.com](https://www.namecheap.com)

#### 2. **GoDaddy**
- **Precio:** ~$12-20 USD/año (puede variar)
- **Ventajas:**
  - Muy conocido
  - Fácil de usar
  - Promociones frecuentes
- **Desventajas:**
  - Puede ser más caro después del primer año
- **Sitio:** [www.godaddy.com](https://www.godaddy.com)

#### 3. **Google Domains** (ahora Squarespace Domains)
- **Precio:** ~$12 USD/año
- **Ventajas:**
  - Interfaz simple
  - Integración con Google
- **Sitio:** [domains.google](https://domains.google)

#### 4. **NEUBOX** (Mencionado por Negox)
- **Precio:** Variable según extensión
- **Ventajas:**
  - Proveedor relacionado con Negox
  - Precios competitivos
- **Sitio:** [www.neubox.com](https://www.neubox.com)

#### 5. **Cloudflare**
- **Precio:** ~$8-10 USD/año (solo costo de registro, sin margen)
- **Ventajas:**
  - Precio al costo (muy económico)
  - Protección DDoS incluida
  - DNS rápido
- **Sitio:** [www.cloudflare.com](https://www.cloudflare.com)

---

## 💰 Precios Típicos de Dominios

### Extensiones Comunes:

| Extensión | Precio Aproximado/Año | Ejemplo |
|-----------|----------------------|---------|
| **.com** | $10-15 USD | hotelprado.com |
| **.net** | $10-15 USD | hotelprado.net |
| **.org** | $10-15 USD | hotelprado.org |
| **.co** | $15-30 USD | hotelprado.co |
| **.com.mx** | $15-25 USD | hotelprado.com.mx |
| **.mx** | $20-40 USD | hotelprado.mx |

**Nota:** Los precios pueden variar según el proveedor y promociones.

---

## 🔧 Opción 3: Comprar Dominio y Hosting Juntos (Alternativa)

### Si prefieres tener todo en un solo lugar:

Puedes comprar dominio y hosting en el mismo proveedor, pero **NO en Negox** (porque no venden dominios).

### Proveedores que venden ambos:

1. **GoDaddy**
   - Dominio + Hosting Windows
   - Puede ser más caro que Negox para hosting

2. **HostGator**
   - Dominio + Hosting Windows
   - Precios competitivos

3. **Namecheap**
   - Dominio + Hosting (pero no especializado en ASP.NET)

**Recomendación:** ⚠️ Mejor comprar dominio en un proveedor y hosting en Negox (especializado en ASP.NET)

---

## 🎯 Recomendación para Hotel Prado

### Mejor Opción:

1. **Comprar dominio en Namecheap o Cloudflare**
   - Precio: ~$10-15 USD/año
   - Fácil de configurar

2. **Contratar hosting en Negox**
   - Plan Avanzado: $114 USD/año
   - Especializado en ASP.NET

3. **Configurar DNS**
   - Cambiar nameservers en Namecheap/Cloudflare
   - Apuntar a los nameservers de Negox
   - Tiempo de propagación: 24-48 horas

**Total:** ~$124-129 USD/año (dominio + hosting)

---

## 📝 Pasos Detallados para Configurar Dominio

### Paso 1: Comprar Dominio

1. Elige un proveedor (ej: Namecheap)
2. Busca tu dominio (ej: `hotelprado.com`)
3. Agrégalo al carrito
4. Completa la compra
5. Recibirás acceso al panel de control del dominio

### Paso 2: Contratar Hosting en Negox

1. Contrata el plan de hosting (ej: Plan Avanzado)
2. Negox te dará:
   - Nameservers (ej: `ns1.negox.com`, `ns2.negox.com`)
   - O instrucciones de configuración DNS

### Paso 3: Configurar DNS

**Opción A: Usar Nameservers (Más Fácil)**

1. Ve al panel de control de tu dominio (Namecheap, etc.)
2. Busca "Nameservers" o "DNS"
3. Cambia los nameservers a los que te dio Negox:
   ```
   ns1.negox.com
   ns2.negox.com
   ```
4. Guarda los cambios
5. Espera 24-48 horas para propagación

**Opción B: Configurar Registros DNS Manualmente**

1. En el panel de tu dominio, busca "DNS Records" o "Zona DNS"
2. Agrega estos registros (Negox te dará los valores exactos):
   - **A Record:** Apunta a la IP del servidor de Negox
   - **CNAME Record:** Para www (ej: www.hotelprado.com)
3. Guarda los cambios
4. Espera 24-48 horas para propagación

### Paso 4: Configurar en Negox

1. En el panel de Plesk de Negox
2. Ve a "Websites & Domains"
3. Agrega tu dominio
4. Configura el sitio para que apunte a tu carpeta de archivos

---

## ⚠️ Consideraciones Importantes

### 1. Tiempo de Propagación DNS
- Los cambios de DNS pueden tardar **24-48 horas** en propagarse
- Durante este tiempo, el sitio puede no estar accesible
- **Solución:** Planifica con anticipación

### 2. Protección de Privacidad WHOIS
- Algunos proveedores incluyen protección de privacidad gratis
- Oculta tu información personal en el registro WHOIS
- **Recomendación:** Actívala si está disponible

### 3. Renovación Automática
- Los dominios se renuevan anualmente
- **Recomendación:** Activa renovación automática para evitar perder el dominio

### 4. Transferencia de Dominio
- Si ya tienes dominio en otro proveedor, puedes transferirlo
- Puede haber costo adicional
- **Recomendación:** Si funciona bien donde está, déjalo ahí y solo cambia DNS

---

## 💡 Preguntas Frecuentes

### ¿Puedo comprar dominio directamente en Negox?
**No.** Negox no vende dominios, solo hosting.

### ¿Cuánto cuesta un dominio?
**~$10-15 USD/año** para extensiones comunes como .com, .net, .org

### ¿Puedo usar mi dominio existente con Negox?
**Sí.** Puedes usar tu dominio existente sin costo adicional, solo necesitas configurar DNS.

### ¿Necesito comprar dominio y hosting en el mismo lugar?
**No.** Puedes comprar dominio en un proveedor y hosting en otro (como Negox).

### ¿Qué pasa si ya tengo dominio en otro proveedor?
**No hay problema.** Solo cambia los DNS para que apunten a Negox. No necesitas transferir el dominio.

### ¿Cuánto tarda en funcionar después de configurar DNS?
**24-48 horas** típicamente, aunque puede ser más rápido.

---

## 📊 Resumen de Costos

### Opción Recomendada:
- **Dominio (.com):** $10-15 USD/año (Namecheap/Cloudflare)
- **Hosting Negox (Plan Avanzado):** $114 USD/año
- **Total:** ~$124-129 USD/año

### Si ya tienes dominio:
- **Hosting Negox (Plan Avanzado):** $114 USD/año
- **Total:** $114 USD/año

---

## 🎯 Pasos a Seguir

1. ✅ **Decide si necesitas comprar dominio nuevo**
   - Si ya tienes → Usa el existente
   - Si no tienes → Compra en Namecheap/Cloudflare

2. ✅ **Contrata hosting en Negox**
   - Plan Avanzado recomendado: $114/año

3. ✅ **Configura DNS**
   - Cambia nameservers o registros DNS
   - Espera 24-48 horas

4. ✅ **Configura dominio en Plesk**
   - Agrega dominio en el panel de Negox
   - Apunta a tu carpeta de archivos

5. ✅ **¡Listo!**
   - Tu sitio estará accesible en tu dominio

---

## 🔗 Enlaces Útiles

- **Namecheap:** [www.namecheap.com](https://www.namecheap.com)
- **Cloudflare:** [www.cloudflare.com](https://www.cloudflare.com)
- **GoDaddy:** [www.godaddy.com](https://www.godaddy.com)
- **Negox:** [www.negox.com](https://www.negox.com)

---

**En resumen:** Negox no vende dominios, pero puedes usar tu dominio existente o comprar uno en otro proveedor (Namecheap, Cloudflare, etc.) y conectarlo fácilmente a Negox cambiando los DNS.



