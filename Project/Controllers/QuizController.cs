using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project.Filters;
using Project.Models;

namespace Project.Controllers {
    public class QuizController : Controller {
        private ProjectEntities db = new ProjectEntities();

        // GET: Quiz
        [ HttpGet ]
        [ CheckSession ]
        public ActionResult Index(string courseId) {
            int userID = Convert.ToInt32( Session["user_id"].ToString() );

            var numberLessonLearned = (from lesson in db.Lessions
                join userAndLession in db.UserAndLessions on lesson.LessionID equals userAndLession.LessionID
                where lesson.CourseID.ToString() == courseId && userAndLession.UserID == userID
                select lesson).ToList().Count;

            var totalLesson = (from lesson in db.Lessions where lesson.CourseID.ToString() == courseId select lesson)
                .ToList().Count;

            if ( numberLessonLearned != totalLesson ) {
                return RedirectToAction( "Index", "Course" );
            }

            var QuizIDList = (from quiz in db.Quizs where quiz.CourseID.ToString() == courseId select quiz).ToList();

            if ( QuizIDList.Count == 0 ) {
                return RedirectToAction( "Index", "Error" );
            }

            var QuizID = QuizIDList[0].QuizID;

            int temp = 0;

            try {
                temp = Convert.ToInt32( QuizID );
            } catch ( Exception e ) {
                return RedirectToAction( "Index", "Course" );
            }

            var tempData =
                from a in db.QuizQuestions where temp == a.QuizID select a;

            List<string> t = new List<string>();
            foreach ( QuizQuestion question in tempData ) {
                t.Add( question.QuesID.ToString() );
            }

            ViewBag.listQuestion = String.Join( "-", t.ToArray() );
            ViewBag.quizId = QuizID;
            ViewBag.NameCourse = (from course in db.Courses
                where course.CourseID.ToString() == courseId.Trim()
                select course).ToList()[0].Name;

            return View( tempData.ToList() );
        }

        [ HttpPost ]
        [ CheckSession ]
        public ActionResult SubmitAction(FormCollection testCollection) {
            var value1 = Request.Form["listQuestion"];
            var quizId = Request.Form["quizId"];

            string[] listQuestion = value1.Split( '-' );
            int totalQues = listQuestion.Length;
            int correctQues = 0;

            foreach ( string s in listQuestion ) {
                string tempS = Request.Form["ques" + s];

                if ( tempS == null ) {
                    continue;
                }

                var temp = from a in db.QuizQuestionAnswers where a.AnsID.ToString() == tempS select a;
                if ( temp.ToList()[0].IsTrue ) {
                    correctQues++;
                }
            }

            float score = correctQues * 100 / totalQues;

            var courseId = (from d in db.Quizs where d.QuizID.ToString() == quizId select d.CourseID).ToList()[0];

            int userId = Convert.ToInt32( Session["user_id"].ToString() );

            var check = (from e in db.UserAndCourses
                where e.CourseID == courseId && e.UserID == userId
                select e.Score).ToList();

            if ( check[0] == null || check[0] < score ) {
                var update = db.UserAndCourses.First( el => (el.CourseID == courseId && el.UserID == userId) );
                update.Score = score;

                db.SaveChanges();
            }

            TempData["score"] = score;
            return RedirectToAction( "Result" );
        }

        [ HttpGet ]
        [ CheckSession ]
        public ActionResult Result() {
            int score = 0;
            var temp = TempData["score"];

            if ( temp != null ) {
                score = Convert.ToInt32( TempData["score"].ToString() );
            } else {
                return RedirectToAction( "Index", "Error" );
            }

            string red = "#c2382f";
            string yellow = "#e0cf31";
            string green = "#34e031";

            if ( score < 50 ) {
                ViewBag.color = red;
            } else if ( score < 80 ) {
                ViewBag.color = yellow;
            } else {
                ViewBag.color = green;
            }

            ViewBag.score = score;

            return View();
        }
    }
}