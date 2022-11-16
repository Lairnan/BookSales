using System.Linq;
using System.Data.Entity;
using BookSales.Context;

namespace BookSales
{
    public class BookingsConsist
    {
        private Orders _order;
        public Orders Order
        {
            get => _order;
            set
            {
                _order = value;
                SetBookings();
            }
        }
        public int Count { get; private set; } = 0;
        public decimal Price { get; private set; } = 0;

        private void SetBookings()
        {
            using(var db = new BookSalesEntities())
            {
                var orderConsist = db.OrderConsist.Include(s => s.Books).Where(s => s.idOrder == Order.id).ToList();
                foreach(var order in orderConsist)
                {
                    Price += order.Books.retailPrice * order.amount;
                    Count += order.amount;
                }
            }
        }
    }
}
