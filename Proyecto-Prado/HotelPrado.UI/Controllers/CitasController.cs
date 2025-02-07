using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Citas.Listar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Bitacora.Registrar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Citas.Editar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Citas.Listar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Citas.ObtenerEnlaces;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Citas.ObtenerPorId;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Citas.Registrar;
using HotelPrado.Abstracciones.Modelos.Bitacora;
using HotelPrado.Abstracciones.Modelos.Citas;
using HotelPrado.Abstracciones.Modelos.Departamento;
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
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace HotelPrado.UI.Controllers
{
    public class CitasController : Controller
    {
        IListarCitasLN _listarCitas;
        IRegistrarCitasLN _registrarCitas;
        IEditarCitasLN _editarCitasLN;
        Contexto _contexto;
        IObtenerCitaPorIdLN _obtenerCitaPorId;
        IRegistrarBitacoraEventosLN _registrarBitacoraEventosLN;
        IObtenerEnlacesLN _obtenerEnlaces;
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
        // GET: Citas
        public ActionResult IndexCitas(int id)
        {
            ViewBag.Title = "La Cita";
            ViewBag.IdDepartamento = id; // Pasar el IdDepartamento a la vista

            // Obtener las citas con los enlaces
            var citas = _obtenerEnlaces.ObtenerCitasConEnlaces();

            // Obtener las citas filtradas por id
            List<CitasDTO> laListaDeCita = _listarCitas.Listar(id);

            // Combinar las listas si es necesario
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


        // GET: Citas/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Citas/Create
        public ActionResult Create(int id)
        {
            var modelo = new CitasDTO { IdDepartamento = id };
            return View(modelo);
        }

        // POST: Citas/Create
        [HttpPost]
        public async Task<ActionResult> Create(CitasDTO citas)
        {
            try
            {
                // TODO: Add insert logic here
                citas.Estado = "Respuesta Pendiente";
                int cantidadDeDatosGuardados = await _registrarCitas.Guardar(citas);

                return RedirectToAction("IndexCitas", new { id = citas.IdDepartamento });
            }
            catch
            {
                return View();
            }
        }

        // GET: Citas/Edit/5
        public ActionResult Edit(int IdCita)
        {
            // Obtener la cita desde la base de datos
            var lacita = _obtenerCitaPorId.Obtener(IdCita);
            if (lacita == null)
            {
                return HttpNotFound(); // Retorna un error 404 si la cita no existe
            }

            // Obtener la lista de colaboradores
            // Obtener los colaboradores de la base de datos
            var colaboradoresDb = _contexto.ColaboradorTabla
                .Select(c => new
                {
                    Id = c.IdColaborador,
                    Nombre = c.NombreColaborador,
                    Apellido = c.PrimerApellidoColaborador
                })
                .ToList(); // Trae los datos a memoria

            // Concatenar los nombres en memoria
            var colaboradores = colaboradoresDb
                .Select(c => new
                {
                    Id = c.Id,
                    NombreCompleto = $"{c.Nombre} {c.Apellido}"
                })
                .ToList();


            // Pasar los colaboradores a la vista
            ViewBag.Colaborador = new SelectList(colaboradores, "Id", "NombreCompleto");

            return View(lacita);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(CitasDTO laCita)
        {
            if (!ModelState.IsValid)
            {
                return View(laCita);
            }

            try
            {
                int cantidadDeDatosActualizados = await _editarCitasLN.Actualizar(laCita);
                if (cantidadDeDatosActualizados == 0)
                {
                    ViewBag.mensaje = "Ocurrió un error inesperado, favor intente nuevamente.";
                    return View(laCita);
                }

                Console.WriteLine("ID Cita: " + laCita.IdCita);
                return RedirectToAction("IndexCitas", new { id = laCita.IdDepartamento });
            }
            catch (Exception ex)
            {
                ViewBag.mensaje = "Ocurrió un error inesperado, favor intente nuevamente.";
                return View(laCita);
            }
        }

        // GET: Citas/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Citas/Delete/5
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
        public ActionResult ToggleEstado(int IdCita, string Estado)
        {
            try
            {
                var citas = _contexto.CitasTabla.FirstOrDefault(d => d.IdCita == IdCita);
                if (citas != null)
                {
                    // Actualiza el estado con el valor recibido del formulario
                    citas.Estado = Estado;
                    _contexto.SaveChanges();

                    // Registrar en la bitácora
                    string datosJson = $@"
            {{
                ""IdCita"": {citas.IdCita},
                ""IdDepartamento"": {citas.IdDepartamento},
                ""IdColaborador"": {citas.IdColaborador},
                ""FechaHoraInicio"": ""{citas.FechaHoraInicio}"",
                ""FechaHoraFin"": ""{citas.FechaHoraFin}"",
                ""Estado"": ""{citas.Estado}"",  // Asegúrate que el estado esté en comillas si es un string
                ""Observaciones"": ""{citas.Observaciones}"",
                ""FechaCreacion"": ""{citas.FechaCreacion}"",
                ""Nombre"": ""{citas.Nombre}"",
                ""PrimerApellido"": ""{citas.PrimerApellido}"",
                ""SegundoApellido"": ""{citas.SegundoApellido}"",
                ""Telefono"": ""{citas.Telefono}"",
                ""MedioContacto"": ""{citas.MedioContacto}"",
                ""Correo"": ""{citas.Correo}""
            }}";

                    var bitacora = new BitacoraEventosDTO
                    {
                        IdEvento = 0,
                        TablaDeEvento = "ModuloDepartamento",
                        TipoDeEvento = "Cambiar Estado",
                        FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                        DescripcionDeEvento = "Se actualizó el estado de un departamento.",
                        StackTrace = "no hubo error",
                        DatosAnteriores = datosJson,
                        DatosPosteriores = datosJson
                    };

                    _registrarBitacoraEventosLN.RegistrarBitacora(bitacora);

                    return RedirectToAction("IndexCitas", new { id = citas.IdDepartamento });
                }

                // Si no se encuentra la cita, redirigir a IndexCitas
                return RedirectToAction("IndexCitas");
            }
            catch (Exception ex)
            {
                var bitacora = new BitacoraEventosDTO
                {
                    IdEvento = 0,
                    TablaDeEvento = "ModuloDepartamento",
                    TipoDeEvento = "Error",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = "Error al actualizar el estado del departamento.",
                    StackTrace = ex.StackTrace,
                    DatosAnteriores = "NA",
                    DatosPosteriores = "NA"
                };

                _registrarBitacoraEventosLN.RegistrarBitacora(bitacora);

                return RedirectToAction("IndexCitas", "Citas");
            }
        }
    }
}
