using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project.Filters;
using Project.Models;
using System.Net.Mail;
using System.Net;

namespace Project.Controllers
{
    public class LoginController : Controller
    {
        private ProjectEntities db = new ProjectEntities();

        [HttpGet]
        [CheckNotSession]
        public ActionResult Index()
        {
            ViewBag.isLogged = TempData["isLogged"];
            TempData.Clear();
            return View();
        }

        [HttpPost]
        [CheckNotSession]
        public ActionResult Login(string email, string password)
        {
            var result = from e in db.Users where e.Email == email && e.Password == password select e;

            if (result.ToList().Count != 0)
            {
                Session["email"] = result.ToList()[0].Email;
                Session["user_id"] = result.ToList()[0].UserID;
                Session["name"] = result.ToList()[0].Name;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["isLogged"] = false;
                return RedirectToAction("Index", "Login");
            }
        }

        [HttpGet]
        [CheckSession]
        public ActionResult Logout()
        {
            Session.Clear();

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [CheckNotSession]
        public ActionResult Register()
        {
            ViewBag.isCreated = TempData["isCreated"];
            TempData.Clear();
            return View();
        }

        [HttpPost]
        [CheckNotSession]
        public ActionResult RegisterAccount(string name, string email, string phone, string password, string address,
            string gender, string username)
        {
            var result = from e in db.Users
                         where e.Email == email || e.Username == username || e.PhoneNumber == phone
                         select e;

            if (result.ToList().Count != 0)
            {
                TempData["isCreated"] = false;
                return RedirectToAction("Register", "Login");
            }

            User newUser = new User()
            {
                Name = name,
                Email = email,
                PhoneNumber = phone,
                Password = password,
                Address = address,
                Gender = gender == "male",
                Username = username
            };

            db.Users.Add(newUser);
            db.SaveChanges();

            return RedirectToAction("Index", "Login");
        }

        [HttpGet]
        [CheckNotSession]
        public ActionResult ForgotPassword()
        {
            ViewBag.SuccessMessage = "Mật khẩu đã được gửi về Email của bạn!"; //message when send pass to email success
            ViewBag.FailMessage = "Email bạn vừa nhập chưa đúng hoặc không tồn tại trong hệ thống!"; //message when email is not exist in database
            ViewBag.Message = TempData["forgotPass"];
            TempData.Clear();
            return View();
        }

        [HttpPost]
        [CheckNotSession]
        public ActionResult SendPasswordToMail(string email)
        {
            var infor = from e in db.Users where e.Email == email select e;
            if (infor.ToList().Count != 0)
            {
                //case when email exist in database
                string name = infor.ToList()[0].Name; //get name
                string password = infor.ToList()[0].Password; //get password
                using (MailMessage mm = new MailMessage("daylamaildungchoproject@gmail.com", email.Trim())) //MailMessage(mailFrom, mailTo);
                {
                    mm.Subject = "Mật khẩu của bạn tại E-Learning"; //email's title
                    mm.Body = string.Format("Hi {0}!<br /><br />Mật khẩu của bạn là: {1}<br /><br />Lần sau đừng quên mật khẩu nữa nhaaa^^", name, password); //email's body
                    mm.IsBodyHtml = true;
                    //send email by using simple mail tranfer protocol
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = "smtp.gmail.com";
                    smtp.EnableSsl = true;
                    //provide credential password-based authentication schemes
                    NetworkCredential networkCred = new NetworkCredential();
                    networkCred.UserName = "daylamaildungchoproject@gmail.com"; //tên dùng để đăng nhập gmail
                    networkCred.Password = "hehe123456789"; //mật khẩu dùng để đăng nhập gmail
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = networkCred;
                    smtp.Port = 587;
                    smtp.Send(mm);
                    TempData["forgotPass"] = true;
                    return RedirectToAction("ForgotPassword", "Login");
                }
            }
            else
            {
                //case when have no email in database
                TempData["forgotPass"] = false;
                return RedirectToAction("ForgotPassword", "Login");
            }
        }
    }
}