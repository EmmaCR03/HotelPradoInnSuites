using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Bitacora.Registrar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Cargos.Editar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Cargos.Eliminar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Cargos.Listar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Cargos.ObtenerPorId;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Cargos.Registrar;
using HotelPrado.Abstracciones.Modelos.Bitacora;
using HotelPrado.Abstracciones.Modelos.Cargos;
using HotelPrado.AccesoADatos;
using HotelPrado.LN.Bitacora.Registrar;
using HotelPrado.LN.Cargos.Editar;
using HotelPrado.LN.Cargos.Eliminar;
using HotelPrado.LN.Cargos.Listar;
using HotelPrado.LN.Cargos.ObtenerPorId;
using HotelPrado.LN.Cargos.Registrar;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace HotelPrado.UI.Controllers
{
    [Authorize(Roles = "Administrador, Colaborador")]
    public class CargosController : Controller
    {
        private readonly IListarCargosLN _listarCargosLN;
        private readonly IRegistrarCargosLN _registrarCargosLN;
        private readonly IEditarCargosLN _editarCargosLN;
        private readonly IObtenerCargosPorIdLN _obtenerCargosPorIdLN;
        private readonly IEliminarCargosLN _eliminarCargosLN;
        private readonly Contexto _contexto;
        private readonly IRegistrarBitacoraEventosLN _registrarBitacoraEventosLN;

        public CargosController()
        {
            _listarCargosLN = new ListarCargosLN();
            _registrarCargosLN = new RegistrarCargosLN();
            _editarCargosLN = new EditarCargosLN();
            _obtenerCargosPorIdLN = new ObtenerCargosPorIdLN();
            _eliminarCargosLN = new EliminarCargosLN();
            _contexto = new Contexto();
            _registrarBitacoraEventosLN = new RegistrarBitacoraEventosLN();
        }

        // GET: Cargos
        public ActionResult Index()
        {
            ViewBag.Title = "Gestión de Cargos";
            var laListaDeCargos = _listarCargosLN.Listar();
            return View(laListaDeCargos);
        }

        // GET: Cargos/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            try
            {
                if (!id.HasValue)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var cargo = await _obtenerCargosPorIdLN.Obtener(id.Value);
                if (cargo == null)
                {
                    return HttpNotFound();
                }
                return View(cargo);
            }
            catch (Exception ex)
            {
                var bitacora = new BitacoraEventosDTO
                {
                    IdEvento = 0,
                    TablaDeEvento = "ModuloCargos",
                    TipoDeEvento = "Error",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = "Error al obtener los detalles del cargo.",
                    StackTrace = ex.StackTrace,
                    DatosAnteriores = "NA",
                    DatosPosteriores = "NA"
                };

                await _registrarBitacoraEventosLN.RegistrarBitacora(bitacora);
                return RedirectToAction("Index");
            }
        }

        // GET: Cargos/Create
        public ActionResult Create()
        {
                                    CargarListasParaVista();

            var nuevoCargo = new CargosDTO
            {
                FechaHora = DateTime.Now,
                Cancelado = false,
                EnFacturaExtras = false,
                CuentaError = false,
                PagoImpuesto = false,
                Facturar = false,
                NoContable = false
            };

            return View(nuevoCargo);
        }

        // POST: Cargos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CargosDTO modelo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Validar que el monto total sea requerido
                    if (!modelo.MontoTotal.HasValue || modelo.MontoTotal.Value <= 0)
                    {
                        ModelState.AddModelError("MontoTotal", "El monto total es requerido y debe ser mayor a 0.");
                    }

                    if (ModelState.IsValid)
                    {
                        int cantidadDeDatosGuardados = await _registrarCargosLN.Guardar(modelo);
                        if (cantidadDeDatosGuardados > 0)
                        {
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Error al guardar el cargo.");
                        }
                    }
                }

                // Recargar listas si hay errores
                CargarListasParaVista();
                return View(modelo);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al guardar: " + ex.Message);
                
                CargarListasParaVista();
                return View(modelo);
            }
        }

        private void CargarListasParaVista()
        {
            // Cargar lista de CheckIns
            try
            {
                var checkIns = new System.Collections.Generic.List<SelectListItem>();
                using (var connection = new System.Data.SqlClient.SqlConnection(_contexto.Database.Connection.ConnectionString))
                {
                    connection.Open();
                    using (var command = new System.Data.SqlClient.SqlCommand(
                        @"SELECT ci.IdCheckIn, ci.NombreCliente, ci.CodigoFolio, h.NumeroHabitacion 
                          FROM CheckIn ci
                          LEFT JOIN Habitaciones h ON ci.IdHabitacion = h.IdHabitacion
                          ORDER BY ci.IdCheckIn DESC", connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var folio = reader.IsDBNull(2) ? reader.GetInt32(0).ToString() : reader.GetInt32(2).ToString();
                                var nombre = reader.IsDBNull(1) ? "N/A" : reader.GetString(1);
                                var habitacion = reader.IsDBNull(3) ? "" : reader.GetString(3);
                                checkIns.Add(new SelectListItem
                                {
                                    Value = reader.GetInt32(0).ToString(),
                                    Text = $"{folio}-{folio} - {nombre} - Hab: {habitacion}"
                                });
                            }
                        }
                    }
                }
                ViewBag.IdCheckIn = new SelectList(checkIns, "Value", "Text");
            }
            catch
            {
                ViewBag.IdCheckIn = new SelectList(new System.Collections.Generic.List<SelectListItem>(), "Value", "Text");
            }

            // Cargar lista de clientes
            try
            {
                var clientes = new System.Collections.Generic.List<SelectListItem>();
                using (var connection = new System.Data.SqlClient.SqlConnection(_contexto.Database.Connection.ConnectionString))
                {
                    connection.Open();
                    using (var command = new System.Data.SqlClient.SqlCommand(
                        "SELECT DISTINCT NombreCliente FROM CheckIn WHERE NombreCliente IS NOT NULL ORDER BY NombreCliente", connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                clientes.Add(new SelectListItem
                                {
                                    Value = reader.GetString(0),
                                    Text = reader.GetString(0)
                                });
                            }
                        }
                    }
                }
                ViewBag.Clientes = new SelectList(clientes, "Value", "Text");
            }
            catch
            {
                ViewBag.Clientes = new SelectList(new System.Collections.Generic.List<SelectListItem>(), "Value", "Text");
            }

            // Cargar lista de habitaciones
            try
            {
                var habitaciones = _contexto.HabitacionesTabla
                    .Select(h => new SelectListItem
                    {
                        Value = h.NumeroHabitacion ?? h.IdHabitacion.ToString(),
                        Text = h.NumeroHabitacion ?? h.IdHabitacion.ToString()
                    })
                    .OrderBy(h => h.Text)
                    .ToList();
                ViewBag.Habitaciones = new SelectList(habitaciones, "Value", "Text");
            }
            catch
            {
                ViewBag.Habitaciones = new SelectList(new System.Collections.Generic.List<SelectListItem>(), "Value", "Text");
            }

            // Lista de tipos de cargo
            var tiposCargo = new System.Collections.Generic.List<SelectListItem>
            {
                new SelectListItem { Value = "1PAGO CON TARJETA", Text = "1PAGO CON TARJETA" },
                new SelectListItem { Value = "2PAGO EN EFECTIVO", Text = "2PAGO EN EFECTIVO" },
                new SelectListItem { Value = "3PAGO POR TRANSFERENCIA", Text = "3PAGO POR TRANSFERENCIA" },
                new SelectListItem { Value = "4HOSPEDAJE", Text = "4HOSPEDAJE" },
                new SelectListItem { Value = "5PERSONA ADICIONAL", Text = "5PERSONA ADICIONAL" },
                new SelectListItem { Value = "6NOTA CREDITO", Text = "6NOTA CREDITO" },
                new SelectListItem { Value = "7CORTESIA", Text = "7CORTESIA" }
            };
            ViewBag.TiposCargo = new SelectList(tiposCargo, "Value", "Text");
        }

        // GET: Cargos/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var cargo = await _obtenerCargosPorIdLN.Obtener(id.Value);
            if (cargo == null)
            {
                return HttpNotFound();
            }

            // Cargar lista de CheckIns
            try
            {
                var checkIns = new System.Collections.Generic.List<SelectListItem>();
                using (var connection = new System.Data.SqlClient.SqlConnection(_contexto.Database.Connection.ConnectionString))
                {
                    connection.Open();
                    using (var command = new System.Data.SqlClient.SqlCommand(
                        "SELECT IdCheckIn, NombreCliente FROM CheckIn ORDER BY IdCheckIn DESC", connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var idCheckIn = reader.GetInt32(0);
                                checkIns.Add(new SelectListItem
                                {
                                    Value = idCheckIn.ToString(),
                                    Text = $"Check-In #{idCheckIn} - Cliente: {(reader.IsDBNull(1) ? "N/A" : reader.GetString(1))}",
                                    Selected = idCheckIn == cargo.IdCheckIn
                                });
                            }
                        }
                    }
                }
                ViewBag.IdCheckIn = new SelectList(checkIns, "Value", "Text");
            }
            catch
            {
                ViewBag.IdCheckIn = new SelectList(new System.Collections.Generic.List<SelectListItem>(), "Value", "Text");
            }
            return View(cargo);
        }

        // POST: Cargos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(CargosDTO modelo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Validar que el monto total sea requerido
                    if (!modelo.MontoTotal.HasValue || modelo.MontoTotal.Value <= 0)
                    {
                        ModelState.AddModelError("MontoTotal", "El monto total es requerido y debe ser mayor a 0.");
                    }

                    if (ModelState.IsValid)
                    {
                        int cantidadDeDatosActualizados = await _editarCargosLN.Actualizar(modelo);
                        if (cantidadDeDatosActualizados > 0)
                        {
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Error al actualizar el cargo.");
                        }
                    }
                }

                // Recargar listas si hay errores
                try
                {
                    var checkIns = new System.Collections.Generic.List<SelectListItem>();
                    using (var connection = new System.Data.SqlClient.SqlConnection(_contexto.Database.Connection.ConnectionString))
                    {
                        connection.Open();
                        using (var command = new System.Data.SqlClient.SqlCommand(
                            "SELECT IdCheckIn, NombreCliente FROM CheckIn ORDER BY IdCheckIn DESC", connection))
                        {
                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var idCheckIn = reader.GetInt32(0);
                                    checkIns.Add(new SelectListItem
                                    {
                                        Value = idCheckIn.ToString(),
                                        Text = $"Check-In #{idCheckIn} - Cliente: {(reader.IsDBNull(1) ? "N/A" : reader.GetString(1))}",
                                        Selected = idCheckIn == modelo.IdCheckIn
                                    });
                                }
                            }
                        }
                    }
                    ViewBag.IdCheckIn = new SelectList(checkIns, "Value", "Text");
                }
                catch
                {
                    ViewBag.IdCheckIn = new SelectList(new System.Collections.Generic.List<SelectListItem>(), "Value", "Text");
                }
                return View(modelo);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al actualizar: " + ex.Message);
                
                try
                {
                    var checkIns = new System.Collections.Generic.List<SelectListItem>();
                    using (var connection = new System.Data.SqlClient.SqlConnection(_contexto.Database.Connection.ConnectionString))
                    {
                        connection.Open();
                        using (var command = new System.Data.SqlClient.SqlCommand(
                            "SELECT IdCheckIn, NombreCliente FROM CheckIn ORDER BY IdCheckIn DESC", connection))
                        {
                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var idCheckIn = reader.GetInt32(0);
                                    checkIns.Add(new SelectListItem
                                    {
                                        Value = idCheckIn.ToString(),
                                        Text = $"Check-In #{idCheckIn} - Cliente: {(reader.IsDBNull(1) ? "N/A" : reader.GetString(1))}",
                                        Selected = idCheckIn == modelo.IdCheckIn
                                    });
                                }
                            }
                        }
                    }
                    ViewBag.IdCheckIn = new SelectList(checkIns, "Value", "Text");
                }
                catch
                {
                    ViewBag.IdCheckIn = new SelectList(new System.Collections.Generic.List<SelectListItem>(), "Value", "Text");
                }
                return View(modelo);
            }
        }

        // GET: Cargos/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var cargo = await _obtenerCargosPorIdLN.Obtener(id.Value);
            if (cargo == null)
            {
                return HttpNotFound();
            }
            return View(cargo);
        }

        // POST: Cargos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            try
            {
                int cantidadDeDatosEliminados = await _eliminarCargosLN.Eliminar(id);
                if (cantidadDeDatosEliminados > 0)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Error al eliminar el cargo.");
                    var cargo = await _obtenerCargosPorIdLN.Obtener(id);
                    return View(cargo);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al eliminar: " + ex.Message);
                var cargo = await _obtenerCargosPorIdLN.Obtener(id);
                return View(cargo);
            }
        }
    }
}

