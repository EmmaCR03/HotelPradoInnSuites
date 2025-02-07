using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Colaborador.Listar;
using HotelPrado.Abstracciones.Modelos.Colaborador;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.AccesoADatos.Colaborador.Listar
{

    public class ListarColaboradorAD : IListarColaboradorAD
    {
        Contexto _contexto;

        public ListarColaboradorAD()
        {
            _contexto = new Contexto();
        }

        public List<ColaboradorDTO> Listar()
        {
            var laListaDeColaboradores = (from elColaborador in _contexto.ColaboradorTabla
                                          select new ColaboradorDTO
                                          {
                                              IdColaborador = elColaborador.IdColaborador,
                                              NombreColaborador = elColaborador.NombreColaborador,
                                              PrimerApellidoColaborador = elColaborador.PrimerApellidoColaborador,
                                              SegundoApellidoColaborador = elColaborador.SegundoApellidoColaborador,
                                              CedulaColaborador = (int)elColaborador.CedulaColaborador,
                                              PuestoColaborador = elColaborador.PuestoColaborador,
                                              EstadoLaboral = elColaborador.EstadoLaboral,
                                              IngresoColaborador = elColaborador.IngresoColaborador,
                                          }).ToList();


            return laListaDeColaboradores;
        }
    }
}