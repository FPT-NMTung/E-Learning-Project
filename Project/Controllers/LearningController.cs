﻿using Project.Filters;
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
            var learningInfo = from l in db.Lessions
                               join c in db.Courses on l.CourseID equals c.CourseID
                               join ul in db.UserAndLessions on l.LessionID equals ul.LessionID
                               where l.CourseID == courseId && ul.UserID == userId
                               select new TempLession
                               {
                                   lesson = l,
                                   userAndLession = ul,
                                   course = c
                               };

            var srcVideo = from l in learningInfo where l.lesson.LessionID == lessonId select l;
            ViewBag.srcVideo = srcVideo.ToList()[0].lesson.Video;
            ViewBag.description = learningInfo.ToList()[0].lesson.Description;
            ViewBag.title = learningInfo.ToList()[0].lesson.Name;
            //var course = from c in db.Courses where c.CourseID == courseId select c;
            //ViewBag.courseName = course.ToList()[0].Name;
            ViewBag.courseName = learningInfo.ToList()[0].course.Name;
            return View(learningInfo.ToList());
        }
    }
}