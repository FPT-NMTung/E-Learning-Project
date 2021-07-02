using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project.Filters;
using Project.Models;

namespace Project.Controllers {
    public class LoginController : Controller {
        private ProjectEntities db = new ProjectEntities();

        [ HttpGet ]
        [ CheckNotSession ]
        public ActionResult Index() {
            ViewBag.isLogged = TempData["isLogged"];
            TempData.Clear();
            return View();
        }

        [ HttpPost ]
        [ CheckNotSession ]
        public ActionResult Login(string email, string password) {
            var result = from e in db.Users where e.Email == email && e.Password == password select e;

            if ( result.ToList().Count != 0 ) {
                Session["email"] = result.ToList()[0].Email;
                Session["user_id"] = result.ToList()[0].UserID;
                Session["name"] = result.ToList()[0].Name;
                return RedirectToAction( "Index", "Home" );
            } else {
                TempData["isLogged"] = false;
                return RedirectToAction( "Index", "Login" );
            }
        }

        [ HttpGet ]
        [ CheckSession ]
        public ActionResult Logout() {
            Session.Clear();

            return RedirectToAction( "Index", "Home" );
        }

        [ HttpGet ]
        [ CheckNotSession ]
        public ActionResult Register() {
            ViewBag.isCreated = TempData["isCreated"];
            TempData.Clear();
            return View();
        }

        [ HttpPost ]
        [ CheckNotSession ]
        public ActionResult RegisterAccount(string name, string email, string phone, string password, string address,
            string gender, string username) {
            var result = from e in db.Users
                where e.Email == email || e.Username == username || e.PhoneNumber == phone
                select e;

            if ( result.ToList().Count != 0 ) {
                TempData["isCreated"] = false;
                return RedirectToAction( "Register", "Login" );
            }

            User newUser = new User() {
                Name = name,
                Email = email,
                PhoneNumber = phone,
                Password = password,
                Address = address,
                Gender = gender == "male",
                Username = username
            };

            db.Users.Add( newUser );
            db.SaveChanges();

            return RedirectToAction( "Index", "Login" );
        }
    }
}