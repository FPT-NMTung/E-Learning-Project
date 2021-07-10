using System;
using System.Collections.Generic;
using System.Linq;
using Project.Filters;
using System.Web;
using System.Web.Mvc;

namespace Project.Controllers
{
    public class LearningController : Controller
    {
        // GET: Learning
        [CheckSession]
        public ActionResult Index()
        {
            return View();
        }
    }
}