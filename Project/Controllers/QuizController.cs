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
        public ActionResult Index(string QuizID) {
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

            return View( tempData.ToList() );
        }

        [ HttpPost ]
        [ CheckSession ]
        public string SubmitAction(FormCollection testCollection) {
            var value1 = Request.Form["listQuestion"];
            var quizId = Request.Form [ "quizId" ];

            string [] listQuestion = value1.Split( '-' );
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
            var check = from e in db.UserAndCourses
                where e.CourseID == courseId && e.UserID.ToString() == Session["user_id"].ToString()
                select e.Score;

            if (check.ToList().Count == 0) {
                var tempObj = new UserAndCourse() {
                    CourseID = courseId,
                    UserID = Convert.ToInt32( Session["user_id"] ),
                    Score = score
                };

                db.UserAndCourses.Add( tempObj );
                db.SaveChanges();
            } else {
                //update
            }

            return score.ToString();
        }
    }
}