using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project.Filters;
using Project.Models;

namespace Project.Controllers {
    public class AdminUserController : Controller {
        private ProjectEntities db = new ProjectEntities();

        // GET: AdminUser
        [ HttpGet ]
        [ CheckSessionAdmin ]
        public ActionResult Index() {
            var result = (from user in db.Users select user).ToList();

            return View( result );
        }

        [ HttpPost ]
        [ CheckSessionAdmin ]
        public ActionResult Search(string s) {
            var result = (from user in db.Users where user.Name.Contains( s ) select user).ToList();

            return View( result );
        }

        [ HttpGet ]
        [ CheckSessionAdmin ]
        public ActionResult Detail(string userid) {
            var result = (from user in db.Users where user.UserID.ToString() == userid select user).ToList();

            if ( result.Count == 0 ) {
                return RedirectToAction( "Index" );
            }

            ViewBag.isUpdate = TempData["isUpdate"];
            return View( result[0] );
        }

        [ HttpPost ]
        [ CheckSessionAdmin ]
        public ActionResult Update(string userid, string name, string email, string phone, string address,
            string gender) {
            string updateName = "";
            string updateEmail = "";
            string updatePhone = "";
            string updateAddress = "";
            bool updateGender = true;

            //validate input
            try {
                updateName = name.Trim();
                updateEmail = email.Trim();
                long temp = Convert.ToInt64( phone );
                updatePhone = phone.Trim();
                updateAddress = address.Trim();
                updateGender = (gender == "male");
            } catch ( Exception e ) {
                return RedirectToAction( "Index" );
            }

            if ( name.Trim() == "" || email.Trim() == "" || phone.Trim() == "" ||
                 address.Trim() == "" || gender.Trim() == "" ) {
                return RedirectToAction( "Index" );
            }

            int id = Convert.ToInt32( userid );
            var result = db.Users.First( user => (user.UserID == id) );

            if ( result == null ) {
                return RedirectToAction( "Index" );
            }

            bool passEmail = false;
            bool passPhone = false;

            var check1 = (from user in db.Users where user.Email == updateEmail select user).ToList();
            if ( check1.Count != 0 && check1[0].UserID.ToString() == userid ) {
                passEmail = true;
            }

            var check2 = (from user in db.Users where user.PhoneNumber == updatePhone select user).ToList();
            if ( check2.Count != 0 && check2[0].UserID.ToString() == userid ) {
                passPhone = true;
            }

            if ( passPhone && passEmail ) {
                result.Name = updateName;
                result.Email = updateEmail;
                result.PhoneNumber = updatePhone;
                result.Address = updateAddress;
                result.Gender = updateGender;
                db.SaveChanges();

                TempData["isUpdate"] = true;
                return RedirectToAction( "Detail", id );
            }

            TempData["isUpdate"] = false;
            return RedirectToAction( "Detail", id );
        }

        public ActionResult Delete(string userid) {
            var result = (from user in db.Users where user.UserID.ToString() == userid select user).ToList();
            db.Users.RemoveRange( result );
            db.SaveChanges();

            return RedirectToAction( "Index" );
        }
    }
}