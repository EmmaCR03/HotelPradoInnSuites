using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Mantenimiento.Registrar;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Mantenimiento;
using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace HotelPrado.AccesoADatos.Mantenimiento.Registrar
{
    public class RegistrarMantenimientoAD : IRegistrarMantenimientoAD
    {
        Contexto _contexto;

        public RegistrarMantenimientoAD()
        {
            _contexto = new Contexto();
        }

        public async Task<int> Guardar(MantenimientoTabla elMantenimientoAGuardar)
        {
            try
            {

                _contexto.MantenimientoTabla.Add(elMantenimientoAGuardar);
                EntityState estado = _contexto.Entry(elMantenimientoAGuardar).State = System.Data.Entity.EntityState.Added;
                int cantidadDeDatosAlmacenados = await _contexto.SaveChangesAsync();
                return cantidadDeDatosAlmacenados;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al guardar el Mantenimiento:  " + ex.Message);
                return 0;
            }
        }

    }
}
