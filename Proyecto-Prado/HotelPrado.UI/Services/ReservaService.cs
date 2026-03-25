using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Reservas;
using HotelPrado.Abstracciones.Modelos.Reservas;
using System.Collections.Generic;
using System.Linq;

namespace HotelPrado.UI.Services
{
    /// <summary>
    /// Servicio para manejar la lógica de negocio relacionada con reservas
    /// Sigue el principio de Responsabilidad Única (SRP)
    /// </summary>
    public class ReservaService
    {
        private readonly IReservasLN _reservasLN;
        private const string NUMERO_EMPRESA = "+50685406105";
        private const string CORREO_EMPRESA = "info@pradoinn.com";

        public ReservaService(IReservasLN reservasLN)
        {
            _reservasLN = reservasLN;
        }

        /// <summary>
        /// Obtiene las reservas de un usuario y les asigna la configuración de la empresa
        /// </summary>
        public IEnumerable<ReservasDTO> ObtenerReservasConConfiguracion(string idCliente)
        {
            var reservas = _reservasLN.ListarReservasUsuario(idCliente);
            
            // Asignar valores predeterminados (DRY - centralizado aquí)
            foreach (var reserva in reservas)
            {
                reserva.NumeroEmpresa = NUMERO_EMPRESA;
                reserva.CorreoEmpresa = CORREO_EMPRESA;
            }

            return reservas;
        }
    }
}

