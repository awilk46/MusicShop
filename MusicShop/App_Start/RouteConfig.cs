using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MusicShop
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                 name: "AlbumySzczegoly",
                 url: "album-{id}.html",
                 defaults: new { controller = "Albumy", action = "Szczegoly" });

            routes.MapRoute(
                 name: "KursyList",
                 url: "Kategoria/{nazwaKategorii}",
                 defaults: new { controller = "Albumy", action = "Lista" });

            routes.MapRoute(
                name: "StronyStatyczne",
                url: "strona/{nazwa}.html",
                defaults: new { controller = "Home", action = "StronyStatyczne" });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
