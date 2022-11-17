using BookSales.Context;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BookSales.Windows
{
    /// <summary>
    /// Логика взаимодействия для BasketWindow.xaml
    /// </summary>
    public partial class BasketWindow : Window
    {
        public BasketWindow()
        {
            InitializeComponent();
            BasketViewList.ItemsSource = BasketOrder.BasketOrders;
            UpdateOrder();
        }

        private void RemoveBook_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var basket = btn.DataContext as BasketOrder;
            var count = btn.CommandParameter as string;
            int countInt;
            if (string.IsNullOrWhiteSpace(count) || !int.TryParse(count, out countInt)) countInt = 1;
            BasketOrder.Remove(basket.Book, countInt);
            UpdateOrder();

            if (BasketViewList.Items.Count > 0) return;
            var styleTemplate = BasketViewList.Style;
            BasketViewList.Style = null;
            BasketViewList.Style = styleTemplate;
        }

        private void AddBook_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var basket = btn.DataContext as BasketOrder;
            var count = btn.CommandParameter as string;
            int countInt;
            if (string.IsNullOrWhiteSpace(count) || !int.TryParse(count, out countInt)) countInt = 1;
            BasketOrder.Add(basket.Book, countInt, basket.Stock);
            UpdateOrder();
        }

        private void UpdateOrder()
        {
            BasketViewList.Items.Refresh();
            if (BasketOrder.BasketOrders.Any()) OrderPrice.Text = $"Итоговая цена является: {BasketOrder.BasketOrders.Sum(s => s.Book.retailPrice * s.Count):0.00} руб.";
            else OrderPrice.Text = string.Empty;

            OrderBtn.IsEnabled = BasketOrder.BasketOrders.Any();
        }

        private async void OrderBtn_Click(object sender, RoutedEventArgs e)
        {
            OrderBtn.IsEnabled = false;
            var confirmWindow = new ConfirmOrderWindow();
            if(confirmWindow.ShowDialog() == true)
            {
                using (var db = new BookSalesEntities())
                {
                    var order = GetOrder();

                    db.Orders.Add(order);

                    foreach (var basket in BasketOrder.BasketOrders)
                    {
                        var orderConsist = new OrderConsist
                        {
                            idOrder = order.id,
                            idBook = basket.Book.id,
                            amount = basket.Count
                        };

                        db.OrderConsist.Add(orderConsist);
                    }
                    await db.SaveChangesAsync();
                    MessageBox.Show("Заказ успешно сформирован");
                    BasketOrder.BasketOrders.Clear();
                    this.Close();
                    UpdateOrder();
                }
            }
            OrderBtn.IsEnabled = true;
        }

        private Orders GetOrder()
        {
            return new Orders
            {
                idUser = AuthStaticUser.AuthUser.id,
                dateOrder = DateTime.Now,
                paid = true,
                performed = false
            };
        }
    }
}
