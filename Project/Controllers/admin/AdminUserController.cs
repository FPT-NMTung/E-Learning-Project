using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project.Filters;
using Project.Models;

namespace Project.Controllers {
    public class AdminUserController : Controller {
        private ProjectEntities db = new ProjectEntities();

        // GET: AdminUser
        [HttpGet]
        [CheckSessionAdmin]
        public ActionResult Index() {
            var result = (from user in db.Users select user).ToList();
            
            return View( result );
        }

        [HttpPost]
        [CheckSessionAdmin]
        public ActionResult Search(string s) {
            var result = ( from user in db.Users where user.Name.Contains( s ) select user ).ToList();

            return View( result );
        }
    }
}