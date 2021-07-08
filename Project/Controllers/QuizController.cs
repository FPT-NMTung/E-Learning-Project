using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project.Filters;
using Project.Models;

namespace Project.Controllers {
    public class QuizController : Controller {
        private ProjectEntities db = new ProjectEntities();

        // GET: Quiz
        [HttpGet]
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

            return View(tempData.ToList());
        }

        [HttpPost]
        public string SubmitAction (FormCollection testCollection) {
            var value1 = Request.Form[ "listQuestion" ];

            string[] listQuestion = value1.Split( '-' );
            int totalQues = listQuestion.Length;
            int correctQues = 0;

            foreach ( string s in listQuestion ) {
                string tempS = Request.Form [ "ques" + s ];

                if ( tempS == null) {
                    continue;
                }

                var temp = from a in db.QuizQuestionAnswers where a.AnsID.ToString() == tempS select a.IsTrue;
                if ( temp.ToList()[0]) {
                    correctQues++;
                }
            }

            float score = (float) correctQues / totalQues;

            return score.ToString();
        }
    }
}