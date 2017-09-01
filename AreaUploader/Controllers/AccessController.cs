using AreaUploader.Models;
using LoginLibrary;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace AreaUploader.Controllers
{
    public class AccessController : Controller
    {
        // GET: Access
       
        public ActionResult Login()
        {
            ViewBag.ErrorMsg = null;
            ViewBag.ErrorMsg = TempData["ErrorMsg"];
            return View();
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            //Session["Username"] = null;
            //Session["Password"] = null;

            return RedirectToAction("Login", "Access");
        }

        [HttpPost]
        public ActionResult LoginCheck(UserLogin model)
        {
            if (ModelState.IsValid)
            {
                var objUserDetail = new UserDetails();
                var user = objUserDetail.GetLoginDetails(model.Username, model.Password);
                if (user != null)
                {

                    FormsAuthenticationTicket oTicket = new FormsAuthenticationTicket(1, model.Username, DateTime.Now, DateTime.Now.AddMinutes(30), true, FormsAuthentication.FormsCookiePath);

                    string cookiestr = FormsAuthentication.Encrypt(oTicket);
                    HttpCookie ck = new HttpCookie(FormsAuthentication.FormsCookieName, cookiestr);
                    ck.Path = FormsAuthentication.FormsCookiePath;
                    Response.Cookies.Add(ck);
                    Session["Username"] = model.Username;
                    Session["Password"] = model.Password;
                    return RedirectToAction("../Home/Index");
                }
                else
                {
                    TempData["ErrorMsg"] = "Username or Password provided is incorrect";
                    return RedirectToAction("Login");
                }
            }
            return View();
        }

        // Add User
        public ActionResult SaveUser(UserLogin model)
        {
            try
            {
                UserDetails objUser = new UserDetails();
                var userSave = objUser.AddUser(model.Username, model.Email, model.Password);
                if (userSave.Contains("Success"))
                {
                    return RedirectToAction("Login", "Access");
                }
                else
                {
                    ViewBag.ErrorMsg = userSave;
                    return View("../Shared/Error");
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        //Checking Username, Accountname, Email exist
        public JsonResult CheckUserExist(string userName, string email, int userId)
        {
            try
            {
                UserDetails objCheckUserExist = new UserDetails();
                string result = objCheckUserExist.GetUserExist(userName, email, userId);
                return Json(new { response = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}