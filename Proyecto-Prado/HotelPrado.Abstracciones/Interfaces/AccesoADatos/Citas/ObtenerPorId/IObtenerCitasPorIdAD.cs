using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Citas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.AccesoADatos.Citas.ObtenerPorId
{
    public interface IObtenerCitasPorIdAD
    {
        CitasTabla Obtener(int IdCita);
    }
}
