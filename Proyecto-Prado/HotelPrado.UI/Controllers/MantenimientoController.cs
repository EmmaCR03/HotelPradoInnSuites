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
using System.Threading.Tasks;
using System.Web.Mvc;

namespace HotelPrado.UI.Controllers
{
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
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Mantenimiento/Create
        public ActionResult Create(int? IdDepartamento)
        {
            if (IdDepartamento.HasValue)
            {
                // Obtener el nombre del departamento basado en el IdDepartamento
                var departamento = _contexto.DepartamentoTabla.FirstOrDefault(d => d.IdDepartamento == IdDepartamento.Value);
                if (departamento != null)
                {
                    ViewBag.IdDepartamento = IdDepartamento.Value;
                    ViewBag.DepartamentoNombre = departamento.Nombre; // Pasar el nombre del departamento
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
                if (modeloDeMantenimiento.idDepartamento == 0)
                {
                    ViewBag.Error = "El ID del departamento es obligatorio.";
                    return View(modeloDeMantenimiento);
                }

                // Obtener el nombre del departamento desde la base de datos
                var departamento = _contexto.DepartamentoTabla.FirstOrDefault(d => d.IdDepartamento == modeloDeMantenimiento.idDepartamento);
                if (departamento != null)
                {
                    modeloDeMantenimiento.DepartamentoNombre = departamento.Nombre;
                }

                int cantidadDeDatosGuardados = await _registrarMantenimientoLN.Guardar(modeloDeMantenimiento);
                return RedirectToAction("IndexMantenimiento");
            }
            catch
            {
                return View();
            }
        }

        // GET: Mantenimiento/Edit/5
        public async Task<ActionResult> Edit(int IdMantenimiento)
        {
            var mantenimiento = await _obtenerMantenimientoPorIdLN.Obtener(IdMantenimiento);
            if (mantenimiento == null)
            {
                return HttpNotFound();
            }
            return View(mantenimiento);
        }

        // POST: Mantenimiento/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(MantenimientoDTO elmantenimiento)
        {
            Console.WriteLine($"Estado recibido: {elmantenimiento.Estado}");

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
                        ""idHabitacion"": {mantenimiento.idHabitacion}
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
