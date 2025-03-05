using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Mantenimiento.Listar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Mantenimiento.Listar;
using HotelPrado.Abstracciones.Modelos.Mantenimiento;
using HotelPrado.AccesoADatos.Mantenimiento.Listar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.LN.Mantenimiento.Listar
{
    public class ListarMantenimientoLN : IListarMantenimientoLN
    {
        IListarMantenimientoAD _listarMantenimientoAD;

        public ListarMantenimientoLN()
        {
            _listarMantenimientoAD = new ListarMantenimientoAD();
        }
        public List<MantenimientoDTO> Listar(int IdMantenimiento)
        {
            List<MantenimientoDTO> laListaDeMantenimiento = _listarMantenimientoAD.Listar(IdMantenimiento);
            return laListaDeMantenimiento;
        }


    }
}

