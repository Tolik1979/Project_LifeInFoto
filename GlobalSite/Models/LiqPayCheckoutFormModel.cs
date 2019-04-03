using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlobalSite.Models
{
    /// <summary>
    /// Данные, которые передаются в представление для формирования кнопки оплаты LiqPay
    /// </summary>
    public class LiqPayCheckoutFormModel
    {
        public string Data { get; set; }
        public string Signature { get; set; }
    }
}