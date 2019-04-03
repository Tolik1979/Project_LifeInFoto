using GlobalSite.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList.Mvc;
using PagedList;

namespace GlobalSite.Controllers
{
    public class AlbumController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        #region GetUser
        //для получения пользователя
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AlbumController()
        {
        }

        public AlbumController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        // получить конкретный альбом клиенту (незарегестрированному пользователю)
        public ActionResult Index(string id, int? page)
        {
            Album album = db.Albums.Find(id);
            if (album != null)
            {
                int pageSize = 3;
                int pageNumber = (page ?? 1);
                return View(album.Photos.ToPagedList(pageNumber, pageSize));
            }
            else return HttpNotFound();
        }

        #region CRUD_Album
        // Частичное представление "Список альбомов"
        [HttpGet]
        [Authorize]
        public ActionResult ListAlbum()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());

            if (user != null)
            {
                return PartialView("_ListAlbum", user.Albums);
            }
            return View();
        }

        // создать новый альбом
        [HttpGet]
        [Authorize]
        public ActionResult CreateAlbum()
        {
            Album album = new Album
            {
                UserId = User.Identity.GetUserId()
            };
            return View(album);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult CreateAlbum(Album album)
        {
            if (album != null)
            {
                if (album.Id.Length < 4)
                {
                    TempData["albumId"] = "Пин код должен содержать не менее четырех символов.";
                    return View();
                }
                foreach (var alId in db.Albums)
                {
                    if (alId.Id == album.Id)
                    {
                        TempData["albumId"] = "Этот пин код уже занят.";
                        return View();
                    }
                }
                db.Entry(album).State = EntityState.Modified;
                db.Albums.Add(album);
                db.SaveChanges();
            }
            return RedirectToAction("Dashboard", "Home");
        }

        // редактировать конкретный альбом
        [HttpGet]
        [Authorize]
        public ActionResult EditAlbum(string id)
        {
            Album album = db.Albums.Find(id);
            if (album != null)
            {
                ViewBag.PageName = "EditAlbum";
                return View(album);
            }
            return RedirectToAction("Dashboard", "Home");
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult EditAlbum(Album album)
        {
            db.Entry(album).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Dashboard", "Home");
        }

        // удалить альбом
        [HttpGet]
        [Authorize]
        public ActionResult DeleteAlbum(string id)
        {
            Album album = db.Albums.Find(id);
            if (album != null)
            {
                return View(album);
            }
            return RedirectToAction("Dashboard", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("DeleteAlbum")]
        public ActionResult DeleteAl(string id)
        {
            Album album = db.Albums.Find(id);
            if (album != null)
            {
                db.Albums.Remove(album);
                db.SaveChanges();
            }
            return RedirectToAction("Dashboard", "Home");
        }

        #endregion

        #region Portfolio
        // получить портфолио
        [AllowAnonymous]
        public ActionResult Portfolio(string id)
        {
            if (id != null)
            {
                ApplicationUser user = db.Users.FirstOrDefault(u => u.LastName == id);
                if (user != null)
                {
                    ViewBag.facebook = user.Facebook;
                    ViewBag.instagram = user.Instagram;

                    Album album = user.Albums.FirstOrDefault(a => a.Name == "portfolio");
                    return View(album.Photos.ToList());
                }
            }
            return HttpNotFound();
        }
        #endregion

        #region CRUD_Photo
        // Частичное представление "Список фотографий"
        [Authorize]
        public ActionResult ListPhoto(string id)
        {
            Album album = db.Albums.Find(id);
            return PartialView("_ListPhoto", album.Photos.ToList());
        }

        // Добавление
        [HttpGet]
        [Authorize]
        public ActionResult AddPhoto()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult AddPhoto(string id, Photo photo, HttpPostedFileBase uploadImage)
        {
            if (uploadImage != null)
            {
                byte[] imageData = null;
                // считываем переданный файл в массив байтов
                using (var binaryReader = new BinaryReader(uploadImage.InputStream))
                {
                    imageData = binaryReader.ReadBytes(uploadImage.ContentLength);
                }
                // установка массива байтов
                photo.Image = imageData;
                // обязательная привязка к альбому
                photo.AlbumId = id;

                db.Photos.Add(photo);
                db.SaveChanges();
            }
            return RedirectToAction("EditAlbum", new { id });
        }

        // Редактирование
        [HttpGet]
        [Authorize]
        public ActionResult EditPhoto(int id)
        {
            Photo photo = db.Photos.Find(id);
            if (photo != null)
            {
                return View(photo);
            }
            return RedirectToAction("EditAlbum");
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult EditPhoto(Photo photo)
        {
            db.Entry(photo).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("EditAlbum");
        }

        // Удаление
        [HttpGet]
        [Authorize]
        public ActionResult DeletePhoto(int id)
        {
            Photo photo = db.Photos.Find(id);
            if (photo != null)
            {
                return View(photo);
            }
            return RedirectToAction("EditAlbum");
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        [ActionName("DeletePhoto")]
        public ActionResult DeletePh(int id)
        {
            Photo photo = db.Photos.Find(id);

            if (photo != null)
            {
                db.Photos.Remove(photo);
                db.SaveChanges();
            }
            return RedirectToAction("EditAlbum");
        }
        #endregion
    }
}