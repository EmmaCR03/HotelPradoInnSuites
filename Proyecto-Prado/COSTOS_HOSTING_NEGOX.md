# 💰 Costos de Hosting en Negox - Hotel Prado

## 📊 Planes y Precios Anuales (Información Oficial)

Basado en la información oficial de [Negox](https://www.negox.com/es/hosting/), aquí están los planes detallados:

---

## 💵 Planes Disponibles

### 🟢 Plan Estándar
**Ideal para:** Sitios pequeños con tráfico bajo a medio

**Precios Anuales:**
- **1 año:** $4.50 USD/mes = **$54.00 USD/año**
- **2 años:** $4.00 USD/mes = **$96.00 USD/2 años** (ahorro de $12)
- **3 años:** $3.50 USD/mes = **$126.00 USD/3 años** (ahorro de $36)
- **Mensual:** $5.00 USD/mes

**Características Reales (según Negox):**
- ✅ **Espacio en disco:** 5 GB
- ✅ **Transferencia mensual:** 50 GB
- ✅ **Sitios web:** 1 sitio
- ✅ **Base de datos SQL Server:** 1 base de datos
- ✅ **Base de datos MySQL/MariaDB:** 1 base de datos
- ✅ **Cuentas de correo:** 10 cuentas
- ✅ **Cuentas FTP:** 5 cuentas
- ✅ **SSL:** Let's Encrypt gratis
- ✅ **Soporte:** 24/7
- ✅ **Garantía:** 99.9% tiempo en línea

**Recomendación:** ⚠️ Puede ser suficiente si es un hotel pequeño y optimizas imágenes

---

### 🟡 Plan Avanzado
**Ideal para:** Sitios medianos con tráfico moderado

**Precios Anuales:**
- **1 año:** $9.50 USD/mes = **$114.00 USD/año**
- **2 años:** $9.00 USD/mes = **$216.00 USD/2 años** (ahorro de $12)
- **3 años:** $8.50 USD/mes = **$306.00 USD/3 años** (ahorro de $36)
- **Mensual:** $10.00 USD/mes

**Características Reales (según Negox):**
- ✅ **Espacio en disco:** 10 GB
- ✅ **Transferencia mensual:** 100 GB
- ✅ **Sitios web:** 2 sitios
- ✅ **Base de datos SQL Server:** 2 bases de datos
- ✅ **Base de datos MySQL/MariaDB:** 2 bases de datos
- ✅ **Cuentas de correo:** 20 cuentas
- ✅ **Cuentas FTP:** 10 cuentas
- ✅ **SSL:** Let's Encrypt gratis
- ✅ **Soporte:** 24/7
- ✅ **Garantía:** 99.9% tiempo en línea

**Recomendación:** ✅ **RECOMENDADO para Hotel Prado** (tráfico moderado)

---

### 🔴 Plan Premium
**Ideal para:** Sitios grandes con alto tráfico

**Precios Anuales:**
- **1 año:** $19.50 USD/mes = **$234.00 USD/año**
- **2 años:** $19.00 USD/mes = **$456.00 USD/2 años** (ahorro de $12)
- **3 años:** $18.50 USD/mes = **$666.00 USD/3 años** (ahorro de $36)
- **Mensual:** $20.00 USD/mes

**Características Reales (según Negox):**
- ✅ **Espacio en disco:** 25 GB
- ✅ **Transferencia mensual:** 250 GB
- ✅ **Sitios web:** 5 sitios
- ✅ **Base de datos SQL Server:** 5 bases de datos
- ✅ **Base de datos MySQL/MariaDB:** 5 bases de datos
- ✅ **Cuentas de correo:** 50 cuentas
- ✅ **Cuentas FTP:** 25 cuentas
- ✅ **SSL:** Let's Encrypt gratis
- ✅ **Soporte:** 24/7
- ✅ **Garantía:** 99.9% tiempo en línea

**Recomendación:** ✅ Solo si esperas mucho tráfico o múltiples usuarios simultáneos

---

## 🏨 Análisis para Hotel Prado (Tráfico No Alto)

### Factores que Afectan el Costo:

1. **Tráfico Esperado** (BAJO-MEDIO según mencionaste)
   - Usuarios simultáneos: Pocos (personal del hotel + algunos clientes)
   - Reservas por día: Bajo a moderado
   - Visitas al sitio web: Bajo a moderado
   - **Conclusión:** No necesitas plan Premium

2. **Base de Datos SQL Server**
   - Tu proyecto necesita: **1 base de datos SQL Server**
   - Datos a almacenar:
     - Reservas (crece con el tiempo, pero lento)
     - Clientes (crece lento)
     - Habitaciones (fijo, pocas)
     - Facturas (crece con reservas)
     - Mantenimientos (crece lento)
     - Bitácora de eventos (puede crecer más rápido)
   - **Estimación:** 1-3 GB de base de datos debería ser suficiente inicialmente
   - **Conclusión:** Plan Estándar (1 BD) o Avanzado (2 BD) son suficientes

3. **Almacenamiento de Imágenes** ⚠️ IMPORTANTE
   - Tu proyecto tiene muchas imágenes:
     - Imágenes de habitaciones
     - Imágenes de departamentos
     - Hero banners
     - Imágenes del sitio
   - **Estimación:** 1-5 GB de imágenes (depende de calidad y cantidad)
   - **Conclusión:** Plan Estándar (5 GB) puede ser justo, Avanzado (10 GB) es más seguro

4. **Espacio Total Necesario**
   - Aplicación ASP.NET: ~200-500 MB
   - Base de datos: 1-3 GB
   - Imágenes: 1-5 GB
   - Logs y temporales: ~500 MB
   - **Total estimado:** 3-9 GB
   - **Conclusión:** Plan Estándar (5 GB) puede ser ajustado, Avanzado (10 GB) es más cómodo

5. **Transferencia Mensual**
   - Con tráfico bajo-medio: 10-30 GB/mes debería ser suficiente
   - **Conclusión:** Plan Estándar (50 GB) es suficiente

6. **Correo Electrónico**
   - ¿Necesitas cuentas de correo para el hotel?
   - Plan Estándar: 10 cuentas
   - Plan Avanzado: 20 cuentas
   - **Conclusión:** Ambos planes son suficientes

7. **Dominio**
   - Puedes usar tu dominio existente sin costo adicional
   - No necesitas comprar dominio nuevo

---

## 💡 Recomendación Específica para Hotel Prado (Tráfico No Alto)

### 🎯 RECOMENDACIÓN PRINCIPAL: Plan Avanzado

**Plan Recomendado:** 🟡 **Plan Avanzado**
- **Costo anual:** $114 USD/año ($9.50/mes)
- **Costo mensual:** $10.00 USD/mes (si pagas mes a mes)

**¿Por qué Plan Avanzado?**
1. ✅ **10 GB de espacio** - Suficiente para aplicación + base de datos + imágenes con margen
2. ✅ **100 GB transferencia** - Más que suficiente para tráfico bajo-medio
3. ✅ **2 bases de datos SQL Server** - Tienes 1 principal, puedes usar la 2da para pruebas/backup
4. ✅ **2 sitios web** - Puedes tener sitio principal + sitio de pruebas
5. ✅ **20 cuentas de correo** - Suficiente para personal del hotel
6. ✅ **Mejor relación precio/recursos** - No es mucho más caro que Estándar pero te da más margen

---

### 🟢 Alternativa: Plan Estándar (Si quieres ahorrar)

**Plan Alternativo:** 🟢 **Plan Estándar**
- **Costo anual:** $54 USD/año ($4.50/mes)
- **Ahorro:** $60 USD/año vs Plan Avanzado

**Ventajas:**
- ✅ Más económico
- ✅ Suficiente si optimizas bien las imágenes
- ✅ Puedes escalar después si necesitas más

**Desventajas:**
- ⚠️ **5 GB puede ser ajustado** si tienes muchas imágenes
- ⚠️ Solo 1 base de datos (no tienes margen para pruebas)
- ⚠️ Solo 1 sitio web

**Recomendación:** Si estás seguro de que:
- Optimizarás las imágenes (comprimir, usar formatos eficientes)
- No necesitas sitio de pruebas
- El tráfico será realmente bajo
- **→ Entonces Plan Estándar puede funcionar**

---

### 🔴 Plan Premium: NO RECOMENDADO (Para tu caso)

**Plan Premium:** 🔴 **$234 USD/año**
- **Razón para NO recomendarlo:** 
  - Tienes tráfico bajo-medio
  - 25 GB es excesivo para tus necesidades
  - 250 GB transferencia es innecesario
  - Cuesta el doble que Avanzado sin beneficio real para ti

**Solo considera Premium si:**
- En el futuro creces mucho
- Tienes múltiples hoteles
- Necesitas varios sitios web
- El tráfico aumenta significativamente

---

## 💰 Ahorro con Contratos Largos

### Comparación de Ahorro (Plan Avanzado):

| Duración | Precio Mensual | Precio Total | Ahorro vs 1 año |
|----------|----------------|--------------|-----------------|
| 1 año    | $9.50/mes      | $114.00      | -               |
| 2 años   | $9.00/mes      | $216.00      | $12.00          |
| 3 años   | $8.50/mes      | $306.00      | $36.00          |

**Recomendación:** Si estás seguro de usar Negox a largo plazo, el contrato de 3 años te ahorra dinero.

---

## 📋 Costos Adicionales a Considerar

### 1. Dominio (si no lo tienes)
- **Costo aproximado:** $10-15 USD/año
- **Importante:** Negox NO vende dominios directamente
- **Opciones:**
  - Usar dominio existente (sin costo adicional con Negox)
  - Comprar dominio en otro proveedor (Namecheap, Cloudflare, GoDaddy, etc.)
  - Luego configurar DNS para apuntar a Negox
- Ver documento `OPCIONES_DOMINIO_NEGOX.md` para más detalles

### 2. Certificado SSL
- **Let's Encrypt:** Gratis (incluido en muchos planes)
- **Certificado Premium:** $50-200 USD/año (opcional)

### 3. Backup Automático
- Verifica si está incluido en el plan
- Si no, puede costar $2-5 USD/mes adicionales

### 4. Soporte Técnico Premium
- Generalmente incluido en planes avanzados
- Soporte 24/7 puede tener costo adicional

### 5. Migración de Datos
- Si necesitas ayuda para migrar, puede haber costo
- Pregunta a Negox si ofrecen migración gratuita

---

## 💵 Resumen de Costos Anuales Estimados

### Opción Económica (Plan Estándar)
- **Hosting:** $54 USD/año
- **Dominio:** $12 USD/año (si no lo tienes)
- **Total:** ~$66 USD/año

### Opción Recomendada (Plan Avanzado)
- **Hosting:** $114 USD/año
- **Dominio:** $12 USD/año (si no lo tienes)
- **Total:** ~$126 USD/año

### Opción Premium (Plan Premium)
- **Hosting:** $234 USD/año
- **Dominio:** $12 USD/año (si no lo tienes)
- **Total:** ~$246 USD/año

---

## 🎯 Plan de Acción Recomendado

### Fase 1: Inicio (Primeros 3-6 meses)
1. **Contrata Plan Avanzado** ($9.50/mes)
2. **Prueba el sistema** con datos reales
3. **Monitorea el uso** de recursos (espacio, tráfico, base de datos)

### Fase 2: Evaluación (Después de 6 meses)
1. **Analiza el uso real:**
   - ¿Se está usando todo el espacio?
   - ¿El tráfico es el esperado?
   - ¿Hay problemas de rendimiento?

2. **Decide:**
   - Si todo va bien → Mantén Plan Avanzado
   - Si necesitas más recursos → Sube a Premium
   - Si usas menos de lo esperado → Considera bajar a Estándar

### Fase 3: Optimización (Después de 1 año)
1. Si estás satisfecho, **renueva por 2-3 años** para ahorrar
2. Si necesitas más, **escala al plan superior**

---

## ⚠️ Preguntas Importantes para Negox

Antes de contratar, pregunta a Negox:

1. **¿Cuánto espacio en disco incluye cada plan?**
   - Necesitas espacio para: aplicación, base de datos, imágenes

2. **¿Cuántas bases de datos SQL Server incluye?**
   - Tu proyecto necesita al menos 1 base de datos

3. **¿Cuál es el límite de transferencia mensual?**
   - Depende del tráfico esperado

4. **¿Incluyen backup automático?**
   - Muy importante para un sistema de hotel

5. **¿Ofrecen migración gratuita?**
   - Si ya tienes el sitio en otro hosting

6. **¿Cuántas cuentas de correo incluyen?**
   - Si necesitas correos para el hotel

7. **¿Hay límite de usuarios simultáneos?**
   - Importante para el sistema de reservas

8. **¿Ofrecen certificado SSL gratis?**
   - Let's Encrypt generalmente es gratis

---

## 🔄 Alternativas y Comparación

### Otras Opciones de Hosting Windows:

1. **Azure App Service (Microsoft)**
   - **Costo:** ~$13-55 USD/mes (depende del plan)
   - **Ventaja:** Escalable, muy confiable
   - **Desventaja:** Puede ser más costoso

2. **HostGator Windows**
   - **Costo:** ~$5-10 USD/mes
   - **Ventaja:** Económico
   - **Desventaja:** Menos especializado en ASP.NET

3. **GoDaddy Windows**
   - **Costo:** ~$5-15 USD/mes
   - **Ventaja:** Conocido, fácil de usar
   - **Desventaja:** Soporte variable

**Comparación con Negox:**
- ✅ Negox está **especializado en ASP.NET** (ventaja)
- ✅ Negox tiene **Plesk** (panel moderno)
- ✅ Negox tiene **precios competitivos**
- ⚠️ Verifica que Negox tenga todo lo que necesitas

---

## 📞 Próximos Pasos

1. **Visita el sitio de Negox:** [www.negox.com](https://www.negox.com)
2. **Revisa los planes detallados** y sus características
3. **Contacta a Negox** con las preguntas de la sección anterior
4. **Compara con otras opciones** si es necesario
5. **Elige el plan** según tus necesidades
6. **Contrata** y comienza el despliegue

---

## 💡 Consejos Finales

- ✅ **Empieza con Plan Avanzado** si no estás seguro (puedes escalar después)
- ✅ **Considera contrato de 2-3 años** si estás seguro (ahorras dinero)
- ✅ **Verifica que incluye backup** (muy importante para hoteles)
- ✅ **Pregunta sobre migración gratuita** si ya tienes hosting
- ✅ **Lee los términos y condiciones** antes de contratar
- ✅ **Prueba primero** con un subdominio si es posible

---

## 📅 Recordatorio

**Los precios pueden variar:**
- ⚠️ Los precios mostrados son aproximados y pueden cambiar
- ⚠️ Puede haber promociones especiales
- ⚠️ Verifica precios actualizados en el sitio de Negox
- ⚠️ Los precios pueden variar según tu país/moneda

**Última actualización:** Diciembre 2024

---

## 📊 Tabla Comparativa Detallada

| Característica | Plan Estándar | Plan Avanzado ⭐ | Plan Premium |
|----------------|---------------|------------------|--------------|
| **Precio/Mes (1 año)** | $4.50 | $9.50 | $19.50 |
| **Precio/Año** | $54 | $114 | $234 |
| **Espacio en disco** | 5 GB | 10 GB | 25 GB |
| **Transferencia/mes** | 50 GB | 100 GB | 250 GB |
| **Sitios web** | 1 | 2 | 5 |
| **SQL Server DB** | 1 | 2 | 5 |
| **Cuentas correo** | 10 | 20 | 50 |
| **Para Hotel Prado** | ⚠️ Ajustado | ✅ **RECOMENDADO** | ❌ Excesivo |

---

---

## 🎯 RECOMENDACIÓN FINAL (Basada en tu Situación)

### Para Hotel Prado con Tráfico No Alto:

**⭐ PLAN RECOMENDADO: Plan Avanzado**

**Costo:** $114 USD/año ($9.50/mes) o $10/mes si pagas mensual

**¿Por qué?**
1. ✅ **10 GB de espacio** - Te da margen para crecer sin preocuparte
2. ✅ **100 GB transferencia** - Más que suficiente para tráfico bajo-medio
3. ✅ **2 bases de datos** - Puedes tener producción + pruebas
4. ✅ **2 sitios web** - Sitio principal + sitio de desarrollo/pruebas
5. ✅ **Solo $5 más/mes** que Estándar pero mucho más cómodo
6. ✅ **Puedes escalar recursos** si necesitas más (ver sección de recursos adicionales)

### Si Quieres Ahorrar (Plan Estándar):

**Costo:** $54 USD/año ($4.50/mes)

**Considera Plan Estándar si:**
- ✅ Estás seguro de optimizar bien las imágenes (comprimir, usar WebP)
- ✅ No necesitas sitio de pruebas
- ✅ El tráfico será realmente bajo
- ✅ Estás dispuesto a monitorear el uso de espacio

**⚠️ Advertencia:** 5 GB puede ser ajustado si tienes muchas imágenes sin optimizar.

### Recursos Adicionales (Si Necesitas Más)

Si en el futuro necesitas más recursos, Negox ofrece:
- **+10 GB espacio:** $5/mes adicionales
- **+100 GB transferencia:** $10/mes adicionales
- **+1 sitio web:** $3/mes adicionales
- **+1 base de datos SQL:** $3/mes adicionales

**Ventaja:** Puedes empezar con Plan Avanzado y agregar recursos según necesites, sin cambiar de plan.

---

## 💰 Comparación de Costos Reales

### Plan Estándar (1 año)
- **Mensual:** $4.50/mes = $54/año
- **Con recursos adicionales si necesitas:**
  - +10 GB espacio: +$5/mes = $9.50/mes total
  - **Total:** $114/año (igual que Plan Avanzado pero con menos sitios/BD)

### Plan Avanzado (1 año) ⭐ RECOMENDADO
- **Mensual:** $9.50/mes = $114/año
- **Incluye:** Todo lo que necesitas sin preocuparte
- **Ventaja:** Mejor relación precio/recursos

### Plan Premium (1 año)
- **Mensual:** $19.50/mes = $234/año
- **Para tu caso:** Excesivo, no lo necesitas

---

## ✅ Decisión Final

**Para Hotel Prado con tráfico no alto, te recomiendo:**

### 🟡 Plan Avanzado - $114 USD/año

**Razones:**
- ✅ Espacio suficiente (10 GB) con margen de crecimiento
- ✅ Transferencia más que suficiente (100 GB)
- ✅ Flexibilidad (2 sitios, 2 BD) para desarrollo
- ✅ Precio razonable ($9.50/mes)
- ✅ Puedes agregar recursos si creces

**Alternativa económica:**
- 🟢 Plan Estándar - $54 USD/año (si optimizas imágenes y monitoreas uso)

**NO recomendado:**
- 🔴 Plan Premium - $234 USD/año (excesivo para tus necesidades)

---

**¿Necesitas ayuda para decidir?** Considera:
- Tamaño de tu hotel
- Tráfico esperado (bajo-medio según mencionaste)
- Presupuesto disponible
- Necesidades futuras

**Fuente:** Información oficial de [Negox Hosting](https://www.negox.com/es/hosting/)

¡Buena suerte con tu despliegue! 🚀

