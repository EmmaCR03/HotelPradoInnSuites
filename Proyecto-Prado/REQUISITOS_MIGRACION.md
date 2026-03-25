# 📋 Requisitos para Ejecutar la Migración

## ✅ Estado Actual

- ✅ Tabla **Empresas** creada
- ✅ Tabla **Cliente** ajustada (campos agregados, tipos corregidos)
- ✅ Tabla **Reservas** ajustada (campos agregados, IdCliente debe ser INT)

## 🔧 Requisitos Previos

### 1. Instalar Python (si no lo tienes)
- Python 3.7 o superior
- Verificar: `python --version`

### 2. Instalar pyodbc (para conectar a SQL Server)
```bash
pip install pyodbc
```

### 3. Verificar que SQL Server está ejecutándose
- La base de datos `HotelPrado` debe existir
- Debe estar en `localhost` o ajustar la cadena de conexión

## 📝 Archivos Necesarios

Los archivos DBF deben estar en:
- `dbViejaHotel/sicre_d/EXTRAS_AUT.dbf`
- `dbViejaHotel/sicre_d/RESERVAS.dbf`
- `dbViejaHotel/sicre_d/CHEKIN.dbf` (opcional)

## 🚀 Ejecutar Migración

```bash
python MigrarDatosDBF.py
```

## ⚠️ Notas Importantes

1. **Backup**: Hacer backup de la base de datos antes de migrar
2. **Duplicados**: El script evita duplicados por cédula en clientes
3. **Relaciones**: Las reservas se relacionan con clientes por cédula o nombre
4. **IdHabitacion**: Las reservas se insertan con IdHabitacion=1 temporalmente (se puede actualizar después)


