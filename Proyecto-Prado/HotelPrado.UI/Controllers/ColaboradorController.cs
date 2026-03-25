using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Bitacora.Registrar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Colaborador.Editar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Colaborador.Listar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Colaborador.ObtenerPorId;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Colaborador.Registrar;
using HotelPrado.Abstracciones.Modelos.Bitacora;
using HotelPrado.Abstracciones.Modelos.Colaborador;
using HotelPrado.AccesoADatos;
using HotelPrado.LN.Bitacora.Registrar;
using HotelPrado.LN.Colaborador.Editar;
using HotelPrado.LN.Colaborador.Listar;
using HotelPrado.LN.Colaborador.ObtenerPorId;
using HotelPrado.LN.Colaborador.Registrar;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace HotelPrado.UI.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class ColaboradorController : Controller
    {
        IListarColaboradorLN _listarColaboradorLN;
        IRegistrarColaboradorLN _registrarColaboradorLN;
        Contexto _contexto;
        IObtenerColaboradorPorIdLN _obtenerColaboradorPorId;
        IEditarColaboradorLN _editarColaboradorLN;
        IRegistrarBitacoraEventosLN _registrarBitacoraEventosLN;

        public ColaboradorController()
        {
            _listarColaboradorLN = new ListarColaboradorLN();
            _registrarColaboradorLN = new RegistrarColaboradorLN();
            _contexto = new Contexto();
            _obtenerColaboradorPorId = new ObtenerColaboradorPorIdLN();
            _editarColaboradorLN = new EditarColaboradorLN();
            _registrarBitacoraEventosLN = new RegistrarBitacoraEventosLN();
        }
        // GET: Colaborador (caché 60 s para aligerar en host)
        [OutputCache(Duration = 60, VaryByParam = "none", Location = System.Web.UI.OutputCacheLocation.Server)]
        public ActionResult IndexColaborador()
        {
            ViewBag.Title = "El Colaborador";
            var laListaDeColaborador = _listarColaboradorLN.Listar();
            return View(laListaDeColaborador);
        }

        // GET: Colaborador/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Colaborador/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Colaborador/Create
        [HttpPost]
        public async Task<ActionResult> Create(ColaboradorDTO modeloDeColaborador)
        {
            try
            {
                int cantidadDeDatosGuardados = await _registrarColaboradorLN.Guardar(modeloDeColaborador);
                return RedirectToAction("IndexColaborador");
            }
            catch
            {
                return View();
            }
        }

        // GET: Colaborador/Edit/5
        public async Task<ActionResult> Edit(int IdColaborador)
        {
            // Obtener el colaborador desde la base de datos
            var colaborador = await _obtenerColaboradorPorId.Obtener(IdColaborador);

            if (colaborador == null)
            {
                return HttpNotFound();
            }


            var puestosDb = new List<SelectListItem>
            {
                new SelectListItem { Value = "Recepcionista", Text = "Recepcionista" },
                new SelectListItem { Value = "Limpieza", Text = "Limpieza" },
                new SelectListItem { Value = "Administrador", Text = "Administrador" },
                new SelectListItem { Value = "Seguridad", Text = "Seguridad" }
            };
            ViewBag.PuestoColaborador = new SelectList(puestosDb, "Value", "Text", colaborador.PuestoColaborador);

            var estadosLaborales = new List<SelectListItem>
            {
                new SelectListItem { Value = "Activo", Text = "Activo" },
                new SelectListItem { Value = "De Vacaciones", Text = "De Vacaciones" },
                new SelectListItem { Value = "Incapacitado", Text = "Incapacitado" },
                new SelectListItem { Value = "Licencia con goce de salario", Text = "Licencia con goce de salario" },
                new SelectListItem { Value = "Licencia sin goce de salario", Text = "Licencia sin goce de salario" },
                new SelectListItem { Value = "Permiso Especial", Text = "Permiso Especial" }
            };
            ViewBag.EstadoLaboral = new SelectList(estadosLaborales, "Value", "Text", colaborador.EstadoLaboral);

            // Retornar la vista con el colaborador obtenido
            return View(colaborador);
        }





        // POST: Colaborador/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(ColaboradorDTO elcolaborador)
        {
            Console.WriteLine($"EstadoLaboral recibido: {elcolaborador.EstadoLaboral}");

            if (ModelState.IsValid)
            {
                try
                {
                    var cantidadDeDatosActualizados = await _editarColaboradorLN.Actualizar(elcolaborador);
                    if (cantidadDeDatosActualizados == 0)
                    {
                        ViewBag.mensaje = "Ocurrió un error inesperado, favor intente nuevamente.";
                        return View(elcolaborador);
                    }
                    return RedirectToAction("IndexColaborador");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al actualizar: {ex.Message}");
                    ViewBag.mensaje = "Ocurrió un error inesperado, favor intente nuevamente.";
                    return View(elcolaborador);
                }
            }
            return View(elcolaborador);
        }


        // GET: Colaborador/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            var colaborador = await _obtenerColaboradorPorId.Obtener(id);
            if (colaborador == null)
                return HttpNotFound();
            return View(colaborador);
        }

        // POST: Colaborador/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var colaborador = await _obtenerColaboradorPorId.Obtener(id);
            if (colaborador == null)
                return HttpNotFound();

            try
            {
                var entidad = _contexto.ColaboradorTabla.Find(id);
                if (entidad != null)
                {
                    _contexto.ColaboradorTabla.Remove(entidad);
                    await _contexto.SaveChangesAsync();

                    var bitacora = new BitacoraEventosDTO
                    {
                        IdEvento = 0,
                        TablaDeEvento = "ModuloColaborador",
                        TipoDeEvento = "Eliminar colaborador",
                        FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                        DescripcionDeEvento = $"Se eliminó al colaborador {entidad.NombreColaborador} {entidad.PrimerApellidoColaborador} (Id: {id}).",
                        StackTrace = "Sin errores",
                        DatosAnteriores = $@"{{ ""IdColaborador"": {id}, ""Nombre"": ""{entidad.NombreColaborador}"" }}",
                        DatosPosteriores = "{}",
                        Usuario = (User?.Identity?.IsAuthenticated ?? false) ? User.Identity.GetUserName() : "Sistema"
                    };
                    await _registrarBitacoraEventosLN.RegistrarBitacora(bitacora);
                }
                TempData["MensajeExito"] = "Colaborador eliminado correctamente.";
                return RedirectToAction("IndexColaborador");
            }
            catch (DbUpdateException ex) when (ex.InnerException?.InnerException is SqlException sqlEx && (sqlEx.Number == 547 || sqlEx.Number == 2627))
            {
                ModelState.AddModelError("", "No se puede eliminar este colaborador porque tiene citas u otros registros relacionados.");
                return View(colaborador);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al eliminar: " + ex.Message);
                return View(colaborador);
            }
        }

        [HttpPost]
        public ActionResult ToggleEstado(int IdColaborador, string Estado)

        {
            try
            {
                var colaborador = _contexto.ColaboradorTabla.FirstOrDefault(d => d.IdColaborador == IdColaborador);
                if (colaborador != null)
                {
                    // Actualiza el estado con el valor recibido del formulario
                    colaborador.EstadoLaboral = Estado;
                    _contexto.SaveChanges();

                    string datosJson = $@"
            {{
                    ""IdColaborador"": {colaborador.IdColaborador},
                    ""NombreColaborador"": {colaborador.NombreColaborador},
                    ""PrimerApellidoColaborador"": ""{colaborador.PrimerApellidoColaborador}"",
                    ""SegundoApellidoColaborador"": ""{colaborador.SegundoApellidoColaborador}"",
                    ""CedulaColaborador"": {colaborador.CedulaColaborador},
                    ""PuestoColaborador"": {colaborador.PuestoColaborador},
                    ""Estado"": ""{colaborador.EstadoLaboral}""
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
