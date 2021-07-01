using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Login.Filters;

namespace Project.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        [HttpGet]
        [CheckSession]
        public ActionResult Index() {
            return View();
        }
    }
}