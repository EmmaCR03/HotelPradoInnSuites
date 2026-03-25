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
using HotelPrado.UI.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace HotelPrado.UI.Controllers
{
    [Authorize(Roles = "Administrador, Colaborador")]
    public class ReservasAController : Controller
    {
        IListarReservasLN _listarReservasLN;
        IRegistrarReservaLN _registrarReservaLN;
        Contexto _contexto;
        IEditarReservaLN _editarReservaLN;
        IRegistrarBitacoraEventosLN _registrarBitacoraEventosLN;
        IObtenerReservaPorIdLN _obtenerReservaPorId;
        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? this.HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ReservasAController()
        {
            _listarReservasLN = new ListarReservasLN();
            _registrarReservaLN = new RegistrarReservaLN();
            _contexto = new Contexto();
            _obtenerReservaPorId = new ObtenerReservaPorIdLN();
            _editarReservaLN = new EditarReservaLN();
            _registrarBitacoraEventosLN = new RegistrarBitacoraEventosLN();
        }
        // GET: ReservasA (caché 60 s para aligerar carga en host)
        [OutputCache(Duration = 60, VaryByParam = "page;pageSize", Location = System.Web.UI.OutputCacheLocation.Server)]
        public ActionResult IndexReservasA(int page = 1, int pageSize = 20)
        {
            ViewBag.Title = "La Reserva";
            
            // Obtener todas las reservas para estadísticas y conteo total
            var todasLasReservas = _listarReservasLN.Listar();
            
            // Calcular paginación
            var totalReservas = todasLasReservas.Count;
            var totalPages = (int)Math.Ceiling((double)totalReservas / pageSize);
            
            // Validar página
            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;
            
            // Obtener solo las reservas de la página actual
            var reservasPaginadas = todasLasReservas
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            
            // Pasar información de paginación a la vista
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalReservas = totalReservas;
            ViewBag.TodasLasReservas = todasLasReservas; // Para estadísticas y filtros
            
            return View(reservasPaginadas);
        }

        // GET: ReservasA/SolicitudesPendientes
        public ActionResult SolicitudesPendientes()
        {
            ViewBag.Title = "Solicitudes de Reserva Pendientes";
            // Obtener solo las reservas con estado "Solicitada"
            var solicitudesPendientes = _listarReservasLN.Listar()
                .Where(r => r.EstadoReserva == "Solicitada")
                .OrderBy(r => r.FechaInicio)
                .ToList();

            // Crear diccionario para información de contacto y detalles del cliente
            var contactos = new Dictionary<int, Dictionary<string, string>>();
            foreach (var solicitud in solicitudesPendientes)
            {
                // Verificar que IdCliente no sea null antes de buscar el usuario
                if (!string.IsNullOrEmpty(solicitud.IdCliente))
                {
                    var usuario = UserManager.FindById(solicitud.IdCliente);
                    if (usuario != null)
                    {
                        contactos[solicitud.IdReserva] = new Dictionary<string, string>
                        {
                            { "NombreCompleto", usuario.NombreCompleto ?? solicitud.NombreCliente ?? "" },
                            { "Cedula", usuario.cedula ?? "" },
                            { "Email", usuario.Email ?? "" },
                            { "Telefono", usuario.Telefono ?? "" }
                        };
                    }
                }
            }
            ViewBag.Contactos = contactos;

            return View(solicitudesPendientes);
        }

        // GET: ReservasA/ListaEspera - orden (quién entró primero), filtro por habitación y ordenamiento
        public ActionResult ListaEspera(string ordenarPor = "posicion", string filtroHabitacion = null)
        {
            ViewBag.Title = "Lista de Espera";
            ViewBag.OrdenarPor = ordenarPor ?? "posicion";
            ViewBag.FiltroHabitacion = filtroHabitacion ?? "";

            var listaEspera = _listarReservasLN.Listar()
                .Where(r => r.EstadoReserva == "En Lista de Espera")
                .ToList();

            if (!string.IsNullOrEmpty(filtroHabitacion))
            {
                int idHab;
                if (int.TryParse(filtroHabitacion, out idHab))
                    listaEspera = listaEspera.Where(r => r.IdHabitacion == idHab).ToList();
                else
                {
                    int numHab;
                    if (int.TryParse(filtroHabitacion, out numHab))
                        listaEspera = listaEspera.Where(r => r.NumeroHabitacion == numHab).ToList();
                }
            }

            switch (ordenarPor ?? "posicion")
            {
                case "habitacion":
                    listaEspera = listaEspera.OrderBy(r => r.NumeroHabitacion > 0 ? r.NumeroHabitacion : r.IdHabitacion).ThenBy(r => r.IdReserva).ToList();
                    break;
                case "fecha":
                    listaEspera = listaEspera.OrderBy(r => r.FechaInicio).ThenBy(r => r.IdReserva).ToList();
                    break;
                default:
                    // posicion: quien entró primero en lista (por IdReserva ascendente = primero en lista primero)
                    listaEspera = listaEspera.OrderBy(r => r.IdReserva).ToList();
                    break;
            }

            var habitacionesEnLista = _listarReservasLN.Listar()
                .Where(r => r.EstadoReserva == "En Lista de Espera")
                .GroupBy(r => r.IdHabitacion)
                .Select(g => new SelectListItem { Value = g.Key.ToString(), Text = "Hab. " + (g.First().NumeroHabitacion > 0 ? g.First().NumeroHabitacion.ToString() : g.Key.ToString()) })
                .OrderBy(x => x.Text)
                .ToList();
            ViewBag.HabitacionesEnLista = habitacionesEnLista;

            var contactos = new Dictionary<int, Dictionary<string, string>>();
            foreach (var reserva in listaEspera)
            {
                if (!string.IsNullOrEmpty(reserva.IdCliente))
                {
                    var usuario = UserManager.FindById(reserva.IdCliente);
                    if (usuario != null)
                    {
                        contactos[reserva.IdReserva] = new Dictionary<string, string>
                        {
                            { "NombreCompleto", usuario.NombreCompleto ?? reserva.NombreCliente ?? "" },
                            { "Cedula", usuario.cedula ?? "" },
                            { "Email", usuario.Email ?? "" },
                            { "Telefono", usuario.Telefono ?? "" }
                        };
                    }
                }
            }
            ViewBag.Contactos = contactos;

            return View(listaEspera);
        }

        // Clase auxiliar mínima para leer clientes directamente desde la tabla Cliente
        private class ClienteSimpleParaReserva
        {
            public int IdCliente { get; set; }
            public string NombreCliente { get; set; }
        }

        /// <summary>Llena ViewBag.IdCliente para el dropdown de Create (y al devolver errores de validación).</summary>
        private void CargarListaClientesParaReserva()
        {
            try
            {
                var clientes = _contexto.Database.SqlQuery<ClienteSimpleParaReserva>(
                    "SELECT IdCliente, NombreCliente FROM Cliente")
                    .ToList();
                ViewBag.IdCliente = clientes
                    .Select(c => new SelectListItem
                    {
                        Value = c.IdCliente.ToString(),
                        Text = string.IsNullOrWhiteSpace(c.NombreCliente) ? "Sin nombre" : c.NombreCliente
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error cargando clientes para ReservasA/Create: " + ex.Message);
                ViewBag.IdCliente = new List<SelectListItem>();
            }
        }

        /// <summary>Estados de reserva que bloquean la habitación (no se puede hacer otra reserva en las mismas fechas).</summary>
        private static readonly string[] EstadosQueOcupanHabitacion = { "Confirmada", "Solicitada", "Realizado", "Pendiente" };

        /// <summary>Indica si la habitación está libre en el rango de fechas (sin solapamiento con otras reservas activas).</summary>
        /// <param name="idHabitacion">ID de la habitación.</param>
        /// <param name="fechaInicio">Check-in.</param>
        /// <param name="fechaFinal">Check-out.</param>
        /// <param name="excluirIdReserva">Si se indica, se ignora esta reserva (útil al editar).</param>
        private bool HabitacionDisponibleEnRango(int idHabitacion, DateTime? fechaInicio, DateTime? fechaFinal, int? excluirIdReserva = null)
        {
            if (!fechaInicio.HasValue || !fechaFinal.HasValue || fechaInicio.Value >= fechaFinal.Value)
                return false;

            var checkIn = fechaInicio.Value;
            var checkOut = fechaFinal.Value;

            var hayConflicto = _contexto.ReservasTabla.Any(r =>
                r.IdHabitacion == idHabitacion &&
                (excluirIdReserva == null || r.IdReserva != excluirIdReserva.Value) &&
                EstadosQueOcupanHabitacion.Contains(r.EstadoReserva ?? "") &&
                r.FechaInicio.HasValue &&
                r.FechaFinal.HasValue &&
                (
                    (checkIn >= r.FechaInicio.Value && checkIn < r.FechaFinal.Value) ||
                    (checkOut > r.FechaInicio.Value && checkOut <= r.FechaFinal.Value) ||
                    (checkIn <= r.FechaInicio.Value && checkOut >= r.FechaFinal.Value)
                ));

            return !hayConflicto;
        }

        // GET: ReservasA/Create
        public ActionResult Create()
        {
            ViewBag.IdCliente = new List<SelectListItem>();
            try
            {
                CargarListaClientesParaReserva();
                var nuevoModelo = new ReservasDTO();
                return View(nuevoModelo);
            }
            catch (Exception ex)
            {
                var msg = (ex.InnerException != null ? ex.InnerException.Message + " " : "") + ex.Message;
                if (msg.Length > 350) msg = msg.Substring(0, 350) + "...";
                if (msg.IndexOf("Invalid object name", StringComparison.OrdinalIgnoreCase) >= 0
                    || msg.IndexOf("Cliente", StringComparison.OrdinalIgnoreCase) >= 0 && msg.IndexOf("exist", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    ViewBag.ErrorMessageCreate = "No se pudo cargar la lista de clientes. En el servidor verifique que exista la tabla Cliente en la base de datos.";
                }
                else if (msg.IndexOf("timeout", StringComparison.OrdinalIgnoreCase) >= 0
                    || msg.IndexOf("connection", StringComparison.OrdinalIgnoreCase) >= 0
                    || msg.IndexOf("conexión", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    ViewBag.ErrorMessageCreate = "No se pudo conectar a la base de datos. Verifique la cadena de conexión en el hosting.";
                }
                else
                {
                    ViewBag.ErrorMessageCreate = "No se pudo cargar el formulario. " + msg;
                }
                return View(new ReservasDTO());
            }
        }

        // POST: ReservasA/Create
        [HttpPost]
        public async Task<ActionResult> Create(ReservasDTO modelo)
        {
            // Validar que las fechas estén completas
            if (!modelo.FechaInicio.HasValue || !modelo.FechaFinal.HasValue)
            {
                ModelState.AddModelError("", "Debe indicar fecha de entrada y fecha de salida.");
                CargarListaClientesParaReserva();
                return View(modelo);
            }
            if (modelo.FechaInicio.Value >= modelo.FechaFinal.Value)
            {
                ModelState.AddModelError("", "La fecha de salida debe ser posterior a la fecha de entrada.");
                CargarListaClientesParaReserva();
                return View(modelo);
            }

            // Validar que la habitación no esté ocupada o con reserva en ese período
            if (!HabitacionDisponibleEnRango(modelo.IdHabitacion, modelo.FechaInicio, modelo.FechaFinal, excluirIdReserva: null))
            {
                ModelState.AddModelError("", "La habitación no está disponible en las fechas elegidas. Ya existe una reserva o check-in en ese período. Elija otras fechas o otra habitación.");
                CargarListaClientesParaReserva();
                return View(modelo);
            }

            // Calcular MontoTotal según tipo de tarifa elegido por el admin
            var tipoTarifa = Request["TipoTarifa"] ?? "General";
            var habitacionCreate = _contexto.HabitacionesTabla.FirstOrDefault(h => h.IdHabitacion == modelo.IdHabitacion);
            if (habitacionCreate != null)
            {
                int nochesCreate = (int)(modelo.FechaFinal.Value - modelo.FechaInicio.Value).TotalDays;
                if (nochesCreate < 1) nochesCreate = 1;
                modelo.MontoTotal = ObtenerPrecioNocheSegunTarifa(habitacionCreate, modelo.cantidadPersonas, tipoTarifa) * nochesCreate;
            }

            try
            {
                int cantidadDeDatosGuardados = await _registrarReservaLN.Guardar(modelo);
                return RedirectToAction("IndexReservasA");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al guardar: " + ex.Message);
                CargarListaClientesParaReserva();
                return View(modelo);
            }
        }

        /// <summary>Devuelve habitaciones disponibles en un rango de fechas (sin reservas confirmadas/solicitadas/realizado/pendiente). Para el modal de Create.</summary>
        [HttpGet]
        public ActionResult HabitacionesDisponibles(DateTime? fechaInicio, DateTime? fechaFinal)
        {
            if (!fechaInicio.HasValue || !fechaFinal.HasValue || fechaInicio.Value >= fechaFinal.Value)
            {
                return Json(new { ok = false, mensaje = "Indique fechas de entrada y salida válidas." }, JsonRequestBehavior.AllowGet);
            }
            var checkIn = fechaInicio.Value;
            var checkOut = fechaFinal.Value;
            var list = _contexto.HabitacionesTabla
                .Where(h => !_contexto.ReservasTabla.Any(r =>
                    r.IdHabitacion == h.IdHabitacion &&
                    EstadosQueOcupanHabitacion.Contains(r.EstadoReserva ?? "") &&
                    r.FechaInicio.HasValue &&
                    r.FechaFinal.HasValue &&
                    (
                        (checkIn >= r.FechaInicio.Value && checkIn < r.FechaFinal.Value) ||
                        (checkOut > r.FechaInicio.Value && checkOut <= r.FechaFinal.Value) ||
                        (checkIn <= r.FechaInicio.Value && checkOut >= r.FechaFinal.Value)
                    )))
                .Select(h => new { id = h.IdHabitacion, numero = h.NumeroHabitacion })
                .OrderBy(x => x.numero)
                .ToList();
            return Json(new { ok = true, habitaciones = list }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>Obtiene el precio por noche según tipo de tarifa (General, Gobierno, Corporativo) o por capacidad si es General y no hay PrecioGeneral.</summary>
        private static decimal ObtenerPrecioNocheSegunTarifa(HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Habitaciones.HabitacionesTabla habitacion, int cantidadPersonas, string tipoTarifa)
        {
            int capacidad = Math.Min(Math.Max(cantidadPersonas, 1), 4);
            decimal precioNoche = 0;
            var tipo = (tipoTarifa ?? "").Trim();
            if (tipo.Equals("Gobierno", StringComparison.OrdinalIgnoreCase) && habitacion.PrecioGobierno > 0)
                precioNoche = habitacion.PrecioGobierno;
            else if (tipo.Equals("Corporativo", StringComparison.OrdinalIgnoreCase) && habitacion.PrecioCorporativo > 0)
                precioNoche = habitacion.PrecioCorporativo;
            else if (habitacion.PrecioGeneral > 0)
                precioNoche = habitacion.PrecioGeneral;
            else
                precioNoche = capacidad == 1 ? habitacion.PrecioPorNoche1P : capacidad == 2 ? habitacion.PrecioPorNoche2P : capacidad == 3 ? habitacion.PrecioPorNoche3P : habitacion.PrecioPorNoche4P;
            return precioNoche;
        }

        /// <summary>
        /// Calcula el monto total estimado de una reserva para el admin. Acepta tipoTarifa: General, Gobierno, Corporativo.
        /// </summary>
        [HttpGet]
        public ActionResult CalcularMonto(int idHabitacion, int cantidadPersonas, DateTime? fechaInicio, DateTime? fechaFinal, string tipoTarifa = "General")
        {
            if (!fechaInicio.HasValue || !fechaFinal.HasValue || fechaInicio.Value >= fechaFinal.Value)
            {
                return Json(new { ok = false, mensaje = "Fechas inválidas para calcular el monto." }, JsonRequestBehavior.AllowGet);
            }

            var habitacion = _contexto.HabitacionesTabla.FirstOrDefault(h => h.IdHabitacion == idHabitacion);
            if (habitacion == null)
            {
                return Json(new { ok = false, mensaje = "Habitación no encontrada." }, JsonRequestBehavior.AllowGet);
            }

            int noches = (int)(fechaFinal.Value.Date - fechaInicio.Value.Date).TotalDays;
            if (noches <= 0) noches = 1;

            decimal precioNoche = ObtenerPrecioNocheSegunTarifa(habitacion, cantidadPersonas, tipoTarifa);
            decimal monto = precioNoche * noches;
            return Json(new { ok = true, monto, noches }, JsonRequestBehavior.AllowGet);
        }

        // GET: ReservasA/Edit/5 (incluye opción de cambiar habitación a última hora)
        public async Task<ActionResult> Edit(int IdReserva)
        {
            var reserva = await _obtenerReservaPorId.Obtener(IdReserva);
            if (reserva == null)
            {
                return HttpNotFound();
            }
            // Lista de habitaciones para cambiar: la actual + las disponibles en el rango de fechas de la reserva
            var todasHabitaciones = _contexto.HabitacionesTabla.OrderBy(h => h.NumeroHabitacion).ToList();
            var disponiblesEnRango = todasHabitaciones
                .Where(h => h.IdHabitacion == reserva.IdHabitacion || HabitacionDisponibleEnRango(h.IdHabitacion, reserva.FechaInicio, reserva.FechaFinal, IdReserva))
                .Select(h => new SelectListItem
                {
                    Value = h.IdHabitacion.ToString(),
                    Text = string.IsNullOrEmpty(h.NumeroHabitacion) ? "Habitación (Id " + h.IdHabitacion + ")" : "Habitación " + h.NumeroHabitacion,
                    Selected = h.IdHabitacion == reserva.IdHabitacion
                })
                .ToList();
            ViewBag.Habitaciones = disponiblesEnRango;
            return View(reserva);
        }

        // POST: ReservasA/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(ReservasDTO laReserva)
        {
            // Validar disponibilidad de la habitación en el rango (excluyendo esta reserva)
            if (laReserva.FechaInicio.HasValue && laReserva.FechaFinal.HasValue && laReserva.FechaInicio.Value < laReserva.FechaFinal.Value)
            {
                if (!HabitacionDisponibleEnRango(laReserva.IdHabitacion, laReserva.FechaInicio, laReserva.FechaFinal, excluirIdReserva: laReserva.IdReserva))
                {
                    ModelState.AddModelError("", "La habitación no está disponible en las fechas elegidas. Ya existe otra reserva o check-in en ese período.");
                }
            }
            else if (laReserva.FechaInicio.HasValue || laReserva.FechaFinal.HasValue)
            {
                ModelState.AddModelError("", "Debe indicar fecha de entrada y fecha de salida, y la salida debe ser posterior a la entrada.");
            }

            // Repoblar dropdown de habitaciones por si hay que devolver la vista
            var todasHabEdit = _contexto.HabitacionesTabla.OrderBy(h => h.NumeroHabitacion).ToList();
            ViewBag.Habitaciones = todasHabEdit
                .Where(h => h.IdHabitacion == laReserva.IdHabitacion || HabitacionDisponibleEnRango(h.IdHabitacion, laReserva.FechaInicio, laReserva.FechaFinal, laReserva.IdReserva))
                .Select(h => new SelectListItem { Value = h.IdHabitacion.ToString(), Text = string.IsNullOrEmpty(h.NumeroHabitacion) ? "Habitación (Id " + h.IdHabitacion + ")" : "Habitación " + h.NumeroHabitacion, Selected = h.IdHabitacion == laReserva.IdHabitacion })
                .ToList();

            if (ModelState.IsValid)
            {
                // Recalcular MontoTotal según tipo de tarifa si el admin lo eligió
                var tipoTarifaEdit = Request["TipoTarifa"] ?? "General";
                var habitacionEdit = _contexto.HabitacionesTabla.FirstOrDefault(h => h.IdHabitacion == laReserva.IdHabitacion);
                if (habitacionEdit != null && laReserva.FechaInicio.HasValue && laReserva.FechaFinal.HasValue && laReserva.FechaFinal.Value > laReserva.FechaInicio.Value)
                {
                    int nochesEdit = (int)(laReserva.FechaFinal.Value - laReserva.FechaInicio.Value).TotalDays;
                    if (nochesEdit < 1) nochesEdit = 1;
                    laReserva.MontoTotal = ObtenerPrecioNocheSegunTarifa(habitacionEdit, laReserva.cantidadPersonas, tipoTarifaEdit) * nochesEdit;
                }

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
                        ""IdCliente"": ""{reserva.IdCliente ?? "NULL"}"",
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

        // POST: ReservasA/ActualizarFechasReserva
        [HttpPost]
        public async Task<JsonResult> ActualizarFechasReserva(int IdReserva, string FechaInicio, string FechaFin)
        {
            try
            {
                // Obtener la reserva actual
                var reserva = await _obtenerReservaPorId.Obtener(IdReserva);
                if (reserva == null)
                {
                    return Json(new { success = false, message = "Reserva no encontrada" });
                }

                // Convertir fechas
                DateTime nuevaFechaInicio = DateTime.Parse(FechaInicio);
                DateTime nuevaFechaFin = DateTime.Parse(FechaFin);

                // Verificar disponibilidad (excepto para la misma reserva)
                bool disponible = VerificarDisponibilidad(reserva.IdHabitacion, nuevaFechaInicio, nuevaFechaFin, IdReserva);
                
                if (!disponible)
                {
                    return Json(new { success = false, message = "La habitación no está disponible en las nuevas fechas seleccionadas" });
                }

                // Guardar estado anterior para bitácora
                string datosAnteriores = $@"{{
                    ""IdReserva"": {reserva.IdReserva},
                    ""FechaInicio"": ""{reserva.FechaInicio}"",
                    ""FechaFin"": ""{reserva.FechaFinal}"",
                    ""IdHabitacion"": {reserva.IdHabitacion}
                }}";

                // Actualizar fechas
                reserva.FechaInicio = nuevaFechaInicio;
                reserva.FechaFinal = nuevaFechaFin;

                // Guardar cambios
                var cantidadActualizada = await _editarReservaLN.Actualizar(reserva);
                
                if (cantidadActualizada > 0)
                {
                    // Registrar en bitácora
                    string datosPosteriores = $@"{{
                        ""IdReserva"": {reserva.IdReserva},
                        ""FechaInicio"": ""{nuevaFechaInicio}"",
                        ""FechaFin"": ""{nuevaFechaFin}"",
                        ""IdHabitacion"": {reserva.IdHabitacion}
                    }}";

                    var bitacora = new BitacoraEventosDTO
                    {
                        IdEvento = 0,
                        TablaDeEvento = "ModuloReservas",
                        TipoDeEvento = "Modificación",
                        FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                        DescripcionDeEvento = "Se actualizaron las fechas de una reserva desde el calendario.",
                        StackTrace = "Sin errores",
                        DatosAnteriores = datosAnteriores,
                        DatosPosteriores = datosPosteriores,
                        Usuario = this.HttpContext.User?.Identity?.IsAuthenticated == true ? this.HttpContext.User.Identity.GetUserName() : "Sistema"
                    };

                    await _registrarBitacoraEventosLN.RegistrarBitacora(bitacora);

                    return Json(new { success = true, message = "Fechas actualizadas correctamente" });
                }
                else
                {
                    return Json(new { success = false, message = "No se pudo actualizar la reserva" });
                }
            }
            catch (Exception ex)
            {
                // Registrar error en bitácora
                var bitacora = new BitacoraEventosDTO
                {
                    IdEvento = 0,
                    TablaDeEvento = "ModuloReservas",
                    TipoDeEvento = "Error",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = "Error al actualizar fechas de reserva desde calendario: " + ex.Message,
                    StackTrace = ex.StackTrace,
                    DatosAnteriores = "NA",
                    DatosPosteriores = "NA",
                    Usuario = this.HttpContext.User?.Identity?.IsAuthenticated == true ? this.HttpContext.User.Identity.GetUserName() : "Sistema"
                };

                await _registrarBitacoraEventosLN.RegistrarBitacora(bitacora);

                return Json(new { success = false, message = "Error al actualizar: " + ex.Message });
            }
        }

        // Método auxiliar para verificar disponibilidad usando SQL directo
        private bool VerificarDisponibilidad(int idHabitacion, DateTime fechaInicio, DateTime fechaFinal, int? idReservaExcluir = null)
        {
            try
            {
                // Usar SQL directo para evitar problemas con Entity Framework
                using (var contextoLimpio = new Contexto())
                {
                    string sqlQuery = @"
                        SELECT COUNT(*) 
                        FROM Reservas 
                        WHERE IdHabitacion = @IdHabitacion
                          AND IdReserva != @IdReservaExcluir
                          AND (EstadoReserva = 'Confirmada' OR EstadoReserva = 'Solicitada')
                          AND FechaInicio IS NOT NULL
                          AND FechaFinal IS NOT NULL
                          AND (
                              (@FechaInicio >= FechaInicio AND @FechaInicio < FechaFinal) OR
                              (@FechaFinal > FechaInicio AND @FechaFinal <= FechaFinal) OR
                              (@FechaInicio <= FechaInicio AND @FechaFinal >= FechaFinal)
                          )";
                    
                    var resultado = contextoLimpio.Database.SqlQuery<int>(sqlQuery,
                        new System.Data.SqlClient.SqlParameter("@IdHabitacion", idHabitacion),
                        new System.Data.SqlClient.SqlParameter("@IdReservaExcluir", idReservaExcluir ?? 0),
                        new System.Data.SqlClient.SqlParameter("@FechaInicio", fechaInicio),
                        new System.Data.SqlClient.SqlParameter("@FechaFinal", fechaFinal)
                    ).FirstOrDefault();
                    
                    return resultado == 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al verificar disponibilidad: {ex.Message}");
                // En caso de error, asumir que está disponible para no bloquear la operación
                return true;
            }
        }

        // POST: ReservasA/AprobarSolicitud
        [HttpPost]
        public ActionResult AprobarSolicitud(int IdReserva)
        {
            System.Diagnostics.Debug.WriteLine($"=== INICIO AprobarSolicitud - IdReserva: {IdReserva} ===");
            
            // Variables que se usarán fuera del bloque using
            int idReserva = 0;
            int idHabitacion = 0;
            string estadoAnterior = "";
            string idCliente = "";
            string nombreCliente = "";
            int numeroHabitacionFinal = 0;
            
            try
            {
                // Usar un contexto completamente nuevo y separado para evitar cualquier tracking
                using (var contextoLimpio = new Contexto())
                {
                    // Deshabilitar completamente el tracking de cambios
                    contextoLimpio.Configuration.AutoDetectChangesEnabled = false;
                    contextoLimpio.Configuration.ValidateOnSaveEnabled = false;
                    
                    // Cargar reserva sin tracking usando SQL directo para evitar problemas
                    System.Diagnostics.Debug.WriteLine("Cargando reserva desde BD...");
                    var reserva = contextoLimpio.Database.SqlQuery<ReservaParaAprobar>(
                        "SELECT IdReserva, IdHabitacion, EstadoReserva, IdCliente, NombreCliente, FechaInicio, FechaFinal FROM Reservas WHERE IdReserva = @IdReserva",
                        new System.Data.SqlClient.SqlParameter("@IdReserva", IdReserva)
                    ).FirstOrDefault();
                    
                    if (reserva == null)
                    {
                        System.Diagnostics.Debug.WriteLine("ERROR: Reserva no encontrada");
                        return Json(new { success = false, message = "Reserva no encontrada" });
                    }
                    
                    System.Diagnostics.Debug.WriteLine($"Reserva encontrada - IdHabitacion: {reserva.IdHabitacion}, Estado: {reserva.EstadoReserva}");
                    
                    // Obtener datos necesarios antes de cualquier operación
                    idReserva = reserva.IdReserva;
                    idHabitacion = reserva.IdHabitacion;
                    estadoAnterior = reserva.EstadoReserva;
                    idCliente = reserva.IdCliente ?? "";
                    nombreCliente = reserva.NombreCliente ?? "";

                    // Verificar que la reserva tenga una habitación válida
                    if (idHabitacion <= 0)
                    {
                        System.Diagnostics.Debug.WriteLine("ERROR: IdHabitacion inválido");
                        return Json(new { success = false, message = "La reserva no tiene una habitación válida asignada" });
                    }

                    // Obtener la habitación usando SQL directo
                    System.Diagnostics.Debug.WriteLine($"Obteniendo habitación - IdHabitacion: {idHabitacion}");
                    var habitacion = contextoLimpio.Database.SqlQuery<HabitacionParaAprobar>(
                        "SELECT IdHabitacion, NumeroHabitacion FROM Habitaciones WHERE IdHabitacion = @IdHabitacion",
                        new System.Data.SqlClient.SqlParameter("@IdHabitacion", idHabitacion)
                    ).FirstOrDefault();
                    
                    if (habitacion == null)
                    {
                        System.Diagnostics.Debug.WriteLine("ERROR: Habitación no encontrada");
                        return Json(new { success = false, message = "La habitación asociada a la reserva no existe" });
                    }
                    
                    System.Diagnostics.Debug.WriteLine($"Habitación encontrada - NumeroHabitacion: {habitacion.NumeroHabitacion ?? "NULL"}");

                    // Establecer NumeroHabitacion SIEMPRE antes de guardar (no puede ser 0 o null)
                    numeroHabitacionFinal = idHabitacion; // Usar IdHabitacion como valor por defecto
                    
                    // Intentar obtener el número de habitación desde la tabla Habitaciones
                    if (!string.IsNullOrEmpty(habitacion.NumeroHabitacion))
                    {
                        int numeroHabitacionInt;
                        if (int.TryParse(habitacion.NumeroHabitacion, out numeroHabitacionInt) && numeroHabitacionInt > 0)
                        {
                            numeroHabitacionFinal = numeroHabitacionInt;
                        }
                    }
                    
                    System.Diagnostics.Debug.WriteLine($"NumeroHabitacionFinal calculado: {numeroHabitacionFinal}");

                    // Verificar disponibilidad antes de aprobar
                    if (reserva.FechaInicio.HasValue && reserva.FechaFinal.HasValue)
                    {
                        System.Diagnostics.Debug.WriteLine("Verificando disponibilidad...");
                        bool disponible = VerificarDisponibilidad(idHabitacion, reserva.FechaInicio.Value, reserva.FechaFinal.Value, idReserva);
                        
                        if (!disponible)
                        {
                            System.Diagnostics.Debug.WriteLine("ERROR: Habitación no disponible");
                            return Json(new { success = false, message = "La habitación no está disponible en las fechas solicitadas. Puede ponerla en lista de espera." });
                        }
                    }

                    // Usar la MISMA conexión del contexto (no abrir una segunda) para evitar "Login failed" en hosting con límite de conexiones
                    System.Diagnostics.Debug.WriteLine("Actualizando reserva con la conexión del contexto...");
                    var connection = contextoLimpio.Database.Connection;
                    if (connection.State != System.Data.ConnectionState.Open)
                        connection.Open();

                    // Verificar si la columna NumeroHabitacion existe en la tabla
                    bool columnaExiste = false;
                    try
                    {
                        string checkColumnQuery = @"
                            SELECT COUNT(*) 
                            FROM INFORMATION_SCHEMA.COLUMNS 
                            WHERE TABLE_NAME = 'Reservas' 
                              AND COLUMN_NAME = 'NumeroHabitacion'";
                        
                        using (var checkCommand = connection.CreateCommand())
                        {
                            checkCommand.CommandText = checkColumnQuery;
                            int count = (int)checkCommand.ExecuteScalar();
                            columnaExiste = count > 0;
                            System.Diagnostics.Debug.WriteLine($"Columna NumeroHabitacion existe: {columnaExiste}");
                        }
                    }
                    catch (Exception exCheck)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error al verificar columna: {exCheck.Message}");
                        columnaExiste = false;
                    }
                    
                    // Actualizar NumeroHabitacion y Estado en una sola consulta
                    string updateQuery = @"UPDATE Reservas SET EstadoReserva = @EstadoReserva";
                    if (columnaExiste)
                    {
                        updateQuery += @", NumeroHabitacion = @NumeroHabitacion";
                        System.Diagnostics.Debug.WriteLine($"Actualizando NumeroHabitacion a: {numeroHabitacionFinal}");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Columna NumeroHabitacion no existe, solo actualizando EstadoReserva");
                    }
                    updateQuery += @" WHERE IdReserva = @IdReserva";
                    
                    System.Diagnostics.Debug.WriteLine($"Query SQL: {updateQuery}");
                    using (var updateCommand = connection.CreateCommand())
                    {
                        updateCommand.CommandText = updateQuery;
                        var pEstado = updateCommand.CreateParameter();
                        pEstado.ParameterName = "@EstadoReserva";
                        pEstado.Value = "Confirmada";
                        updateCommand.Parameters.Add(pEstado);
                        var pId = updateCommand.CreateParameter();
                        pId.ParameterName = "@IdReserva";
                        pId.Value = idReserva;
                        updateCommand.Parameters.Add(pId);
                        if (columnaExiste)
                        {
                            var pNum = updateCommand.CreateParameter();
                            pNum.ParameterName = "@NumeroHabitacion";
                            pNum.Value = numeroHabitacionFinal;
                            updateCommand.Parameters.Add(pNum);
                        }
                        
                        int rowsAffected = updateCommand.ExecuteNonQuery();
                        System.Diagnostics.Debug.WriteLine($"Filas afectadas: {rowsAffected}");
                        
                        if (rowsAffected == 0)
                        {
                            System.Diagnostics.Debug.WriteLine("ERROR: No se pudo actualizar la reserva");
                            return Json(new { success = false, message = "No se pudo actualizar la reserva" });
                        }
                    }
                    
                    System.Diagnostics.Debug.WriteLine("Reserva actualizada exitosamente con SQL directo");
                }

                // Registrar en bitácora usando los datos guardados
                System.Diagnostics.Debug.WriteLine("Registrando en bitácora...");
                string datosJson = $@"
                {{
                    ""IdReserva"": {idReserva},
                    ""EstadoAnterior"": ""{estadoAnterior}"",
                    ""EstadoNuevo"": ""Confirmada"",
                    ""IdCliente"": ""{idCliente}"",
                    ""NombreCliente"": ""{nombreCliente}"",
                    ""IdHabitacion"": {idHabitacion}
                }}";

                var bitacora = new BitacoraEventosDTO
                {
                    IdEvento = 0,
                    TablaDeEvento = "ModuloReservas",
                    TipoDeEvento = "Aprobar",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = "Se aprobó una solicitud de reserva.",
                    StackTrace = "No hubo error",
                    DatosAnteriores = datosJson,
                    DatosPosteriores = datosJson
                };

                try
                {
                    _registrarBitacoraEventosLN.RegistrarBitacora(bitacora);
                    System.Diagnostics.Debug.WriteLine("Bitácora registrada exitosamente");
                }
                catch (Exception exBitacora)
                {
                    System.Diagnostics.Debug.WriteLine($"ERROR al registrar bitácora: {exBitacora.Message}");
                    // Continuar aunque falle la bitácora
                }

                System.Diagnostics.Debug.WriteLine("=== FIN AprobarSolicitud - ÉXITO ===");
                return Json(new { success = true, message = "Solicitud aprobada exitosamente" });
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                System.Diagnostics.Debug.WriteLine($"=== ERROR DbEntityValidationException ===");
                System.Diagnostics.Debug.WriteLine($"Mensaje: {dbEx.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {dbEx.StackTrace}");
                
                // Capturar errores de validación de Entity Framework
                var errores = new List<string>();
                foreach (var validationError in dbEx.EntityValidationErrors)
                {
                    foreach (var error in validationError.ValidationErrors)
                    {
                        System.Diagnostics.Debug.WriteLine($"  - {error.PropertyName}: {error.ErrorMessage}");
                        errores.Add($"{error.PropertyName}: {error.ErrorMessage}");
                    }
                }
                
                var bitacora = new BitacoraEventosDTO
                {
                    IdEvento = 0,
                    TablaDeEvento = "ModuloReservas",
                    TipoDeEvento = "Error",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = "Error de validación al aprobar la solicitud de reserva: " + string.Join("; ", errores),
                    StackTrace = dbEx.StackTrace ?? "Sin stack trace",
                    DatosAnteriores = "NA",
                    DatosPosteriores = "NA"
                };

                try
                {
                    _registrarBitacoraEventosLN.RegistrarBitacora(bitacora);
                }
                catch
                {
                    // Si falla el registro en bitácora, continuar
                }
                
                return Json(new { success = false, message = "Error de validación al aprobar la solicitud: " + string.Join("; ", errores) });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"=== ERROR Exception ===");
                System.Diagnostics.Debug.WriteLine($"Tipo: {ex.GetType().Name}");
                System.Diagnostics.Debug.WriteLine($"Mensaje: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"InnerException: {ex.InnerException.Message}");
                    System.Diagnostics.Debug.WriteLine($"InnerException StackTrace: {ex.InnerException.StackTrace}");
                }
                
                var bitacora = new BitacoraEventosDTO
                {
                    IdEvento = 0,
                    TablaDeEvento = "ModuloReservas",
                    TipoDeEvento = "Error",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = $"Error al aprobar la solicitud de reserva: {ex.GetType().Name} - {ex.Message ?? "Error desconocido"}",
                    StackTrace = ex.StackTrace ?? "Sin stack trace",
                    DatosAnteriores = "NA",
                    DatosPosteriores = "NA"
                };

                try
                {
                    _registrarBitacoraEventosLN.RegistrarBitacora(bitacora);
                }
                catch
                {
                    // Si falla el registro en bitácora, continuar sin interrumpir
                }
                
                var mensaje = ex.Message ?? "Error desconocido.";
                if (ex is System.Data.SqlClient.SqlException sqlEx && (sqlEx.Message ?? "").Contains("Login failed"))
                    mensaje = "Error de conexión a la base de datos (Login failed). Compruebe en el hosting que el usuario de SQL tenga acceso y que la cadena de conexión en Web.config sea correcta. Si acaba de funcionar en otra pantalla, intente aprobar de nuevo.";
                return Json(new { success = false, message = $"Error al aprobar la solicitud: {mensaje}" });
            }
        }
        
        // Clases auxiliares para SQL directo
        private class ReservaParaAprobar
        {
            public int IdReserva { get; set; }
            public int IdHabitacion { get; set; }
            public string EstadoReserva { get; set; }
            public string IdCliente { get; set; }
            public string NombreCliente { get; set; }
            public DateTime? FechaInicio { get; set; }
            public DateTime? FechaFinal { get; set; }
        }
        
        private class HabitacionParaAprobar
        {
            public int IdHabitacion { get; set; }
            public string NumeroHabitacion { get; set; }
        }

        // POST: ReservasA/RechazarSolicitud
        [HttpPost]
        public ActionResult RechazarSolicitud(int IdReserva, string motivo = "")
        {
            try
            {
                var reserva = _contexto.ReservasTabla.FirstOrDefault(r => r.IdReserva == IdReserva);
                if (reserva == null)
                {
                    return Json(new { success = false, message = "Reserva no encontrada" });
                }

                string estadoAnterior = reserva.EstadoReserva;

                // Cambiar estado a "Rechazada"
                reserva.EstadoReserva = "Rechazada";
                _contexto.SaveChanges();

                // Registrar en bitácora
                string datosJson = $@"
                {{
                    ""IdReserva"": {reserva.IdReserva},
                    ""EstadoAnterior"": ""{estadoAnterior}"",
                    ""EstadoNuevo"": ""Rechazada"",
                    ""Motivo"": ""{motivo}"",
                    ""IdCliente"": ""{reserva.IdCliente ?? "NULL"}"",
                    ""NombreCliente"": ""{reserva.NombreCliente ?? ""}""
                }}";

                var bitacora = new BitacoraEventosDTO
                {
                    IdEvento = 0,
                    TablaDeEvento = "ModuloReservas",
                    TipoDeEvento = "Rechazar",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = "Se rechazó una solicitud de reserva.",
                    StackTrace = "No hubo error",
                    DatosAnteriores = datosJson,
                    DatosPosteriores = datosJson
                };

                _registrarBitacoraEventosLN.RegistrarBitacora(bitacora);

                return Json(new { success = true, message = "Solicitud rechazada" });
            }
            catch (Exception ex)
            {
                var bitacora = new BitacoraEventosDTO
                {
                    IdEvento = 0,
                    TablaDeEvento = "ModuloReservas",
                    TipoDeEvento = "Error",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = "Error al rechazar la solicitud de reserva.",
                    StackTrace = ex.StackTrace,
                    DatosAnteriores = "NA",
                    DatosPosteriores = "NA"
                };

                _registrarBitacoraEventosLN.RegistrarBitacora(bitacora);
                return Json(new { success = false, message = "Error al rechazar la solicitud: " + ex.Message });
            }
        }

        // POST: ReservasA/PonerEnListaEspera
        [HttpPost]
        public ActionResult PonerEnListaEspera(int IdReserva)
        {
            System.Diagnostics.Debug.WriteLine($"=== INICIO PonerEnListaEspera - IdReserva: {IdReserva} ===");
            
            // Variables que se usarán fuera del bloque using
            int idReserva = 0;
            int idHabitacion = 0;
            string estadoAnterior = "";
            string idCliente = "";
            string nombreCliente = "";
            
            try
            {
                // Usar un contexto completamente nuevo y separado para evitar cualquier tracking
                using (var contextoLimpio = new Contexto())
                {
                    // Deshabilitar completamente el tracking de cambios
                    contextoLimpio.Configuration.AutoDetectChangesEnabled = false;
                    contextoLimpio.Configuration.ValidateOnSaveEnabled = false;
                    
                    // Cargar reserva sin tracking usando SQL directo
                    System.Diagnostics.Debug.WriteLine("Cargando reserva desde BD...");
                    var reserva = contextoLimpio.Database.SqlQuery<ReservaParaAprobar>(
                        "SELECT IdReserva, IdHabitacion, EstadoReserva, IdCliente, NombreCliente, FechaInicio, FechaFinal FROM Reservas WHERE IdReserva = @IdReserva",
                        new System.Data.SqlClient.SqlParameter("@IdReserva", IdReserva)
                    ).FirstOrDefault();
                    
                    if (reserva == null)
                    {
                        System.Diagnostics.Debug.WriteLine("ERROR: Reserva no encontrada");
                        return Json(new { success = false, message = "Reserva no encontrada" });
                    }
                    
                    System.Diagnostics.Debug.WriteLine($"Reserva encontrada - Estado: {reserva.EstadoReserva}");
                    
                    // Obtener datos necesarios antes de cualquier operación
                    idReserva = reserva.IdReserva;
                    idHabitacion = reserva.IdHabitacion;
                    estadoAnterior = reserva.EstadoReserva;
                    idCliente = reserva.IdCliente ?? "";
                    nombreCliente = reserva.NombreCliente ?? "";

                    // Usar la MISMA conexión del contexto (igual que en AprobarSolicitud)
                    // para evitar problemas de login/conexiones extra en el hosting
                    System.Diagnostics.Debug.WriteLine("Actualizando reserva con la conexión del contexto (Lista de Espera)...");
                    var connection = contextoLimpio.Database.Connection;
                    if (connection.State != System.Data.ConnectionState.Open)
                        connection.Open();

                    string updateQuery = @"UPDATE Reservas SET EstadoReserva = @EstadoReserva WHERE IdReserva = @IdReserva";

                    System.Diagnostics.Debug.WriteLine($"Query SQL: {updateQuery}");
                    System.Diagnostics.Debug.WriteLine($"Parámetros - IdReserva: {idReserva}, EstadoReserva: En Lista de Espera");

                    using (var updateCommand = connection.CreateCommand())
                    {
                        updateCommand.CommandText = updateQuery;

                        var pEstado = updateCommand.CreateParameter();
                        pEstado.ParameterName = "@EstadoReserva";
                        pEstado.Value = "En Lista de Espera";
                        updateCommand.Parameters.Add(pEstado);

                        var pId = updateCommand.CreateParameter();
                        pId.ParameterName = "@IdReserva";
                        pId.Value = idReserva;
                        updateCommand.Parameters.Add(pId);

                        int rowsAffected = updateCommand.ExecuteNonQuery();
                        System.Diagnostics.Debug.WriteLine($"Filas afectadas: {rowsAffected}");

                        if (rowsAffected == 0)
                        {
                            System.Diagnostics.Debug.WriteLine("ERROR: No se pudo actualizar la reserva");
                            return Json(new { success = false, message = "No se pudo actualizar la reserva" });
                        }
                    }

                    System.Diagnostics.Debug.WriteLine("Reserva actualizada exitosamente (Lista de Espera) con SQL directo");
                }

                // Registrar en bitácora usando los datos guardados
                System.Diagnostics.Debug.WriteLine("Registrando en bitácora...");
                string datosJson = $@"
                {{
                    ""IdReserva"": {idReserva},
                    ""EstadoAnterior"": ""{estadoAnterior}"",
                    ""EstadoNuevo"": ""En Lista de Espera"",
                    ""IdCliente"": ""{idCliente}"",
                    ""NombreCliente"": ""{nombreCliente}"",
                    ""IdHabitacion"": {idHabitacion}
                }}";

                var bitacora = new BitacoraEventosDTO
                {
                    IdEvento = 0,
                    TablaDeEvento = "ModuloReservas",
                    TipoDeEvento = "ListaEspera",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = "Se puso una solicitud de reserva en lista de espera.",
                    StackTrace = "No hubo error",
                    DatosAnteriores = datosJson,
                    DatosPosteriores = datosJson
                };

                try
                {
                    _registrarBitacoraEventosLN.RegistrarBitacora(bitacora);
                    System.Diagnostics.Debug.WriteLine("Bitácora registrada exitosamente");
                }
                catch (Exception exBitacora)
                {
                    System.Diagnostics.Debug.WriteLine($"ERROR al registrar bitácora: {exBitacora.Message}");
                    // Si falla el registro en bitácora, continuar
                }
                
                System.Diagnostics.Debug.WriteLine("=== FIN PonerEnListaEspera - ÉXITO ===");
                return Json(new { success = true, message = "Solicitud puesta en lista de espera" });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR General: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                
                var bitacora = new BitacoraEventosDTO
                {
                    IdEvento = 0,
                    TablaDeEvento = "ModuloReservas",
                    TipoDeEvento = "Error",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = "Error al poner la solicitud en lista de espera: " + (ex.Message ?? "Error desconocido"),
                    StackTrace = ex.StackTrace ?? "Sin stack trace",
                    DatosAnteriores = "NA",
                    DatosPosteriores = "NA"
                };

                try
                {
                    _registrarBitacoraEventosLN.RegistrarBitacora(bitacora);
                }
                catch
                {
                    // Si falla el registro en bitácora, continuar sin interrumpir
                }
                
                System.Diagnostics.Debug.WriteLine("=== FIN PonerEnListaEspera - ERROR ===");
                return Json(new { success = false, message = "Error al poner la solicitud en lista de espera: " + (ex.Message ?? "Error desconocido") });
            }
        }
    }
}



