using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace GlobalSite.Models
{
    public class LiqPayHelper
    {
        static private readonly string _private_key;
        static private readonly string _public_key;

        static LiqPayHelper()
        {
            // Public Key компании, который можно найти в личном кабинете на сайте liqpay.ua
            _public_key = "*****";
            // Private Key компании, который можно найти в личном кабинете на сайте liqpay.ua
            _private_key = "*****";
        }

        /// <summary>
        /// Сформировать данные для LiqPay (data, signature)
        /// </summary>
        /// <param name="order_id">Номер заказа</param>
        /// <returns></returns>
        static public LiqPayCheckoutFormModel GetLiqPayModel(string order_id, decimal totalPrice)
        {
            // Заполнение данных для их передачи для LiqPay
            var signature_source = new LiqPayCheckout()
            {
                public_key = _public_key,
                version = 3,
                action = "pay",
                amount = totalPrice,
                currency = "UAH",
                description = "Оплата заказа",
                order_id = order_id,
                sandbox = 1,
                result_url = "http://localhost:48983/Cart/Redirect"
            };
            var json_string = JsonConvert.SerializeObject(signature_source);
            var data_hash = Convert.ToBase64String(Encoding.UTF8.GetBytes(json_string));
            var signature_hash = GetLiqPaySignature(data_hash);

            // Данные для передачи в представление
            var model = new LiqPayCheckoutFormModel
            {
                Data = data_hash,
                Signature = signature_hash
            };
            return model;
        }

        /// <summary>
        /// Формирование сигнатуры
        /// </summary>
        /// <param name="data">Json string с параметрами для LiqPay</param>
        /// <returns></returns>
        static public string GetLiqPaySignature(string data)
        {
            return Convert.ToBase64String(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(_private_key + data + _private_key)));
        }
    }
}