using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Reservas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.AccesoADatos.Reservas.Registrar
{
    public interface IRegistrarReservaAD
    {
        Task<int> Guardar(ReservasTabla laReservaAGuardar);
    }
}
