using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Project
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Detail Courses",
                url: "course/detail",
                defaults: new { controller = "Course", action = "Detail" }
            );

            routes.MapRoute(
                name: "Courses" ,
                url: "course" ,
                defaults: new { controller = "Course" , action = "Index" }
            );

            routes.MapRoute(
                name: "logout" ,
                url: "logout" ,
                defaults: new { controller = "Login" , action = "Logout"}
            );

            routes.MapRoute(
                name: "Login" ,
                url: "login" ,
                defaults: new { controller = "Login" , action = "Index"}
            );

            routes.MapRoute(
                name: "Register" ,
                url: "register" ,
                defaults: new { controller = "Login" , action = "Register" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
        
    }
}
