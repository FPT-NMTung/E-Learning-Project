using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project.Filters;
using Project.Models;

namespace Project.Controllers.admin {
    public class AdminLessonController : Controller {
        private ProjectEntities db = new ProjectEntities();

        // GET: AdminLesson
        [ HttpGet ]
        [ CheckSessionAdmin ]
        public ActionResult Index(string courseid) {
            var checkExist = (from course in db.Courses where course.CourseID.ToString() == courseid select course)
                .ToList().Count == 0;

            if ( checkExist ) {
                return RedirectToAction( "Index", "Error" );
            }

            var result
                = (from lesson in db.Lessions where lesson.CourseID.ToString() == courseid select lesson).ToList();

            if ( result.Count == 0 ) {
                ViewBag.isEmpty = true;
            }

            ViewBag.courseID = courseid;
            return View( result );
        }
    }
}