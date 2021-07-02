using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Controllers {
    public class CourseController : Controller {
        private ProjectEntities db = new ProjectEntities();

        // GET: Course
        [ HttpGet ]
        public ActionResult Index() {
            var courses = from e in db.Courses select e;

            return View(courses.ToList());
        }
        [HttpGet]
        public ActionResult Detail()
        {
            return View();
        }
    }
}