using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlobalSite.Models
{
    public class ShoppingCart
    {
        // коллекция фотографий выбранных пользователем
        private List<CartLine> lineCollection = new List<CartLine>();

        // свойство позволяющее обратиться к содержимому корзины
        public IEnumerable<CartLine> Lines { get { return lineCollection; } }

        // добавление фотографии в корзину
        public void AddItem(Photo photo)
        {
            CartLine line = lineCollection.Where(p => p.Photo.Id == photo.Id).FirstOrDefault();

            if (line == null)
            {
                lineCollection.Add(new CartLine
                {
                    Photo = photo,
                    Quantity = 1
                });
            }
        }

        // удаление фотографии из корзины
        public void RemoveLine(Photo photo)
        {
            lineCollection.RemoveAll(l => l.Photo.Id == photo.Id);
        }

        // вычисление общей стоимости фотографий в корзине
        public decimal PriceTotalValue()
        {
            return (decimal)lineCollection.Sum(e => e.Photo.Price);
        }

        // удаление содержимого корзины
        public void Clear()
        {
            lineCollection.Clear();
        }
    }

    // фотография выбранная пользователем
    public class CartLine
    {
        public Photo Photo { get; set; }
        public int Quantity { get; set; }
    }
}