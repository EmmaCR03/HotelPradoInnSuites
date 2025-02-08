using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Colaborador.ObtenerPorId;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Colaborador.ObtenerPorId;
using HotelPrado.Abstracciones.Modelos.Colaborador;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Colaborador;
using HotelPrado.AccesoADatos.Colaborador.ObtenerPorId;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.LN.Colaborador.ObtenerPorId
{
    public class ObtenerColaboradorPorIdLN : IObtenerColaboradorPorIdLN
    {
        IObtenerColaboradorPorIdAD _obtenerporId;

        public ObtenerColaboradorPorIdLN()
        {
            _obtenerporId = new ObtenerColaboradorPorIdAD();
        }

        public async Task<ColaboradorDTO> Obtener(int IdColaborador)
        {
            // Llamada asíncrona para obtener el colaborador
            ColaboradorTabla colaboradorEnBaseDeDatos = await _obtenerporId.Obtener(IdColaborador);
            ColaboradorDTO elColaboradorAMostrar = ConvertirAColaboradorAMostrar(colaboradorEnBaseDeDatos);
            return elColaboradorAMostrar;
        }

        private ColaboradorDTO ConvertirAColaboradorAMostrar(ColaboradorTabla colaboradorEnBaseDeDatos)
        {
            return new ColaboradorDTO
            {
                IdColaborador = colaboradorEnBaseDeDatos.IdColaborador,
                NombreColaborador = colaboradorEnBaseDeDatos.NombreColaborador,
                PrimerApellidoColaborador = colaboradorEnBaseDeDatos.PrimerApellidoColaborador,
                SegundoApellidoColaborador = colaboradorEnBaseDeDatos.SegundoApellidoColaborador,
                CedulaColaborador = (int)colaboradorEnBaseDeDatos.CedulaColaborador,
                PuestoColaborador = colaboradorEnBaseDeDatos.PuestoColaborador,
                EstadoLaboral = colaboradorEnBaseDeDatos.EstadoLaboral,
                IngresoColaborador = colaboradorEnBaseDeDatos.IngresoColaborador
            };
        }
    }
}
