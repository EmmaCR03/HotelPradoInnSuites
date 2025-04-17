using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Bitacora.Registrar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Mantenimiento.Editar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Mantenimiento.Listar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Mantenimiento.ObtenerPorId;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Mantenimiento.Registrar;
using HotelPrado.Abstracciones.Modelos.Bitacora;
using HotelPrado.Abstracciones.Modelos.Mantenimiento;
using HotelPrado.AccesoADatos;
using HotelPrado.LN.Bitacora.Registrar;
using HotelPrado.LN.Mantenimiento.Editar;
using HotelPrado.LN.Mantenimiento.Listar;
using HotelPrado.LN.Mantenimiento.ObtenerPorId;
using HotelPrado.LN.Mantenimiento.Registrar;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace HotelPrado.UI.Controllers
{
    [Authorize(Roles = "Administrador, Colaborador")]
    public class MantenimientoController : Controller
    {
        private readonly IListarMantenimientoLN _listarMantenimientoLN;
        private readonly IRegistrarMantenimientoLN _registrarMantenimientoLN;
        private readonly IEditarMantenimientoLN _editarMantenimientoLN;
        private readonly Contexto _contexto;
        private readonly IObtenerMantenimientoPorIdLN _obtenerMantenimientoPorIdLN;
        private readonly IRegistrarBitacoraEventosLN _registrarBitacoraEventosLN;

        public MantenimientoController()
        {
            _registrarMantenimientoLN = new RegistrarMantenimientoLN();
            _listarMantenimientoLN = new ListarMantenimientoLN();
            _editarMantenimientoLN = new EditarMantenimientoLN();
            _contexto = new Contexto();
            _obtenerMantenimientoPorIdLN = new ObtenerMantenimientoPorIdLN();
            _registrarBitacoraEventosLN = new RegistrarBitacoraEventosLN();
        }

        // GET: Mantenimiento
        public ActionResult IndexMantenimiento()
        {
            ViewBag.Title = "El Mantenimiento";
            var laListaDeMantenimiento = _listarMantenimientoLN.Listar();
            return View(laListaDeMantenimiento);
        }

        // GET: Mantenimiento/Details/5
        public async Task<ActionResult> Details(int id)
        {
            try
            {
                var mantenimiento = await _obtenerMantenimientoPorIdLN.Obtener(id);
                if (mantenimiento == null)
                {
                    return HttpNotFound();
                }
                return View(mantenimiento);
            }
            catch (Exception ex)
            {
                // Registrar el error en la bitácora
                var bitacora = new BitacoraEventosDTO
                {
                    IdEvento = 0,
                    TablaDeEvento = "ModuloMantenimiento",
                    TipoDeEvento = "Error",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = "Error al obtener los detalles del mantenimiento.",
                    StackTrace = ex.StackTrace,
                    DatosAnteriores = "NA",
                    DatosPosteriores = "NA"
                };

                await _registrarBitacoraEventosLN.RegistrarBitacora(bitacora);
                return RedirectToAction("IndexMantenimiento");
            }
        }

        // GET: Mantenimiento/Create
        public ActionResult Create(int? IdDepartamento)
        {
            // Obtener todos los departamentos
            var departamentosQuery = _contexto.DepartamentoTabla
                .Where(d => d.Estado == "Disponible");  // Solo departamentos disponibles

            var departamentos = departamentosQuery
                .ToList()  // Ejecutar la consulta primero
                .Select(d => new SelectListItem  // Luego hacer la transformación en memoria
                {
                    Value = d.IdDepartamento.ToString(),
                    Text = d.NumeroDepartamento + " - " + d.Nombre
                })
                .ToList();

            ViewBag.Departamentos = new SelectList(departamentos, "Value", "Text");

            if (IdDepartamento.HasValue)
            {
                // Si se proporciona un ID de departamento, seleccionarlo en el dropdown
                var departamento = _contexto.DepartamentoTabla.FirstOrDefault(d => d.IdDepartamento == IdDepartamento.Value);
                if (departamento != null)
                {
                    ViewBag.IdDepartamento = IdDepartamento.Value;
                }
            }

            return View();
        }

        // POST: Mantenimiento/Create
        [HttpPost]
        public async Task<ActionResult> Create(MantenimientoDTO modeloDeMantenimiento)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // Recargar la lista de departamentos si el modelo no es válido
                    var departamentosQuery = _contexto.DepartamentoTabla
                        .Where(d => d.Estado == "Disponible");

                    var departamentos = departamentosQuery
                        .ToList()  // Ejecutar la consulta primero
                        .Select(d => new SelectListItem  // Luego hacer la transformación en memoria
                        {
                            Value = d.IdDepartamento.ToString(),
                            Text = d.NumeroDepartamento + " - " + d.Nombre
                        })
                        .ToList();

                    ViewBag.Departamentos = new SelectList(departamentos, "Value", "Text");
                    return View(modeloDeMantenimiento);
                }

                // Obtener el nombre del departamento desde la base de datos
                var departamento = _contexto.DepartamentoTabla.FirstOrDefault(d => d.IdDepartamento == modeloDeMantenimiento.idDepartamento);
                if (departamento != null)
                {
                    modeloDeMantenimiento.DepartamentoNombre = departamento.Nombre;
                }
                else
                {
                    ModelState.AddModelError("", "El departamento seleccionado no existe.");
                    return View(modeloDeMantenimiento);
                }

                int cantidadDeDatosGuardados = await _registrarMantenimientoLN.Guardar(modeloDeMantenimiento);
                return RedirectToAction("IndexMantenimiento");
            }
            catch (Exception ex)
            {
                // Recargar la lista de departamentos en caso de error
                var departamentosQuery = _contexto.DepartamentoTabla
                    .Where(d => d.Estado == "Disponible");

                var departamentos = departamentosQuery
                    .ToList()  // Ejecutar la consulta primero
                    .Select(d => new SelectListItem  // Luego hacer la transformación en memoria
                    {
                        Value = d.IdDepartamento.ToString(),
                        Text = d.NumeroDepartamento + " - " + d.Nombre
                    })
                    .ToList();

                ViewBag.Departamentos = new SelectList(departamentos, "Value", "Text");
                ModelState.AddModelError("", "Error al guardar el mantenimiento: " + ex.Message);
                return View(modeloDeMantenimiento);
            }
        }

        // GET: Mantenimiento/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "El ID del mantenimiento es requerido.");
            }

            var mantenimiento = await _obtenerMantenimientoPorIdLN.Obtener(id.Value);
            if (mantenimiento == null)
            {
                return HttpNotFound();
            }

            // Obtener la lista de departamentos
            var departamentosQuery = _contexto.DepartamentoTabla
                .Where(d => d.Estado == "Disponible");

            var departamentos = departamentosQuery
                .ToList()
                .Select(d => new SelectListItem
                {
                    Value = d.IdDepartamento.ToString(),
                    Text = d.NumeroDepartamento + " - " + d.Nombre,
                    Selected = d.IdDepartamento == mantenimiento.idDepartamento
                })
                .ToList();

            ViewBag.Departamentos = new SelectList(departamentos, "Value", "Text");
            ViewBag.Estados = new SelectList(new[] { "Pendiente", "En Proceso", "Completado" }, mantenimiento.Estado);

            return View(mantenimiento);
        }

        // POST: Mantenimiento/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(MantenimientoDTO elmantenimiento)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Obtener el nombre del departamento
                    var departamento = _contexto.DepartamentoTabla.FirstOrDefault(d => d.IdDepartamento == elmantenimiento.idDepartamento);
                    if (departamento != null)
                    {
                        elmantenimiento.DepartamentoNombre = departamento.Nombre;
                    }

                    var cantidadDeDatosActualizados = await _editarMantenimientoLN.Actualizar(elmantenimiento);
                    if (cantidadDeDatosActualizados == 0)
                    {
                        ModelState.AddModelError("", "Ocurrió un error al actualizar el mantenimiento.");
                    }
                    else
                    {
                        return RedirectToAction("IndexMantenimiento");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al actualizar: " + ex.Message);
                }
            }

            // Si llegamos aquí, algo falló, volvemos a cargar los dropdowns
            var departamentosQuery = _contexto.DepartamentoTabla
                .Where(d => d.Estado == "Disponible");

            var departamentos = departamentosQuery
                .ToList()
                .Select(d => new SelectListItem
                {
                    Value = d.IdDepartamento.ToString(),
                    Text = d.NumeroDepartamento + " - " + d.Nombre,
                    Selected = d.IdDepartamento == elmantenimiento.idDepartamento
                })
                .ToList();

            ViewBag.Departamentos = new SelectList(departamentos, "Value", "Text");
            ViewBag.Estados = new SelectList(new[] { "Pendiente", "En Proceso", "Completado" }, elmantenimiento.Estado);

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
                // TODO: Agregar lógica de eliminación
                return RedirectToAction("Index");
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
                    mantenimiento.Estado = Estado;
                    _contexto.SaveChanges();

                    string datosJson = $@"
                    {{
                        ""IdMantenimiento"": {mantenimiento.IdMantenimiento},
                        ""Descripcion"": ""{mantenimiento.Descripcion}"",
                        ""Estado"": ""{mantenimiento.Estado}"",
                        ""idDepartamento"": {mantenimiento.idDepartamento},
                        ""DepartamentoNombre"": {mantenimiento.DepartamentoNombre}
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
