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

        [ HttpGet ]
        [ CheckSessionAdmin ]
        public ActionResult Add(string courseid) {
            var checkExist = (from course in db.Courses where course.CourseID.ToString() == courseid select course)
                .ToList().Count == 0;

            if ( checkExist ) {
                return RedirectToAction( "Index", "Error" );
            }

            ViewBag.isUpdate = TempData["isUpdate"];
            ViewBag.courseID = courseid;

            return View();
        }

        [ HttpPost ]
        [ ActionName( "Add" ) ]
        [ CheckSessionAdmin ]
        public ActionResult AddLesson(string courseid, string id, string name, string description, string time) {
            // your code
            if ( id == null || name == null || description == null || time == null ) {
                TempData["isUpdate"] = false;
                return Redirect( $"/admin/lesson/{courseid}/add" );
            }

            if ( id.Trim() == "" || name.Trim() == "" || description.Trim() == "" || time.Trim() == "" ) {
                TempData["isUpdate"] = false;
                return Redirect( $"/admin/lesson/{courseid}/add" );
            }

            var checkExist = (from lesson in db.Lessions where lesson.Video == id.Trim() select lesson).ToList();

            if ( checkExist.Count != 0 ) {
                TempData["isUpdate"] = false;
                return Redirect( $"/admin/lesson/{courseid}/add" );
            }

            Lession newLession = new Lession() {
                CourseID = Convert.ToInt32( courseid ),
                Name = name.Trim(),
                Video = id.Trim(),
                Description = description.Trim(),
                Time = time.Trim()
            };

            db.Lessions.Add( newLession );
            db.SaveChanges();

            return RedirectToAction( "Index" );
        }

        [ HttpGet ]
        [ CheckSessionAdmin ]
        public ActionResult Edit(string courseid, string lessonid) {
            var result = (from lesson in db.Lessions where lesson.LessionID.ToString() == lessonid.Trim() && lesson.CourseID.ToString() == courseid.Trim() select lesson)
                .ToList();

            if ( result.Count == 0 ) {
                return Redirect( $"/admin/lesson/{courseid}" );
            }

            try {
                int.Parse( courseid );
                int.Parse( lessonid );
            } catch ( Exception e ) {
                return RedirectToAction( "Index", "Error" );
            }

            return View( result[0] );
        }
    }
}