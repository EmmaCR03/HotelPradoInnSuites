using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Mantenimiento.Registrar;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Mantenimiento;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.AccesoADatos.Mantenimiento.Registrar
{
    public class RegistrarMantenimientoAD : IRegistrarMantenimientoAD
    {
        private readonly Contexto _contexto;

        public RegistrarMantenimientoAD()
        {
            _contexto = new Contexto();
        }

        public async Task<int> Guardar(MantenimientoTabla modelo)
        {
            try
            {
                // Agregar la entidad al contexto
                _contexto.MantenimientoTabla.Add(modelo);

                // Guardar los cambios en la base de datos
                int resultado = await _contexto.SaveChangesAsync();

                return resultado; // Retorna el número de registros afectados
            }
            catch (Exception ex)
            {
                // Manejo de errores
                Console.WriteLine($"Error al guardar: {ex.Message}");
                return 0; // Retorna 0 si no se guardó nada
            }
        }
    }
}
