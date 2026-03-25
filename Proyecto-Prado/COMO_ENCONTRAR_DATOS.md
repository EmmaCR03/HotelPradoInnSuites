# 🔍 Cómo Encontrar los Datos del Sistema Antiguo

## 📋 Situación Actual

Solo encontré `FOXUSER.DBF` y `FOXUSER.FPT` en la carpeta `exe`, pero estos son archivos del sistema de Visual FoxPro, **NO contienen los datos del hotel**.

---

## 🔎 Dónde Pueden Estar los Datos

### 1. **Unidad de Red** (Más Probable)
Los archivos `.bat` muestran que el sistema se conecta a:
- `\\server\delicias` (mapeado como unidad M:)

**Para verificar:**
```bash
# Ejecuta esto en CMD:
net use
# Verás si hay unidades de red mapeadas

# O intenta acceder a:
dir M:\*.DBF
```

### 2. **Otra Carpeta en la Misma Computadora**
Los datos pueden estar en:
- `C:\hotel\`
- `C:\datos\`
- `C:\hoteleria\`
- O cualquier otra carpeta

**Para buscar:**
```bash
# Buscar todos los DBF en C:\
dir /s /b C:\*.DBF
```

### 3. **Otra Computadora o Servidor**
- Puede estar en el servidor del hotel
- O en otra computadora que usa el sistema

### 4. **Backup o Copia de Seguridad**
- Puede haber una copia en un disco externo
- O en una carpeta de respaldo

---

## 🛠️ Soluciones

### Opción A: Preguntar a los Dueños

**Preguntas importantes:**
1. ¿Dónde está instalado el sistema antiguo?
2. ¿Dónde guarda los datos el programa?
3. ¿Tienen una copia de los archivos DBF?
4. ¿Pueden exportar los datos a Excel o CSV desde el programa?

### Opción B: Buscar Manualmente

1. **Ejecuta el script de búsqueda:**
   ```bash
   BUSCAR_DBF.bat
   ```

2. **O busca manualmente:**
   - Abre el Explorador de Windows
   - Busca archivos `.DBF` en toda la computadora
   - Revisa unidades de red si están conectadas

### Opción C: Ejecutar el Programa Antiguo

Si puedes ejecutar el programa (por ejemplo, `aiho.exe` o `aiaf.exe`):

1. **Ejecuta el programa**
2. **Abre cualquier ventana que muestre datos** (clientes, reservas, etc.)
3. **Revisa la configuración del programa** - puede mostrar dónde está la base de datos
4. **O usa el Administrador de Tareas** para ver qué archivos está abriendo el programa

### Opción D: Exportar desde el Programa

Si el programa tiene opción de exportar:
1. Exporta los datos a Excel o CSV
2. Luego podemos importarlos a SQL Server

---

## 📝 Archivos DBF que Necesitamos

Basándome en tu base de datos SQL Server, necesitarías estos archivos:

1. **CLIENTE.DBF** o **CLIENTES.DBF** → Datos de clientes
2. **RESERVA.DBF** o **RESERVAS.DBF** → Datos de reservas
3. **HABITACION.DBF** o **HABITACIONES.DBF** → Datos de habitaciones
4. **DEPARTAMENTO.DBF** → Datos de departamentos
5. **COLABORADOR.DBF** → Datos de colaboradores
6. **CITA.DBF** o **CITAS.DBF** → Datos de citas

Y posiblemente:
- **TIPOHABITACION.DBF**
- **TIPODEPARTAMENTO.DBF**

---

## ✅ Próximos Pasos

1. **Ejecuta `BUSCAR_DBF.bat`** para buscar automáticamente
2. **Pregunta a los dueños** dónde están los datos
3. **Si encuentras los archivos**, usa `leer_dbf.py` para leerlos
4. **Si no los encuentras**, podemos trabajar con exportaciones a Excel/CSV

---

## 💡 Alternativa: Trabajar sin los DBF

Si no puedes acceder a los archivos DBF originales, podemos:

1. **Usar los datos que ya tienes** en SQL Server (si hay algo)
2. **Crear la estructura correcta** en SQL Server
3. **Los dueños pueden ingresar los datos manualmente** o exportarlos desde el programa antiguo

¿Qué opción prefieres intentar primero?






