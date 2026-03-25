using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Reservas.Listar;
using HotelPrado.Abstracciones.Modelos.Reservas;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace HotelPrado.AccesoADatos.Reservas.Listar
{

    public class ListarReservaAD : IListarReservaAD
    {
        Contexto _contexto;

        public ListarReservaAD()
        {
            _contexto = new Contexto();
        }

        public List<ReservasDTO> Listar()
        {
            var laListaDeReservas = new List<ReservasDTO>();
            
            using (var connection = new SqlConnection(_contexto.Database.Connection.ConnectionString))
            {
                connection.Open();
                
                // Usar SQL directo para evitar problemas de mapeo INT -> string
                string query = @"
                    SELECT 
                        r.IdReserva,
                        CAST(r.IdCliente AS NVARCHAR(50)) AS IdCliente,
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
                    ORDER BY r.FechaInicio DESC";
                
                using (var command = new SqlCommand(query, connection))
                {
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

                            laListaDeReservas.Add(new ReservasDTO
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

            return laListaDeReservas;
        }
    }
}