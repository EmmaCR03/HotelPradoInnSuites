using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Citas.ObtenerEnlaces;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Reservas;
using HotelPrado.Abstracciones.Modelos.Reservas;
using HotelPrado.LN.Citas.Enlaces;
using HotelPrado.LN.Reservas;
using Microsoft.AspNet.Identity;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace HotelPrado.UI.Controllers
{
    public class ReservasController : Controller
    {
        private readonly IReservasLN _reservasLN;
        private readonly IObtenerEnlacesLN _obtenerEnlaces;

        public ReservasController()
        {
            _reservasLN = new ReservasLN();
            _obtenerEnlaces = new ObtenerEnlacesLN();
        }

        [Authorize]
        [HttpGet]
        public ActionResult ConfirmarReserva(int id, DateTime checkIn, DateTime checkOut, decimal totalPrecio, int cantidadPersonas)
        {
            var model = new ReservasDTO
            {
                IdHabitacion = id,
                FechaInicio = checkIn,
                FechaFinal = checkOut,
                cantidadPersonas = cantidadPersonas,
                MontoTotal = totalPrecio
            };
            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmarReserva(ReservasDTO model)
        {
            if (ModelState.IsValid)
            {
                model.IdCliente = User.Identity.GetUserId();
                model.EstadoReserva = "Pendiente";
                model.NombreCliente = User.Identity.GetUserName();

                int resultado = await _reservasLN.CrearReservasUsuario(model);
                if (resultado > 0)
                {
                    return RedirectToAction("Confirmacion");
                }
                ModelState.AddModelError("", "No se pudo crear la reserva. Inténtelo de nuevo.");
            }
            return View(model);
        }
        [Authorize]
        public ActionResult Confirmacion()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult ReservasUsuario()
        {
            ViewBag.Title = "Mis Reservas";

            // Obtener el ID del usuario actual
            string idCliente = User.Identity.GetUserId();

            // Obtener la lista de reservas del usuario
            var laListaReservasUsuario = _reservasLN.ListarReservasUsuario(idCliente);

            // Asignar valores predeterminados a cada reserva
            foreach (var reserva in laListaReservasUsuario)
            {
                reserva.NumeroEmpresa = "+50685406105"; // Número de la empresa
                reserva.CorreoEmpresa = "info@pradoinn.com"; // Correo de la empresa
            }

            return View(laListaReservasUsuario); // Pasar la lista de reservas a la vista
        }

    }
}