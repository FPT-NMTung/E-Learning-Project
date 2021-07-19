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
        //[CheckSessionAdmin]
        public ActionResult Index()
        {
            var listCourse = from c in db.Courses
                             select new ListCourse
                             {
                                 course = c
                             };

            return View(listCourse);
        }
    }
}