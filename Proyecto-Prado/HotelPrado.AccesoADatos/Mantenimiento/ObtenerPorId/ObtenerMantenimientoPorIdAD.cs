using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Mantenimiento.ObtenerPorId;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Mantenimiento;
using System;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Linq;

namespace HotelPrado.AccesoADatos.Mantenimiento.ObtenerPorId
{
    public class ObtenerMantenimientoPorIdAD : IObtenerMantenimientoPorIdAD
    {
        private readonly Contexto _contexto;

        public ObtenerMantenimientoPorIdAD()
        {
            _contexto = new Contexto();
        }

        public async Task<MantenimientoTabla> Obtener(int IdMantenimiento)
        {
            using (var connection = new SqlConnection(_contexto.Database.Connection.ConnectionString))
            {
                await connection.OpenAsync();
                
                // Verificar qué columnas existen en la tabla Mantenimiento
                var columnasExistentes = new System.Collections.Generic.List<string>();
                string checkColumnsQuery = @"
                    SELECT COLUMN_NAME 
                    FROM INFORMATION_SCHEMA.COLUMNS 
                    WHERE TABLE_NAME = 'Mantenimiento'";
                
                using (var checkCommand = new SqlCommand(checkColumnsQuery, connection))
                {
                    using (var reader = await checkCommand.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            columnasExistentes.Add(reader.GetString(0));
                        }
                    }
                }
                
                // Verificar si existe alguna columna relacionada con habitación
                bool existeColumnaHabitacion = columnasExistentes.Any(c => 
                    c.Equals("IdHabitacion", System.StringComparison.OrdinalIgnoreCase) || 
                    c.Equals("idHabitacion", System.StringComparison.OrdinalIgnoreCase));
                
                string nombreColumnaHabitacion = columnasExistentes.FirstOrDefault(c => 
                    c.Equals("IdHabitacion", System.StringComparison.OrdinalIgnoreCase) || 
                    c.Equals("idHabitacion", System.StringComparison.OrdinalIgnoreCase));
                
                // Construir la consulta SQL dinámicamente
                string query;
                if (existeColumnaHabitacion && !string.IsNullOrEmpty(nombreColumnaHabitacion))
                {
                    query = $@"
                        SELECT 
                            IdMantenimiento,
                            Descripcion,
                            Estado,
                            idDepartamento,
                            DepartamentoNombre,
                            [{nombreColumnaHabitacion}] as idHabitacion
                        FROM Mantenimiento
                        WHERE IdMantenimiento = @IdMantenimiento";
                }
                else
                {
                    query = @"
                        SELECT 
                            IdMantenimiento,
                            Descripcion,
                            Estado,
                            idDepartamento,
                            DepartamentoNombre,
                            0 as idHabitacion
                        FROM Mantenimiento
                        WHERE IdMantenimiento = @IdMantenimiento";
                }
                
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdMantenimiento", IdMantenimiento);
                    
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new MantenimientoTabla
                            {
                                IdMantenimiento = reader.GetInt32(reader.GetOrdinal("IdMantenimiento")),
                                Descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion")) ? null : reader.GetString(reader.GetOrdinal("Descripcion")),
                                Estado = reader.IsDBNull(reader.GetOrdinal("Estado")) ? null : reader.GetString(reader.GetOrdinal("Estado")),
                                idDepartamento = reader.IsDBNull(reader.GetOrdinal("idDepartamento")) ? 0 : reader.GetInt32(reader.GetOrdinal("idDepartamento")),
                                DepartamentoNombre = reader.IsDBNull(reader.GetOrdinal("DepartamentoNombre")) ? null : reader.GetString(reader.GetOrdinal("DepartamentoNombre")),
                                idHabitacion = reader.IsDBNull(reader.GetOrdinal("idHabitacion")) ? 0 : reader.GetInt32(reader.GetOrdinal("idHabitacion"))
                            };
                        }
                    }
                }
            }
            
            return null;
        }

        public async Task<bool> ActualizarMantenimiento(MantenimientoTabla mantenimiento)
        {
            using (var connection = new SqlConnection(_contexto.Database.Connection.ConnectionString))
            {
                await connection.OpenAsync();
                
                // Verificar qué columnas existen en la tabla Mantenimiento
                var columnasExistentes = new System.Collections.Generic.List<string>();
                string checkColumnsQuery = @"
                    SELECT COLUMN_NAME 
                    FROM INFORMATION_SCHEMA.COLUMNS 
                    WHERE TABLE_NAME = 'Mantenimiento'";
                
                using (var checkCommand = new SqlCommand(checkColumnsQuery, connection))
                {
                    using (var reader = await checkCommand.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            columnasExistentes.Add(reader.GetString(0));
                        }
                    }
                }
                
                // Construir la consulta UPDATE dinámicamente (sin incluir idHabitacion si no existe)
                string updateQuery = @"
                    UPDATE Mantenimiento 
                    SET Descripcion = @Descripcion,
                        Estado = @Estado,
                        idDepartamento = @idDepartamento,
                        DepartamentoNombre = @DepartamentoNombre
                    WHERE IdMantenimiento = @IdMantenimiento";
                
                using (var command = new SqlCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue("@IdMantenimiento", mantenimiento.IdMantenimiento);
                    command.Parameters.AddWithValue("@Descripcion", (object)mantenimiento.Descripcion ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Estado", (object)mantenimiento.Estado ?? DBNull.Value);
                    command.Parameters.AddWithValue("@idDepartamento", mantenimiento.idDepartamento);
                    command.Parameters.AddWithValue("@DepartamentoNombre", (object)mantenimiento.DepartamentoNombre ?? DBNull.Value);
                    
                    int filasAfectadas = await command.ExecuteNonQueryAsync();
                    return filasAfectadas > 0;
                }
            }
        }
    }
}
