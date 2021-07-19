using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project.Filters;
using Project.Models;

namespace Project.Controllers {
    public class AdminLoginController : Controller {
        private ProjectEntities db = new ProjectEntities();

        // GET: AdminLogin
        [ HttpGet ]
        [ CheckNotSessionAdmin ]
        public ActionResult Index() {
            ViewBag.isLogged = TempData["isLogged"];
            TempData.Clear();
            return View();
        }

        [ HttpPost ]
        [ CheckNotSessionAdmin ]
        public ActionResult Login(string email, string password) {
            var temp = (from admin in db.Admins where admin.Email == email && admin.Password == password select admin)
                .ToList();

            if ( temp.Count != 0 ) {
                Session["admin_id"] = temp[0].AdminID;
                Session["name"] = temp[0].Name;

                return RedirectToAction( "Index", "AdminUser" );
            }

            TempData["isLogged"] = false;

            return RedirectToAction( "Index" );
        }

        [ HttpGet ]
        [ CheckSessionAdmin ]
        public ActionResult Logout() {
            Session.Clear();

            return RedirectToAction( "Index" );
        }
    }
}