using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project.Filters;
using Project.Models;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Project.Controllers {
    public class LoginController : Controller {
        private ProjectEntities db = new ProjectEntities();

        [ HttpGet ]
        [ CheckNotSession ]
        [ CheckNotSessionAdmin ]
        public ActionResult Index() {
            ViewBag.isLogged = TempData["isLogged"];
            TempData.Clear();
            return View();
        }

        [ HttpPost ]
        [ CheckNotSession ]
        [ CheckNotSessionAdmin ]
        public ActionResult Login(string email, string password) {
            if ( email == null || password == null ) {
                TempData["isLogged"] = false;
                return RedirectToAction( "Index", "Login" );
            }

            if ( email.Trim() == "" || password.Trim() == "" ) {
                TempData["isLogged"] = false;
                return RedirectToAction( "Index", "Login" );
            }

            var result = from e in db.Users where e.Email == email.Trim() && e.Password == password.Trim() select e;

            if ( result.ToList().Count != 0 ) {
                Session["email"] = result.ToList()[0].Email;
                Session["user_id"] = result.ToList()[0].UserID;
                Session["name"] = result.ToList()[0].Name;
                return RedirectToAction( "Index", "Home" );
            } else {
                TempData["isLogged"] = false;
                return RedirectToAction( "Index", "Login" );
            }
        }

        [ HttpGet ]
        [ CheckSession ]
        [ CheckNotSessionAdmin ]
        public ActionResult Logout() {
            Session.Clear();

            return RedirectToAction( "Index", "Home" );
        }

        [ HttpGet ]
        [ CheckNotSession ]
        [ CheckNotSessionAdmin ]
        public ActionResult Register() {
            ViewBag.isCreated = TempData["isCreated"];
            TempData.Clear();
            return View();
        }

        [ HttpPost ]
        [ CheckNotSession ]
        [ CheckNotSessionAdmin ]
        public ActionResult RegisterAccount(string name, string email, string phone, string password, string address,
            string gender, string username) {
            if ( name == null || email == null || phone == null || password == null || address == null ||
                 gender == null || username == null ) {
                TempData["isCreated"] = false;
                return RedirectToAction( "Register", "Login" );
            }

            if ( name.Trim() == "" || email.Trim() == "" || phone.Trim() == "" || password.Trim() == "" ||
                 address.Trim() == "" || gender.Trim() == "" || username.Trim() == "" ) {
                TempData["isCreated"] = false;
                return RedirectToAction( "Register", "Login" );
            }


            // check name
            var match = Regex.Match( name,
                "[`~!@#$%^&*()_+\\-=\\[\\]\\{\\}\\|;:\'\",./<>?0-9]" );
            if (match.Success) {
                TempData [ "isCreated" ] = false;
                return RedirectToAction( "Register" , "Login" ); }
            match = Regex.Match( address ,
                "[`~!@#$%^&*()_+\\-=\\[\\]\\{\\}\\|;:\'\",./<>?0-9]" );
            if ( match.Success ) {
                TempData [ "isCreated" ] = false;
                return RedirectToAction( "Register" , "Login" ); }
            match = Regex.Match( username ,
                "[`~!@#$%^&*()_+\\-=\\[\\]\\{\\}\\|;:\'\",./<>?0-9]" );
            if ( match.Success ) {
                TempData [ "isCreated" ] = false;
                return RedirectToAction( "Register" , "Login" ); }



            var result = from e in db.Users
                where e.Email == email.Trim() || e.Username == username.Trim() || e.PhoneNumber == phone.Trim()
                select e;

            if ( result.ToList().Count != 0 ) {
                TempData["isCreated"] = false;
                return RedirectToAction( "Register", "Login" );
            }

            User newUser = new User() {
                Name = name.Trim(),
                Email = email.Trim(),
                PhoneNumber = phone.Trim(),
                Password = password.Trim(),
                Address = address.Trim(),
                Gender = gender.Trim() == "male",
                Username = username.Trim()
            };

            db.Users.Add( newUser );
            db.SaveChanges();

            return RedirectToAction( "Index", "Login" );
        }

        [ HttpGet ]
        [ CheckNotSession ]
        [ CheckNotSessionAdmin ]
        public ActionResult ForgotPassword() {
            ViewBag.SuccessMessage
                = "Mật khẩu mới đã được gửi về Email của bạn!"; //message when send pass to email success
            ViewBag.FailMessage
                = "Email bạn vừa nhập chưa đúng hoặc không tồn tại trong hệ thống!"; //message when email is not exist in database
            ViewBag.Message = TempData["forgotPass"];
            TempData.Clear();
            return View();
        }

        [ HttpPost ]
        [ CheckNotSession ]
        [ CheckNotSessionAdmin ]
        public ActionResult SendPasswordToMail(string email) {
            var infor = from e in db.Users where e.Email == email select e;
            if ( infor.ToList().Count != 0 ) {
                //case when email exist in database
                string name = infor.ToList()[0].Name; //get name
                string newPass = "";
                //generate new password for user
                try {
                    var getUser = db.Users.First( g => g.Email == email );
                    newPass = generateRandomPassword( 16 ); //generate new pass with 16 character
                    getUser.Password = newPass.Trim(); //update pass in database
                    db.SaveChanges();
                } catch ( Exception ) {
                    TempData["forgotPass"] = false;
                    return RedirectToAction( "ForgotPassword", "Login" );
                }

                //sending email
                using ( MailMessage
                    mm = new MailMessage( "daylamaildungchoproject@gmail.com",
                        email.Trim() ) ) //MailMessage(mailFrom, mailTo);
                {
                    mm.Subject = "Mật khẩu của bạn tại E-Learning"; //email's title
                    mm.Body = string.Format(
                        "Hi {0}!<br /><br />Mật khẩu của bạn là: {1}<br /><br />Lần sau đừng quên mật khẩu nữa nhaaa^^",
                        name, newPass ); //email's body
                    mm.IsBodyHtml = true;
                    //send email by using simple mail tranfer protocol
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = "smtp.gmail.com";
                    smtp.EnableSsl = true;
                    //provide credential password-based authentication schemes
                    NetworkCredential networkCred = new NetworkCredential();
                    networkCred.UserName = "daylamaildungchoproject@gmail.com"; //gmail's account
                    networkCred.Password = "hehe123456789"; //gmail's password
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = networkCred;
                    smtp.Port = 587;
                    smtp.Send( mm );
                    TempData["forgotPass"] = true;
                    return RedirectToAction( "ForgotPassword", "Login" );
                }
            } else {
                //case when have no email in database
                TempData["forgotPass"] = false;
                return RedirectToAction( "ForgotPassword", "Login" );
            }
        }

        private string generateRandomPassword(int length) {
            const string VALID_PASS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890~!@#$%^&*";
            StringBuilder res = new StringBuilder();
            Random random = new Random();
            while ( 0 < length-- ) {
                res.Append( VALID_PASS[random.Next( VALID_PASS.Length )] );
            }

            return res.ToString();
        }
    }
}