using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Reservas.Editar;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Reservas;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HotelPrado.AccesoADatos.Reservas.Editar
{
    public class EditarReservaAD : IEditarReservaAD
    {
        private readonly Contexto _contexto;

        public EditarReservaAD()
        {
            _contexto = new Contexto();
        }

        public async Task<int> Editar(ReservasTabla laReservaActualizar)
        {
            try
            {
                // IMPORTANTE: IdCliente en Reservas es NVARCHAR(128) que referencia a AspNetUsers.Id (GUID)
                // NO convertir a INT, usar el GUID directamente
                if (string.IsNullOrEmpty(laReservaActualizar.IdCliente))
                {
                    throw new Exception("IdCliente (GUID del usuario) es requerido.");
                }

                // Usar SQL directo para evitar problemas de mapeo
                using (var connection = new SqlConnection(_contexto.Database.Connection.ConnectionString))
                {
                    await connection.OpenAsync();
                    
                    string query = @"
                        UPDATE Reservas 
                        SET 
                            IdCliente = @IdCliente,
                            NombreCliente = @NombreCliente,
                            cantidadPersonas = @cantidadPersonas,
                            IdHabitacion = @IdHabitacion,
                            FechaInicio = @FechaInicio,
                            FechaFinal = @FechaFinal,
                            EstadoReserva = @EstadoReserva,
                            MontoTotal = @MontoTotal
                        WHERE IdReserva = @IdReserva";
                    
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@IdReserva", laReservaActualizar.IdReserva);
                        // Usar el GUID directamente, no convertir a INT
                        command.Parameters.AddWithValue("@IdCliente", laReservaActualizar.IdCliente);
                        command.Parameters.AddWithValue("@NombreCliente", (object)laReservaActualizar.NombreCliente ?? DBNull.Value);
                        command.Parameters.AddWithValue("@cantidadPersonas", laReservaActualizar.cantidadPersonas);
                        command.Parameters.AddWithValue("@IdHabitacion", laReservaActualizar.IdHabitacion);
                        command.Parameters.AddWithValue("@FechaInicio", (object)laReservaActualizar.FechaInicio ?? DBNull.Value);
                        command.Parameters.AddWithValue("@FechaFinal", (object)laReservaActualizar.FechaFinal ?? DBNull.Value);
                        command.Parameters.AddWithValue("@EstadoReserva", (object)laReservaActualizar.EstadoReserva ?? DBNull.Value);
                        command.Parameters.AddWithValue("@MontoTotal", laReservaActualizar.MontoTotal);
                        
                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected;
                    }
                }
            }
            catch (Exception ex)
            {
                // Registrar el error
                throw new Exception("Error al editar la reserva.", ex);
            }
        }
    }
}
