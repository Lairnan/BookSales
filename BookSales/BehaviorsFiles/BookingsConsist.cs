using System.Linq;
using System.Data.Entity;
using BookSales.Context;
using System.Collections.Generic;

namespace BookSales.BehaviorsFiles
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
        public List<OrderConsist> OrderConsist { get; private set; }
        public int Count { get; private set; } = 0;

        private void SetBookings()
        {
            using(var db = new BookSalesEntities())
            {
                OrderConsist = db.OrderConsist.Include(s => s.Books).Where(s => s.idOrder == Order.id).ToList();
                foreach(var order in OrderConsist)
                {
                    Count += order.amount;
                }
            }
        }
    }
}
