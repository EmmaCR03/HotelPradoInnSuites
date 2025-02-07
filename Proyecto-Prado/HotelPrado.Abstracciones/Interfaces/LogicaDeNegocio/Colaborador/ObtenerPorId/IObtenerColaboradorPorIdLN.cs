using HotelPrado.Abstracciones.Modelos.Colaborador;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Colaborador.ObtenerPorId
{
    public interface IObtenerColaboradorPorIdLN
    {
        ColaboradorDTO Obtener(int IdColaborador);
    }
}
