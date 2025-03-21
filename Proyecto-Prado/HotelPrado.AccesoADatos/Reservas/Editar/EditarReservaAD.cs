using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Reservas.Editar;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Reservas;
using System;
using System.Data.Entity;
using System.Linq;
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
                // Buscar el colaborador en la base de datos
                var laReservaEnBaseDeDatos = await _contexto.ReservasTabla
                    .Where(c => c.IdReserva == laReservaActualizar.IdReserva)
                    .FirstOrDefaultAsync();


                // Actualizar los campos con los nuevos valores
                laReservaEnBaseDeDatos.cantidadPersonas = laReservaActualizar.cantidadPersonas;
                laReservaEnBaseDeDatos.NombreCliente = laReservaActualizar.NombreCliente;
                laReservaEnBaseDeDatos.IdHabitacion = laReservaActualizar.IdHabitacion;
                laReservaEnBaseDeDatos.FechaInicio = laReservaActualizar.FechaInicio;
                laReservaEnBaseDeDatos.FechaFinal = laReservaActualizar.FechaFinal;
                laReservaEnBaseDeDatos.EstadoReserva = laReservaActualizar.EstadoReserva;
                laReservaEnBaseDeDatos.MontoTotal = laReservaActualizar.MontoTotal;

                // Cambiar el estado de la entidad a "Modified" para que Entity Framework realice el seguimiento del cambio
                _contexto.Entry(laReservaEnBaseDeDatos).State = EntityState.Modified;

                // Guardar los cambios en la base de datos
                int cantidadDeDatosAlmacenados = await _contexto.SaveChangesAsync();

                // Devolver la cantidad de registros actualizados
                return cantidadDeDatosAlmacenados;
            }
            catch (Exception ex)
            {
                // Registrar el error
                throw new Exception("Error al editar la reserva.", ex);
            }
        }
    }
}
