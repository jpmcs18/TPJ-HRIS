using System;
using System.Web.Mvc;
using System.Web.Routing;
using DataAccessLayer.System;

namespace WebTemplate
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            var page = new Page();
            var pages = page.GetRoutes();
            foreach (var p in pages)
            {
                routes.MapRoute(
                    name: p.URL,
                    url: String.Format("{0}/{1}", p.URL, "{action}"),
                    defaults: new { controller = p.Controller, action = p.Action },
                    namespaces: new[] { p.Namespace }
                );
            }

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}",
                defaults: new { controller = "Home", action = "Index" }
            );
        }
    }
}
