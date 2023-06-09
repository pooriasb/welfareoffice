﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace BehzistiMaskan
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.MapRoute(
            //    name: "GetDocumentImage",
            //    url: "Client/GetClientDocumentImage/{id}/{documentTypeId}",
            //    defaults: new { controller = "Client", action = "GetClientDocumentImage" }
            //);

            routes.MapMvcAttributeRoutes();

            routes.MapRoute(
                name: "WithMessage",
                url: "{controller}/{action}/{id}/{message}",
                defaults: new { controller = "Home", action = "Index" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );




        }
    }
}
