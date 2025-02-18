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
using System.IO;
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
            _habDisponibles = new HabDisponiblesLN();
        }

        // GET: Habitacion
        public ActionResult IndexHabitaciones(int? capacidad, string estado)
        {
            ViewBag.Title = "La Habitacion";
            var laListaDeHabitaciones = _listarHabitacionesLN.Listar(capacidad, estado);
            return View(laListaDeHabitaciones);
        }

        // GET: Habitacion/IndexHabitacionesUsuario
        public ActionResult IndexHabitacionesUsuario(DateTime? check_in, DateTime? check_out, int? capacidad)
        {
            // Verifica si check_in y check_out no tienen valores, si no, asigna valores por defecto
            if (!check_in.HasValue || !check_out.HasValue)
            {
                check_in = check_in ?? DateTime.Now;
                check_out = check_out ?? DateTime.Now.AddDays(1); // La fecha de salida será el día siguiente al de entrada
            }

            // Si capacidad es nula, asigna un valor por defecto (por ejemplo, 1)
            capacidad = capacidad ?? 1;

            ViewBag.Title = "La Habitacion";
            var laListaDeHabitacionesDisponibles = _habDisponibles.ListarDisponibles(check_in.Value, check_out.Value, capacidad.Value);
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
            // Cargar los tipos de habitación desde la lógica de negocio
            var tipoHabitacion = _listarTipoHabitacionLN.Listar();  

            ViewBag.TipoHabitacion = new SelectList(tipoHabitacion, "IdTipoHabitacion", "Nombre");

            return View();
        }



        // POST: Habitacion/Create
        [HttpPost]
        public async Task<ActionResult> Create(HabitacionesDTO modeloDeHabitaciones)
        {
            var tipoHabitacion = _listarTipoHabitacionLN.Listar();
            ViewBag.TipoHabitacion = new SelectList(tipoHabitacion, "IdTipoHabitacion", "Nombre");

            if (ModelState.IsValid)
            {
                try
                {
                    List<string> rutasImagenes = new List<string>();

            
                    if (Request.Files.Count > 0)
                    {
                        foreach (string file in Request.Files)
                        {
                            var archivo = Request.Files[file];
                            if (archivo != null && archivo.ContentLength > 0)
                            {
                                string rutaImagen = Server.MapPath("~/Images/") + Path.GetFileName(archivo.FileName);

                                archivo.SaveAs(rutaImagen);

                                rutasImagenes.Add("/Images/" + Path.GetFileName(archivo.FileName));
                            }
                        }

                        modeloDeHabitaciones.ListaImagenes = rutasImagenes;
                    }

                    int cantidadDeDatosGuardados = await _registrarHabitacionesLN.Guardar(modeloDeHabitaciones);

                    if (cantidadDeDatosGuardados > 0)
                    {
                        return RedirectToAction("IndexHabitaciones");
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "No se pudo registrar la habitación. Intente nuevamente.";
                        return View(modeloDeHabitaciones);
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = "Hubo un error al registrar la habitación. Intente nuevamente.";
                    Console.WriteLine($"Error: {ex.Message}");
                    return View(modeloDeHabitaciones);
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Por favor, corrija los errores de validación.";
                return View(modeloDeHabitaciones);
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
                    return RedirectToAction("IndexHabitaciones");
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
                        TablaDeEvento = "ModuloHabitaciones",
                        TipoDeEvento = "Actualizar",
                        FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                        DescripcionDeEvento = "Se actualizó el estado de la habitacion.",
                        StackTrace = "no hubo error",
                        DatosAnteriores = datosJson,
                        DatosPosteriores = datosJson
                    };

                    _registrarBitacoraEventosLN.RegistrarBitacora(bitacora);

                    return RedirectToAction("IndexHabitaciones");
                }

                // Si no se encuentra la habitacion, redirigir a IndexHabitaciones
                return RedirectToAction("IndexHabitaciones");
            }
            catch (Exception ex)
            {
                var bitacora = new BitacoraEventosDTO
                {
                    IdEvento = 0,
                    TablaDeEvento = "ModuloHabitaciones",
                    TipoDeEvento = "Error",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = "Error al actualizar el estado de la habitacion.",
                    StackTrace = ex.StackTrace,
                    DatosAnteriores = "NA",
                    DatosPosteriores = "NA"
                };

                _registrarBitacoraEventosLN.RegistrarBitacora(bitacora);

                return RedirectToAction("IndexHabitaciones", "Habitacion");
            }
        }
    }
}
