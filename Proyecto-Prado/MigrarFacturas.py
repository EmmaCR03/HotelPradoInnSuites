#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Script de Migración de Facturas desde DBF a SQL Server
Migra datos desde FACTURAS.dbf (sicre_d) a SQL Server
"""
import os
import sys
from struct import unpack
import pyodbc
from datetime import datetime
from datetime import timedelta

# Configuración de conexión a SQL Server
CONNECTION_STRING = "DRIVER={ODBC Driver 17 for SQL Server};SERVER=localhost;DATABASE=HotelPrado;Trusted_Connection=yes;"

def leer_registro_dbf(archivo, campos, record_num, header_size, record_size):
    """Lee un registro específico de un archivo DBF"""
    try:
        with open(archivo, 'rb') as f:
            f.seek(header_size + (record_num * record_size))
            
            delete_flag = unpack('B', f.read(1))[0]
            if delete_flag == 0x2A:
                return None
            
            registro = {}
            for campo in campos:
                field_data = f.read(campo['longitud'])
                try:
                    if campo['tipo'] == 'C':
                        valor = field_data.decode('latin-1', errors='ignore').strip()
                        valor = valor.encode('ascii', errors='ignore').decode('ascii')
                    elif campo['tipo'] == 'N':
                        valor_str = field_data.decode('ascii', errors='ignore').strip()
                        try:
                            if campo['decimales'] > 0:
                                valor = float(valor_str) if valor_str else None
                            else:
                                valor = int(valor_str) if valor_str else None
                        except:
                            valor = None
                    elif campo['tipo'] == 'D':
                        fecha_str = field_data.decode('ascii', errors='ignore').strip()
                        if len(fecha_str) == 8:
                            try:
                                year = int(fecha_str[:4])
                                month = int(fecha_str[4:6])
                                day = int(fecha_str[6:8])
                                valor = datetime(year, month, day) if year > 1900 else None
                            except:
                                valor = None
                        else:
                            valor = None
                    elif campo['tipo'] == 'T':
                        # DateTime: 8 bytes
                        if len(field_data) >= 8:
                            try:
                                days = unpack('<I', field_data[0:4])[0]
                                ms = unpack('<I', field_data[4:8])[0]
                                base_date = datetime(1, 1, 1)
                                fecha = base_date + timedelta(days=days-1)
                                fecha = fecha + timedelta(milliseconds=ms)
                                valor = fecha if fecha.year > 1900 else None
                            except:
                                valor = None
                        else:
                            valor = None
                    elif campo['tipo'] == 'L':
                        valor = field_data[0] in [b'T', b't', b'Y', b'y', ord('T'), ord('t'), ord('Y'), ord('y')]
                    elif campo['tipo'] == 'I':
                        valor = unpack('<i', field_data[:4])[0] if len(field_data) >= 4 else None
                    else:
                        valor = field_data.decode('latin-1', errors='ignore').strip()
                except:
                    valor = None
                
                registro[campo['nombre']] = valor
            
            return registro
    except Exception as e:
        return None

def leer_estructura_dbf(archivo):
    """Lee la estructura de un archivo DBF"""
    try:
        with open(archivo, 'rb') as f:
            signature = unpack('B', f.read(1))[0]
            year, month, day = unpack('BBB', f.read(3))
            
            f.seek(4)
            num_records = unpack('<I', f.read(4))[0]
            
            f.seek(8)
            header_size = unpack('<H', f.read(2))[0]
            
            f.seek(10)
            record_size = unpack('<H', f.read(2))[0]
            
            num_fields = (header_size - 33) // 32
            
            campos = []
            f.seek(32)
            
            for i in range(num_fields):
                field_data = f.read(32)
                field_name = field_data[0:11].decode('ascii', errors='ignore').strip('\x00')
                field_type = chr(field_data[11])
                field_length = field_data[16]
                field_decimals = field_data[17]
                
                if field_name:
                    campos.append({
                        'nombre': field_name,
                        'tipo': field_type,
                        'longitud': field_length,
                        'decimales': field_decimals
                    })
            
            return {
                'exito': True,
                'num_records': num_records,
                'header_size': header_size,
                'record_size': record_size,
                'campos': campos
            }
    except Exception as e:
        return {
            'exito': False,
            'error': str(e)
        }

def migrar_facturas(archivo_dbf, conexion):
    """Migra facturas desde FACTURAS.dbf"""
    print("=" * 80)
    print("MIGRANDO FACTURAS DESDE FACTURAS.dbf")
    print("=" * 80)
    print()
    
    estructura = leer_estructura_dbf(archivo_dbf)
    if not estructura['exito']:
        print(f"Error leyendo estructura: {estructura['error']}")
        return
    
    print(f"Total registros: {estructura['num_records']}")
    print()
    
    cursor = conexion.cursor()
    
    # Contadores
    facturas_insertadas = 0
    facturas_con_error = 0
    facturas_duplicadas = 0
    
    # Procesar registros
    for record_num in range(estructura['num_records']):
        registro = leer_registro_dbf(
            archivo_dbf, 
            estructura['campos'], 
            record_num, 
            estructura['header_size'], 
            estructura['record_size']
        )
        
        if registro is None:
            continue
        
        try:
            # Mapear campos del DBF a SQL Server
            numero_factura = registro.get('NO_FACT')
            no_emplea = registro.get('NO_EMPLEA')
            fech_ho_fa = registro.get('FECH_HO_FA')
            tot1 = registro.get('TOT1')
            aimp_ict = registro.get('AIMP_ICT')
            aimp_serv = registro.get('AIMP_SERV')
            aimp_ven = registro.get('AIMP_VEN')
            tot_cons = registro.get('TOT_CONS')
            q_paga = registro.get('Q_PAGA')
            cerrado = registro.get('CERRADO')
            en_fextras = registro.get('EN_FEXTRAS')
            
            # Validar que NumeroFactura no sea None
            if numero_factura is None:
                facturas_con_error += 1
                continue
            
            # Verificar si ya existe (por NumeroFactura)
            cursor.execute("""
                SELECT COUNT(*) FROM Facturas 
                WHERE NumeroFactura = ?
            """, numero_factura)
            
            if cursor.fetchone()[0] > 0:
                facturas_duplicadas += 1
                continue
            
            # Insertar factura
            cursor.execute("""
                INSERT INTO Facturas (
                    NumeroFactura,
                    IdEmpleado,
                    FechaHoraFactura,
                    TotalConsumos,
                    ImpuestoICT,
                    ImpuestoServicio,
                    ImpuestoVentas,
                    TotalGeneral,
                    QuienPaga,
                    Cerrado,
                    EnFacturaExtras,
                    FechaCreacion,
                    FechaModificacion
                ) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, GETDATE(), GETDATE())
            """, 
                numero_factura,
                no_emplea,
                fech_ho_fa,
                tot_cons,
                aimp_ict,
                aimp_serv,
                aimp_ven,
                tot1,
                q_paga,
                cerrado if cerrado is not None else False,
                en_fextras if en_fextras is not None else False
            )
            
            facturas_insertadas += 1
            
            if facturas_insertadas % 100 == 0:
                conexion.commit()
                print(f"Procesadas {facturas_insertadas} facturas...")
        
        except Exception as e:
            facturas_con_error += 1
            if facturas_con_error <= 5:  # Mostrar solo los primeros 5 errores
                print(f"Error en registro {record_num}: {e}")
    
    # Commit final
    conexion.commit()
    
    print()
    print("=" * 80)
    print("RESUMEN DE MIGRACION")
    print("=" * 80)
    print(f"Facturas insertadas: {facturas_insertadas:,}")
    print(f"Facturas duplicadas (omitidas): {facturas_duplicadas:,}")
    print(f"Facturas con error: {facturas_con_error:,}")
    print()

def main():
    print("=" * 80)
    print("SCRIPT DE MIGRACION DE FACTURAS")
    print("=" * 80)
    print()
    
    base_path = "dbViejaHotel/sicre_d"
    archivo_facturas = os.path.join(base_path, "FACTURAS.dbf")
    
    # Intentar también con sicre_dv si no existe en sicre_d
    if not os.path.exists(archivo_facturas):
        archivo_facturas = os.path.join("dbViejaHotel/sicre_dv", "FACTURAS.dbf")
    
    if not os.path.exists(archivo_facturas):
        print(f"ERROR: No se encontró {archivo_facturas}")
        print("Buscando archivos FACTURAS.dbf...")
        
        # Buscar en todas las ubicaciones
        posibles = []
        for root, dirs, files in os.walk("dbViejaHotel"):
            for file in files:
                if file.upper() == "FACTURAS.DBF":
                    posibles.append(os.path.join(root, file))
        
        if posibles:
            print(f"Archivos encontrados:")
            for i, archivo in enumerate(posibles, 1):
                print(f"  {i}. {archivo}")
            archivo_facturas = posibles[0]
            print(f"\nUsando: {archivo_facturas}")
        else:
            print("No se encontraron archivos FACTURAS.dbf")
            return
    
    try:
        print("Conectando a SQL Server...")
        conexion = pyodbc.connect(CONNECTION_STRING)
        print("Conexión exitosa")
        print()
    except Exception as e:
        print(f"ERROR al conectar a SQL Server: {e}")
        print("Asegúrate de que:")
        print("1. SQL Server está ejecutándose")
        print("2. La base de datos HotelPrado existe")
        print("3. Tienes permisos de escritura")
        return
    
    try:
        migrar_facturas(archivo_facturas, conexion)
        
        print("=" * 80)
        print("MIGRACION COMPLETADA")
        print("=" * 80)
        
    except Exception as e:
        print(f"ERROR durante la migración: {e}")
        import traceback
        traceback.print_exc()
    finally:
        conexion.close()

if __name__ == "__main__":
    main()

