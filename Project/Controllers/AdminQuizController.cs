﻿using Project.Filters;
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
            var listQuestion = from q in db.Quizs
                               join qq in db.QuizQuestions on q.QuizID equals qq.QuizID
                               where q.CourseID.ToString() == courseID
                               select new ListQuiz
                               {
                                   quiz = q,
                                   quizQuestion = qq
                               };

            if (answer1.Trim().Equals("") || answer2.Trim().Equals("") || answer3.Trim().Equals("") || answer4.Trim().Equals("")
                || answer1.Trim().Equals(answer2.Trim()) || answer1.Trim().Equals(answer3.Trim()) || answer1.Trim().Equals(answer4.Trim())
                || answer2.Trim().Equals(answer3.Trim()) || answer2.Trim().Equals(answer4.Trim()) || answer3.Trim().Equals(answer4.Trim()))
            {
                TempData["message"] = false;
                return RedirectToAction("QuizAdd", "AdminQuiz");
            }
            
            var quizID = from e in db.Quizs where e.CourseID.ToString() == courseID select e;
            try
            {
                QuizQuestion newQuestion = new QuizQuestion();
                // fields to be insert
                newQuestion.Question = question.Trim();
                newQuestion.QuizID = quizID.ToList()[0].QuizID;
                // executes the commands to implement the changes to the database
                //add question
                db.QuizQuestions.Add(newQuestion);
                db.SaveChanges();

                //add answer
                var getQuestionID = (from e in db.QuizQuestions
                                     orderby e.QuesID descending
                                     select e).First();

                QuizQuestionAnswer newAnswer = new QuizQuestionAnswer();

                newAnswer.QuesID = getQuestionID.QuesID;
                newAnswer.IsTrue = trueAns.Equals("ans1") ? true : false;
                newAnswer.Answer = answer1.Trim();
                db.QuizQuestionAnswers.Add(newAnswer);
                db.SaveChanges();

                newAnswer.IsTrue = trueAns.Equals("ans2") ? true : false;
                newAnswer.Answer = answer2.Trim();
                db.QuizQuestionAnswers.Add(newAnswer);
                db.SaveChanges();

                newAnswer.IsTrue = trueAns.Equals("ans3") ? true : false;
                newAnswer.Answer = answer3.Trim();
                db.QuizQuestionAnswers.Add(newAnswer);
                db.SaveChanges();

                newAnswer.IsTrue = trueAns.Equals("ans4") ? true : false;
                newAnswer.Answer = answer4.Trim();
                db.QuizQuestionAnswers.Add(newAnswer);
                db.SaveChanges();

                TempData["message"] = true;
                return RedirectToAction("QuizAdd", "AdminQuiz");
            }
            catch (Exception)
            {
                TempData["message"] = false;
                return RedirectToAction("QuizAdd", "AdminQuiz");
            }
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

            if (answer1.Trim().Equals("") || answer2.Trim().Equals("") || answer3.Trim().Equals("") || answer4.Trim().Equals("")
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

                //update answer
                var updateAnswer1 = db.QuizQuestionAnswers.First(g => g.AnsID == selectedQuestion.ToList()[0].quizQuestionAnswer.AnsID);
                updateAnswer1.Answer = answer1.Trim();
                updateAnswer1.IsTrue = trueAns.Equals("ans1") ? true : false;

                var updateAnswer2 = db.QuizQuestionAnswers.First(g => g.AnsID == selectedQuestion.ToList()[1].quizQuestionAnswer.AnsID);
                updateAnswer2.Answer = answer2.Trim();
                updateAnswer2.IsTrue = trueAns.Equals("ans2") ? true : false;

                var updateAnswer3 = db.QuizQuestionAnswers.First(g => g.AnsID == selectedQuestion.ToList()[2].quizQuestionAnswer.AnsID);
                updateAnswer3.Answer = answer3.Trim();
                updateAnswer3.IsTrue = trueAns.Equals("ans3") ? true : false;

                var updateAnswer4 = db.QuizQuestionAnswers.First(g => g.AnsID == selectedQuestion.ToList()[3].quizQuestionAnswer.AnsID);
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
    }
}