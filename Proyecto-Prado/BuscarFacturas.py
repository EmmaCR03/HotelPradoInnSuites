#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Script para buscar archivos relacionados con Facturas en la base de datos vieja
"""

import os
import sys
from pathlib import Path

# Configurar codificación para Windows
if sys.platform == 'win32':
    import codecs
    sys.stdout = codecs.getwriter('utf-8')(sys.stdout.buffer, 'strict')
    sys.stderr = codecs.getwriter('utf-8')(sys.stderr.buffer, 'strict')

def buscar_archivos_facturas(base_path='dbViejaHotel'):
    """
    Busca todos los archivos DBF relacionados con facturas
    """
    base = Path(base_path)
    
    if not base.exists():
        print(f"[ERROR] La carpeta '{base_path}' no existe")
        return []
    
    print("=" * 80)
    print("BUSCANDO ARCHIVOS DE FACTURAS")
    print("=" * 80)
    print()
    
    # Patrones de búsqueda
    patrones = ['*fact*', '*FACT*', '*Factura*', '*FACTURA*', '*factura*']
    
    archivos_encontrados = []
    
    for patron in patrones:
        archivos = list(base.rglob(f'{patron}.dbf'))
        archivos_encontrados.extend(archivos)
    
    # Eliminar duplicados
    archivos_encontrados = list(set(archivos_encontrados))
    
    # Ordenar por fecha de modificación (más recientes primero)
    archivos_encontrados.sort(key=lambda x: x.stat().st_mtime if x.exists() else 0, reverse=True)
    
    if archivos_encontrados:
        print(f"[OK] Encontrados {len(archivos_encontrados)} archivo(s) relacionado(s) con facturas:\n")
        print("(Ordenados por fecha de modificación - más recientes primero)\n")
        
        for i, archivo in enumerate(archivos_encontrados, 1):
            tamaño = archivo.stat().st_size if archivo.exists() else 0
            tamaño_kb = tamaño / 1024
            fecha_mod = archivo.stat().st_mtime if archivo.exists() else 0
            from datetime import datetime
            fecha_str = datetime.fromtimestamp(fecha_mod).strftime('%Y-%m-%d %H:%M:%S') if fecha_mod > 0 else "Desconocida"
            
            print(f"{i}. {archivo}")
            print(f"   Tamaño: {tamaño_kb:.2f} KB")
            print(f"   Ultima modificacion: {fecha_str}")
            print(f"   Ruta completa: {archivo.absolute()}")
            print()
    else:
        print("[INFO] No se encontraron archivos relacionados con facturas")
        print()
        print("Buscando en subdirectorios...")
        
        # Listar todos los archivos DBF para referencia
        todos_dbf = list(base.rglob('*.dbf'))
        print(f"\n[INFO] Total de archivos DBF encontrados: {len(todos_dbf)}")
        print("\nAlgunos archivos que podrían ser relevantes:")
        
        # Buscar archivos que podrían estar relacionados
        posibles = []
        for dbf in todos_dbf:
            nombre = dbf.name.lower()
            if any(palabra in nombre for palabra in ['cargo', 'pago', 'folio', 'cuenta', 'cobro']):
                posibles.append(dbf)
        
        if posibles:
            print(f"\n[INFO] Archivos que podrían estar relacionados ({len(posibles)}):")
            for archivo in posibles[:10]:  # Mostrar solo los primeros 10
                print(f"   - {archivo}")
            if len(posibles) > 10:
                print(f"   ... y {len(posibles) - 10} más")
    
    return archivos_encontrados

def analizar_archivo(archivo_path):
    """
    Intenta analizar un archivo DBF si está disponible el módulo
    """
    try:
        from EscanearDBF_Importantes import leer_estructura_dbf
        
        print(f"\n[ANALISIS] Analizando: {archivo_path}")
        print("-" * 80)
        
        resultado = leer_estructura_dbf(str(archivo_path))
        
        if resultado['exito']:
            print(f"[OK] Archivo valido")
            print(f"   Registros: {resultado['num_records']:,}")
            print(f"   Campos: {len(resultado['campos'])}")
            if 'fecha' in resultado:
                print(f"   Ultima modificacion: {resultado['fecha']}")
            print("\n   Estructura:")
            for campo in resultado['campos'][:15]:  # Mostrar primeros 15 campos
                decimales_str = f", {campo['decimales']} dec" if campo.get('decimales', 0) > 0 else ""
                print(f"     - {campo['nombre']}: {campo['tipo']} ({campo['longitud']}{decimales_str})")
            if len(resultado['campos']) > 15:
                print(f"     ... y {len(resultado['campos']) - 15} campos más")
            
            # Mostrar muestras de datos si están disponibles
            if 'muestras' in resultado and resultado['muestras']:
                print("\n   Muestra de datos (primeros registros):")
                for idx, muestra in enumerate(resultado['muestras'][:3], 1):
                    print(f"\n     Registro #{idx}:")
                    for campo_nombre, valor in list(muestra.items())[:10]:  # Primeros 10 campos
                        valor_str = str(valor)[:50]  # Limitar longitud
                        print(f"       {campo_nombre}: {valor_str}")
                    if len(muestra) > 10:
                        print(f"       ... y {len(muestra) - 10} campos más")
        else:
            print(f"[ERROR] Error al leer el archivo: {resultado.get('error', 'Desconocido')}")
    except ImportError:
        print("\n[ADVERTENCIA] Modulo EscanearDBF_Importantes no disponible")
        print("   Para analizar la estructura, usa el LectorDBF en C#")
    except Exception as e:
        print(f"\n[ERROR] Error al analizar: {e}")

if __name__ == '__main__':
    import sys
    
    print()
    archivos = buscar_archivos_facturas()
    
    if archivos:
        # Si se pasa el argumento --analizar
        if len(sys.argv) > 2 and sys.argv[1] == '--analizar':
            archivo_a_analizar = sys.argv[2]
            analizar_archivo(archivo_a_analizar)
        # Si se pasa el argumento --recientes
        elif len(sys.argv) > 1 and sys.argv[1] == '--recientes':
            num_archivos = int(sys.argv[2]) if len(sys.argv) > 2 and sys.argv[2].isdigit() else 3
            print("=" * 80)
            print(f"ANALIZANDO LOS {num_archivos} ARCHIVOS MAS RECIENTES")
            print("=" * 80)
            print()
            for archivo in archivos[:num_archivos]:
                analizar_archivo(str(archivo))
                print()
        else:
            print("=" * 80)
            print("¿Deseas analizar algún archivo?")
            print("=" * 80)
            print()
            print("Opciones:")
            print("  python BuscarFacturas.py --analizar <ruta_al_archivo>")
            print("  python BuscarFacturas.py --recientes [numero]  (analiza los N más recientes, default: 3)")
            print()
            print("O usa el LectorDBF:")
            print("  cd LectorDBF")
            print(f"  dotnet run \"{archivos[0]}\"")
            print()

