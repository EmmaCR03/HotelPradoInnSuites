using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Habitaciones.Editar;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Habitaciones;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.AccesoADatos.Habitacion.Editar
{
    public class EditarHabitacionesAD : IEditarHabitacionesAD
    {
        Contexto _contexto;

        public EditarHabitacionesAD()
        {
            _contexto = new Contexto();
        }

        public async Task<int> Editar(HabitacionesTabla laHabitacionActualizar)
        {
            HabitacionesTabla laHabitacionEnBaseDeDatos = _contexto.HabitacionesTabla
                .Where(laHabitacion => laHabitacion.IdHabitacion == laHabitacionActualizar.IdHabitacion)
                .FirstOrDefault();
            laHabitacionEnBaseDeDatos.NumeroHabitacion = laHabitacionActualizar.NumeroHabitacion;
            laHabitacionEnBaseDeDatos.PrecioPorNoche1P = laHabitacionActualizar.PrecioPorNoche1P;
            laHabitacionEnBaseDeDatos.PrecioPorNoche2P = laHabitacionActualizar.PrecioPorNoche2P;
            laHabitacionEnBaseDeDatos.PrecioPorNoche3P = laHabitacionActualizar.PrecioPorNoche3P;
            laHabitacionEnBaseDeDatos.PrecioPorNoche4P = laHabitacionActualizar.PrecioPorNoche4P;
            laHabitacionEnBaseDeDatos.IdTipoHabitacion = laHabitacionActualizar.IdTipoHabitacion;
            laHabitacionEnBaseDeDatos.CapacidadMax = laHabitacionActualizar.CapacidadMax;
            laHabitacionEnBaseDeDatos.CapacidadMin = laHabitacionActualizar.CapacidadMin;
            laHabitacionEnBaseDeDatos.Estado = laHabitacionActualizar.Estado;
            laHabitacionEnBaseDeDatos.CapacidadMax = laHabitacionActualizar.CapacidadMax;
            laHabitacionEnBaseDeDatos.CapacidadMin = laHabitacionActualizar.CapacidadMin;
            EntityState estado = _contexto.Entry(laHabitacionEnBaseDeDatos).State = System.Data.Entity.EntityState.Modified;
            int cantidadDeDatosAlmacenados = await _contexto.SaveChangesAsync();
            return cantidadDeDatosAlmacenados;
        }
    }
}

