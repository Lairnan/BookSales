using BookSales.Context;
using System.Windows.Controls;
using System.Linq;
using System.Data.Entity;

namespace BookSales.Pages.MainPages.ViewsPages
{
    /// <summary>
    /// Логика взаимодействия для ViewOrdersPage.xaml
    /// </summary>
    public partial class ViewOrdersPage : Page
    {
        public ViewOrdersPage()
        {
            InitializeComponent();
            using (var db = new BookSalesEntities())
            {
                OrdersViewList.ItemsSource = db.Orders.Include(s => s.Users).Select(s => new BookingsConsist
                {
                    Order = s
                }).ToList();
            }
        }
    }
}
