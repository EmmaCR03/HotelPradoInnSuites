using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Departamentos.ObtenerPorId;
using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Departamentos.Registrar;
using HotelPrado.Abstracciones.Interfaces.AccesoADatos.TipoDeDepartamento.Listar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Bitacora.Registrar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Departamentos.Editar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Departamentos.Listar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Departamentos.ObtenerPorId;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Departamentos.Registrar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.TipoDeDepartamentos;
using HotelPrado.Abstracciones.Modelos.Bitacora;
using HotelPrado.Abstracciones.Modelos.Departamento;
using HotelPrado.Abstracciones.Modelos.ImagenesDepartamento;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.ImagenesDepartamento;
using HotelPrado.AccesoADatos;
using HotelPrado.LN.Bitacora.Registrar;
using HotelPrado.LN.Departamentos.Editar;
using HotelPrado.LN.Departamentos.Listar;
using HotelPrado.LN.Departamentos.ObtenerPorId;
using HotelPrado.LN.Departamentos.Registrar;
using HotelPrado.LN.TipoDeDepartamento.Listar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.WebSockets;

namespace HotelPrado.UI.Controllers
{
    public class DepartamentoController : Controller
    {
        IListarDepartamentosLN _listarDepartamentosLN;
        IListarTipoDeDepartamenoLN _listarTipoDeDepartamentosLN;
        IRegistrarDepartamentoLN _registrarDepartamentosLN;
        Contexto _contexto;
        IObtenerCitasPorIdLN _obtenerDepartamentoPorId;
        IEditarDepartamentosLN _editarDepartamentoLN;
        IRegistrarBitacoraEventosLN _registrarBitacoraEventosLN;
        public DepartamentoController()
        {
            _listarDepartamentosLN = new ListarDepartamentoLN();
            _listarTipoDeDepartamentosLN = new ListarTipoDeDepartamenoLN();
            _registrarDepartamentosLN = new RegistrarDepartamentoLN();
            _contexto = new Contexto();
            _obtenerDepartamentoPorId = new ObtenerCitasPorIdLN();
            _editarDepartamentoLN = new EditarDepartamentosLN();
            _registrarBitacoraEventosLN = new RegistrarBitacoraEventosLN();
        }

        // GET: Departamento
        public ActionResult IndexDepartamentos()
        {
            ViewBag.Title = "El Departamento";
            var laListaDeDepartamentos = _listarDepartamentosLN.Listar();
            return View(laListaDeDepartamentos);
        }

        public ActionResult IndexDepartamentosClientes()
        {
            ViewBag.Title = "El Departamento";

            // Obtener departamentos
            var laListaDeDepartamentos = _listarDepartamentosLN.Listar();

            // Verificar si hay datos en la lista
            if (!laListaDeDepartamentos.Any())
            {
                return View(new List<DepartamentoDTO>()); // Si no hay departamentos, retorna una lista vacía
            }

            // Asegurar que todos los departamentos tengan valores
            foreach (var depto in laListaDeDepartamentos)
            {
                depto.NumeroEmpresa = depto.NumeroEmpresa ?? "+50685406105"; // Si es null, asigna un valor
                depto.CorreoEmpresa = depto.CorreoEmpresa ?? "info@pradoinn.com";
            }

            // Filtrar los departamentos disponibles
            var departamentosDisponibles = laListaDeDepartamentos
                .Where(d => d.Estado == "Disponible" || d.Estado == "Activo")
                .ToList();

            return View(departamentosDisponibles);
        }



        // GET: Departamento/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Departamento/Create

        public ActionResult Create()
        {
            // Obtener los datos desde la base de datos
            var tiposDepartamentoDb = _contexto.TipoDepartamentoTabla
                .Select(td => new
                {
                    Id = td.IdTipoDepartamento,
                    NumeroHabitaciones = td.NumeroHabitaciones,
                    Amueblado = td.Amueblado
                })
                .ToList();

            // Si hay datos, construimos la lista
            var tiposDepartamento = tiposDepartamentoDb
                .Select(td => new SelectListItem
                {
                    Value = td.Id.ToString(),
                    Text = $"{td.NumeroHabitaciones} habitación(es) - {(td.Amueblado ? "Amueblado" : "No amueblado")}"
                })
                .ToList();

            // Asignamos la lista al ViewBag
            ViewBag.TipoDepartamento = new SelectList(tiposDepartamento, "Value", "Text");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(DepartamentoDTO modeloDeDepartamento)
        {
            if (ModelState.IsValid)
            {
                Console.WriteLine("IdTipoDepartamento recibido: " + modeloDeDepartamento.IdTipoDepartamento);

                // Si el valor es null o 0, el problema está en la vista
                if (modeloDeDepartamento.IdTipoDepartamento == null || modeloDeDepartamento.IdTipoDepartamento == 0)
                {
                    ModelState.AddModelError("IdTipoDepartamento", "Debe seleccionar un tipo de habitación.");
                    return View(modeloDeDepartamento);
                }
              


                // Lista de archivos a almacenar
                var archivos = new List<string>();

                // Verificar si la carpeta "Uploads" existe, si no, crearla
                string uploadDirectory = Server.MapPath("~/Uploads/");
                if (!Directory.Exists(uploadDirectory))
                {
                    Directory.CreateDirectory(uploadDirectory);
                }

                // Verificar si hay archivos cargados
                if (Request.Files.Count > 0)
                {
                    foreach (string fileName in Request.Files)
                    {
                        var file = Request.Files[fileName];
                        if (file != null && file.ContentLength > 0)
                        {
                            // Guardar el archivo en un directorio específico
                            string path = Path.Combine(uploadDirectory, Path.GetFileName(file.FileName));
                            file.SaveAs(path);

                            // Agregar la URL del archivo a la lista
                            archivos.Add("/Uploads/" + Path.GetFileName(file.FileName));
                        }
                    }

                    // Aquí puedes agregar la lista de imágenes a tu modelo
                    modeloDeDepartamento.UrlImagenes = string.Join(",", archivos);
                }

                // Guardar el departamento en la base de datos
                int cantidadDeDatosGuardados = await _registrarDepartamentosLN.Guardar(modeloDeDepartamento);
                await _contexto.SaveChangesAsync();

                // Guardar las imágenes asociadas a este departamento
                foreach (var url in archivos) // Usa "archivos" para recorrer las URLs
                {
                    var imagen = new ImagenesDepartamentoTabla
                    {
                        IdDepartamento = modeloDeDepartamento.IdDepartamento,
                        UrlImagen = url
                    };

                    _contexto.ImagenesDepartamentoTabla.Add(imagen);
                }

                // Guardar cambios en la base de datos

                return RedirectToAction("IndexDepartamentos");
            }

            // Si no es válido, regresamos el modelo para que se pueda corregir
            return View(modeloDeDepartamento);
        }


        // GET: Departamento/Edit/5
        public ActionResult Edit(int IdDepartamento)
        {
            // Obtener los datos desde la base de datos
            var tiposDepartamentoDb = _contexto.TipoDepartamentoTabla
                .Select(td => new
                {
                    Id = td.IdTipoDepartamento,
                    NumeroHabitaciones = td.NumeroHabitaciones,
                    Amueblado = td.Amueblado
                })
                .ToList();

            // Formatear los datos en memoria
            var tiposDepartamento = tiposDepartamentoDb
                .Select(td => new
                {
                    Id = td.Id,
                    Detalle = $"{td.NumeroHabitaciones} habitación(es) - {(td.Amueblado ? "Amueblado" : "No amueblado")}"
                })
                .ToList();

            // Asignar la lista a ViewBag
            ViewBag.TipoDepartamento = new SelectList(tiposDepartamento, "Id", "Detalle");


            var eldepartamento = _obtenerDepartamentoPorId.Obtener(IdDepartamento);
            return View(eldepartamento);
        }


        // POST: Persona/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(DepartamentoDTO eldepartamento)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int cantidadDeDatosActualizados = await _editarDepartamentoLN.Actualizar(eldepartamento);
                    if (cantidadDeDatosActualizados == 0)
                    {
                        ViewBag.mensaje = "Ocurrió un error inesperado, favor intente nuevamente.";
                        return View(eldepartamento);
                    }
                    return RedirectToAction("IndexDepartamentos");
                }
                catch (Exception ex)
                {
                    ViewBag.mensaje = "Ocurrió un error inesperado, favor intente nuevamente.";
                    return View(eldepartamento);
                }
            }
            return View(eldepartamento);
        }

        // GET: Departamento/Delete/5
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
        public ActionResult ToggleEstado(int IdDepartamento, string Estado)
        {
            try
            {
                var departamento = _contexto.DepartamentoTabla.FirstOrDefault(d => d.IdDepartamento == IdDepartamento);
                if (departamento != null)
                {
                    // Actualiza el estado con el valor recibido del formulario
                    departamento.Estado = Estado;
                    _contexto.SaveChanges();

                    string datosJson = $@"
            {{
                ""IdDepartamento"": {departamento.IdDepartamento},
                ""Nombre"": ""{departamento.Nombre}"",
                ""IdTipoDepartamento"": {departamento.IdTipoDepartamento},
                ""Precio"": ""{departamento.Precio}"",
                ""Estado"": ""{departamento.Estado}"",
                ""IdCliente"": ""{departamento.IdCliente}""
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

                    return RedirectToAction("IndexDepartamentos");
                }

                // Si no se encuentra el departamento, redirigir a IndexDepartamentos
                return RedirectToAction("IndexDepartamentos");
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

                return RedirectToAction("Index", "Home");
            }
        }

    }
}
