using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Bitacora.Registrar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Habitaciones.Editar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Habitaciones.HabDisponibles;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Habitaciones.Listar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Habitaciones.ObtenerPorId;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Habitaciones.Registrar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.TipoHabitacion.Listar;
using HotelPrado.Abstracciones.Modelos.Bitacora;
using HotelPrado.Abstracciones.Modelos.Habitaciones;
using HotelPrado.AccesoADatos;
using HotelPrado.LN.Bitacora.Registrar;
using HotelPrado.LN.Habitaciones.Editar;
using HotelPrado.LN.Habitaciones.HabDisponibles;
using HotelPrado.LN.Habitaciones.Listar;
using HotelPrado.LN.Habitaciones.ObtenerPorId;
using HotelPrado.LN.Habitaciones.Registrar;
using HotelPrado.LN.TipoHabitacion.Listar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace HotelPrado.UI.Controllers
{
    public class HabitacionController : Controller
    {
        IListarHabitacionesLN _listarHabitacionesLN;
        IHabDisponiblesLN _habDisponiblesLN;
        IListarTipoHabitacionLN _listarTipoHabitacionLN;
        IRegistrarHabitacionesLN _registrarHabitacionesLN;
        Contexto _contexto;
        IEditarHabitacionesLN _editarHabitacionesLN;
        IRegistrarBitacoraEventosLN _registrarBitacoraEventosLN;
        IObtenerHabitacionesPorIdLN _obtenerHabitacionesPorId;
        IHabDisponiblesLN _habDisponibles;

        public HabitacionController()
        {
            _listarHabitacionesLN = new ListarHabitacionesLN();
            _listarTipoHabitacionLN = new ListarTipoHabitacionLN();
            _registrarHabitacionesLN = new RegistrarHabitacionesLN();
            _contexto = new Contexto();
            _obtenerHabitacionesPorId = new ObtenerHabitacionesPorIdLN();
            _editarHabitacionesLN = new EditarHabitacionesLN();
            _registrarBitacoraEventosLN = new RegistrarBitacoraEventosLN();
            _habDisponibles=new HabDisponiblesLN();
        }
        // GET: Habitacion
        public ActionResult IndexHabitaciones(int? capacidad,string estado)
        {
            ViewBag.Title = "La Habitacion";
            var laListaDeHabitaciones = _listarHabitacionesLN.Listar(capacidad,estado);
            return View(laListaDeHabitaciones);
        }

        public ActionResult IndexHabitacionesUsuario(DateTime check_in, DateTime check_out, int capacidad)
        {
            ViewBag.Title = "La Habitacion";
            var laListaDeHabitacionesDisponibles = _habDisponibles.ListarDisponibles(check_in, check_out,capacidad);
            return View(laListaDeHabitacionesDisponibles);
        }

        // GET: Habitacion/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Habitacion/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Habitacion/Create
        [HttpPost]
        public async Task<ActionResult> Create(HabitacionesDTO modeloDeHabitaciones)
        {
            try
            {
                int cantidadDeDatosGuardados = await _registrarHabitacionesLN.Guardar(modeloDeHabitaciones);
                return RedirectToAction("IndexHabitaciones");
            }
            catch
            {
                return View();
            }
        }

        // GET: Habitacion/Edit/5
        public ActionResult Edit(int id)
        {
            var datosHabitacion = _obtenerHabitacionesPorId.Obtener(id);

            ViewBag.Estados = new List<SelectListItem>
    {
        new SelectListItem { Text = "Disponible", Value = "Disponible" },
        new SelectListItem { Text = "En Mantenimiento", Value = "En Mantenimiento" },
        new SelectListItem { Text = "Ocupado", Value = "Ocupado" }
    };
            return View(datosHabitacion);
        }

        // POST: Habitacion/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(HabitacionesDTO laHabitacion)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    var cantidadDeDatosActualizados = await _editarHabitacionesLN.Actualizar(laHabitacion);
                    if (cantidadDeDatosActualizados == 0)
                    {
                        ViewBag.mensaje = "Ocurrió un error inesperado, favor intente nuevamente.";
                        return View(laHabitacion);
                    }
                    return RedirectToAction("IndexColaborador");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al actualizar: {ex.Message}");
                    ViewBag.mensaje = "Ocurrió un error inesperado, favor intente nuevamente.";
                    return View(laHabitacion);
                }
            }
            return View(laHabitacion);
        }

        // GET: Habitacion/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Habitacion/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        [HttpPost]

        public ActionResult ToggleEstado(int IdHabitacion, string Estado)
        {
            try
            {
                var Habitacion = _contexto.HabitacionesTabla.FirstOrDefault(d => d.IdHabitacion == IdHabitacion);
                if (Habitacion != null)
                {
                    // Actualiza el estado con el valor recibido del formulario
                    Habitacion.Estado = Estado;
                    _contexto.SaveChanges();
                    string datosJson = $@"
            {{
                    ""IdHabitacion"": {Habitacion.IdHabitacion},
                    ""NumeroHabitacion"": {Habitacion.NumeroHabitacion},
                    ""PrecioPorNoche1P"": ""{Habitacion.PrecioPorNoche1P}"",
                    ""PrecioPorNoche2P"": ""{Habitacion.PrecioPorNoche2P}"",
                    ""PrecioPorNoche3P"": ""{Habitacion.PrecioPorNoche3P}"",
                    ""PrecioPorNoche4P"": ""{Habitacion.PrecioPorNoche4P}"",
                    ""IdTipoHabitacion"": ""{Habitacion.IdTipoHabitacion}"",
                    ""Capacidad"": ""{Habitacion.Capacidad}"",
                    ""Estado"": ""{Habitacion.Estado}""
            }}";

                    var bitacora = new BitacoraEventosDTO
                    {
                        IdEvento = 0,
                        TablaDeEvento = "ModuloColaborador",
                        TipoDeEvento = "Actualizar",
                        FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                        DescripcionDeEvento = "Se actualizó el estado de un Colaborador.",
                        StackTrace = "no hubo error",
                        DatosAnteriores = datosJson,
                        DatosPosteriores = datosJson
                    };

                    _registrarBitacoraEventosLN.RegistrarBitacora(bitacora);

                    return RedirectToAction("IndexColaborador");
                }

                // Si no se encuentra el Colaborador, redirigir a IndexColaborador
                return RedirectToAction("IndexColaborador");
            }
            catch (Exception ex)
            {
                var bitacora = new BitacoraEventosDTO
                {
                    IdEvento = 0,
                    TablaDeEvento = "ModuloColaborador",
                    TipoDeEvento = "Error",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = "Error al actualizar el estado del Colaborador.",
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
