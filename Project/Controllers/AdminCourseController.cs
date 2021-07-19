using Project.Filters;
using Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Controllers
{
    public class ListCourse
    {
        public Course course { get; set; }
    }
    public class AdminCourseController : Controller
    {
        private ProjectEntities db = new ProjectEntities();
        // GET: AdminCourse
        [HttpGet]
        [CheckSessionAdmin]
        public ActionResult Index()
        {
            //get admin name
            int adminID = Convert.ToInt32(Session["admin_id"].ToString());

            var admin = from a in db.Admins where a.AdminID == adminID select a;
            ViewBag.adminName = admin.ToList()[0].Name;

            var listCourse = from c in db.Courses
                             select new ListCourse
                             {
                                 course = c
                             };
            return View(listCourse);

        }

        [HttpGet]
        [CheckSessionAdmin]
        public ActionResult CourseDetail(int courseID)
        {

            int adminID = Convert.ToInt32(Session["admin_id"].ToString());

            var admin = from a in db.Admins where a.AdminID == adminID select a;
            ViewBag.adminName = admin.ToList()[0].Name;

            var selectedCourse = from c in db.Courses where c.CourseID == courseID select c;
            if (selectedCourse.Count() == 0)
            {
                return RedirectToAction("Index", "Error");
            }
            ViewBag.courseID = selectedCourse.ToList()[0].CourseID;
            ViewBag.courseName = selectedCourse.ToList()[0].Name;
            ViewBag.description = selectedCourse.ToList()[0].Description;
            ViewBag.image = selectedCourse.ToList()[0].Image;

            ViewBag.message = TempData["message"];
            TempData.Clear();
            return View();
        }

        [HttpPost]
        [CheckSessionAdmin]
        public ActionResult UpdateCourse(int id, string name, string description, string image)
        {
            try
            {
                //update user profile
                var update = db.Courses.First(g => g.CourseID == id);
                update.Name = name.Trim();
                update.Description = description.Trim();
                update.Image = image.Trim();
                db.SaveChanges();
                TempData["message"] = true;
                return RedirectToAction("CourseDetail", "AdminCourse", new { courseID = id });
            }
            catch (Exception)
            {
                TempData["message"] = false;
                return RedirectToAction("CourseDetail", "AdminCourse", new { courseID = id });
            }
        }

        [HttpPost]
        [CheckSessionAdmin]
        public ActionResult DeleteCourse(int id)
        {
            try
            {
                var remove = (from e in db.Courses
                              where e.CourseID == id                             
                              select e).FirstOrDefault();
                if (remove != null)
                {
                    db.Courses.Remove(remove);
                    db.SaveChanges();
                }
                return RedirectToAction("Index", "AdminCourse");
            }
            catch (Exception)
            {
                return RedirectToAction("CourseDetail", "AdminCourse", new { courseID = id });
            }
        }
    }
}