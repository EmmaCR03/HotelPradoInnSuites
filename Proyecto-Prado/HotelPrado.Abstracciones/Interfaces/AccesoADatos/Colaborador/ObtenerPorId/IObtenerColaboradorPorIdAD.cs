using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Colaborador;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.AccesoADatos.Colaborador.ObtenerPorId
{
    public interface IObtenerColaboradorPorIdAD
    {
        ColaboradorTabla Obtener(int IdColaborador);
    }
}
