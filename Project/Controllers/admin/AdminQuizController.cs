using Project.Filters;
using Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Controllers
{
    public class ListQuiz
    {
        public Quiz quiz { get; set; }
        public QuizQuestion quizQuestion { get; set; }
        public QuizQuestionAnswer quizQuestionAnswer { get; set; }
    }
    public class AdminQuizController : Controller
    {
        private ProjectEntities db = new ProjectEntities();
        // GET: AdminQuiz
        [HttpGet]
        [CheckSessionAdmin]
        public ActionResult Index(string courseID)
        {
            //get admin name
            int adminID = Convert.ToInt32(Session["admin_id"].ToString());
            var admin = from a in db.Admins where a.AdminID == adminID select a;
            ViewBag.adminName = admin.ToList()[0].Name;

            var listQuestion = from q in db.Quizs
                               join qq in db.QuizQuestions on q.QuizID equals qq.QuizID
                               where q.CourseID.ToString() == courseID
                               select new ListQuiz
                               {
                                   quiz = q,
                                   quizQuestion = qq
                               };
            if (listQuestion.Count() != 0)
            {
                var questionID = from e in listQuestion
                                 where e.quiz.CourseID.ToString() == courseID
                                 select e;

                ViewBag.questionID = questionID.ToList()[0].quizQuestion.QuesID;
            }
            //get courseID
            ViewBag.courseID = courseID;
            return View(listQuestion.ToList());
        }

        [HttpGet]
        [CheckSessionAdmin]
        public ActionResult QuizAdd(string courseID)
        {
            //get admin name
            int adminID = Convert.ToInt32(Session["admin_id"].ToString());
            var admin = from a in db.Admins where a.AdminID == adminID select a;
            ViewBag.adminName = admin.ToList()[0].Name;

            ViewBag.courseID = courseID;
            //set message
            ViewBag.message = TempData["message"];
            TempData.Clear();
            return View();
        }

        [HttpPost]
        [CheckSessionAdmin]
        public ActionResult AddQuestion(string courseID, string question, string answer1, string answer2, string answer3, string answer4,
                                         string trueAns)
        {
            /*var listQuestion = from q in db.Quizs
                               join qq in db.QuizQuestions on q.QuizID equals qq.QuizID
                               where q.CourseID.ToString() == courseID
                               select new ListQuiz
                               {
                                   quiz = q,
                                   quizQuestion = qq
                               };*/

            if (question.Trim().Equals("") || answer1.Trim().Equals("") || answer2.Trim().Equals("") || answer3.Trim().Equals("") || answer4.Trim().Equals("")
                || answer1.Trim().Equals(answer2.Trim()) || answer1.Trim().Equals(answer3.Trim()) || answer1.Trim().Equals(answer4.Trim())
                || answer2.Trim().Equals(answer3.Trim()) || answer2.Trim().Equals(answer4.Trim()) || answer3.Trim().Equals(answer4.Trim()))
            {
                TempData["message"] = false;
                return RedirectToAction("QuizAdd", "AdminQuiz", new { courseID = courseID });
            }




            var listQuiz = (from e in db.Quizs where e.CourseID.ToString() == courseID select e).ToList();

            if (listQuiz.Count == 0)
            {
                Quiz newQuiz = new Quiz()
                {
                    Name = "nmtung",
                    Description = "nmtung",
                    CourseID = Convert.ToInt32(courseID.Trim())
                };

                db.Quizs.Add(newQuiz);
                db.SaveChanges();

                var quizID = (from e in db.Quizs where e.CourseID.ToString() == courseID select e).ToList()[0].QuizID;

                var checkQues = (from q in db.QuizQuestions where q.Question == question.Trim() && q.QuizID == quizID select q).ToList();

                if (checkQues.Count != 0)
                {
                    TempData["message"] = false;
                    return RedirectToAction("QuizAdd", "AdminQuiz", new { courseID = courseID });
                }

                QuizQuestion newQuestion = new QuizQuestion()
                {
                    Question = question.Trim(),
                    QuizID = quizID
                };
                db.QuizQuestions.Add(newQuestion);
                db.SaveChanges();

                var questionID = (from q in db.QuizQuestions where q.Question == question.Trim() && q.QuizID == quizID select q).ToList()[0].QuesID;

                List<QuizQuestionAnswer> list = new List<QuizQuestionAnswer>();
                List<string> ans = new List<string>();
                ans.Add(answer1);
                ans.Add(answer2);
                ans.Add(answer3);
                ans.Add(answer4);


                for (int i = 0; i < 4; i++)
                {
                    string temp1 = "ans" + (i + 1);

                    list.Add(new QuizQuestionAnswer()
                    {
                        QuesID = questionID,
                        Answer = ans[i],
                        IsTrue = temp1 == trueAns
                    });
                }

                db.QuizQuestionAnswers.AddRange(list);
                db.SaveChanges();
            }
            else
            {
                int quizID = listQuiz[0].QuizID;

                var checkQues = (from q in db.QuizQuestions where q.Question == question.Trim() && q.QuizID == quizID select q).ToList();

                if (checkQues.Count != 0)
                {
                    TempData["message"] = false;
                    return RedirectToAction("QuizAdd", "AdminQuiz", new { courseID = courseID });
                }

                QuizQuestion newQuestion = new QuizQuestion()
                {
                    Question = question.Trim(),
                    QuizID = quizID
                };
                db.QuizQuestions.Add(newQuestion);
                db.SaveChanges();

                var questionID = (from q in db.QuizQuestions where q.Question == question.Trim() && q.QuizID == quizID select q).ToList()[0].QuesID;

                List<QuizQuestionAnswer> list = new List<QuizQuestionAnswer>();
                List<string> ans = new List<string>();
                ans.Add(answer1);
                ans.Add(answer2);
                ans.Add(answer3);
                ans.Add(answer4);


                for (int i = 0; i < 4; i++)
                {
                    string temp1 = "ans" + (i + 1);

                    list.Add(new QuizQuestionAnswer()
                    {
                        QuesID = questionID,
                        Answer = ans[i],
                        IsTrue = temp1 == trueAns
                    });
                }

                db.QuizQuestionAnswers.AddRange(list);
                db.SaveChanges();
            }

            TempData["message"] = true;
            return RedirectToAction("QuizAdd", "AdminQuiz", new { courseID = courseID });



        }

        [HttpGet]
        [CheckSessionAdmin]
        public ActionResult QuizDetail(string courseID, string questionID)
        {
            //get admin name
            int adminID = Convert.ToInt32(Session["admin_id"].ToString());
            var admin = from a in db.Admins where a.AdminID == adminID select a;
            ViewBag.adminName = admin.ToList()[0].Name;
            //ViewBag.courseID = courseID;

            var selectedQuestion = from q in db.Quizs
                                   join qq in db.QuizQuestions on q.QuizID equals qq.QuizID
                                   join qqa in db.QuizQuestionAnswers on qq.QuesID equals qqa.QuesID
                                   where q.CourseID.ToString() == courseID && qq.QuesID.ToString() == questionID
                                   select new ListQuiz
                                   {
                                       quiz = q,
                                       quizQuestion = qq,
                                       quizQuestionAnswer = qqa
                                   };

            ViewBag.question = selectedQuestion.ToList()[0].quizQuestion.Question;

            ViewBag.checkAnswer1 = selectedQuestion.ToList()[0].quizQuestionAnswer.IsTrue ? "checked" : "";
            ViewBag.checkAnswer2 = selectedQuestion.ToList()[1].quizQuestionAnswer.IsTrue ? "checked" : "";
            ViewBag.checkAnswer3 = selectedQuestion.ToList()[2].quizQuestionAnswer.IsTrue ? "checked" : "";
            ViewBag.checkAnswer4 = selectedQuestion.ToList()[3].quizQuestionAnswer.IsTrue ? "checked" : "";

            ViewBag.answer1 = selectedQuestion.ToList()[0].quizQuestionAnswer.Answer;
            ViewBag.answer2 = selectedQuestion.ToList()[1].quizQuestionAnswer.Answer;
            ViewBag.answer3 = selectedQuestion.ToList()[2].quizQuestionAnswer.Answer;
            ViewBag.answer4 = selectedQuestion.ToList()[3].quizQuestionAnswer.Answer;
            ViewBag.courseID = courseID;
            ViewBag.questionID = questionID;
            //set message
            ViewBag.message = TempData["message"];
            TempData.Clear();
            return View();
        }

        [HttpPost]
        [CheckSessionAdmin]
        public ActionResult EditQuestion(string courseID, string questionID, string question, string answer1, string answer2, string answer3, string answer4,
                                         string trueAns)
        {
            var selectedQuestion = from q in db.Quizs
                                   join qq in db.QuizQuestions on q.QuizID equals qq.QuizID
                                   join qqa in db.QuizQuestionAnswers on qq.QuesID equals qqa.QuesID
                                   where q.CourseID.ToString() == courseID && qq.QuesID.ToString() == questionID
                                   select new ListQuiz
                                   {
                                       quiz = q,
                                       quizQuestion = qq,
                                       quizQuestionAnswer = qqa
                                   };
            //error page
            if (selectedQuestion.Count() == 0)
            {
                return RedirectToAction("Index", "Error");
            }
            //check question exist in db
            var checkQues = (from qq in db.QuizQuestions
                             join q in db.Quizs on qq.QuizID equals q.QuizID
                             where qq.Question == question.Trim() && q.CourseID.ToString() == courseID
                             select qq).ToList();


            if (checkQues.Count != 0)
            {
                //get questionId
                if (checkQues[0].QuesID.ToString() != questionID)
                {
                    TempData["message"] = false;
                    return RedirectToAction("QuizDetail", "AdminQuiz", new { courseID = courseID, questionID = questionID });
                }
            }


            //check duplicated answer
            if (question.Trim().Equals("") || answer1.Trim().Equals("") || answer2.Trim().Equals("") || answer3.Trim().Equals("") || answer4.Trim().Equals("")
                || answer1.Trim().Equals(answer2.Trim()) || answer1.Trim().Equals(answer3.Trim()) || answer1.Trim().Equals(answer4.Trim())
                || answer2.Trim().Equals(answer3.Trim()) || answer2.Trim().Equals(answer4.Trim()) || answer3.Trim().Equals(answer4.Trim()))
            {
                TempData["message"] = false;
                return RedirectToAction("QuizDetail", "AdminQuiz", new { courseID = courseID, questionID = questionID });
            }

            try
            {
                //update question
                var updateQuestion = db.QuizQuestions.First(g => g.QuesID.ToString() == questionID);
                updateQuestion.Question = question.Trim();

                //get answerID of for answer of the question
                int answerID1 = selectedQuestion.ToList()[0].quizQuestionAnswer.AnsID;
                int answerID2 = selectedQuestion.ToList()[1].quizQuestionAnswer.AnsID;
                int answerID3 = selectedQuestion.ToList()[2].quizQuestionAnswer.AnsID;
                int answerID4 = selectedQuestion.ToList()[3].quizQuestionAnswer.AnsID;
                //update answer
                var updateAnswer1 = db.QuizQuestionAnswers.First(g => g.AnsID == answerID1);
                updateAnswer1.Answer = answer1.Trim();
                updateAnswer1.IsTrue = trueAns.Equals("ans1") ? true : false;

                var updateAnswer2 = db.QuizQuestionAnswers.First(g => g.AnsID == answerID2);
                updateAnswer2.Answer = answer2.Trim();
                updateAnswer2.IsTrue = trueAns.Equals("ans2") ? true : false;

                var updateAnswer3 = db.QuizQuestionAnswers.First(g => g.AnsID == answerID3);
                updateAnswer3.Answer = answer3.Trim();
                updateAnswer3.IsTrue = trueAns.Equals("ans3") ? true : false;

                var updateAnswer4 = db.QuizQuestionAnswers.First(g => g.AnsID == answerID4);
                updateAnswer4.Answer = answer4.Trim();
                updateAnswer4.IsTrue = trueAns.Equals("ans4") ? true : false;

                db.SaveChanges();
                TempData["message"] = true;
                return RedirectToAction("QuizDetail", "AdminQuiz", new { courseID = courseID, questionID = questionID });
            }
            catch (Exception)
            {
                TempData["message"] = false;
                return RedirectToAction("QuizDetail", "AdminQuiz", new { courseID = courseID, questionID = questionID });
            }

        }

        [HttpPost]
        [CheckSessionAdmin]
        public ActionResult DeleteQuestion(int courseID, int questionID)
        {
            try
            {
                var removeQuestion = (from e in db.QuizQuestions
                                      where e.QuesID == questionID
                                      select e).FirstOrDefault();

                var removeAnswer = from e in db.QuizQuestionAnswers
                                   where e.QuesID == questionID
                                   select e;
                var anser1 = removeAnswer.ToList()[0];
                var anser2 = removeAnswer.ToList()[1];
                var anser3 = removeAnswer.ToList()[2];
                var anser4 = removeAnswer.ToList()[3];
                if (removeQuestion != null && anser1 != null && anser2 != null && anser3 != null && anser4 != null)
                {
                    db.QuizQuestions.Remove(removeQuestion);
                    db.QuizQuestionAnswers.Remove(anser1);
                    db.QuizQuestionAnswers.Remove(anser2);
                    db.QuizQuestionAnswers.Remove(anser3);
                    db.QuizQuestionAnswers.Remove(anser4);
                    db.SaveChanges();
                }
                return RedirectToAction("Index", "AdminQuiz", new { courseID = courseID });
            }
            catch (Exception)
            {
                return RedirectToAction("QuizDetail", "AdminQuiz", new { courseID = courseID, questionID = questionID });
            }
        }
    }
}