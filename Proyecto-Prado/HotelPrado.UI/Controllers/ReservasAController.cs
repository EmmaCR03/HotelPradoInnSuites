using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Bitacora.Registrar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Reservas.Editar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Reservas.Listar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Reservas.ObtenerPorId;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Reservas.Registrar;
using HotelPrado.Abstracciones.Modelos.Bitacora;
using HotelPrado.Abstracciones.Modelos.Reservas;
using HotelPrado.AccesoADatos;
using HotelPrado.AccesoADatos.Reservas.Editar;
using HotelPrado.AccesoADatos.Reservas.ObtenerPorId;
using HotelPrado.AccesoADatos.Reservas.Registrar;
using HotelPrado.LN.Bitacora.Registrar;
using HotelPrado.LN.Reservas.Editar;
using HotelPrado.LN.Reservas.Listar;
using HotelPrado.LN.Reservas.ObtenerPorId;
using HotelPrado.LN.Reservas.Registrar;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace HotelPrado.UI.Controllers
{
    public class ReservasAController : Controller
    {
        IListarReservasLN _listarReservasLN;
        IRegistrarReservaLN _registrarReservaLN;
        Contexto _contexto;
        IEditarReservaLN _editarReservaLN;
        IRegistrarBitacoraEventosLN _registrarBitacoraEventosLN;
        IObtenerReservaPorIdLN _obtenerReservaPorId;

        public ReservasAController()
        {
            _listarReservasLN = new ListarReservasLN();
            _registrarReservaLN = new RegistrarReservaLN();
            _contexto = new Contexto();
            _obtenerReservaPorId = new ObtenerReservaPorIdLN();
            _editarReservaLN = new EditarReservaLN();
            _registrarBitacoraEventosLN = new RegistrarBitacoraEventosLN();
        }
        // GET: ReservasA
        public ActionResult IndexReservasA()
        {
            ViewBag.Title = "La Reserva";
            var laListaDeReserva = _listarReservasLN.Listar();

            // Refrescar datos si es necesario
            _contexto.ReservasTabla.ToList();

            return View(laListaDeReserva);
        }

        // GET: ReservasA/Create
        public ActionResult Create()
        {
            ViewBag.IdCliente = _contexto.ClienteTabla
                .Select(c => new SelectListItem
                {
                    Value = c.IdCliente.ToString(),
                    Text = c.NombreCliente
                })
                .ToList();

            return View();
        }

        // POST: ReservasA/Create
        [HttpPost]
        public async Task<ActionResult> Create(ReservasDTO modelo)
        {
            try
            {
                int cantidadDeDatosGuardados = await _registrarReservaLN.Guardar(modelo);
                return RedirectToAction("IndexReservasA");
            }
            catch
            {
                return View(modelo);
            }
        }


        // GET: ReservasA/Edit/5
        public async Task<ActionResult> Edit(int IdReserva)
        {
            var reserva = await _obtenerReservaPorId.Obtener(IdReserva);
            if (reserva == null)
            {
                return HttpNotFound();
            }
            return View(reserva);
        }

        // POST: ReservasA/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(ReservasDTO laReserva)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var cantidadDeDatosActualizados = await _editarReservaLN.Actualizar(laReserva);
                    if (cantidadDeDatosActualizados == 0)
                    {
                        ViewBag.mensaje = "Ocurrió un error inesperado, favor intente nuevamente.";
                        return View(laReserva);
                    }
                    return RedirectToAction("IndexReservasA");
                }
                catch (Exception ex)
                {
                    ViewBag.mensaje = $"Error al actualizar la reserva: {ex.Message}";
                    return View(laReserva);
                }
            }
            return View(laReserva);
        }

        // POST: ReservasA/ToggleEstado
        [HttpPost]
        public ActionResult ToggleEstado(int IdReserva)
        {
            try
            {
                var reserva = _contexto.ReservasTabla.FirstOrDefault(d => d.IdReserva == IdReserva);
                if (reserva != null)
                {
                    // Cambiar estado de la reserva
                    reserva.EstadoReserva = reserva.EstadoReserva == "Pendiente" ? "Realizado" : "Pendiente";
                    _contexto.SaveChanges(); // Guardar cambios

                    string datosJson = $@"
                    {{
                        ""IdReserva"": {reserva.IdReserva},
                        ""IdCliente"": {reserva.IdCliente},
                        ""cantidadPersonas"": ""{reserva.cantidadPersonas}"",
                        ""NombreCliente"": ""{reserva.NombreCliente}"",
                        ""IdHabitacion"": {reserva.IdHabitacion},
                        ""FechaInicio"": {reserva.FechaInicio},
                        ""FechaFinal"": ""{reserva.FechaFinal}"",
                        ""EstadoReserva"": ""{reserva.EstadoReserva}"",
                        ""MontoTotal"": {reserva.MontoTotal}
                    }}";

                    var bitacora = new BitacoraEventosDTO
                    {
                        IdEvento = 0,
                        TablaDeEvento = "ModuloReservas",
                        TipoDeEvento = "Actualizar",
                        FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                        DescripcionDeEvento = "Se actualizó el estado de una Reserva.",
                        StackTrace = "No hubo error",
                        DatosAnteriores = datosJson,
                        DatosPosteriores = datosJson
                    };

                    _registrarBitacoraEventosLN.RegistrarBitacora(bitacora);
                    return RedirectToAction("IndexReservasA");
                }

                return RedirectToAction("IndexReservasA");
            }
            catch (Exception ex)
            {
                var bitacora = new BitacoraEventosDTO
                {
                    IdEvento = 0,
                    TablaDeEvento = "ModuloReservas",
                    TipoDeEvento = "Error",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = "Error al actualizar el estado de la Reserva.",
                    StackTrace = ex.StackTrace,
                    DatosAnteriores = "NA",
                    DatosPosteriores = "NA"
                };

                _registrarBitacoraEventosLN.RegistrarBitacora(bitacora);
                return RedirectToAction("Index", "Home");
            }
        }
    }
}


