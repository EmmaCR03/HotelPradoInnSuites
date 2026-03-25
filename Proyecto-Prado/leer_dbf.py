#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Lector de archivos DBF (Visual FoxPro)
Permite leer y analizar archivos .DBF del sistema antiguo
"""

import sys
import os
from struct import unpack

def leer_dbf(archivo):
    """Lee un archivo DBF y muestra su estructura y datos"""
    
    if not os.path.exists(archivo):
        print(f"❌ Error: No se encontró el archivo: {archivo}")
        return
    
    print("=" * 60)
    print("  LECTOR DE ARCHIVOS DBF (Visual FoxPro)")
    print("=" * 60)
    print()
    print(f"📂 Archivo: {os.path.basename(archivo)}")
    print(f"   Ruta: {archivo}")
    print()
    
    try:
        with open(archivo, 'rb') as f:
            # Leer encabezado
            signature = unpack('B', f.read(1))[0]
            print(f"📋 Firma: 0x{signature:02X}")
            
            # Leer fecha
            year, month, day = unpack('BBB', f.read(3))
            print(f"📅 Última modificación: {day:02d}/{month:02d}/{1900 + year}")
            
            # Leer número de registros
            f.seek(4)
            num_records = unpack('<I', f.read(4))[0]
            print(f"📊 Número de registros: {num_records}")
            
            # Leer tamaño del encabezado
            f.seek(8)
            header_size = unpack('<H', f.read(2))[0]
            print(f"📏 Tamaño del encabezado: {header_size} bytes")
            
            # Leer tamaño del registro
            f.seek(10)
            record_size = unpack('<H', f.read(2))[0]
            print(f"📏 Tamaño del registro: {record_size} bytes")
            
            # Calcular número de campos
            num_fields = (header_size - 33) // 32
            print(f"📋 Número de campos: {num_fields}")
            print()
            
            # Leer campos
            print("=" * 60)
            print("  ESTRUCTURA DE CAMPOS")
            print("=" * 60)
            print()
            
            campos = []
            f.seek(32)  # Inicio de definición de campos
            
            for i in range(num_fields):
                field_data = f.read(32)
                field_name = field_data[0:11].decode('ascii', errors='ignore').strip('\x00')
                field_type = chr(field_data[11])
                field_length = field_data[16]
                field_decimals = field_data[17]
                
                campos.append({
                    'nombre': field_name,
                    'tipo': field_type,
                    'longitud': field_length,
                    'decimales': field_decimals
                })
                
                tipo_desc = {
                    'C': 'Carácter (Texto)',
                    'N': 'Numérico',
                    'D': 'Fecha',
                    'L': 'Lógico (Boolean)',
                    'M': 'Memo (Texto largo)',
                    'F': 'Flotante',
                    'I': 'Entero',
                    'Y': 'Moneda',
                    'T': 'Fecha/Hora'
                }.get(field_type, 'Desconocido')
                
                print(f"  Campo {i + 1}: {field_name}")
                print(f"    Tipo: {field_type} ({tipo_desc})")
                print(f"    Longitud: {field_length}")
                if field_decimals > 0:
                    print(f"    Decimales: {field_decimals}")
                print()
            
            # Leer datos
            print("=" * 60)
            print(f"  DATOS (Mostrando primeros 10 registros)")
            print("=" * 60)
            print()
            
            f.seek(header_size)
            registros_mostrados = 0
            max_registros = min(10, num_records)
            
            for record_num in range(num_records):
                if registros_mostrados >= max_registros:
                    break
                
                delete_flag = unpack('B', f.read(1))[0]
                if delete_flag == 0x2A:  # Registro marcado para borrar
                    f.seek(record_size - 1, 1)
                    continue
                
                print(f"📝 Registro #{record_num + 1}:")
                for campo in campos:
                    field_data = f.read(campo['longitud'])
                    try:
                        if campo['tipo'] == 'C':
                            valor = field_data.decode('latin-1', errors='ignore').strip()
                        elif campo['tipo'] == 'N':
                            valor = field_data.decode('ascii', errors='ignore').strip()
                        elif campo['tipo'] == 'D':
                            valor = field_data.decode('ascii', errors='ignore').strip()
                        elif campo['tipo'] == 'L':
                            valor = 'T' if field_data[0] in [b'T', b't', b'Y', b'y'] else 'F'
                        else:
                            valor = field_data.decode('latin-1', errors='ignore').strip()
                    except:
                        valor = str(field_data)
                    
                    print(f"   {campo['nombre']}: {valor}")
                print()
                registros_mostrados += 1
            
            if num_records > max_registros:
                print(f"... y {num_records - max_registros} registros más")
            
            # Generar sugerencia SQL
            print()
            print("=" * 60)
            print("  SUGERENCIA DE ESTRUCTURA SQL")
            print("=" * 60)
            print()
            
            table_name = os.path.splitext(os.path.basename(archivo))[0]
            print(f"CREATE TABLE [dbo].[{table_name}] (")
            print(f"    [Id] INT IDENTITY(1,1) PRIMARY KEY,")
            
            for campo in campos:
                sql_type = convertir_tipo_sql(campo['tipo'], campo['longitud'], campo['decimales'])
                nullable = "NULL" if campo['tipo'] in ['C', 'M'] else "NOT NULL"
                print(f"    [{campo['nombre']}] {sql_type} {nullable},")
            
            print(");")
            print()
            
    except Exception as e:
        print(f"❌ Error al leer el archivo: {e}")
        import traceback
        traceback.print_exc()

def convertir_tipo_sql(tipo, longitud, decimales):
    """Convierte tipo DBF a tipo SQL"""
    conversiones = {
        'C': f"VARCHAR({longitud})",
        'N': f"DECIMAL({longitud},{decimales})" if decimales > 0 else "INT",
        'D': "DATETIME",
        'T': "DATETIME",
        'L': "BIT",
        'M': "TEXT",
        'F': "FLOAT",
        'I': "INT",
        'Y': "MONEY"
    }
    return conversiones.get(tipo, "VARCHAR(255)")

if __name__ == "__main__":
    if len(sys.argv) < 2:
        print("Uso: python leer_dbf.py <archivo.DBF>")
        print()
        print("Ejemplo:")
        print("  python leer_dbf.py C:\\ruta\\archivo.DBF")
        sys.exit(1)
    
    leer_dbf(sys.argv[1])






