using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Web;
using System.Web.Mvc;
using Project.Models;

namespace Project.Controllers {

    public class CourseController : Controller {
        private ProjectEntities db = new ProjectEntities();

        // GET: Course
        [ HttpGet ]
        public ActionResult Index() {
            var courses = from e in db.Courses select e;
            var detaillession = from l in db.Lessions select l;

            return View( courses.ToList() );
        }

        [ HttpGet ]
        public ActionResult Detail(string id) {
            var result = from e in db.Courses where e.CourseID.ToString() == id select e;
            //var lession = from l in db.Lessions where l.LessionID.ToString() == id select l;
            if ( result.ToList().Count == 0 ) {
                return RedirectToAction( "Index" , "Course" );
            }

            ViewBag.name = result.ToList()[0].Name;
            ViewBag.image = result.ToList()[0].Image;
            ViewBag.courseId = result.ToList()[0].CourseID;
            ViewBag.description = result.ToList()[0].Description;

            //if (lession.ToList().Count == 0)
            //{
            //    return RedirectToAction("Index", "Course");
            //}
            //ViewBag.countvid = lession.ToList().Count[0].Countvid;


            return View();
        }
    }
}