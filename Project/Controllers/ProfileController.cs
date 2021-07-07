using Project.Filters;
using Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace Project.Controllers
{
    public class TempUserAndCourse
    {
        public double Score { get; set; }
        public Course course { get; set; }
        public UserAndCourse userAndCourse { get; set; }
    }
    public class ProfileController : Controller
    {
        private ProjectEntities db = new ProjectEntities();
        // GET: Profile
        [HttpGet]
        [CheckSession]
        public ActionResult Index()
        {
            int id = -1;
            if (Session["user_id"] != null)
            {
                int.TryParse(Session["user_id"].ToString(), out id);
            }
            var infor = from e in db.Users where e.UserID == id select e;
            ViewBag.name = infor.ToList()[0].Name;
            ViewBag.email = infor.ToList()[0].Email;
            string gender = "Nam";
            if (infor.ToList()[0].Gender == false)
            {
                gender = "Nữ";
            }
            ViewBag.gender = gender;
            ViewBag.phone = infor.ToList()[0].PhoneNumber;
            ViewBag.address = infor.ToList()[0].Address;
            ViewBag.successInfor = "Thay đổi thông tin người dùng thành công!";
            ViewBag.successPass = "Thay đổi mật khẩu thành công!";
            ViewBag.inforMessage = TempData["infor"];
            ViewBag.passMessage = TempData["pass"];
            TempData.Clear();

            var check = from e in db.UserAndCourses where e.UserID == id select e;
            if (check.ToList().Count > 0)
            {
                List<Course> course = db.Courses.ToList();
                List<UserAndCourse> userAndCourse = db.UserAndCourses.ToList();

                var showScore = from c in course
                                join uc in userAndCourse on c.CourseID equals uc.CourseID
                                select new TempUserAndCourse
                                {
                                    course = c,
                                    Score = (double)uc.Score,
                                    userAndCourse = uc,
                                };
                return View(showScore.ToList());
            }
            else
            {
                return View();
            }
        }

        [HttpGet]
        [CheckSession]
        public ActionResult EditProfile()
        {
            int id = -1;
            if (Session["user_id"] != null)
            {
                int.TryParse(Session["user_id"].ToString(), out id);
            }
            var infor = from e in db.Users where e.UserID == id select e;
            ViewBag.name = infor.ToList()[0].Name;
            ViewBag.email = infor.ToList()[0].Email;
            ViewBag.phone = infor.ToList()[0].PhoneNumber;
            ViewBag.address = infor.ToList()[0].Address;
            ViewBag.genderMale = infor.ToList()[0].Gender == true ? "checked" : "";
            ViewBag.genderFemale = infor.ToList()[0].Gender == false ? "checked" : "";
            ViewBag.failInfor = "Thay đổi thông tin người dùng thất bại!";
            ViewBag.failPass = "Thay đổi mật khẩu thất bại!";
            ViewBag.inforMessage = TempData["infor"];
            ViewBag.passMessage = TempData["pass"];
            TempData.Clear();
            return View();
        }

        [HttpPost]
        [CheckSession]
        public ActionResult EditUserProfile(string name, string gender, string phone, string address)
        {
            int id = -1;
            if (Session["user_id"] != null)
            {
                int.TryParse(Session["user_id"].ToString(), out id);
            }
            try
            {
                var update = db.Users.First(g => g.UserID == id);
                update.Name = name.Trim();
                update.Gender = gender == "male";
                update.PhoneNumber = phone.Trim();
                update.Address = address.Trim();
                db.SaveChanges();
                TempData["infor"] = true;
                Session["name"] = name.Trim();
                return RedirectToAction("Index", "Profile");
            }
            catch (Exception)
            {
                TempData["infor"] = false;
                return RedirectToAction("EditProfile", "Profile");
            }
        }

        [HttpPost]
        [CheckSession]
        public ActionResult ChangePassword(string oldPass, string newPass, string rePass)
        {
            int id = -1;
            if (Session["user_id"] != null)
            {
                int.TryParse(Session["user_id"].ToString(), out id);
            }
            try
            {
                var update = db.Users.First(g => g.UserID == id);
                if (newPass.Equals(rePass) && oldPass.Equals(update.Password) && !newPass.Equals(oldPass))
                {
                    update.Password = newPass.Trim();
                    db.SaveChanges();
                    TempData["pass"] = true;
                    return RedirectToAction("Index", "Profile");
                }
                else
                {
                    TempData["pass"] = false;
                    return RedirectToAction("EditProfile", "Profile");
                }
            }
            catch (Exception)
            {
                TempData["pass"] = false;
                return RedirectToAction("EditProfile", "Profile");
            }
        }
    }
}