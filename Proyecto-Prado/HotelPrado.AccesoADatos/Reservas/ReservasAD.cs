using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Reservas;
using HotelPrado.Abstracciones.Modelos.Reservas;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Reservas;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace HotelPrado.AccesoADatos.Reservas
{
    public class ReservasAD : IReservasDA
    {
        Contexto _contexto;

        public ReservasAD()
        {
            _contexto = new Contexto();
        }



        public async Task<int> CrearReservaUsuario(ReservasTabla laReservaAGuardar, int IdHabitacion)
        {
            laReservaAGuardar.IdHabitacion = IdHabitacion;

            // IMPORTANTE: IdCliente en Reservas es NVARCHAR(128) que referencia a AspNetUsers.Id (GUID)
            // NO convertir a INT, usar el GUID directamente
            if (string.IsNullOrEmpty(laReservaAGuardar.IdCliente))
            {
                throw new Exception("IdCliente (GUID del usuario) es requerido.");
            }

            // Usar SQL directo para evitar problemas de mapeo
            using (var connection = new SqlConnection(_contexto.Database.Connection.ConnectionString))
            {
                await connection.OpenAsync();
                
                string query = @"
                    INSERT INTO Reservas 
                    (IdCliente, NombreCliente, cantidadPersonas, IdHabitacion, FechaInicio, FechaFinal, EstadoReserva, MontoTotal)
                    VALUES 
                    (@IdCliente, @NombreCliente, @cantidadPersonas, @IdHabitacion, @FechaInicio, @FechaFinal, @EstadoReserva, @MontoTotal);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";
                
                using (var command = new SqlCommand(query, connection))
                {
                    // Usar el GUID directamente, no convertir a INT
                    command.Parameters.AddWithValue("@IdCliente", laReservaAGuardar.IdCliente);
                    command.Parameters.AddWithValue("@NombreCliente", (object)laReservaAGuardar.NombreCliente ?? DBNull.Value);
                    command.Parameters.AddWithValue("@cantidadPersonas", laReservaAGuardar.cantidadPersonas);
                    command.Parameters.AddWithValue("@IdHabitacion", laReservaAGuardar.IdHabitacion);
                    command.Parameters.AddWithValue("@FechaInicio", (object)laReservaAGuardar.FechaInicio ?? DBNull.Value);
                    command.Parameters.AddWithValue("@FechaFinal", (object)laReservaAGuardar.FechaFinal ?? DBNull.Value);
                    command.Parameters.AddWithValue("@EstadoReserva", (object)laReservaAGuardar.EstadoReserva ?? DBNull.Value);
                    command.Parameters.AddWithValue("@MontoTotal", laReservaAGuardar.MontoTotal);
                    
                    var result = await command.ExecuteScalarAsync();
                    if (result != null)
                    {
                        return 1; // Reserva creada exitosamente
                    }
                }
            }
            
            return 0;
        }





        public List<ReservasDTO> ObtenerReservasPorUsuario(string IdUsuario)
        {
            // IdCliente en Reservas es NVARCHAR(128) que referencia directamente a AspNetUsers.Id
            // Por lo tanto, podemos usar IdUsuario directamente sin pasar por la tabla Cliente
            if (string.IsNullOrEmpty(IdUsuario))
            {
                return new List<ReservasDTO>();
            }

            // Usar SQL directo para evitar problemas de mapeo
            var resultado = new List<ReservasDTO>();
            
            using (var connection = new SqlConnection(_contexto.Database.Connection.ConnectionString))
            {
                connection.Open();
                
                string query = @"
                    SELECT 
                        r.IdReserva,
                        r.IdCliente,
                        r.NombreCliente,
                        r.cantidadPersonas,
                        r.IdHabitacion,
                        ISNULL(h.NumeroHabitacion, '') AS NumeroHabitacion,
                        r.FechaInicio,
                        r.FechaFinal,
                        r.EstadoReserva,
                        r.MontoTotal
                    FROM Reservas r
                    LEFT JOIN Habitaciones h ON r.IdHabitacion = h.IdHabitacion
                    WHERE r.IdCliente = @IdUsuario
                    ORDER BY r.FechaInicio DESC";
                
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdUsuario", IdUsuario);
                    
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int numeroHabitacion = 0;
                            string numeroHabitacionStr = reader.IsDBNull(reader.GetOrdinal("NumeroHabitacion")) 
                                ? "" 
                                : reader.GetString(reader.GetOrdinal("NumeroHabitacion"));
                            
                            if (!string.IsNullOrEmpty(numeroHabitacionStr))
                            {
                                int.TryParse(numeroHabitacionStr, out numeroHabitacion);
                            }
                            else
                            {
                                numeroHabitacion = reader.GetInt32(reader.GetOrdinal("IdHabitacion"));
                            }

                            resultado.Add(new ReservasDTO
                            {
                                IdReserva = reader.GetInt32(reader.GetOrdinal("IdReserva")),
                                IdCliente = reader.IsDBNull(reader.GetOrdinal("IdCliente")) 
                                    ? null 
                                    : reader.GetString(reader.GetOrdinal("IdCliente")),
                                NombreCliente = reader.IsDBNull(reader.GetOrdinal("NombreCliente")) 
                                    ? null 
                                    : reader.GetString(reader.GetOrdinal("NombreCliente")),
                                cantidadPersonas = reader.GetInt32(reader.GetOrdinal("cantidadPersonas")),
                                IdHabitacion = reader.GetInt32(reader.GetOrdinal("IdHabitacion")),
                                NumeroHabitacion = numeroHabitacion,
                                FechaInicio = reader.IsDBNull(reader.GetOrdinal("FechaInicio")) 
                                    ? (DateTime?)null 
                                    : reader.GetDateTime(reader.GetOrdinal("FechaInicio")),
                                FechaFinal = reader.IsDBNull(reader.GetOrdinal("FechaFinal")) 
                                    ? (DateTime?)null 
                                    : reader.GetDateTime(reader.GetOrdinal("FechaFinal")),
                                EstadoReserva = reader.IsDBNull(reader.GetOrdinal("EstadoReserva")) 
                                    ? null 
                                    : reader.GetString(reader.GetOrdinal("EstadoReserva")),
                                MontoTotal = reader.GetDecimal(reader.GetOrdinal("MontoTotal"))
                            });
                        }
                    }
                }
            }

            return resultado;
        }

    }
}