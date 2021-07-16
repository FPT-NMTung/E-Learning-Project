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
            ViewBag.quizId = QuizID;

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

            var check = from e in db.UserAndCourses
                where e.CourseID == courseId && e.UserID == userId
                select e.Score;

            if ( check.ToList().Count == 0 ) {
                var tempObj = new UserAndCourse() {
                    CourseID = courseId,
                    UserID = Convert.ToInt32( Session["user_id"] ),
                    Score = score
                };

                db.UserAndCourses.Add( tempObj );
                db.SaveChanges();
            } else {
                if ( check.ToList()[0] < score ) {
                    var update = db.UserAndCourses.First( el => (el.CourseID == courseId && el.UserID == userId) );
                    update.Score = score;

                    db.SaveChanges();
                }
            }

            return Redirect( $"/exam/result/{courseId}" );
        }

        [ HttpGet ]
        [ CheckSession ]
        public ActionResult Result(string courseId) {
            string red = "#c2382f";
            string yellow = "#e0cf31";
            string green = "#34e031";

            int userId = Convert.ToInt32( Session["user_id"].ToString() );
            int course = Convert.ToInt32( courseId );

            var score = (from a in db.UserAndCourses where a.CourseID == course && a.UserID == userId select a.Score).ToList()[0];
            var name = (from b in db.Courses where b.CourseID == course select b.Name).ToList()[0];

            if (score < 50) {
                ViewBag.color = red;
            } else if ( score < 80 ) {
                ViewBag.color = yellow;
            } else {
                ViewBag.color = green;
            }

            ViewBag.score = score;
            ViewBag.name = name;

            return View();
        }
        //public void Timer1_Tick(object sender, EventArgs e)
        //{
        //    TimeSpan ts5sec = new TimeSpan(0, 0, 5); // 5 seconds
        //    TimeSpan ts = (TimeSpan)Session["CountdownTimer"]; // current remaining time from Session
        //    TimeSpan current = ts - ts5sec; // Subtract 5 seconds
        //    ViewBag.Label1 = current.ToString("%m") + " minutes and " + current.ToString("%s") + " seconds";
        //    Session["CountdownTimer"] = current;  // put new remaining time in Session 
           
        //    //Label1.Text = DateTime.Now.Second.ToString();
        //}
        //static int hh, mm, ss;

        //static double TimeAllSecondes = 5;
        //protected void Page_Load(object sender, EventArgs e)
        //{

        //}

        //protected void Timer1_Tick(object sender, EventArgs e)
        //{
        //    if (TimeAllSecondes > 0)
        //    {
        //        TimeAllSecondes = TimeAllSecondes - 1;
        //    }

        //    TimeSpan time_Span = TimeSpan.FromSeconds(TimeAllSecondes);
        //    hh = time_Span.Hours;
        //    mm = time_Span.Minutes;
        //    ss = time_Span.Seconds;

        //    ViewBag.Label2 = "  " + hh + ":" + mm + ":" + ss;
        //}
    }
}