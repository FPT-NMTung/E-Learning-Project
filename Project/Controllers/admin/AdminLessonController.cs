using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
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

            try {
                db.Lessions.Add( newLession );
                db.SaveChanges();
            } catch ( DbEntityValidationException dbEx ) {
                foreach ( var validationErrors in dbEx.EntityValidationErrors ) {
                    foreach ( var validationError in validationErrors.ValidationErrors ) {
                        System.Console.WriteLine( "Property: {0} Error: {1}", validationError.PropertyName,
                            validationError.ErrorMessage );
                    }
                }
            }

            return RedirectToAction( "Index" );
        }

        [ HttpGet ]
        [ CheckSessionAdmin ]
        public ActionResult Edit(string courseid, string lessonid) {
            var result = (from lesson in db.Lessions
                    where lesson.LessionID.ToString() == lessonid.Trim() &&
                          lesson.CourseID.ToString() == courseid.Trim()
                    select lesson)
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

            ViewBag.courseID = courseid;
            ViewBag.lessonID = lessonid;

            ViewBag.isExist = TempData["isExist"];
            ViewBag.isSave = TempData["isSave"];

            return View( result[0] );
        }

        [ HttpPost ]
        [ CheckSessionAdmin ]
        public ActionResult Update(string courseid, string lessonid, string id, string name, string description,
            string time) {
            if ( id == null || name == null || description == null || time == null ) {
                return RedirectToAction( "Index", "Error" );
            }

            if ( id.Trim() == "" || name.Trim() == "" || description.Trim() == "" || time.Trim() == "" ) {
                return RedirectToAction( "Index", "Error" );
            }

            var temp = db.Lessions.First( (e) => (e.LessionID.ToString() == lessonid.Trim()) );

            if ( temp.Video != id.Trim() ) {
                var check = (from a in db.Lessions where a.Video == id.Trim() select a).ToList();
                if ( check.Count != 0 ) {
                    TempData["isExist"] = true;
                    return Redirect( $"/admin/lesson/{courseid.Trim()}/{lessonid.Trim()}/edit" );
                }
            }

            int hour = Convert.ToInt32( time ) / 3600;
            int minute = (Convert.ToInt32( time ) % 3600) / 60;
            int second = Convert.ToInt32( time ) % 60;

            string resultTime = "";

            if ( hour == 0) {
                resultTime = $"{minute}:{second}";
            } else {
                resultTime = $"{hour}:{minute}:{second}";
            }

            var result = db.Lessions.First( (e) => (e.LessionID.ToString() == lessonid.Trim()) );

            result.Video = id.Trim();
            result.Name = name.Trim();
            result.Description = description.Trim();
            result.Time = resultTime;

            db.SaveChanges();

            TempData["isSave"] = true;
            return Redirect( $"/admin/lesson/{courseid.Trim()}/{lessonid.Trim()}/edit" );
        }

        [ HttpGet ]
        [ CheckSessionAdmin ]
        public ActionResult Delete(string courseid, string lessonid) {
            var result = db.Lessions.First( (e) => (e.LessionID.ToString() == lessonid.Trim()) );

            if ( result != null ) {
                db.Lessions.Remove( result );
                db.SaveChanges();
                return Redirect( $"/admin/lesson/{courseid.Trim()}" );
            }

            return Redirect( $"/admin/lesson/{courseid.Trim()}/{lessonid.Trim()}/edit" );
        }
    }
}