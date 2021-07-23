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
            return View(listCourse.ToList());

        }

        [HttpPost]
        [CheckSessionAdmin]
        public ActionResult Search(string s)
        {
            //get admin name
            int adminID = Convert.ToInt32(Session["admin_id"].ToString());

            var admin = from a in db.Admins where a.AdminID == adminID select a;
            ViewBag.adminName = admin.ToList()[0].Name;

            var listCourse = from c in db.Courses
                             where c.Name.Contains(s.Trim())
                             select new ListCourse
                             {
                                 course = c
                             };
            return View(listCourse.ToList());
        }

        [HttpGet]
        [CheckSessionAdmin]
        public ActionResult CourseDetail(string courseID)
        {
            
            int adminID = Convert.ToInt32(Session["admin_id"].ToString());

            var admin = from a in db.Admins where a.AdminID == adminID select a;
            ViewBag.adminName = admin.ToList()[0].Name;

            var selectedCourse = (from c in db.Courses where c.CourseID.ToString() == courseID.Trim() select c).ToList();
            if (selectedCourse.Count == 0)
            {
                return RedirectToAction("Index", "AdminCourse");
            }
            ViewBag.courseID = selectedCourse[0].CourseID;
            ViewBag.courseName = selectedCourse[0].Name;
            ViewBag.description = selectedCourse[0].Description;
            ViewBag.image = selectedCourse[0].Image;

            ViewBag.message = TempData["message"];
            TempData.Clear();
            return View();
        }

        [HttpPost]
        [CheckSessionAdmin]
        public ActionResult UpdateCourse(int id, string name, string description, string image)
        {
            //course name cannot duplicate
            /*var checkExisted = from e in db.Courses
                               where e.Name == name.Trim()
                               select e;

            if (checkExisted.ToList().Count != 0)
            {
                TempData["message"] = false;
                return RedirectToAction("CourseDetail", "AdminCourse", new { courseID = id });
            }*/
            //case when user input only space
            if (name.Trim().Equals("") || description.Trim().Equals("") || image.Trim().Equals(""))
            {
                TempData["message"] = false;
                return RedirectToAction("CourseDetail", "AdminCourse", new { courseID = id });
            }
            //update user profile
            try
            {
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

        [HttpGet]
        [CheckSessionAdmin]
        public ActionResult CourseAdd()
        {
            //get admin name
            int adminID = Convert.ToInt32(Session["admin_id"].ToString());
            var admin = from a in db.Admins where a.AdminID == adminID select a;
            ViewBag.adminName = admin.ToList()[0].Name;

            //set message
            ViewBag.message = TempData["message"];
            TempData.Clear();
            return View();
        }

        [HttpPost]
        [CheckSessionAdmin]
        public ActionResult AddingNewCourse(string name, string description, string image)
        {
            //course name cannot duplicate
            var checkExisted = from e in db.Courses
                               where e.Name == name
                               select e;

            if (checkExisted.ToList().Count != 0)
            {
                TempData["message"] = false;
                return RedirectToAction("CourseAdd", "AdminCourse");
            }
            //case when user input only space
            if(name.Trim().Equals("") || description.Trim().Equals("") || image.Trim().Equals(""))
            {
                TempData["message"] = false;
                return RedirectToAction("CourseAdd", "AdminCourse");
            }
            try
            {
                Course newCourse = new Course();
                // fields to be insert
                newCourse.Name = name.Trim();
                newCourse.Description = description.Trim();
                newCourse.Image = image;
                // executes the commands to implement the changes to the database
                db.Courses.Add(newCourse);
                db.SaveChanges();
                TempData["message"] = true;
                return RedirectToAction("CourseAdd", "AdminCourse");
            }
            catch (Exception)
            {
                TempData["message"] = false;
                return RedirectToAction("CourseAdd", "AdminCourse");
            }
        }
    }
}