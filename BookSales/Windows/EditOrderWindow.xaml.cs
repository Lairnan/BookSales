using BookSales.Context;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Data.Entity;
using BookSales.BehaviorsFiles;
using System;

namespace BookSales.Windows
{
    /// <summary>
    /// Логика взаимодействия для EditOrderWindow.xaml
    /// </summary>
    public partial class EditOrderWindow : Window
    {
        public EditOrderWindow(BookingsConsist bookingsConsist)
        {
            InitializeComponent();
            BookingOrder = bookingsConsist;
            IdOrderBox.Text = bookingsConsist.Order.id.ToString();
            DateOrderBox.Text = bookingsConsist.Order.dateOrder.ToString("dd MMMM yyyy HH:mm:ss", CultureInfo.CurrentCulture);
            PaidBox.IsChecked = bookingsConsist.Order.paid;
            PerformedBox.IsChecked = bookingsConsist.Order.performed;
            PriceBox.Text = bookingsConsist.Order.price.ToString();
            Loaded += EditOrderWindow_Loaded;
        }

        private async void EditOrderWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var listUsers = await Task.Run(() =>
            {
                using(var db = new BookSalesEntities())
                {
                    return db.Users.Include(s => s.Positions).ToList();
                }
            });
            UsersBox.ItemsSource = listUsers;
            UsersBox.SelectedItem = (UsersBox.ItemsSource as List<Users>).Single(s => s.id == BookingOrder.Order.idUser);
            SaveBtn.IsEnabled = true;
        }

        private BookingsConsist BookingOrder { get; set; }

        private void ViewOrderConsist_Click(object sender, RoutedEventArgs e)
        {
            var viewOrderConsist = new ViewOrderConsistWindow(BookingOrder);
            viewOrderConsist.ShowDialog();
        }

        private async void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            using(var db = new BookSalesEntities())
            {
                var order = db.Orders.First(s => s.id == BookingOrder.Order.id);
                order.idUser = (UsersBox.SelectedItem as Users).id;
                order.paid = PaidBox.IsChecked.Value;
                order.performed = PerformedBox.IsChecked.Value;
                if (BookingOrder.Order.performed != order.performed && order.performed == true) order.dateSuccess = DateTime.Now;
                else if (BookingOrder.Order.performed == order.performed && order.performed == true) order.dateSuccess = order.dateSuccess;
                else order.dateSuccess = null;
                await db.SaveChangesAsync();
            }

            DialogResult = true;
            this.Close();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }
    }
}
