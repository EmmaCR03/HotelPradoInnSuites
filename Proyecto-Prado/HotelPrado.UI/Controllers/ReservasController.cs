using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Reservas;
using HotelPrado.Abstracciones.Modelos.Reservas;
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

        public ReservasController()
        {
            _reservasLN = new ReservasLN();
        }

        
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

        public ActionResult Confirmacion()
        {
            return View();
        }


        [HttpGet]
        public ActionResult ReservasUsuario()
        {
            ViewBag.Title = "La Habitacion";
            string idCliente = User.Identity.GetUserId();
            var laListaReservasUsuario = _reservasLN.ListarReservasUsuario(idCliente);
            return View(laListaReservasUsuario);
        }


    }
}