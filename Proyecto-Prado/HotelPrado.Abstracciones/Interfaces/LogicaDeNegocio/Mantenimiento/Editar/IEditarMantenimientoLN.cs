using HotelPrado.Abstracciones.Modelos.Mantenimiento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Mantenimiento.Editar
{
    public interface IEditarMantenimientoLN
    {
        Task<int> Actualizar(MantenimientoDTO elMantenimientoEnVista);
    }
}
