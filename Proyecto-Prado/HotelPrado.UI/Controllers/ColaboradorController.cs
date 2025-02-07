using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Bitacora.Registrar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Colaborador.Editar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Colaborador.Listar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Colaborador.ObtenerPorId;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Colaborador.Registrar;
using HotelPrado.Abstracciones.Modelos.Bitacora;
using HotelPrado.Abstracciones.Modelos.Colaborador;
using HotelPrado.Abstracciones.Modelos.Departamento;
using HotelPrado.AccesoADatos;
using HotelPrado.LN.Bitacora.Registrar;
using HotelPrado.LN.Colaborador.Editar;
using HotelPrado.LN.Colaborador.Listar;
using HotelPrado.LN.Colaborador.ObtenerPorId;
using HotelPrado.LN.Colaborador.Registrar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace HotelPrado.UI.Controllers
{
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
        // GET: Colaborador
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
        public ActionResult Edit(int IdColaborador)
        {


            var elcolaborador = _obtenerColaboradorPorId.Obtener(IdColaborador);
            return View(elcolaborador);
        }

        // POST: Colaborador/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(ColaboradorDTO elcolaborador)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int cantidadDeDatosActualizados = await _editarColaboradorLN.Actualizar(elcolaborador);
                    if (cantidadDeDatosActualizados == 0)
                    {
                        ViewBag.mensaje = "Ocurrió un error inesperado, favor intente nuevamente.";
                        return View(elcolaborador);
                    }
                    return RedirectToAction("IndexDepartamentos");
                }
                catch (Exception ex)
                {
                    ViewBag.mensaje = "Ocurrió un error inesperado, favor intente nuevamente.";
                    return View(elcolaborador);
                }
            }
            return View(elcolaborador);
        }

        // GET: Colaborador/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Departamento/Delete/5
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
