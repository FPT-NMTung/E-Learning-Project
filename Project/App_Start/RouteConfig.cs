using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Policy;
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
                name: "Learning" ,
                url: "learning/{courseId}/{videoId}",
                defaults: new { controller = "Profile" , action = "EditProfile" }
            );

            routes.MapRoute(
                name: "EditProfile",
                url: "profile/edit",
                defaults: new { controller = "Profile", action = "EditProfile" }         
            );

            routes.MapRoute(
                name: "Profile",
                url: "profile",
                defaults: new { controller = "Profile", action = "Index" }
            );

            routes.MapRoute(
                name: "Detail Courses",
                url: "course/{id}",
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
                name: "Home",
                url: "",
                defaults: new { controller = "Home", action = "Index"}
            );

            routes.MapRoute(
                "Default" ,                                              // Route name
                "{controller}/{action}/{id}" ,                           // URL with parameters
                new { controller = "Home" , action = "Index" , id = "" }  // Parameter defaults
            );

            routes.MapRoute(
                name: "404Page" ,
                url: "{*url}" ,
                defaults: new { controller = "Error" , action = "Index" }
            );

            routes.MapRoute(
                name: "Learning1",
                url: "learning",
                defaults: new { controller = "Learning", action = "Index" }
            );
        }
        
    }
}
