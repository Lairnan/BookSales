using BookSales.Context;
using System.Windows.Controls;
using System.Linq;
using System.Data.Entity;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Windows;
using BookSales.Windows;
using BookSales.BehaviorsFiles;

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
            ApplyFilter();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OrdersViewList == null) return;
            ApplyFilter();
        }

        private async void ApplyFilter()
        {
            using (var db = new BookSalesEntities())
            {
                IEnumerable<BookingsConsist> newList = await Task.Run(() => db.Orders
                                    .Include(s => s.Users)
                                    .Select(s => new BookingsConsist { Order = s }));

                newList = await Task.Run(() => newList.Where(s => s.Order.dateOrder.ToString().Contains(FilterText.Text)));

                if (StatusBox.SelectedIndex != 0)
                {
                    switch (StatusBox.SelectedIndex)
                    {
                        case 1:
                            newList = await Task.Run(() => newList.Where(s => s.Order.performed));
                            break;
                        case 2:
                            newList = await Task.Run(() => newList.Where(s => !s.Order.performed));
                            break;
                    }
                }

                switch (DateOrderBox.SelectedIndex)
                {
                    case 0:
                        newList = newList.OrderByDescending(s => s.Order.dateOrder);
                        break;
                    case 1:
                        newList = newList.OrderBy(s => s.Order.dateOrder);
                        break;
                }

                OrdersViewList.ItemsSource = newList.ToList();
            }
        }

        private void FilterText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (OrdersViewList == null) return;
            ApplyFilter();
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            FilterText.Text = string.Empty;
        }

        private async void CompleteOrderBtn_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var order = btn.DataContext as BookingsConsist;
            if (order.Order.performed)
            {
                MessageBox.Show("Заказ уже завершён");
                return;
            }
            if (MessageBox.Show("Завершить заказ?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Information) != MessageBoxResult.Yes) return;

            using(var db = new BookSalesEntities())
            {
                var orderDb = await db.Orders.FirstAsync(s => s.id == order.Order.id);
                orderDb.performed = true;
                orderDb.dateSuccess = DateTime.Now;
                await db.SaveChangesAsync();
            }
            ApplyFilter();
        }

        private void MenuEditOrder_Click(object sender, RoutedEventArgs e)
        {
            var bookingConsist = OrdersViewList.SelectedItem as BookingsConsist;
            if(bookingConsist == null)
            {
                MessageBox.Show("Выберите элемент для редактирования!");
                return;
            }
            var editOrderWindow = new EditOrderWindow(bookingConsist);
            if (editOrderWindow.ShowDialog() == true)
            {
                ApplyFilter();
            }
        }

        private async void MenuDeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            var bookingConsist = OrdersViewList.SelectedItem as BookingsConsist;
            if (bookingConsist == null)
            {
                MessageBox.Show("Выберите элемент для удаления!");
                return;
            }
            if (MessageBox.Show("Удалить заказ?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Information) != MessageBoxResult.Yes) return;

            using (var db = new BookSalesEntities())
            {
                var orderDb = await db.Orders.FirstAsync(s => s.id == bookingConsist.Order.id);
                db.Orders.Remove(orderDb);
                await db.SaveChangesAsync();
            }
            ApplyFilter();
        }

        private void ViewOrderConsist_Click(object sender, RoutedEventArgs e)
        {
            var bookingConsist = OrdersViewList.SelectedItem as BookingsConsist;
            var viewOrderConsist = new ViewOrderConsistWindow(bookingConsist);
            viewOrderConsist.ShowDialog();
        }

        private void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            ApplyFilter();
        }
    }
}
