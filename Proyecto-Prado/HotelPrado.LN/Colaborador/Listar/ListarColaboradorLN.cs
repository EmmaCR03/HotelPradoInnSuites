using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Colaborador.Listar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Colaborador.Listar;
using HotelPrado.Abstracciones.Modelos.Colaborador;
using HotelPrado.AccesoADatos.Colaborador.Listar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.LN.Colaborador.Listar
{
    public class ListarColaboradorLN : IListarColaboradorLN
    {
        IListarColaboradorAD _listarColaboradorAD;

        public ListarColaboradorLN()
        {
            _listarColaboradorAD = new ListarColaboradorAD();
        }

        public List<ColaboradorDTO> Listar()
        {
            List<ColaboradorDTO> laListaDeColaborador = _listarColaboradorAD.Listar();

            return laListaDeColaborador;


        }
    }
}
