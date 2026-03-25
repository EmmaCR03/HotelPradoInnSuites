using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Bitacora.Registrar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Citas.Editar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Citas.Listar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Citas.ObtenerEnlaces;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Citas.ObtenerPorId;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Citas.Registrar;
using HotelPrado.Abstracciones.Modelos.Bitacora;
using HotelPrado.Abstracciones.Modelos.Citas;
using HotelPrado.AccesoADatos;
using HotelPrado.LN.Bitacora.Registrar;
using HotelPrado.LN.Citas.Editar;
using HotelPrado.LN.Citas.Enlaces;
using HotelPrado.LN.Citas.Listar;
using HotelPrado.LN.Citas.ObtenerPorId;
using HotelPrado.LN.Citas.Registrar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace HotelPrado.UI.Controllers
{
    public class CitasController : Controller
    {
        private readonly IListarCitasLN _listarCitas;
        private readonly IRegistrarCitasLN _registrarCitas;
        private readonly IEditarCitasLN _editarCitasLN;
        private readonly Contexto _contexto;
        private readonly IObtenerCitaPorIdLN _obtenerCitaPorId;
        private readonly IRegistrarBitacoraEventosLN _registrarBitacoraEventosLN;
        private readonly IObtenerEnlacesLN _obtenerEnlaces;

        public CitasController()
        {
            _registrarCitas = new RegistrarCitasLN();
            _listarCitas = new ListarCitasLN();
            _editarCitasLN = new EditarCitasLN();
            _contexto = new Contexto();
            _obtenerCitaPorId = new ObtenerCitaPorIdLN();
            _registrarBitacoraEventosLN = new RegistrarBitacoraEventosLN();
            _obtenerEnlaces = new ObtenerEnlacesLN();
        }
        [AllowAnonymous]
        public ActionResult IndexCitas(int id)
        {
            ViewBag.Title = "La Cita";
            ViewBag.IdDepartamento = id;

            var citas = _obtenerEnlaces.ObtenerCitasConEnlaces();
            var laListaDeCita = _listarCitas.Listar(id);

            var model = laListaDeCita.Select(cita =>
            {
                var citaConEnlace = citas.FirstOrDefault(c => c.IdCita == cita.IdCita);
                if (citaConEnlace != null)
                {
                    cita.EnlaceWhatsApp = citaConEnlace.EnlaceWhatsApp;
                    cita.EnlaceCorreo = citaConEnlace.EnlaceCorreo;
                }
                return cita;
            }).ToList();

            return View(model);
        }
        [AllowAnonymous]
        public ActionResult Create(int id)
        {
            return View(new CitasDTO { IdDepartamento = id });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Create(CitasDTO citas)
        {
            try
            {
                citas.Estado = "Respuesta Pendiente";
                await _registrarCitas.Guardar(citas);
                return RedirectToAction("IndexDepartamentosClientes", "Departamento");
            }
            catch
            {
                return View(citas);
            }
        }
        [Authorize(Roles = "Administrador, Colaborador")]
        public ActionResult Details(int id)
        {
            var cita = _obtenerCitaPorId.Obtener(id);
            return cita == null ? HttpNotFound() : (ActionResult)View(cita);
        }
        [Authorize(Roles = "Administrador, Colaborador")]
        public ActionResult Edit(int IdCita)
        {
            var lacita = _obtenerCitaPorId.Obtener(IdCita);
            if (lacita == null) return HttpNotFound();

            var colaboradores = _contexto.ColaboradorTabla
                .Select(c => new SelectListItem
                {
                    Value = c.IdColaborador.ToString(),
                    Text = c.NombreColaborador + " " + c.PrimerApellidoColaborador
                }).ToList();

            ViewBag.Colaborador = colaboradores.Any() ? colaboradores : new List<SelectListItem>();
            return View(lacita);
        }
        [Authorize(Roles = "Administrador, Colaborador")]
        [HttpPost]
        public async Task<ActionResult> Edit(CitasDTO laCita)
        {
            if (!ModelState.IsValid) return View(laCita);

            try
            {
                int result = await _editarCitasLN.Actualizar(laCita);
                if (result == 0)
                {
                    ViewBag.mensaje = "Ocurrió un error inesperado, favor intente nuevamente.";
                    return View(laCita);
                }
                return RedirectToAction("IndexCitas", new { id = laCita.IdDepartamento });
            }
            catch
            {
                ViewBag.mensaje = "Error al actualizar la cita.";
                return View(laCita);
            }
        }

        [HttpPost]
        public ActionResult ToggleEstado(int IdCita, string Estado)
        {
            try
            {
                var cita = _contexto.CitasTabla.FirstOrDefault(d => d.IdCita == IdCita);
                if (cita == null) return RedirectToAction("IndexCitas");

                cita.Estado = Estado;
                _contexto.SaveChanges();

                var bitacora = new BitacoraEventosDTO
                {
                    TablaDeEvento = "ModuloCitas",
                    TipoDeEvento = "Cambiar Estado",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = "Se actualizó el estado de una cita.",
                    StackTrace = "Sin errores",
                    DatosAnteriores = Estado,
                    DatosPosteriores = Estado
                };
                _registrarBitacoraEventosLN.RegistrarBitacora(bitacora);
                return RedirectToAction("IndexCitas", new { id = cita.IdDepartamento });
            }
            catch (Exception ex)
            {
                _registrarBitacoraEventosLN.RegistrarBitacora(new BitacoraEventosDTO
                {
                    TablaDeEvento = "ModuloCitas",
                    TipoDeEvento = "Error",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = "Error al actualizar el estado de la cita.",
                    StackTrace = ex.StackTrace
                });
                return RedirectToAction("IndexCitas");
            }
        }
    }
}
