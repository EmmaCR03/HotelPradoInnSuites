using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Citas.ObtenerEnlaces;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Citas.ObtenerEnlaces;
using HotelPrado.Abstracciones.Modelos.Citas;
using HotelPrado.AccesoADatos;
using HotelPrado.AccesoADatos.Citas.ObtenerEnlaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.LN.Citas.Enlaces
{
    public class ObtenerEnlacesLN : IObtenerEnlacesLN
    {
        IObtenerEnlacesAD _obtenerEnlaces;

        public ObtenerEnlacesLN()
        {
            _obtenerEnlaces = new ObtenerEnlacesAD();
        }

        public List<CitasDTO> ObtenerCitasConEnlaces()
        {
            var citas = _obtenerEnlaces.ObtenerCitas();

            return citas.Select(cita => new CitasDTO
            {
                IdCita = cita.IdCita,
                Estado = cita.Estado,
                Telefono = cita.Telefono,
                Correo = cita.Correo,

                // Verifica que Telefono tenga 8 dígitos y agrega 506 si es necesario
                EnlaceWhatsApp = (cita.Telefono != 0 && cita.Telefono.ToString().Length == 8)
    ? $"https://wa.me/506{cita.Telefono}?text={Uri.EscapeDataString($"Hola {cita.Nombre}, espero que estés teniendo un excelente día. Me comunico contigo para coordinar la asignación de tu cita. ¿Podrías indicarme qué fecha y hora te convienen? ¡Quedo atento a tu respuesta!")}"
    : null,


                // Genera el enlace de correo correctamente
                EnlaceCorreo = !string.IsNullOrEmpty(cita.Correo)
                    ? $"mailto:{cita.Correo}?subject=Consulta%20Cita&body=Hola,%20quiero%20consultar%20sobre%20mi%20cita"
                    : null
            }).ToList();
        }



    }
}