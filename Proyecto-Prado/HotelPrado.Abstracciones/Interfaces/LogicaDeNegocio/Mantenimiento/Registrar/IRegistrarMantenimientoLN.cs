using HotelPrado.Abstracciones.Modelos.Mantenimiento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Mantenimiento.Registrar
{
    public interface IRegistrarMantenimientoLN
    {
        Task<int> Guardar(MantenimientoDTO modelo);
    }
}
