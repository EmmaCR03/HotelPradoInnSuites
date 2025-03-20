using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Reservas;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Reservas;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Linq;
using System.Text;
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

        public async Task<int> Crear(ReservasTabla laReservaAGuardar)
        {
            _contexto.ReservasTabla.Add(laReservaAGuardar);
            EntityState estado = _contexto.Entry(laReservaAGuardar)
                .State = System.Data.Entity.EntityState.Added;
            int cantidadDeDatosAlmacenados = await _contexto.SaveChangesAsync();
            return cantidadDeDatosAlmacenados;
        }

        public async Task<int> CrearReservaUsuario(ReservasTabla laReservaAGuardar, int IdHabitacion)
        {
            laReservaAGuardar.IdHabitacion = IdHabitacion;

            // Agregar la reserva al contexto
            _contexto.ReservasTabla.Add(laReservaAGuardar);

            // Guardar los cambios en la base de datos
            int cantidadDeDatosAlmacenados = await _contexto.SaveChangesAsync();
            return cantidadDeDatosAlmacenados;
        }


             public async Task<int> Editar(ReservasTabla laReservaActualizar)
        {
            var reservaExistente = await _contexto.ReservasTabla
                .FirstOrDefaultAsync(r => r.IdReserva == laReservaActualizar.IdReserva);

            if (reservaExistente != null)
            {
                // Validar que las nuevas fechas no interfieran con otras reservas de la misma habitación
                bool hayConflicto = _contexto.ReservasTabla
                    .Any(r => r.IdHabitacion == laReservaActualizar.IdHabitacion &&
                              r.IdReserva != laReservaActualizar.IdReserva &&
                              ((laReservaActualizar.FechaInicio >= r.FechaInicio && laReservaActualizar.FechaInicio <= r.FechaFinal) ||
                               (laReservaActualizar.FechaFinal >= r.FechaInicio && laReservaActualizar.FechaFinal <= r.FechaFinal)));

                if (hayConflicto)
                {
                    throw new InvalidOperationException("Las fechas seleccionadas interfieren con otra reserva existente.");
                }

                // Actualizar solo las fechas y la cantidad de personas
                reservaExistente.FechaInicio = laReservaActualizar.FechaInicio;
                reservaExistente.FechaFinal = laReservaActualizar.FechaFinal;
                reservaExistente.cantidadPersonas = laReservaActualizar.cantidadPersonas;

                // Calcular el monto total automáticamente
                var habitacion = await _contexto.HabitacionesTabla
                    .FirstOrDefaultAsync(h => h.IdHabitacion == laReservaActualizar.IdHabitacion);

                if (habitacion != null && laReservaActualizar.FechaInicio.HasValue && laReservaActualizar.FechaFinal.HasValue)
                {
                    int totalNoches = (laReservaActualizar.FechaFinal.Value - laReservaActualizar.FechaInicio.Value).Days;
                    decimal precioPorNoche = 0;

                    if (laReservaActualizar.cantidadPersonas == 1)
                    {
                        precioPorNoche = habitacion.PrecioPorNoche1P;
                    }
                    else if (laReservaActualizar.cantidadPersonas == 2)
                    {
                        precioPorNoche = habitacion.PrecioPorNoche2P;
                    }
                    else if (laReservaActualizar.cantidadPersonas == 3)
                    {
                        precioPorNoche = habitacion.PrecioPorNoche3P;
                    }
                    else if (laReservaActualizar.cantidadPersonas == 4)
                    {
                        precioPorNoche = habitacion.PrecioPorNoche4P;
                    }

                    reservaExistente.MontoTotal = totalNoches * precioPorNoche;
                }

                _contexto.Entry(reservaExistente).State = EntityState.Modified;
                return await _contexto.SaveChangesAsync();
            }

            return 0;
        }

        public async Task<int> Eliminar(int IdReserva)
        {
            var reservaExistente = await _contexto.ReservasTabla
                .FirstOrDefaultAsync(r => r.IdReserva == IdReserva);

            if (reservaExistente != null)
            {
                _contexto.ReservasTabla.Remove(reservaExistente);
                return await _contexto.SaveChangesAsync();
            }

            return 0;
        }

        public ReservasTabla Obtener(int IdReserva)
        {
            return _contexto.ReservasTabla
                .Include(r => r.Habitacion)
                .Include(r => r.Cliente)
                .FirstOrDefault(r => r.IdReserva == IdReserva);
        }

        public List<ReservasTabla> ObtenerReservas()
        {
            return _contexto.ReservasTabla
                .Include(r => r.Habitacion)
                .Include(r => r.Cliente)
                .ToList();
        }

        public List<ReservasTabla> ObtenerReservasPorUsuario(string IdUsuario)
        {
            return _contexto.ReservasTabla
                .Include(r => r.Habitacion)
                .Include(r => r.Cliente)
                .Where(r => r.IdCliente == IdUsuario)
                .ToList();
        }
    }
}


