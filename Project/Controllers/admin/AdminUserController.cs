using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
            var result = (from user in db.Users where user.Name.Contains( s.Trim() ) select user).ToList();

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
                 address.Trim() == "" || gender.Trim() == "" || !IsPhoneNumber(phone)) {
                return RedirectToAction( "Index" );
            }

            int id = Convert.ToInt32( userid );
            var result = db.Users.First( user => (user.UserID == id) );

            if ( result == null ) {
                return RedirectToAction( "Index" );
            }

            result.Name = updateName;
            result.Address = updateAddress;
            result.Gender = updateGender;

            if ( result.Email != updateEmail ) {
                var checkMail = (from user in db.Users where user.Email == updateEmail && user.UserID != id select user)
                    .ToList();

                if ( checkMail.Count != 0 ) {

                    TempData [ "isUpdate" ] = false;
                    return RedirectToAction( "Detail" , id );
                }
            }

            if ( result.PhoneNumber != updatePhone ) {
                var checkPhone = (from user in db.Users
                        where user.PhoneNumber == updatePhone && user.UserID != id
                        select user)
                    .ToList();

                if ( checkPhone.Count != 0 ) {

                    TempData [ "isUpdate" ] = false;
                    return RedirectToAction( "Detail" , id );
                }
            }


            result.Email = updateEmail;
            result.PhoneNumber = updatePhone;
            db.SaveChanges();

            TempData["isUpdate"] = true;
            return RedirectToAction( "Detail", id );
        }
        //phone start with 0, after is 3/5/7/8/9 -> 8 number more
        public static bool IsPhoneNumber(string number)
        {
            return Regex.IsMatch(number, @"(0[3|5|7|8|9])+([0-9]{8})\b");
        }
        [HttpGet]
        [CheckSessionAdmin]
        public ActionResult Delete(string userid) {
            var result = (from user in db.Users where user.UserID.ToString() == userid select user).ToList();
            db.Users.RemoveRange( result );
            db.SaveChanges();

            return RedirectToAction( "Index" );
        }
    }
}