using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Habitaciones.Registrar;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Habitaciones;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.AccesoADatos.Habitacion.Registrar
{
    public class RegistrarHabitacionesAD : IRegistrarHabitacionesAD
    {
        Contexto _contexto;

        public RegistrarHabitacionesAD()
        {
            _contexto = new Contexto();
        }

        public async Task<int> Guardar(HabitacionesTabla laHabitacionAGuardar)
        {
            try
            {

                _contexto.HabitacionesTabla.Add(laHabitacionAGuardar);
                EntityState estado = _contexto.Entry(laHabitacionAGuardar).State = System.Data.Entity.EntityState.Added;
                int cantidadDeDatosAlmacenados = await _contexto.SaveChangesAsync();
                return cantidadDeDatosAlmacenados;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al guardar la habitacion:  " + ex.Message);
                return 0;
            }
        }

    }
}
