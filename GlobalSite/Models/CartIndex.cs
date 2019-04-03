using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlobalSite.Models
{
    public class CartIndex
    {
        public ShoppingCart Cart { get; set; }
        public string ReturnUrl { get; set; }
    }
}