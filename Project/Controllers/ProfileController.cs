using Project.Filters;
using Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
            string gender = "Male";
            if (infor.ToList()[0].Gender.Equals("False"))
            {
                gender = "Female";
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
            string gender = "Male";
            if (infor.ToList()[0].Gender.Equals("False"))
            {
                gender = "Female";
            }
            ViewBag.gender = gender;
            ViewBag.phone = infor.ToList()[0].PhoneNumber;
            ViewBag.address = infor.ToList()[0].Address;
            return View();
        }
    }
}