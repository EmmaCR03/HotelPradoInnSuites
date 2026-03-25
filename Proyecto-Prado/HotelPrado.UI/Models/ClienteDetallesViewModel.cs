using HotelPrado.Abstracciones.Modelos.Cliente;
using System;
using System.Collections.Generic;

namespace HotelPrado.UI.Models
{
    /// <summary>
    /// Modelo para mostrar el detalle de un cliente: datos básicos, empresa asociada y su historial de visitas.
    /// </summary>
    public class ClienteDetallesViewModel
    {
        public ClienteDTO Cliente { get; set; }

        // Información de empresa asociada (si aplica)
        public string NombreEmpresa { get; set; }
        public string TelefonoEmpresa { get; set; }
        public string CorreoEmpresa { get; set; }
        public string DireccionEmpresa { get; set; }

        // Empresa sugerida a partir del último check-in (si el cliente no tiene IdEmpresa)
        public int? EmpresaSugeridaId { get; set; }
        public string EmpresaSugeridaNombre { get; set; }

        // Historial de visitas (reservas) del cliente
        public List<HistorialClienteViewModel> Historial { get; set; }

        // Última visita (calculada desde el historial)
        public DateTime? UltimaVisita { get; set; }
    }
}

