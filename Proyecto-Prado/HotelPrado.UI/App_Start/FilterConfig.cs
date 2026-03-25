using System;
using System.Web;
using System.Web.Mvc;

namespace HotelPrado.UI
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            // Agregar filtro para suprimir errores de HttpContextWrapper
            filters.Add(new Filters.SuppressHttpContextExceptionFilter());
        }
    }
}
