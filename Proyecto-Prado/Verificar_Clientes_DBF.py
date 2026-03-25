#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Script para Verificar Clientes en EXTRAS_AUT.dbf
Cuenta cuántos clientes hay en la base de datos vieja sin migrarlos
"""
import os
import sys
from struct import unpack

def leer_estructura_dbf(archivo):
    """Lee la estructura de un archivo DBF"""
    try:
        with open(archivo, 'rb') as f:
            # Leer header
            header = f.read(32)
            if len(header) < 32:
                return {'exito': False, 'error': 'Archivo demasiado corto'}
            
            # Información del header
            tipo_archivo = header[0]
            fecha_ultima_mod = f"{header[1]:02d}/{header[2]:02d}/{1900 + header[3]}"
            num_records = unpack('<I', header[4:8])[0]
            header_size = unpack('<H', header[8:10])[0]
            record_size = unpack('<H', header[10:12])[0]
            
            # Leer descripción de campos
            num_campos = (header_size - 33) // 32
            campos = []
            
            for i in range(num_campos):
                campo_header = f.read(32)
                if len(campo_header) < 32:
                    break
                
                nombre_campo = campo_header[0:11].decode('ascii', errors='ignore').strip('\x00')
                tipo_campo = chr(campo_header[11])
                longitud = campo_header[16]
                decimales = campo_header[17]
                
                campos.append({
                    'nombre': nombre_campo,
                    'tipo': tipo_campo,
                    'longitud': longitud,
                    'decimales': decimales,
                    'offset': sum(c['longitud'] for c in campos)
                })
            
            return {
                'exito': True,
                'num_records': num_records,
                'header_size': header_size,
                'record_size': record_size,
                'campos': campos,
                'fecha_modificacion': fecha_ultima_mod
            }
    except Exception as e:
        return {'exito': False, 'error': str(e)}

def leer_registro_dbf(archivo, campos, record_num, header_size, record_size):
    """Lee un registro específico de un archivo DBF"""
    try:
        with open(archivo, 'rb') as f:
            f.seek(header_size + (record_num * record_size))
            
            delete_flag = unpack('B', f.read(1))[0]
            if delete_flag == 0x2A:  # Registro marcado para borrar
                return None
            
            registro = {}
            for campo in campos:
                valor = f.read(campo['longitud']).decode('latin-1', errors='ignore').strip()
                
                if campo['tipo'] == 'N':  # Numérico
                    try:
                        if campo['decimales'] > 0:
                            valor = float(valor) if valor else None
                        else:
                            valor = int(float(valor)) if valor else None
                    except:
                        valor = None
                elif campo['tipo'] == 'D':  # Fecha
                    try:
                        if len(valor) == 8:
                            año = int(valor[0:4])
                            mes = int(valor[4:6])
                            dia = int(valor[6:8])
                            valor = f"{dia:02d}/{mes:02d}/{año}"
                        else:
                            valor = None
                    except:
                        valor = None
                elif campo['tipo'] == 'L':  # Lógico
                    valor = valor.upper() in ['T', 'Y', 'S']
                elif not valor or valor == '':
                    valor = None
                
                registro[campo['nombre']] = valor
            
            return registro
    except Exception as e:
        return None

def verificar_clientes_dbf(archivo_dbf):
    """Verifica y cuenta clientes en EXTRAS_AUT.dbf"""
    print("=" * 80)
    print("VERIFICACION DE CLIENTES EN EXTRAS_AUT.dbf")
    print("=" * 80)
    print()
    
    if not os.path.exists(archivo_dbf):
        print(f"ERROR: No se encontró el archivo {archivo_dbf}")
        print()
        print("Ubicaciones posibles:")
        print("  - dbViejaHotel/sicre_d/EXTRAS_AUT.dbf")
        print("  - dbViejaHotel/sicre_dv/EXTRAS_AUT.dbf")
        return
    
    estructura = leer_estructura_dbf(archivo_dbf)
    if not estructura['exito']:
        print(f"Error leyendo estructura: {estructura['error']}")
        return
    
    print(f"Archivo: {archivo_dbf}")
    print(f"Fecha última modificación: {estructura['fecha_modificacion']}")
    print(f"Total registros en archivo: {estructura['num_records']:,}")
    print(f"Tamaño de registro: {estructura['record_size']} bytes")
    print(f"Campos encontrados: {len(estructura['campos'])}")
    print()
    
    # Mostrar campos disponibles
    print("Campos disponibles:")
    for campo in estructura['campos']:
        print(f"  - {campo['nombre']} ({campo['tipo']}, {campo['longitud']})")
    print()
    
    # Contar registros válidos
    print("Analizando registros...")
    print()
    
    total_registros = estructura['num_records']
    registros_validos = 0
    registros_con_nombre = 0
    registros_con_cedula = 0
    registros_con_email = 0
    registros_con_telefono = 0
    registros_borrados = 0
    cedulas_unicas = set()
    nombres_unicos = set()
    
    for i in range(total_registros):
        if (i + 1) % 10000 == 0:
            print(f"Procesando registro {i + 1:,}/{total_registros:,}...")
        
        registro = leer_registro_dbf(
            archivo_dbf, 
            estructura['campos'], 
            i, 
            estructura['header_size'], 
            estructura['record_size']
        )
        
        if registro is None:
            registros_borrados += 1
            continue
        
        # Verificar si tiene nombre
        nombre = registro.get('A_NOMBRE_D', '')
        if nombre and nombre.strip():
            registros_con_nombre += 1
            nombres_unicos.add(nombre.strip().lower())
        
        # Verificar cédula
        cedula = registro.get('CEDULA', '')
        if cedula and str(cedula).strip():
            registros_con_cedula += 1
            cedulas_unicas.add(str(cedula).strip())
        
        # Verificar email
        email = registro.get('EMAIL', '')
        if email and str(email).strip():
            registros_con_email += 1
        
        # Verificar teléfono
        telefono = registro.get('TELEFONO')
        if telefono:
            registros_con_telefono += 1
        
        # Si tiene nombre, es un registro válido
        if nombre and nombre.strip():
            registros_validos += 1
    
    print()
    print("=" * 80)
    print("RESUMEN DE VERIFICACION")
    print("=" * 80)
    print()
    print(f"Total registros en archivo:        {total_registros:,}")
    print(f"Registros marcados para borrar:     {registros_borrados:,}")
    print(f"Registros válidos (con nombre):     {registros_validos:,}")
    print()
    print("Datos disponibles:")
    print(f"  - Registros con nombre:           {registros_con_nombre:,}")
    print(f"  - Registros con cédula:           {registros_con_cedula:,}")
    print(f"  - Registros con email:            {registros_con_email:,}")
    print(f"  - Registros con teléfono:         {registros_con_telefono:,}")
    print()
    print("Datos únicos:")
    print(f"  - Nombres únicos:                 {len(nombres_unicos):,}")
    print(f"  - Cédulas únicas:                 {len(cedulas_unicas):,}")
    print()
    print("=" * 80)
    print("RECOMENDACIONES")
    print("=" * 80)
    print()
    
    if registros_validos > 0:
        print(f"[OK] Se pueden migrar aproximadamente {registros_validos:,} clientes validos")
        print()
        print("Consideraciones:")
        print(f"  - Habra {len(cedulas_unicas):,} clientes unicos por cedula")
        print(f"  - {registros_validos - len(cedulas_unicas):,} registros pueden ser duplicados")
        print()
        if registros_validos - len(cedulas_unicas) > 0:
            print("[ADVERTENCIA] Se recomienda migrar usando cedula como identificador unico")
            print("             para evitar duplicados")
    else:
        print("[ADVERTENCIA] No se encontraron registros validos para migrar")
    
    print()
    print("Para migrar los datos, ejecuta: python MigrarDatosDBF.py")
    print()

def main():
    print("=" * 80)
    print("VERIFICADOR DE CLIENTES EN BASE DE DATOS VIEJA")
    print("=" * 80)
    print()
    
    # Buscar el archivo en diferentes ubicaciones
    posibles_ubicaciones = [
        "dbViejaHotel/sicre_d/EXTRAS_AUT.dbf",
        "dbViejaHotel/sicre_dv/EXTRAS_AUT.dbf",
        "EXTRAS_AUT.dbf"
    ]
    
    archivo_encontrado = None
    for ubicacion in posibles_ubicaciones:
        if os.path.exists(ubicacion):
            archivo_encontrado = ubicacion
            break
    
    if not archivo_encontrado:
        print("ERROR: No se encontró el archivo EXTRAS_AUT.dbf")
        print()
        print("Busqué en las siguientes ubicaciones:")
        for ubicacion in posibles_ubicaciones:
            print(f"  - {ubicacion}")
        print()
        print("Por favor, asegúrate de que el archivo existe en una de estas ubicaciones.")
        return
    
    verificar_clientes_dbf(archivo_encontrado)

if __name__ == "__main__":
    main()

