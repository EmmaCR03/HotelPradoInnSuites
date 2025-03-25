using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Bitacora.Registrar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Mantenimiento.Editar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Mantenimiento.Listar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Mantenimiento.ObtenerPorId;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Mantenimiento.Registrar;
using HotelPrado.Abstracciones.Modelos.Bitacora;
using HotelPrado.Abstracciones.Modelos.Citas;
using HotelPrado.Abstracciones.Modelos.Mantenimiento;
using HotelPrado.AccesoADatos;
using HotelPrado.LN.Bitacora.Registrar;
using HotelPrado.LN.Mantenimiento.Editar;
using HotelPrado.LN.Mantenimiento.Listar;
using HotelPrado.LN.Mantenimiento.ObtenerPorId;
using HotelPrado.LN.Mantenimiento.Registrar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace HotelPrado.UI.Controllers
{
    public class MantenimientoController : Controller
    {
        private readonly IListarMantenimientoLN _listarMantenimientoLN;
        private readonly IRegistrarMantenimientoLN _registrarMantenimientoLN;
        private readonly IEditarMantenimientoLN _editarMantenimientoLN;
        private readonly Contexto _contexto;
        private readonly IObtenerMantenimientoPorIdLN _obtenerMantenimientoPorId;
        private readonly IRegistrarBitacoraEventosLN _registrarBitacoraEventosLN;

        public MantenimientoController()
        {
            _registrarMantenimientoLN = new RegistrarMantenimientoLN();
            _listarMantenimientoLN = new ListarMantenimientoLN();
            _editarMantenimientoLN = new EditarMantenimientoLN();
            _contexto = new Contexto();
            _obtenerMantenimientoPorId = new ObtenerMantenimientoPorIdLN();
            _registrarBitacoraEventosLN = new RegistrarBitacoraEventosLN();
        }

        // GET: Mantenimiento
        public ActionResult IndexMantenimiento(int? Id)
        {
            ViewBag.Title = "El Mantenimiento";
            var laListaDeMantenimiento = _listarMantenimientoLN.Listar(Id ?? 0);
            return View(laListaDeMantenimiento);
        }

        // GET: Mantenimiento/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }
        // GET: Mantenimiento/Create
        public ActionResult Create()
        {
            ViewBag.Titulo = "Nuevo Mantenimiento";
            return View();
        }


        // GET: Mantenimiento/CreateHabitacion/5
        public ActionResult CreateHabitacion(int id)
        {
            var modelo = new MantenimientoDTO
            {
                idHabitacion = id,
                Estado = "Pendiente"
            };

            ViewBag.Titulo = "Mantenimiento para Habitación";
            return View("Create", modelo);
        }

        // POST: Mantenimiento/Create
        [HttpPost]
        public async Task<ActionResult> Create(MantenimientoDTO modelo)
        {
            try
            {
                await _registrarMantenimientoLN.Guardar(modelo);
                TempData["SuccessMessage"] = "Mantenimiento creado!";

                return RedirectToAction("IndexMantenimiento");
            }
            catch
            {
                return View(modelo);
            }
        }


        // GET: Mantenimiento/Edit/5
        public ActionResult Edit(int IdMantenimiento)
        {
            // Obtener el Mantenimiento desde la base de datos
            var mantenimiento = _obtenerMantenimientoPorId.Obtener(IdMantenimiento);

            if (mantenimiento == null)
            {
                return HttpNotFound();
            }

            // Lista de estados predefinidos (fuertemente tipada)
            var estadoDb = new List<SelectListItem>
            {
                new SelectListItem { Text = "Pendiente", Value = "Pendiente" },
                new SelectListItem { Text = "Realizado", Value = "Realizado" }
            };

            // Asignar la lista a ViewBag
            ViewBag.Estado = new SelectList(estadoDb, "Value", "Text", mantenimiento.Estado);

            // Retornar la vista con el mantenimiento obtenido
            return View(mantenimiento);
        }

        // POST: Mantenimiento/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(MantenimientoDTO elmantenimiento)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var cantidadDeDatosActualizados = await _editarMantenimientoLN.Actualizar(elmantenimiento);
                    if (cantidadDeDatosActualizados == 0)
                    {
                        ViewBag.mensaje = "Ocurrió un error inesperado, favor intente nuevamente.";
                        return View(elmantenimiento);
                    }
                    return RedirectToAction("IndexMantenimiento");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al actualizar: {ex.Message}");
                    ViewBag.mensaje = "Ocurrió un error inesperado, favor intente nuevamente.";
                    return View(elmantenimiento);
                }
            }
            return View(elmantenimiento);
        }

        // GET: Mantenimiento/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Mantenimiento/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                return RedirectToAction("IndexMantenimiento");
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult ToggleEstado(int IdMantenimiento, string Estado)
        {
            try
            {
                var mantenimiento = _contexto.MantenimientoTabla.FirstOrDefault(d => d.IdMantenimiento == IdMantenimiento);
                if (mantenimiento != null)
                {
                    // Actualiza el estado con el valor recibido del formulario
                    mantenimiento.Estado = Estado;
                    _contexto.SaveChanges();

                    string datosJson = $@"
            {{
                    ""IdMantenimiento"": {mantenimiento.IdMantenimiento},
                    ""Descripcion"": ""{mantenimiento.Descripcion}"",
                    ""Estado"": ""{mantenimiento.Estado}""
            }}";

                    var bitacora = new BitacoraEventosDTO
                    {
                        IdEvento = 0,
                        TablaDeEvento = "ModuloMantenimiento",
                        TipoDeEvento = "Actualizar",
                        FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                        DescripcionDeEvento = "Se actualizó el estado de un Mantenimiento.",
                        StackTrace = "no hubo error",
                        DatosAnteriores = datosJson,
                        DatosPosteriores = datosJson
                    };

                    _registrarBitacoraEventosLN.RegistrarBitacora(bitacora);

                    return RedirectToAction("IndexMantenimiento");
                }

                // Si no se encuentra el Mantenimiento, redirigir a IndexMantenimiento
                return RedirectToAction("IndexMantenimiento");
            }
            catch (Exception ex)
            {
                var bitacora = new BitacoraEventosDTO
                {
                    IdEvento = 0,
                    TablaDeEvento = "ModuloMantenimiento",
                    TipoDeEvento = "Error",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = "Error al actualizar el estado del Mantenimiento.",
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