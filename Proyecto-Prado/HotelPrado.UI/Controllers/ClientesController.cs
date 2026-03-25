using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Cliente.Listar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Reservas;
using HotelPrado.Abstracciones.Modelos.Reservas;
using HotelPrado.Abstracciones.Modelos.Cliente;
using HotelPrado.AccesoADatos;
using HotelPrado.LN.Cliente.Listar;
using HotelPrado.LN.Reservas;
using HotelPrado.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace HotelPrado.UI.Controllers
{
    [Authorize(Roles = "Administrador, Colaborador")]
    public class ClientesController : Controller
    {
        private readonly IListarClientesLN _listarClientesLN;
        private readonly IReservasLN _reservasLN;
        private readonly Contexto _contexto;

        public ClientesController()
        {
            _listarClientesLN = new ListarClientesLN();
            _reservasLN = new ReservasLN();
            _contexto = new Contexto();
        }

        // GET: Clientes
        public ActionResult Index(string busqueda = null, string ordenarPor = "IdCliente", bool ordenAscendente = true, int pagina = 1, int tamanoPagina = 50, string filtroUsuario = null)
        {
            ViewBag.Title = "Gestión de Clientes";
            ViewBag.Busqueda = busqueda;
            ViewBag.OrdenarPor = ordenarPor;
            ViewBag.OrdenAscendente = ordenAscendente;
            ViewBag.FiltroUsuario = filtroUsuario;

            // Validar parámetros
            if (pagina < 1) pagina = 1;
            if (tamanoPagina < 10) tamanoPagina = 10;
            if (tamanoPagina > 200) tamanoPagina = 200;

            // Obtener total de clientes (con filtros aplicados)
            int totalClientes = _listarClientesLN.ContarTotal(busqueda, filtroUsuario);

            // Obtener clientes paginados (con filtros aplicados)
            var laListaDeClientes = _listarClientesLN.ListarPaginado(busqueda, ordenarPor, ordenAscendente, pagina, tamanoPagina, filtroUsuario);

            // Calcular paginación
            int totalPaginas = (int)Math.Ceiling((double)totalClientes / tamanoPagina);
            if (pagina > totalPaginas && totalPaginas > 0) pagina = totalPaginas;

            ViewBag.TotalClientes = totalClientes;
            ViewBag.PaginaActual = pagina;
            ViewBag.TamanoPagina = tamanoPagina;
            ViewBag.TotalPaginas = totalPaginas;

            // Generar HTML de paginación en el controlador (vista sin @if anidados para evitar error Razor en hosting)
            ViewBag.PaginacionHtml = totalPaginas > 1 ? BuildPaginacionHtml(busqueda, filtroUsuario, ordenarPor, ordenAscendente, pagina, tamanoPagina, totalPaginas, totalClientes) : "";

            return View(laListaDeClientes);
        }

        /// <summary>
        /// Muestra el detalle de un cliente: datos básicos, empresa asociada (si existe) y todas sus visitas (reservas).
        /// </summary>
        /// <param name="id">IdCliente</param>
        public ActionResult Detalles(int id)
        {
            // Buscar el cliente en memoria usando la lógica de negocio existente
            var todosLosClientes = _listarClientesLN.Listar(null, "IdCliente", true) ?? new List<ClienteDTO>();
            var cliente = todosLosClientes.FirstOrDefault(c => c.IdCliente == id);

            if (cliente == null)
            {
                return HttpNotFound();
            }

            var modelo = new ClienteDetallesViewModel
            {
                Cliente = cliente,
                Historial = new List<HistorialClienteViewModel>()
            };

            // Cargar empresa asociada (si el cliente tiene IdEmpresa)
            if (cliente.IdEmpresa.HasValue)
            {
                try
                {
                    using (var connection = new SqlConnection(_contexto.Database.Connection.ConnectionString))
                    {
                        connection.Open();
                        using (var command = new SqlCommand(@"
                            SELECT NombreEmpresa, Telefono1, CorreoElectronico, Direccion
                            FROM Empresas
                            WHERE IdEmpresa = @IdEmpresa", connection))
                        {
                            command.Parameters.AddWithValue("@IdEmpresa", cliente.IdEmpresa.Value);

                            using (var reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    modelo.NombreEmpresa = reader.IsDBNull(0) ? null : reader.GetString(0);
                                    modelo.TelefonoEmpresa = reader.IsDBNull(1) ? null : reader.GetString(1);
                                    modelo.CorreoEmpresa = reader.IsDBNull(2) ? null : reader.GetString(2);
                                    modelo.DireccionEmpresa = reader.IsDBNull(3) ? null : reader.GetString(3);
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    // Si hay algún problema al leer la empresa, simplemente no se muestra.
                }
            }

            // Si el cliente no tiene IdEmpresa, intentar obtener la empresa del último check-in
            if (!cliente.IdEmpresa.HasValue)
            {
                try
                {
                    using (var connection = new SqlConnection(_contexto.Database.Connection.ConnectionString))
                    {
                        connection.Open();
                        using (var command = new SqlCommand(@"
                            SELECT TOP 1 ci.IdEmpresa, e.NombreEmpresa, e.Telefono1, e.CorreoElectronico, e.Direccion
                            FROM CheckIn ci
                            LEFT JOIN Empresas e ON ci.IdEmpresa = e.IdEmpresa
                            WHERE ci.IdCliente = @IdCliente AND ci.IdEmpresa IS NOT NULL
                            ORDER BY ci.FechaCheckIn DESC", connection))
                        {
                            command.Parameters.AddWithValue("@IdCliente", cliente.IdCliente);

                            using (var reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    var idEmpresaUltimoCheckIn = reader.IsDBNull(0) ? (int?)null : reader.GetInt32(0);
                                    if (idEmpresaUltimoCheckIn.HasValue)
                                    {
                                        modelo.EmpresaSugeridaId = idEmpresaUltimoCheckIn.Value;
                                        modelo.EmpresaSugeridaNombre = reader.IsDBNull(1) ? null : reader.GetString(1);
                                    }

                                    if (string.IsNullOrEmpty(modelo.NombreEmpresa))
                                    {
                                        modelo.NombreEmpresa = reader.IsDBNull(1) ? null : reader.GetString(1);
                                        modelo.TelefonoEmpresa = reader.IsDBNull(2) ? null : reader.GetString(2);
                                        modelo.CorreoEmpresa = reader.IsDBNull(3) ? null : reader.GetString(3);
                                        modelo.DireccionEmpresa = reader.IsDBNull(4) ? null : reader.GetString(4);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    // Ignorar errores en la carga de empresa sugerida
                }
            }

            // Cargar historial de visitas del cliente usando las reservas ligadas a su IdUsuario
            if (!string.IsNullOrEmpty(cliente.IdUsuario))
            {
                var reservas = _reservasLN.ListarReservasUsuario(cliente.IdUsuario) ?? new List<ReservasDTO>();
                var reservasOrdenadas = reservas
                    .OrderByDescending(r => r.FechaInicio ?? DateTime.MinValue)
                    .ToList();

                int numeroEstancia = 1;
                foreach (var r in reservasOrdenadas)
                {
                    modelo.Historial.Add(new HistorialClienteViewModel
                    {
                        IdCliente = cliente.IdCliente,
                        NombreCompleto = cliente.NombreCompleto,
                        CedulaCliente = cliente.CedulaCliente ?? "",
                        TelefonoCliente = cliente.TelefonoCliente ?? "",
                        EmailCliente = cliente.EmailCliente ?? "",
                        NumeroEstancia = numeroEstancia++,
                        IdReserva = r.IdReserva,
                        FechaEntrada = r.FechaInicio,
                        FechaSalida = r.FechaFinal,
                        NumeroHabitacion = r.NumeroHabitacion,
                        CantidadPersonas = r.cantidadPersonas,
                        MontoTotal = r.MontoTotal,
                        EstadoReserva = r.EstadoReserva ?? "",
                        TieneReservas = true
                    });
                }

                if (modelo.Historial.Any())
                {
                    modelo.UltimaVisita = modelo.Historial
                        .OrderByDescending(h => h.FechaEntrada ?? DateTime.MinValue)
                        .First()
                        .FechaEntrada;
                }
            }

            return View(modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AsignarEmpresa(int idCliente, int? idEmpresa)
        {
            try
            {
                using (var connection = new SqlConnection(_contexto.Database.Connection.ConnectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand("UPDATE Cliente SET IdEmpresa = @IdEmpresa WHERE IdCliente = @IdCliente", connection))
                    {
                        command.Parameters.AddWithValue("@IdCliente", idCliente);
                        command.Parameters.AddWithValue("@IdEmpresa", (object)idEmpresa ?? DBNull.Value);
                        command.ExecuteNonQuery();
                    }
                }

                TempData["MensajeEmpresa"] = "Empresa actualizada correctamente para el cliente.";
            }
            catch (Exception ex)
            {
                TempData["ErrorEmpresa"] = "Error al actualizar la empresa del cliente: " + ex.Message;
            }

            return RedirectToAction("Detalles", new { id = idCliente });
        }

        private string BuildPaginacionHtml(string busqueda, string filtroUsuario, string ordenarPor, bool ordenAscendente, int pagina, int tamanoPagina, int totalPaginas, int totalClientes)
        {
            int inicioPagina = Math.Max(1, pagina - 2);
            int finPagina = Math.Min(totalPaginas, pagina + 2);
            int desde = (pagina - 1) * tamanoPagina + 1;
            int hasta = Math.Min(pagina * tamanoPagina, totalClientes);

            string Link(int p)
            {
                return Url.Action("Index", new { busqueda, filtroUsuario, ordenarPor, ordenAscendente, pagina = p, tamanoPagina }) ?? "#";
            }

            var html = new StringBuilder();
            html.Append("<div class=\"pagination-container mt-4\"><div class=\"d-flex justify-content-between align-items-center flex-wrap gap-3\">");
            html.Append("<div class=\"pagination-info\"><span class=\"text-muted\">Mostrando ").Append(desde).Append(" - ").Append(hasta).Append(" de ").Append(totalClientes).Append(" clientes</span></div>");
            html.Append("<nav aria-label=\"Paginación de clientes\"><ul class=\"pagination mb-0\">");

            if (pagina > 1)
                html.Append("<li class=\"page-item\"><a class=\"page-link\" href=\"").Append(Link(pagina - 1)).Append("\"><i class=\"fas fa-chevron-left\"></i> Anterior</a></li>");

            if (inicioPagina > 1)
            {
                html.Append("<li class=\"page-item\"><a class=\"page-link\" href=\"").Append(Link(1)).Append("\">1</a></li>");
                if (inicioPagina > 2)
                    html.Append("<li class=\"page-item disabled\"><span class=\"page-link\">...</span></li>");
            }

            for (int i = inicioPagina; i <= finPagina; i++)
            {
                string active = i == pagina ? " active" : "";
                html.Append("<li class=\"page-item").Append(active).Append("\"><a class=\"page-link\" href=\"").Append(Link(i)).Append("\">").Append(i).Append("</a></li>");
            }

            if (finPagina < totalPaginas)
            {
                if (finPagina < totalPaginas - 1)
                    html.Append("<li class=\"page-item disabled\"><span class=\"page-link\">...</span></li>");
                html.Append("<li class=\"page-item\"><a class=\"page-link\" href=\"").Append(Link(totalPaginas)).Append("\">").Append(totalPaginas).Append("</a></li>");
            }

            if (pagina < totalPaginas)
                html.Append("<li class=\"page-item\"><a class=\"page-link\" href=\"").Append(Link(pagina + 1)).Append("\">Siguiente <i class=\"fas fa-chevron-right\"></i></a></li>");

            html.Append("</ul></nav></div></div>");
            return html.ToString();
        }

        /// <summary>Consulta historial de clientes: búsqueda por nombre, cédula o teléfono; muestra todas las estancias (check-ins).</summary>
        [HttpGet]
        public ActionResult Historial(string busqueda = null)
        {
            ViewBag.Title = "Historial de Clientes";
            ViewBag.Busqueda = busqueda ?? "";
            var resultados = new List<HistorialClienteViewModel>();
            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                var clientes = _listarClientesLN.Listar(busqueda.Trim(), "NombreCliente", true);
                foreach (var c in clientes)
                {
                    var reservas = !string.IsNullOrEmpty(c.IdUsuario)
                        ? (_reservasLN.ListarReservasUsuario(c.IdUsuario) ?? new List<ReservasDTO>()).OrderByDescending(r => r.FechaInicio).ToList()
                        : new List<ReservasDTO>();
                    if (reservas.Count == 0)
                    {
                        resultados.Add(new HistorialClienteViewModel
                        {
                            IdCliente = c.IdCliente,
                            NombreCompleto = c.NombreCompleto,
                            CedulaCliente = c.CedulaCliente ?? "",
                            TelefonoCliente = c.TelefonoCliente ?? "",
                            EmailCliente = c.EmailCliente ?? "",
                            TieneReservas = false
                        });
                    }
                    else
                    {
                        int n = 1;
                        foreach (var r in reservas)
                        {
                            resultados.Add(new HistorialClienteViewModel
                            {
                                IdCliente = c.IdCliente,
                                NombreCompleto = c.NombreCompleto,
                                CedulaCliente = c.CedulaCliente ?? "",
                                TelefonoCliente = c.TelefonoCliente ?? "",
                                EmailCliente = c.EmailCliente ?? "",
                                NumeroEstancia = n++,
                                IdReserva = r.IdReserva,
                                FechaEntrada = r.FechaInicio,
                                FechaSalida = r.FechaFinal,
                                NumeroHabitacion = r.NumeroHabitacion,
                                CantidadPersonas = r.cantidadPersonas,
                                MontoTotal = r.MontoTotal,
                                EstadoReserva = r.EstadoReserva ?? "",
                                TieneReservas = true
                            });
                        }
                    }
                }
            }
            return View(resultados);
        }
    }
}

