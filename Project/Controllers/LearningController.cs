using Project.Filters;
using Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Controllers
{
    public class TempLession
    {
        public Course course { get; set; }
        public Lession lesson { get; set; }
        public UserAndLession userAndLession { get; set; }

    }
    public class LearningController : Controller
    {
        private ProjectEntities db = new ProjectEntities();
        // GET: Learning
        [CheckSession]
        public ActionResult Index(int courseId, int lessonId)
        {
            int userId = Convert.ToInt32(Session["user_id"].ToString());

            var checkUserLearnLesson = from e in db.UserAndLessions 
                                       where e.UserID == userId && e.LessionID == lessonId
                                       select e;
            //case if user have not learn this lesson before
            if(checkUserLearnLesson.ToList().Count == 0)
            {
                //user learn this lesson
                UserAndLession newUserAndLesson = new UserAndLession()
                {
                    UserID = userId,
                    LessionID = lessonId,
                    Watched = true
                };
                db.UserAndLessions.Add(newUserAndLesson);
                db.SaveChanges();
            }

            //select all lesson in course
            var learningInfo = from l in db.Lessions
                               where l.CourseID == courseId
                               select l;

            //get data in table lesson
            var lessonInfor = from l in learningInfo where l.LessionID == lessonId select l;

            //case when lesson not existed
            if (lessonInfor.ToList().Count == 0)
            {
                return RedirectToAction("Index", "Error");
            }

            //case when have at least 1 lesson
            ViewBag.srcVideo = lessonInfor.ToList()[0].Video;
            ViewBag.description = lessonInfor.ToList()[0].Description;
            ViewBag.title = lessonInfor.ToList()[0].Name;

            var courseName = from c in db.Courses where c.CourseID == courseId select c;
            ViewBag.courseName = courseName.ToList()[0].Name;
            
            return View(learningInfo.ToList());
        }

    }
}