using GlobalSite.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace GlobalSite.Controllers
{
    public class CartController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        #region ShoppingCart

        // Страница "Ваша корзина"
        public ViewResult YourShoppingCart(string returnUrl)
        {
            return View(new CartIndex
            {
                Cart = GetCart(),
                ReturnUrl = returnUrl
            });
        }

        // Добавление фотографии в корзину
        public RedirectToRouteResult AddToCart(int id, string returnUrl)
        {
            Photo photo = db.Photos.Find(id);

            if (photo != null)
            {
                GetCart().AddItem(photo);
            }
            return RedirectToAction("YourShoppingCart", new { returnUrl });
        }

        // Удаление фотографии из корзины
        public RedirectToRouteResult RemoveFromCart(int id, string returnUrl)
        {
            Photo photo = db.Photos.Find(id);

            if (photo != null)
            {
                GetCart().RemoveLine(photo);
            }
            return RedirectToAction("YourShoppingCart", new { returnUrl });
        }

        // Метод для сохранения и извлечения объектов ShoppingCart в состоянии сеанса
        public ShoppingCart GetCart()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart == null)
            {
                cart = new ShoppingCart();
                Session["Cart"] = cart;
            }
            return cart;
        }

        public PartialViewResult SummaryWithBTN()
        {
            ShoppingCart cart = GetCart();
            return PartialView(cart);
        }

        public PartialViewResult SummaryNoBTN()
        {
            ShoppingCart cart = GetCart();
            return PartialView(cart);
        }

        #endregion

        #region LiqPay

        public ActionResult LiqPayBtn()
        {
            ShoppingCart cart = GetCart();
            return View(LiqPayHelper.GetLiqPayModel(Guid.NewGuid().ToString(), cart.PriceTotalValue()));
        }

        /// <summary>
        /// На эту страницу LiqPay отправляет результат оплаты. Она указана в data.result_url
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Redirect()
        {
            // Превращаю ответ LiqPay в Dictionary<string, string> для удобства
            var request_dictionary = Request.Form.AllKeys.ToDictionary(key => key, key => Request.Form[key]);

            // Розшифровываю параметр data ответа LiqPay и превращаю в Dictionary<string, string> для удобства
            byte[] request_data = Convert.FromBase64String(request_dictionary["data"]);
            string decodedString = Encoding.UTF8.GetString(request_data);
            var request_data_dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(decodedString);

            // Получаю сигнатуру для проверки
            var mySignature = LiqPayHelper.GetLiqPaySignature(request_dictionary["data"]);

            // Если сигнатура серевера не совпадает з сигнатурой ответа LiqPay - что-то пошло не так
            if (mySignature != request_dictionary["signature"])
                return View("~/Views/Shared/Error.cshtml");

            // Если статус ответа "Тест" или "Успех" - все хорошо
            if (request_data_dictionary["status"] == "sandbox" || request_data_dictionary["status"] == "success")
            {
                // Здесь можно обновить статус заказа и сделать все необходимые действия. Id заказа можно взять здесь: request_data_dictionary[order_id]
                
                return View("DownloadPhoto");
            }

            return View("~/Views/Shared/Error.cshtml");
        }

        #endregion

        #region DownloadPhoto

        [HttpGet]
        public ActionResult DownloadPhoto()
        {
            return View(GetFiles());
        }

        [HttpPost]
        public ActionResult DownloadPhoto(int id)
        {
            DownloadFile(id);
            return View();
        }

        // Отправка массива байтов
        public FileResult DownloadFile(int? id)
        {
            Photo photo = db.Photos.Find(id);
            byte[] imageData = photo.Image;
            string file_type = "image/jpeg";
            string file_name = "IMG000" + photo.Id + ".jpeg";
            return File(imageData, file_type, file_name);
        }

        private List<Photo> GetFiles()
        {
            List<Photo> files = new List<Photo>();
            ShoppingCart cart = GetCart();

            foreach (var line in cart.Lines)
            {
                files.Add(new Photo
                {
                    Id = line.Photo.Id,
                    Name = line.Photo.Name,
                    Image = line.Photo.Image
                });
            }
            return files;
        }

        #endregion

    }
}