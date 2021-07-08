using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project.Models;

namespace Project.Controllers {
    public class QuizController : Controller {
        private ProjectEntities db = new ProjectEntities();

        // GET: Quiz
        public ActionResult Index(string QuizID) {
            int temp = 0;

            try {
                temp = Convert.ToInt32( QuizID );
            } catch ( Exception e ) {
                return RedirectToAction( "Index", "Course" );
            }

            var tempData =
                from a in db.QuizQuestions where temp == a.QuizID select a;

            return View(tempData.ToList());
        }
    }
}