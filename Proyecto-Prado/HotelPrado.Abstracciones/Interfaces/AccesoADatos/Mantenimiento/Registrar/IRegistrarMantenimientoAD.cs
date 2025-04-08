using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Mantenimiento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.AccesoADatos.Mantenimiento.Registrar
{
    public interface IRegistrarMantenimientoAD
    {
        Task<int> Guardar(MantenimientoTabla elMantenimientoAGuardar);
    }
}
