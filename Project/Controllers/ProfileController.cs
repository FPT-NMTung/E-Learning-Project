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
            return View();
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
                update.Name = name;
                update.Gender = gender == "male";
                update.PhoneNumber = phone;
                update.Address = address;
                db.SaveChanges();
                return RedirectToAction("Index", "Profile");
            }
            catch (Exception)
            {
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
                if (newPass.Equals(rePass) && oldPass.Equals(update.Password)){           
                    update.Password = newPass;
                    db.SaveChanges();
                    return RedirectToAction("Index", "Profile");
                }
                else
                {
                    return RedirectToAction("EditProfile", "Profile");
                }
            }
            catch (Exception)
            {
                return RedirectToAction("EditProfile", "Profile");
            }
        }
    }
}