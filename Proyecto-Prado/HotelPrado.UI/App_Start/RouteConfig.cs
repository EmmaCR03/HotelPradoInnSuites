using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace HotelPrado.UI
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Para que /Error no dé 404 (p. ej. cuando customErrors redirige a /Error)
            routes.MapRoute(
                name: "Error",
                url: "Error",
                defaults: new { controller = "Home", action = "Error" }
            );
            // Prueba: /Ping responde OK si la app corre (no usa BD)
            routes.MapRoute(
                name: "Ping",
                url: "Ping",
                defaults: new { controller = "Home", action = "Ping" }
            );
            // Diagnóstico: OWIN y BD (para hosteado)
            routes.MapRoute(
                name: "Diagnostico",
                url: "Diagnostico",
                defaults: new { controller = "Home", action = "Diagnostico" }
            );
            // Login/Registro de emergencia (vistas mínimas en Home, sin pasar por Account)
            routes.MapRoute(
                name: "Entrar",
                url: "Entrar",
                defaults: new { controller = "Home", action = "Entrar" }
            );
            routes.MapRoute(
                name: "Registro",
                url: "Registro",
                defaults: new { controller = "Home", action = "Registro" }
            );
            // Ver el error real que falla en Register (sirve en el servidor)
            routes.MapRoute(
                name: "VerErrorRegister",
                url: "VerErrorRegister",
                defaults: new { controller = "Home", action = "VerErrorRegister" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
