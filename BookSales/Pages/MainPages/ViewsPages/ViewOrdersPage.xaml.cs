using BookSales.Context;
using System.Windows.Controls;
using System.Linq;

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
                OrdersViewList.ItemsSource = db.Orders.Select(s => new BookingsConsist
                {
                    Order = s
                }).ToList();
            }
        }
    }
}
