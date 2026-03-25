using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Reservas.ObtenerPorId;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Reservas;
using System;
using System.Data.SqlClient;

namespace HotelPrado.AccesoADatos.Reservas.ObtenerPorId
{
    public class ObtenerReservaPorIdAD : IObtenerReservaPorIdAD
    {
        Contexto _contexto;

        public ObtenerReservaPorIdAD()
        {
            _contexto = new Contexto();
        }

        public ReservasTabla Obtener(int IdReserva)
        {
            using (var connection = new SqlConnection(_contexto.Database.Connection.ConnectionString))
            {
                connection.Open();
                
                string query = @"
                    SELECT 
                        IdReserva,
                        CAST(IdCliente AS NVARCHAR(50)) AS IdCliente,
                        NombreCliente,
                        cantidadPersonas,
                        IdHabitacion,
                        NumeroHabitacion,
                        FechaInicio,
                        FechaFinal,
                        EstadoReserva,
                        MontoTotal,
                        NumeroEmpresa,
                        CorreoEmpresa
                    FROM Reservas
                    WHERE IdReserva = @IdReserva";
                
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdReserva", IdReserva);
                    
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int idxIdReserva = reader.GetOrdinal("IdReserva");
                            int idxCantidad = reader.GetOrdinal("cantidadPersonas");
                            int idxIdHabitacion = reader.GetOrdinal("IdHabitacion");
                            int idxNumeroHab = reader.GetOrdinal("NumeroHabitacion");
                            int idxMonto = reader.GetOrdinal("MontoTotal");

                            return new ReservasTabla
                            {
                                IdReserva = reader.GetInt32(idxIdReserva),
                                IdCliente = reader.IsDBNull(reader.GetOrdinal("IdCliente")) ? null : reader.GetString(reader.GetOrdinal("IdCliente")),
                                NombreCliente = reader.IsDBNull(reader.GetOrdinal("NombreCliente")) ? null : reader.GetString(reader.GetOrdinal("NombreCliente")),
                                cantidadPersonas = reader.IsDBNull(idxCantidad) ? 0 : reader.GetInt32(idxCantidad),
                                IdHabitacion = reader.IsDBNull(idxIdHabitacion) ? 0 : reader.GetInt32(idxIdHabitacion),
                                NumeroHabitacion = reader.IsDBNull(idxNumeroHab) ? 0 : reader.GetInt32(idxNumeroHab),
                                FechaInicio = reader.IsDBNull(reader.GetOrdinal("FechaInicio")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("FechaInicio")),
                                FechaFinal = reader.IsDBNull(reader.GetOrdinal("FechaFinal")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("FechaFinal")),
                                EstadoReserva = reader.IsDBNull(reader.GetOrdinal("EstadoReserva")) ? null : reader.GetString(reader.GetOrdinal("EstadoReserva")),
                                MontoTotal = reader.IsDBNull(idxMonto) ? 0m : reader.GetDecimal(idxMonto),
                                NumeroEmpresa = reader.IsDBNull(reader.GetOrdinal("NumeroEmpresa")) ? null : reader.GetString(reader.GetOrdinal("NumeroEmpresa")),
                                CorreoEmpresa = reader.IsDBNull(reader.GetOrdinal("CorreoEmpresa")) ? null : reader.GetString(reader.GetOrdinal("CorreoEmpresa"))
                            };
                        }
                    }
                }
            }
            
            return null;
        }

    }
}