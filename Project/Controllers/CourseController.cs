using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Web;
using System.Web.Mvc;
using Project.Models;

namespace Project.Controllers {
    public class TempModel {
        public Course Cou { get; set; }
        public string Notification { get; set; }
    }

    public class CourseController : Controller {
        private ProjectEntities db = new ProjectEntities();

        // GET: Course
        [ HttpGet ]
        public ActionResult Index() {
            var courses = from e in db.Courses select new TempModel { Cou = e, Notification = "siuaiudaudahw"};

            return View( courses.ToList() );
        }

        [ HttpGet ]
        public ActionResult Detail(string id) {
            var result = from e in db.Courses where e.CourseID.ToString() == id select e;

            if ( result.ToList().Count == 0 ) {
                return RedirectToAction( "Index" , "Course" );
            }

            ViewBag.name = result.ToList()[0].Name;
            ViewBag.image = result.ToList()[0].Image;
            ViewBag.courseId = result.ToList()[0].CourseID;
            ViewBag.description = result.ToList()[0].Description;

            return View();
        }
    }
}