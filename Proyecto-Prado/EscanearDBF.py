#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Escáner de archivos DBF - Genera inventario completo de todas las tablas
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
                
                campos.append({
                    'nombre': field_name,
                    'tipo': field_type,
                    'longitud': field_length,
                    'decimales': field_decimals
                })
            
            # Leer algunos registros de muestra (máximo 5)
            muestras = []
            f.seek(header_size)
            registros_leidos = 0
            max_muestras = min(5, num_records)
            
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
                        # Limpiar caracteres problemáticos para impresión
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
                'num_fields': num_fields,
                'campos': campos,
                'muestras': muestras
            }
    except Exception as e:
        return {
            'exito': False,
            'error': str(e)
        }

def escanear_directorio(directorio, archivos_encontrados=None):
    """Escanea recursivamente un directorio buscando archivos DBF"""
    if archivos_encontrados is None:
        archivos_encontrados = []
    
    try:
        for item in os.listdir(directorio):
            ruta_completa = os.path.join(directorio, item)
            
            if os.path.isdir(ruta_completa):
                # Ignorar carpetas comunes que no tienen DBF
                if item.lower() not in ['bin', 'obj', 'packages', '.git', 'node_modules']:
                    escanear_directorio(ruta_completa, archivos_encontrados)
            elif item.upper().endswith('.DBF'):
                archivos_encontrados.append(ruta_completa)
    except PermissionError:
        pass
    
    return archivos_encontrados

def generar_reporte(directorio_base):
    """Genera un reporte completo de todos los archivos DBF"""
    print("=" * 80)
    print("  ESCANER DE ARCHIVOS DBF - INVENTARIO COMPLETO")
    print("=" * 80)
    print()
    print(f"Directorio base: {directorio_base}")
    print()
    print("Escaneando archivos DBF...")
    print()
    
    # Escanear todos los DBF
    archivos_dbf = escanear_directorio(directorio_base)
    
    print(f"Encontrados {len(archivos_dbf)} archivos DBF")
    print()
    
    # Procesar cada archivo
    resultados = []
    errores = []
    
    for i, archivo in enumerate(archivos_dbf, 1):
        nombre_archivo = os.path.basename(archivo)
        ruta_relativa = os.path.relpath(archivo, directorio_base)
        
        print(f"[{i}/{len(archivos_dbf)}] Procesando: {nombre_archivo}...", end=' ')
        
        resultado = leer_estructura_dbf(archivo)
        
        if resultado['exito']:
            resultado['archivo'] = archivo
            resultado['nombre'] = nombre_archivo
            resultado['ruta_relativa'] = ruta_relativa
            resultados.append(resultado)
            print(f"OK ({resultado['num_records']} registros)")
        else:
            errores.append({
                'archivo': archivo,
                'error': resultado['error']
            })
            print(f"ERROR: {resultado['error']}")
    
    print()
    print("=" * 80)
    print("  RESUMEN")
    print("=" * 80)
    print()
    print(f"Total archivos procesados: {len(archivos_dbf)}")
    print(f"Archivos exitosos: {len(resultados)}")
    print(f"Archivos con errores: {len(errores)}")
    print()
    
    # Generar reporte detallado
    print("=" * 80)
    print("  INVENTARIO DE TABLAS")
    print("=" * 80)
    print()
    
    # Agrupar por directorio
    por_directorio = defaultdict(list)
    for resultado in resultados:
        dir_base = os.path.dirname(resultado['ruta_relativa'])
        por_directorio[dir_base].append(resultado)
    
    # Ordenar por número de registros (más importantes primero)
    resultados_ordenados = sorted(resultados, key=lambda x: x['num_records'], reverse=True)
    
    for resultado in resultados_ordenados:
        print("-" * 80)
        print(f"TABLA: {resultado['nombre']}")
        print(f"Ubicacion: {resultado['ruta_relativa']}")
        print(f"Registros: {resultado['num_records']}")
        print(f"Ultima modificacion: {resultado['fecha']}")
        print(f"Campos: {resultado['num_fields']}")
        print()
        print("  ESTRUCTURA:")
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
            
            print(f"    - {campo['nombre']:<20} {tipo_desc:<10} ({campo['longitud']} bytes)")
        
            if resultado['muestras']:
                print()
                print("  MUESTRA DE DATOS (primer registro):")
                primer_registro = resultado['muestras'][0]
                for campo_nombre, valor in primer_registro.items():
                    try:
                        valor_str = str(valor)[:50]  # Limitar longitud
                        # Limpiar caracteres problemáticos
                        valor_str = valor_str.encode('ascii', errors='ignore').decode('ascii')
                        print(f"    {campo_nombre}: {valor_str}")
                    except:
                        print(f"    {campo_nombre}: [valor no mostrable]")
        print()
    
    # Guardar reporte en archivo
    nombre_reporte = "INVENTARIO_DBF.txt"
    with open(nombre_reporte, 'w', encoding='utf-8') as f:
        f.write("=" * 80 + "\n")
        f.write("  INVENTARIO COMPLETO DE ARCHIVOS DBF\n")
        f.write("=" * 80 + "\n\n")
        f.write(f"Directorio base: {directorio_base}\n")
        f.write(f"Total archivos: {len(archivos_dbf)}\n")
        f.write(f"Archivos exitosos: {len(resultados)}\n")
        f.write(f"Archivos con errores: {len(errores)}\n\n")
        
        f.write("=" * 80 + "\n")
        f.write("  TABLAS ORDENADAS POR NUMERO DE REGISTROS\n")
        f.write("=" * 80 + "\n\n")
        
        for resultado in resultados_ordenados:
            f.write("-" * 80 + "\n")
            f.write(f"TABLA: {resultado['nombre']}\n")
            f.write(f"Ubicacion: {resultado['ruta_relativa']}\n")
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
                f.write(f"    - {campo['nombre']:<20} {tipo_desc:<10} ({campo['longitud']} bytes)\n")
            
            if resultado['muestras']:
                f.write("\n  MUESTRA DE DATOS:\n")
                for idx, muestra in enumerate(resultado['muestras'], 1):
                    f.write(f"    Registro {idx}:\n")
                    for campo_nombre, valor in muestra.items():
                        valor_str = str(valor)[:100]
                        f.write(f"      {campo_nombre}: {valor_str}\n")
            f.write("\n")
        
        if errores:
            f.write("\n" + "=" * 80 + "\n")
            f.write("  ARCHIVOS CON ERRORES\n")
            f.write("=" * 80 + "\n\n")
            for error in errores:
                f.write(f"Archivo: {os.path.basename(error['archivo'])}\n")
                f.write(f"Error: {error['error']}\n\n")
    
    print(f"\nReporte guardado en: {nombre_reporte}")
    print()
    
    # Generar resumen por categorías
    print("=" * 80)
    print("  RESUMEN POR CATEGORIAS (sugerencias)")
    print("=" * 80)
    print()
    
    # Buscar tablas que puedan contener clientes
    posibles_clientes = []
    for resultado in resultados:
        nombre_lower = resultado['nombre'].lower()
        campos_lower = [c['nombre'].lower() for c in resultado['campos']]
        
        if any(palabra in nombre_lower for palabra in ['cliente', 'client', 'huesped', 'guest']):
            posibles_clientes.append(resultado)
        elif any(palabra in campos_lower for palabra in ['nombre', 'apellido', 'cedula', 'telefono', 'email']):
            if resultado['num_records'] > 0:
                posibles_clientes.append(resultado)
    
    if posibles_clientes:
        print("TABLAS QUE PODRIAN CONTENER CLIENTES:")
        for tabla in posibles_clientes:
            print(f"  - {tabla['nombre']} ({tabla['num_records']} registros)")
        print()
    
    # Buscar tablas de reservas
    posibles_reservas = []
    for resultado in resultados:
        nombre_lower = resultado['nombre'].lower()
        if any(palabra in nombre_lower for palabra in ['reserva', 'reserv', 'checkin', 'check-in']):
            posibles_reservas.append(resultado)
    
    if posibles_reservas:
        print("TABLAS QUE PODRIAN CONTENER RESERVAS:")
        for tabla in posibles_reservas:
            print(f"  - {tabla['nombre']} ({tabla['num_records']} registros)")
        print()
    
    # Buscar tablas de habitaciones
    posibles_habitaciones = []
    for resultado in resultados:
        nombre_lower = resultado['nombre'].lower()
        if any(palabra in nombre_lower for palabra in ['habitac', 'habita', 'room', 'cuarto']):
            posibles_habitaciones.append(resultado)
    
    if posibles_habitaciones:
        print("TABLAS QUE PODRIAN CONTENER HABITACIONES:")
        for tabla in posibles_habitaciones:
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
    
    generar_reporte(directorio)

