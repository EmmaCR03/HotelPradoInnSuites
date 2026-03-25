using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace HotelPrado.UI.Controllers
{
    public class TipoCambioController : Controller
    {
        private readonly string API_URL = "https://tipodecambio.cr/api/rates";

        [HttpGet]
        public async Task<JsonResult> ObtenerTipoCambio()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(10);
                    var response = await client.GetStringAsync(API_URL);
                    
                    // La API devuelve un array de objetos con diferentes instituciones
                    // Buscamos el tipo de cambio del BCCR (Banco Central)
                    dynamic data = JsonConvert.DeserializeObject(response);
                    
                    decimal tipoCambio = 0;
                    
                    // Buscar el tipo de cambio del BCCR
                    foreach (var item in data)
                    {
                        if (item.institution != null && item.institution.ToString().ToUpper().Contains("BCCR"))
                        {
                            if (item.buy != null)
                            {
                                tipoCambio = Convert.ToDecimal(item.buy);
                                break;
                            }
                        }
                    }
                    
                    // Si no encontramos BCCR, usar el primero disponible
                    if (tipoCambio == 0 && data.Count > 0)
                    {
                        if (data[0].buy != null)
                        {
                            tipoCambio = Convert.ToDecimal(data[0].buy);
                        }
                    }
                    
                    return Json(new { success = true, tipoCambio = tipoCambio }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                // En caso de error, devolver un valor por defecto o null
                return Json(new { success = false, error = ex.Message, tipoCambio = 0 }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}

