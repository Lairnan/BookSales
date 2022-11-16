using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace BookSales
{
    internal class BasketOrder
    {
        public Books Book { get; set; }
        public int Count { get; set; }
        public int Stock { get; set; }

        public static List<BasketOrder> BasketOrders = new List<BasketOrder>();

        public static void Add(Books book, int count, int stock)
        {
            var tempBask = BasketOrders.SingleOrDefault(s => s.Book == book);
            if (count > stock - (tempBask == null ? 0 : tempBask.Count))
            {
                MessageBox.Show("Такого количества на складе нет");
                return;
            }
            if (!BasketOrders.Where(s => s.Book.id == book.id).Any())
                BasketOrders.Add(new BasketOrder { Book = book, Stock = stock});
            var basket = BasketOrders.Single(s => s.Book.id == book.id);
            if (basket.Count >= stock)
            {
                MessageBox.Show("Товара больше нет в наличии");
                return;
            };
            basket.Count += count;
        }

        public static void Remove(Books book, int count)
        {
            if (!BasketOrders.Select(s => s.Book).Contains(book)) return;
            var basket = BasketOrders.Single(s => s.Book == book);
            basket.Count -= count;
            if (basket.Count < 1)
                BasketOrders.Remove(basket);
        }
    }
}
