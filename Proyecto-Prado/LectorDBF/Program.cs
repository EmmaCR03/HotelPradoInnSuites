using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LectorDBF
{
    /// <summary>
    /// Lector simple de archivos DBF (Visual FoxPro)
    /// Este programa te permite ver la estructura y datos de archivos DBF
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("==========================================");
            Console.WriteLine("  LECTOR DE ARCHIVOS DBF (Visual FoxPro)");
            Console.WriteLine("==========================================");
            Console.WriteLine();

            if (args.Length == 0)
            {
                Console.WriteLine("Uso: LectorDBF.exe <ruta_al_archivo.DBF>");
                Console.WriteLine();
                Console.WriteLine("Ejemplo:");
                Console.WriteLine("  LectorDBF.exe C:\\ruta\\archivo.DBF");
                Console.WriteLine();
                Console.WriteLine("O arrastra un archivo .DBF sobre este programa.");
                Console.WriteLine();
                Console.WriteLine("Presiona cualquier tecla para salir...");
                Console.ReadKey();
                return;
            }

            string dbfPath = args[0];

            if (!File.Exists(dbfPath))
            {
                Console.WriteLine($"❌ Error: No se encontró el archivo: {dbfPath}");
                Console.ReadKey();
                return;
            }

            if (!dbfPath.ToUpper().EndsWith(".DBF"))
            {
                Console.WriteLine($"⚠️  Advertencia: El archivo no tiene extensión .DBF");
                Console.WriteLine($"   Continuando de todas formas...");
                Console.WriteLine();
            }

            try
            {
                LeerArchivoDBF(dbfPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al leer el archivo: {ex.Message}");
                Console.WriteLine($"   Detalles: {ex.StackTrace}");
            }

            Console.WriteLine();
            Console.WriteLine("Presiona cualquier tecla para salir...");
            Console.ReadKey();
        }

        static void LeerArchivoDBF(string filePath)
        {
            Console.WriteLine($"📂 Leyendo archivo: {Path.GetFileName(filePath)}");
            Console.WriteLine($"   Ruta completa: {filePath}");
            Console.WriteLine();

            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (BinaryReader reader = new BinaryReader(fs, Encoding.GetEncoding(1252))) // Windows-1252 para español
            {
                // Leer encabezado DBF
                byte signature = reader.ReadByte();
                Console.WriteLine($"📋 Firma del archivo: 0x{signature:X2}");

                // Leer fecha de última modificación
                byte year = reader.ReadByte();
                byte month = reader.ReadByte();
                byte day = reader.ReadByte();
                Console.WriteLine($"📅 Última modificación: {day:00}/{month:00}/{(1900 + year)}");

                // Leer número de registros
                reader.BaseStream.Position = 4;
                uint numRecords = reader.ReadUInt32();
                Console.WriteLine($"📊 Número de registros: {numRecords}");

                // Leer tamaño del encabezado
                reader.BaseStream.Position = 8;
                ushort headerSize = reader.ReadUInt16();
                Console.WriteLine($"📏 Tamaño del encabezado: {headerSize} bytes");

                // Leer tamaño del registro
                reader.BaseStream.Position = 10;
                ushort recordSize = reader.ReadUInt16();
                Console.WriteLine($"📏 Tamaño del registro: {recordSize} bytes");

                // Calcular número de campos
                int numFields = (headerSize - 33) / 32;
                Console.WriteLine($"📋 Número de campos: {numFields}");
                Console.WriteLine();

                // Leer definición de campos
                Console.WriteLine("═══════════════════════════════════════════════════════════");
                Console.WriteLine("  ESTRUCTURA DE CAMPOS");
                Console.WriteLine("═══════════════════════════════════════════════════════════");
                Console.WriteLine();

                List<CampoDBF> campos = new List<CampoDBF>();

                reader.BaseStream.Position = 32; // Inicio de definición de campos

                for (int i = 0; i < numFields; i++)
                {
                    byte[] fieldData = reader.ReadBytes(32);
                    
                    string fieldName = Encoding.ASCII.GetString(fieldData, 0, 11).TrimEnd('\0');
                    char fieldType = (char)fieldData[11];
                    uint fieldLength = fieldData[16];
                    byte fieldDecimals = fieldData[17];

                    campos.Add(new CampoDBF
                    {
                        Nombre = fieldName,
                        Tipo = fieldType,
                        Longitud = fieldLength,
                        Decimales = fieldDecimals
                    });

                    Console.WriteLine($"  Campo {i + 1}: {fieldName}");
                    Console.WriteLine($"    Tipo: {fieldType} ({ObtenerTipoDescripcion(fieldType)})");
                    Console.WriteLine($"    Longitud: {fieldLength}");
                    if (fieldDecimals > 0)
                        Console.WriteLine($"    Decimales: {fieldDecimals}");
                    Console.WriteLine();
                }

                // Leer datos
                reader.BaseStream.Position = headerSize;

                Console.WriteLine("═══════════════════════════════════════════════════════════");
                Console.WriteLine($"  DATOS (Mostrando primeros 10 registros)");
                Console.WriteLine("═══════════════════════════════════════════════════════════");
                Console.WriteLine();

                int registrosMostrados = 0;
                int maxRegistros = Math.Min(10, (int)numRecords);

                for (int recordNum = 0; recordNum < numRecords && registrosMostrados < maxRegistros; recordNum++)
                {
                    byte deleteFlag = reader.ReadByte();
                    if (deleteFlag == 0x2A) // Registro marcado para borrar
                    {
                        reader.BaseStream.Position += recordSize - 1;
                        continue;
                    }

                    Console.WriteLine($"📝 Registro #{recordNum + 1}:");
                    foreach (var campo in campos)
                    {
                        byte[] fieldData = reader.ReadBytes((int)campo.Longitud);
                        string valor = Encoding.GetEncoding(1252).GetString(fieldData).TrimEnd();
                        
                        Console.WriteLine($"   {campo.Nombre}: {valor}");
                    }
                    Console.WriteLine();
                    registrosMostrados++;
                }

                if (numRecords > maxRegistros)
                {
                    Console.WriteLine($"... y {numRecords - maxRegistros} registros más");
                }

                // Generar script SQL de ejemplo
                Console.WriteLine();
                Console.WriteLine("═══════════════════════════════════════════════════════════");
                Console.WriteLine("  SUGERENCIA DE ESTRUCTURA SQL");
                Console.WriteLine("═══════════════════════════════════════════════════════════");
                Console.WriteLine();

                string tableName = Path.GetFileNameWithoutExtension(filePath);
                Console.WriteLine($"CREATE TABLE [dbo].[{tableName}] (");
                Console.WriteLine($"    [Id] INT IDENTITY(1,1) PRIMARY KEY,");

                foreach (var campo in campos)
                {
                    string sqlType = ConvertirTipoDBFaSQL(campo.Tipo, campo.Longitud, campo.Decimales);
                    string nullable = campo.Tipo == 'C' || campo.Tipo == 'M' ? "NULL" : "NOT NULL";
                    Console.WriteLine($"    [{campo.Nombre}] {sqlType} {nullable},");
                }

                Console.WriteLine($");");
                Console.WriteLine();
            }
        }

        static string ObtenerTipoDescripcion(char tipo)
        {
            switch (tipo)
            {
                case 'C': return "Carácter (Texto)";
                case 'N': return "Numérico";
                case 'D': return "Fecha";
                case 'L': return "Lógico (Boolean)";
                case 'M': return "Memo (Texto largo)";
                case 'F': return "Flotante";
                case 'I': return "Entero";
                case 'Y': return "Moneda";
                case 'T': return "Fecha/Hora";
                case 'B': return "Doble";
                default: return "Desconocido";
            }
        }

        static string ConvertirTipoDBFaSQL(char tipo, uint longitud, byte decimales)
        {
            switch (tipo)
            {
                case 'C':
                    return $"VARCHAR({longitud})";
                case 'N':
                    if (decimales > 0)
                        return $"DECIMAL({longitud},{decimales})";
                    else
                        return $"INT";
                case 'D':
                case 'T':
                    return "DATETIME";
                case 'L':
                    return "BIT";
                case 'M':
                    return "TEXT";
                case 'F':
                    return "FLOAT";
                case 'I':
                    return "INT";
                case 'Y':
                    return "MONEY";
                default:
                    return "VARCHAR(255)";
            }
        }
    }

    class CampoDBF
    {
        public string Nombre { get; set; }
        public char Tipo { get; set; }
        public uint Longitud { get; set; }
        public byte Decimales { get; set; }
    }
}






