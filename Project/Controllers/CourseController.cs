using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Web;
using System.Web.Mvc;
using Project.Filters;
using Project.Models;

namespace Project.Controllers {
    public class CourseList {
        public List<Course> course { get; set; }
        public List<Course> courseJoin { get; set; }
        public List<int> IdVideoList { get; set; }
    }

    public class CourseController : Controller {
        private ProjectEntities db = new ProjectEntities();

        // GET: Course
        [ HttpGet ]
        public ActionResult Index() {
            if ( Session["user_id"] == null ) {
                var courses = from e in db.Courses select e;

                CourseList list = new CourseList();
                list.course = courses.ToList();

                return View( list );
            } else {
                int userId = Convert.ToInt32( Session["user_id"].ToString() );

                var courseJoin = (from cour in db.UserAndCourses
                    join course in db.Courses on cour.CourseID equals course.CourseID
                    where cour.UserID == userId
                    select course).ToList();

                List<int> courseIdList = new List<int>();

                foreach ( var el in courseJoin ) {
                    courseIdList.Add( el.CourseID );
                }

                var courses = (from e in db.Courses select e).ToList();

                List<Course> tempCourses = new List<Course>();
                List<int> tempList = new List<int>();

                foreach ( var el in courses ) {
                    var a = !courseIdList.Contains( el.CourseID );
                    if ( !courseIdList.Contains( el.CourseID ) ) {
                        tempCourses.Add( el );
                    }
                }

                /*foreach ( var el in courseIdList ) {
                    var temp
                        = (from lesson in db.Lessions where lesson.CourseID == el select lesson).ToList()[0].LessionID;
                    tempList.Add( temp );
                }*/

                CourseList list = new CourseList();
                list.course = tempCourses;
                list.courseJoin = courseJoin;
                /*list.IdVideoList = tempList;*/

                return View( list );
            }
        }

        [ HttpGet ]
        public ActionResult Detail(string id) {
            var result = from e in db.Courses where e.CourseID.ToString() == id select e;
            //var lession = from l in db.Lessions where l.LessionID.ToString() == id select l;
            if ( result.ToList().Count == 0 ) {
                return RedirectToAction( "Index", "Course" );
            }

            ViewBag.name = result.ToList()[0].Name;
            ViewBag.image = result.ToList()[0].Image;
            ViewBag.courseId = result.ToList()[0].CourseID;
            ViewBag.description = result.ToList()[0].Description;

            var temp
                = (from lesson in db.Lessions where lesson.CourseID.ToString() == id select lesson).ToList();


            if ( temp.ToList().Count == 0 ) {
                ViewBag.lessonId = -1;
            } else {
                ViewBag.lessonId = temp[0].LessionID;
            }

            //get data for course detail page
            var numberOfLesson = (from e in db.Lessions where e.CourseID.ToString() == id select e).Count();
            ViewBag.numberOfLesson = numberOfLesson;

            var numberOFQuiz = (from e in db.Quizs where e.CourseID.ToString() == id select e).Count();
            ViewBag.numberOFQuiz = numberOFQuiz;

            return View();
        }

        [ HttpGet ]
        [ CheckSession ]
        public ActionResult Register(string courseid) {
            if ( courseid == null ) {
                return RedirectToAction( "Index" );
            }

            if ( courseid.Trim() == "" ) {
                return RedirectToAction( "Index" );
            }

            int userId = Convert.ToInt32( Session["user_id"] );
            int courseID = Convert.ToInt32( courseid.Trim() );

            var check
                = (from unc in db.UserAndCourses where unc.CourseID == courseID && userId == unc.UserID select unc)
                .ToList();

            if ( check.Count == 0 ) {
                UserAndCourse temp = new UserAndCourse() {
                    CourseID = courseID,
                    UserID = userId
                };

                db.UserAndCourses.Add( temp );
                db.SaveChanges();
            }
            // get first lesson id
            // go to learning page

            var listLesson = (from lesson in db.Lessions where lesson.CourseID == courseID select lesson).ToList();

            if (listLesson.Count == 0) {
                return RedirectToAction( "Index" );
            }

            int lessonId = listLesson[0].LessionID;

            return Redirect( $"/learning/{courseID}/{lessonId}" );
        }
    }
}