using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Citas.Registrar;
using HotelPrado.Abstracciones.Modelos.Citas;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Citas;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.AccesoADatos.Citas.Registrar
{
    public class RegistrarCitasAD : IRegistrarCitasAD
    {
        Contexto _contexto;

        public RegistrarCitasAD() 
        {
            _contexto = new Contexto();
        }
        public async Task<int> Guardar(CitasTabla citas)
        {
            try
            {

                _contexto.CitasTabla.Add(citas);
                EntityState estado = _contexto.Entry(citas).State = System.Data.Entity.EntityState.Added;
                int cantidadDeDatosAlmacenados = await _contexto.SaveChangesAsync();
                return cantidadDeDatosAlmacenados;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al guardar registrar la cita:  " + ex.Message);
                return 0;
            }
        }

    }
}

