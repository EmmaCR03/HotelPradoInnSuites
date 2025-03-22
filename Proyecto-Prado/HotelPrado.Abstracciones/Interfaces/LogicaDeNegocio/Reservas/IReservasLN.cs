using HotelPrado.Abstracciones.Modelos.Reservas;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Habitaciones;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Reservas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Reservas
{
    public interface IReservasLN
    {
        Task<int> CrearReservasUsuario(ReservasDTO reserva);
        
        List<ReservasDTO> ListarReservasUsuario(string IdUsuario);

        ReservasTabla Convertir(ReservasDTO laReserva);

    }
}
