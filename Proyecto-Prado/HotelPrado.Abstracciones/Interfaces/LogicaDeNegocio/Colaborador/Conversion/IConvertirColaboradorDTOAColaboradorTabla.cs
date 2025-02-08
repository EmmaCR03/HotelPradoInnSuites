using HotelPrado.Abstracciones.Modelos.Colaborador;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Colaborador;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Colaborador.Conversion
{
    public interface IConvertirColaboradorDTOAColaboradorTabla
    {
        ColaboradorTabla Convertir(ColaboradorDTO elColaborador);

    }
}
