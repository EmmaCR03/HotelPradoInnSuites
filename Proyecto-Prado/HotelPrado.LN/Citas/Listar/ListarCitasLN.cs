using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Citas.Listar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Citas.Listar;
using HotelPrado.Abstracciones.Modelos.Citas;
using HotelPrado.AccesoADatos.Citas.Listar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.LN.Citas.Listar
{
    public class ListarCitasLN : IListarCitasLN
    {
        IListarCitasAD _listarCitasAD;

        public ListarCitasLN() 
        {
            _listarCitasAD = new ListarCitasAD();
        }
        public List<CitasDTO> Listar(int IdDepartamento)
        {
            List<CitasDTO> laListaDeCitas = _listarCitasAD.Listar(IdDepartamento);
            return laListaDeCitas;
        }


    }
}
