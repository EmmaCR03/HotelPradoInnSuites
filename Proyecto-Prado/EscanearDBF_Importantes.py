#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Escáner de archivos DBF - Solo tablas importantes del hotel
Enfocado en sicre_d que parece ser la base de datos principal del hotel
"""
import os
import sys
from struct import unpack
from collections import defaultdict

def leer_estructura_dbf(archivo):
    """Lee la estructura de un archivo DBF sin mostrar datos"""
    try:
        with open(archivo, 'rb') as f:
            # Leer encabezado
            signature = unpack('B', f.read(1))[0]
            
            # Leer fecha
            year, month, day = unpack('BBB', f.read(3))
            
            # Leer número de registros
            f.seek(4)
            num_records = unpack('<I', f.read(4))[0]
            
            # Leer tamaño del encabezado
            f.seek(8)
            header_size = unpack('<H', f.read(2))[0]
            
            # Leer tamaño del registro
            f.seek(10)
            record_size = unpack('<H', f.read(2))[0]
            
            # Calcular número de campos
            num_fields = (header_size - 33) // 32
            
            # Leer campos
            campos = []
            f.seek(32)  # Inicio de definición de campos
            
            for i in range(num_fields):
                field_data = f.read(32)
                field_name = field_data[0:11].decode('ascii', errors='ignore').strip('\x00')
                field_type = chr(field_data[11])
                field_length = field_data[16]
                field_decimals = field_data[17]
                
                if field_name:  # Solo agregar campos con nombre válido
                    campos.append({
                        'nombre': field_name,
                        'tipo': field_type,
                        'longitud': field_length,
                        'decimales': field_decimals
                    })
            
            # Leer algunos registros de muestra (máximo 3)
            muestras = []
            f.seek(header_size)
            registros_leidos = 0
            max_muestras = min(3, num_records)
            
            for record_num in range(num_records):
                if registros_leidos >= max_muestras:
                    break
                
                delete_flag = unpack('B', f.read(1))[0]
                if delete_flag == 0x2A:  # Registro marcado para borrar
                    f.seek(record_size - 1, 1)
                    continue
                
                registro = {}
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
                        # Limpiar caracteres problemáticos
                        valor = valor.encode('ascii', errors='ignore').decode('ascii')
                    except:
                        valor = "[no legible]"
                    
                    registro[campo['nombre']] = valor
                
                muestras.append(registro)
                registros_leidos += 1
            
            return {
                'exito': True,
                'signature': signature,
                'fecha': f"{day:02d}/{month:02d}/{1900 + year}",
                'num_records': num_records,
                'header_size': header_size,
                'record_size': record_size,
                'num_fields': len(campos),
                'campos': campos,
                'muestras': muestras
            }
    except Exception as e:
        return {
            'exito': False,
            'error': str(e)
        }

def generar_reporte_importantes(directorio_base):
    """Genera un reporte solo de las tablas importantes del hotel"""
    print("=" * 80)
    print("  ESCANER DE TABLAS IMPORTANTES DEL HOTEL")
    print("=" * 80)
    print()
    
    # Tablas importantes a buscar (en sicre_d principalmente)
    tablas_importantes = [
        'CHEKIN', 'RESERVAS', 'HABITAC', 'EMPLEADOS', 'EMPRESAS',
        'CARGOS', 'EXTRAS_AUT', 'DT_RESER', 'DT_RESALLOT',
        'TIPOS_HAB', 'TARIFAS', 'TEMPORADAS', 'FORM_PAG',
        'CANTONES', 'DISTRITOS', 'PROVINCIAS', 'NACIONAL'
    ]
    
    # Buscar archivos en sicre_d
    carpeta_principal = os.path.join(directorio_base, 'sicre_d')
    if not os.path.exists(carpeta_principal):
        carpeta_principal = directorio_base
    
    archivos_encontrados = []
    for item in os.listdir(carpeta_principal):
        if item.upper().endswith('.DBF'):
            nombre_sin_ext = os.path.splitext(item)[0].upper()
            if any(tabla in nombre_sin_ext for tabla in tablas_importantes):
                archivos_encontrados.append(os.path.join(carpeta_principal, item))
    
    print(f"Encontradas {len(archivos_encontrados)} tablas importantes")
    print()
    
    # Procesar cada archivo
    resultados = []
    
    for i, archivo in enumerate(archivos_encontrados, 1):
        nombre_archivo = os.path.basename(archivo)
        print(f"[{i}/{len(archivos_encontrados)}] Procesando: {nombre_archivo}...", end=' ')
        
        resultado = leer_estructura_dbf(archivo)
        
        if resultado['exito']:
            resultado['archivo'] = archivo
            resultado['nombre'] = nombre_archivo
            resultados.append(resultado)
            print(f"OK ({resultado['num_records']} registros)")
        else:
            print(f"ERROR: {resultado['error']}")
    
    print()
    print("=" * 80)
    print("  INVENTARIO DE TABLAS IMPORTANTES")
    print("=" * 80)
    print()
    
    # Ordenar por número de registros
    resultados_ordenados = sorted(resultados, key=lambda x: x['num_records'], reverse=True)
    
    # Guardar en archivo
    nombre_reporte = "INVENTARIO_TABLAS_IMPORTANTES.txt"
    with open(nombre_reporte, 'w', encoding='utf-8') as f:
        f.write("=" * 80 + "\n")
        f.write("  INVENTARIO DE TABLAS IMPORTANTES DEL HOTEL\n")
        f.write("=" * 80 + "\n\n")
        
        for resultado in resultados_ordenados:
            f.write("-" * 80 + "\n")
            f.write(f"TABLA: {resultado['nombre']}\n")
            f.write(f"Registros: {resultado['num_records']}\n")
            f.write(f"Ultima modificacion: {resultado['fecha']}\n")
            f.write(f"Campos: {resultado['num_fields']}\n\n")
            f.write("  ESTRUCTURA:\n")
            for campo in resultado['campos']:
                tipo_desc = {
                    'C': 'Texto',
                    'N': 'Numerico',
                    'D': 'Fecha',
                    'L': 'Logico',
                    'M': 'Memo',
                    'F': 'Flotante',
                    'I': 'Entero',
                    'Y': 'Moneda',
                    'T': 'Fecha/Hora'
                }.get(campo['tipo'], 'Desconocido')
                f.write(f"    - {campo['nombre']:<25} {tipo_desc:<12} ({campo['longitud']} bytes)\n")
            
            if resultado['muestras']:
                f.write("\n  MUESTRA DE DATOS:\n")
                for idx, muestra in enumerate(resultado['muestras'], 1):
                    f.write(f"    Registro {idx}:\n")
                    for campo_nombre, valor in muestra.items():
                        valor_str = str(valor)[:100]
                        f.write(f"      {campo_nombre}: {valor_str}\n")
            f.write("\n")
            
            # También imprimir en consola
            print("-" * 80)
            print(f"TABLA: {resultado['nombre']}")
            print(f"Registros: {resultado['num_records']}")
            print(f"Campos: {resultado['num_fields']}")
            print("  Campos principales:")
            for campo in resultado['campos'][:10]:  # Mostrar primeros 10 campos
                print(f"    - {campo['nombre']}")
            if len(resultado['campos']) > 10:
                print(f"    ... y {len(resultado['campos']) - 10} campos más")
            print()
    
    print(f"\nReporte completo guardado en: {nombre_reporte}")
    print()
    
    # Resumen por categoría
    print("=" * 80)
    print("  RESUMEN POR CATEGORIA")
    print("=" * 80)
    print()
    
    categorias = {
        'Clientes/Huespedes': ['CHEKIN', 'EXTRAS_AUT'],
        'Reservas': ['RESERVAS', 'DT_RESER', 'DT_RESALLOT', 'GRUP_RES'],
        'Habitaciones': ['HABITAC', 'TIPOS_HAB', 'HABITACIONESSEPARADAS'],
        'Cargos/Facturacion': ['CARGOS', 'CARGOSREGISTRADOS', 'ACUM_CARGOS'],
        'Configuracion': ['TARIFAS', 'TEMPORADAS', 'FORM_PAG', 'PARAMET'],
        'Empleados': ['EMPLEADOS'],
        'Empresas': ['EMPRESAS'],
        'Ubicaciones': ['CANTONES', 'DISTRITOS', 'PROVINCIAS', 'NACIONAL']
    }
    
    for categoria, palabras_clave in categorias.items():
        tablas_categoria = []
        for resultado in resultados_ordenados:
            nombre_upper = resultado['nombre'].upper()
            if any(palabra in nombre_upper for palabra in palabras_clave):
                tablas_categoria.append(resultado)
        
        if tablas_categoria:
            print(f"{categoria}:")
            for tabla in tablas_categoria:
                print(f"  - {tabla['nombre']} ({tabla['num_records']} registros)")
            print()

if __name__ == "__main__":
    if len(sys.argv) > 1:
        directorio = sys.argv[1]
    else:
        directorio = "dbViejaHotel"
    
    if not os.path.exists(directorio):
        print(f"Error: El directorio {directorio} no existe")
        sys.exit(1)
    
    generar_reporte_importantes(directorio)

