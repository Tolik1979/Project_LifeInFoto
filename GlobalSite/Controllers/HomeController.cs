using GlobalSite.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace GlobalSite.Controllers
{
    public class HomeController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        #region GetUser

        //для получения пользователя
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public HomeController()
        {
        }

        public HomeController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        #endregion

        public ActionResult Index()
        {
            return View();
        }

        #region PinCodeForm

        [HttpGet]
        public ActionResult PinCodeForm()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PinCodeForm(GuestResponse guest)
        {
            //код для работы recaptcha
            var response = Request["g-recaptcha-response"];
            // Secret Key можно найти в личном кабинете на сайте www.google.com/recaptcha
            string secretKey = "*****";
            var client = new WebClient();
            var result = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secretKey, response));
            var obj = JObject.Parse(result);
            var status = (bool)obj.SelectToken("success");

            //поиск альбома по пин коду, он же id альбома
            Album album = db.Albums.Find(guest.PinCode);

            if (ModelState.IsValid && status)
            {
                if (album != null && album.Name != "portfolio")
                {
                    return RedirectToAction("Index", "Album", new { id = guest.PinCode });
                }
                else
                {
                    TempData["pincode"] = "Проверьте правильность пин кода. Такого альбома не существует.";
                    return View();
                }
            }
            else
            {
                TempData["recaptcha"] = "Подтвердите что Вы не робот.";
                return View();
            }
        }
        #endregion

        #region Dashboard

        [Authorize]
        public ActionResult Dashboard(string id)
        {
            var user = UserManager.FindById(User.Identity.GetUserId());

            if (user != null)
            {
                return View(user);
            }
            return HttpNotFound();
        }

        [Authorize]
        public ActionResult EditPersonalData(string id)
        {
            var user = UserManager.FindById(User.Identity.GetUserId());

            if (user != null)
            {
                return View(user);
            }
            return HttpNotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPersonalData(ApplicationUser user)
        {
            if (user != null)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Dashboard", "Home");
            }
            return HttpNotFound();
        }

        #endregion

    }
}