#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Script de Migración de Datos DBF a SQL Server
Migra datos desde archivos DBF (Visual FoxPro) a SQL Server
"""
import os
import sys
from struct import unpack
import pyodbc
from datetime import datetime

# Configuración de conexión a SQL Server
# Ajustar según tu configuración
CONNECTION_STRING = "DRIVER={ODBC Driver 17 for SQL Server};SERVER=localhost;DATABASE=HotelPrado;Trusted_Connection=yes;"
# Si no tienes ODBC Driver 17, prueba con:
# CONNECTION_STRING = "DRIVER={SQL Server};SERVER=localhost;DATABASE=HotelPrado;Trusted_Connection=yes;"

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
                        # Fecha en formato YYYYMMDD
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
                    elif campo['tipo'] == 'L':
                        valor = field_data[0] in [b'T', b't', b'Y', b'y', ord('T'), ord('t'), ord('Y'), ord('y')]
                    else:
                        valor = field_data.decode('latin-1', errors='ignore').strip()
                except:
                    valor = None
                
                registro[campo['nombre']] = valor
            
            return registro
    except Exception as e:
        print(f"Error leyendo registro {record_num}: {e}")
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

def separar_nombre(nombre_completo):
    """Intenta separar nombre completo en nombre y apellidos"""
    if not nombre_completo or len(nombre_completo.strip()) == 0:
        return None, None, None
    
    partes = nombre_completo.strip().split()
    
    if len(partes) == 0:
        return None, None, None
    elif len(partes) == 1:
        return partes[0], None, None
    elif len(partes) == 2:
        return partes[0], partes[1], None
    else:
        # Tomar el primer nombre y los últimos dos como apellidos
        return partes[0], partes[-2] if len(partes) > 1 else None, partes[-1] if len(partes) > 2 else None

def migrar_clientes_desde_extras_aut(archivo_dbf, conexion):
    """Migra clientes desde EXTRAS_AUT.dbf"""
    print("=" * 80)
    print("MIGRANDO CLIENTES DESDE EXTRAS_AUT.dbf")
    print("=" * 80)
    print()
    
    estructura = leer_estructura_dbf(archivo_dbf)
    if not estructura['exito']:
        print(f"Error leyendo estructura: {estructura['error']}")
        return
    
    print(f"Total registros: {estructura['num_records']}")
    print(f"Campos: {len(estructura['campos'])}")
    print()
    
    cursor = conexion.cursor()
    clientes_insertados = 0
    clientes_duplicados = 0
    clientes_con_error = 0
    
    # Crear diccionario para evitar duplicados por cédula
    cedulas_procesadas = set()
    
    for i in range(estructura['num_records']):
        if (i + 1) % 1000 == 0:
            print(f"Procesando registro {i + 1}/{estructura['num_records']}...")
        
        registro = leer_registro_dbf(
            archivo_dbf, 
            estructura['campos'], 
            i, 
            estructura['header_size'], 
            estructura['record_size']
        )
        
        if not registro:
            continue
        
        # Extraer datos
        nombre_completo = registro.get('A_NOMBRE_D', '').strip()
        telefono = registro.get('TELEFONO')
        direccion = registro.get('DIRECCION', '').strip()
        cedula = registro.get('CEDULA', '').strip()
        email = registro.get('EMAIL', '').strip()
        cod_empresa = registro.get('COD_EMPR')
        
        # Validar datos mínimos
        if not nombre_completo or len(nombre_completo) == 0:
            continue
        
        # Evitar duplicados por cédula
        if cedula and cedula in cedulas_procesadas:
            clientes_duplicados += 1
            continue
        
        if cedula:
            cedulas_procesadas.add(cedula)
        
        # Separar nombre
        nombre, primer_apellido, segundo_apellido = separar_nombre(nombre_completo)
        
        # Convertir teléfono
        telefono_str = None
        if telefono:
            try:
                telefono_str = str(int(telefono)) if isinstance(telefono, (int, float)) else str(telefono).strip()
            except:
                telefono_str = str(telefono).strip() if telefono else None
        
        # Buscar IdEmpresa si existe cod_empresa
        id_empresa = None
        if cod_empresa:
            try:
                cursor.execute(
                    "SELECT IdEmpresa FROM Empresas WHERE CodigoEmpresa = ?",
                    (int(cod_empresa),)
                )
                result = cursor.fetchone()
                if result:
                    id_empresa = result[0]
            except:
                pass
        
        # Insertar cliente
        try:
            cursor.execute("""
                INSERT INTO Cliente (
                    NombreCliente, 
                    PrimerApellidoCliente, 
                    SegundoApellidoCliente,
                    EmailCliente, 
                    TelefonoCliente, 
                    DireccionCliente,
                    CedulaCliente,
                    IdEmpresa
                )
                VALUES (?, ?, ?, ?, ?, ?, ?, ?)
            """, (
                nombre or nombre_completo[:100],
                primer_apellido,
                segundo_apellido,
                email[:100] if email else None,
                telefono_str[:15] if telefono_str else None,
                direccion[:200] if direccion else None,
                cedula[:16] if cedula else None,
                id_empresa
            ))
            clientes_insertados += 1
        except Exception as e:
            clientes_con_error += 1
            if clientes_con_error <= 5:  # Mostrar solo los primeros 5 errores
                print(f"Error insertando cliente {i + 1}: {e}")
    
    conexion.commit()
    
    print()
    print("=" * 80)
    print("RESUMEN DE MIGRACION DE CLIENTES")
    print("=" * 80)
    print(f"Clientes insertados: {clientes_insertados}")
    print(f"Clientes duplicados (omitidos): {clientes_duplicados}")
    print(f"Clientes con error: {clientes_con_error}")
    print()

def migrar_reservas_desde_reservas_dbf(archivo_dbf, conexion):
    """Migra reservas desde RESERVAS.dbf"""
    print("=" * 80)
    print("MIGRANDO RESERVAS DESDE RESERVAS.dbf")
    print("=" * 80)
    print()
    
    estructura = leer_estructura_dbf(archivo_dbf)
    if not estructura['exito']:
        print(f"Error leyendo estructura: {estructura['error']}")
        return
    
    print(f"Total registros: {estructura['num_records']}")
    print()
    
    cursor = conexion.cursor()
    reservas_insertadas = 0
    reservas_con_error = 0
    reservas_sin_cliente = 0
    
    for i in range(estructura['num_records']):
        if (i + 1) % 1000 == 0:
            print(f"Procesando registro {i + 1}/{estructura['num_records']}...")
        
        registro = leer_registro_dbf(
            archivo_dbf, 
            estructura['campos'], 
            i, 
            estructura['header_size'], 
            estructura['record_size']
        )
        
        if not registro:
            continue
        
        # Extraer datos
        nombre_cliente = registro.get('CLIENTE', '').strip()
        cedula = registro.get('CEDULA', '').strip()
        telefono = registro.get('TELEFONO')
        direccion = registro.get('DIRECCION', '').strip()
        email = registro.get('EMAIL', '').strip()
        cod_empresa = registro.get('COD_EMPR')
        fecha_in = registro.get('FECHA_IN')
        fecha_out = registro.get('FECHA_OUT')
        n_adultos = registro.get('N_ADULTOS')
        n_ninos = registro.get('N_NINOS')
        total = registro.get('TOTAL')
        estatus = registro.get('ESTATUS')
        observaciones = registro.get('OBSERVA_R', '').strip()
        
        # Buscar cliente por nombre o cédula
        id_cliente = None
        if cedula:
            cursor.execute("SELECT IdCliente FROM Cliente WHERE CedulaCliente = ?", (cedula,))
            result = cursor.fetchone()
            if result:
                id_cliente = result[0]
        
        if not id_cliente and nombre_cliente:
            # Buscar por nombre (primeros 50 caracteres)
            nombre_buscar = nombre_cliente[:50]
            cursor.execute(
                "SELECT TOP 1 IdCliente FROM Cliente WHERE NombreCliente LIKE ?",
                (f"%{nombre_buscar}%",)
            )
            result = cursor.fetchone()
            if result:
                id_cliente = result[0]
        
        if not id_cliente:
            reservas_sin_cliente += 1
            continue
        
        # Buscar IdEmpresa
        id_empresa = None
        if cod_empresa:
            try:
                cursor.execute(
                    "SELECT IdEmpresa FROM Empresas WHERE CodigoEmpresa = ?",
                    (int(cod_empresa),)
                )
                result = cursor.fetchone()
                if result:
                    id_empresa = result[0]
            except:
                pass
        
        # Convertir fechas
        fecha_inicio = None
        fecha_final = None
        if fecha_in:
            fecha_inicio = fecha_in
        if fecha_out:
            fecha_final = fecha_out
        
        # Convertir teléfono
        telefono_str = None
        if telefono:
            try:
                telefono_str = str(int(telefono)) if isinstance(telefono, (int, float)) else str(telefono).strip()
            except:
                telefono_str = str(telefono).strip() if telefono else None
        
        # Determinar estado
        estado_reserva = 'Pendiente'
        if estatus:
            if estatus == 1:
                estado_reserva = 'Confirmada'
            elif estatus == 2:
                estado_reserva = 'Cancelada'
            elif estatus == 3:
                estado_reserva = 'Completada'
        
        # Calcular cantidad de personas
        cantidad_personas = None
        if n_adultos:
            cantidad_personas = int(n_adultos)
        if n_ninos:
            cantidad_personas = (cantidad_personas or 0) + int(n_ninos)
        
        # Insertar reserva (sin IdHabitacion por ahora, se puede actualizar después)
        try:
            cursor.execute("""
                INSERT INTO Reservas (
                    IdCliente,
                    NombreCliente,
                    cantidadPersonas,
                    FechaInicio,
                    FechaFinal,
                    EstadoReserva,
                    MontoTotal,
                    CedulaCliente,
                    TelefonoCliente,
                    DireccionCliente,
                    EmailCliente,
                    NumeroAdultos,
                    NumeroNinos,
                    IdEmpresa,
                    Observaciones,
                    IdHabitacion
                )
                VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
            """, (
                id_cliente,
                nombre_cliente[:128] if nombre_cliente else None,
                cantidad_personas,
                fecha_inicio,
                fecha_final,
                estado_reserva,
                float(total) if total else 0.0,
                cedula[:16] if cedula else None,
                telefono_str[:15] if telefono_str else None,
                direccion[:200] if direccion else None,
                email[:100] if email else None,
                int(n_adultos) if n_adultos else None,
                int(n_ninos) if n_ninos else None,
                id_empresa,
                observaciones[:500] if observaciones else None,
                1  # IdHabitacion temporal (se puede actualizar después)
            ))
            reservas_insertadas += 1
        except Exception as e:
            reservas_con_error += 1
            if reservas_con_error <= 5:
                print(f"Error insertando reserva {i + 1}: {e}")
    
    conexion.commit()
    
    print()
    print("=" * 80)
    print("RESUMEN DE MIGRACION DE RESERVAS")
    print("=" * 80)
    print(f"Reservas insertadas: {reservas_insertadas}")
    print(f"Reservas sin cliente (omitidas): {reservas_sin_cliente}")
    print(f"Reservas con error: {reservas_con_error}")
    print()

def main():
    print("=" * 80)
    print("SCRIPT DE MIGRACION DE DATOS DBF A SQL SERVER")
    print("=" * 80)
    print()
    
    # Verificar que los archivos DBF existen
    base_path = "dbViejaHotel/sicre_d"
    
    archivo_extras_aut = os.path.join(base_path, "EXTRAS_AUT.dbf")
    archivo_reservas = os.path.join(base_path, "RESERVAS.dbf")
    
    if not os.path.exists(archivo_extras_aut):
        print(f"ERROR: No se encontró {archivo_extras_aut}")
        return
    
    if not os.path.exists(archivo_reservas):
        print(f"ERROR: No se encontró {archivo_reservas}")
        return
    
    # Conectar a SQL Server
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
        # Migrar clientes
        migrar_clientes_desde_extras_aut(archivo_extras_aut, conexion)
        
        # Migrar reservas
        migrar_reservas_desde_reservas_dbf(archivo_reservas, conexion)
        
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

