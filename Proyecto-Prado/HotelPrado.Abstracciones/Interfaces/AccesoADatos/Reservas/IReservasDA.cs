using HotelPrado.Abstracciones.Modelos.Reservas;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Reservas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.AccesoADatos.Reservas
{
    public interface IReservasDA
    {
      
        Task<int> CrearReservaUsuario(ReservasTabla laReservaAGuardar,int IdHabitacion);

        List<ReservasDTO> ObtenerReservasPorUsuario(string IdUsuario);



    }
}
