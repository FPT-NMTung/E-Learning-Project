using Project.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Controllers
{
    public class ProfileController : Controller
    {
        // GET: Profile
        [HttpGet]
        [CheckSession]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [CheckSession]
        public ActionResult EditProfile()
        {
            return View();
        }
    }
}