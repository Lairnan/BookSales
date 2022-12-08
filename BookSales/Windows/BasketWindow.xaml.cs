using BookSales.BehaviorsFiles;
using BookSales.Context;
using BookSales.Pages.MainPages;
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
            // Check if user auth how guest
            if (AuthStaticUser.AuthUser == null)
            {
                // Say user that he need auth for add item in basket
                if (MessageBox.Show("Для добавления товара в корзину, вам необходимо пройти авторизация",
                    "Не удалось добавить товар в корзину",
                    MessageBoxButton.OKCancel, MessageBoxImage.Warning) != MessageBoxResult.OK) return;

                if (IsAuthNull()) return;

                // Draw auth user in top panel
                Application.Current.Windows.OfType<MainWindow>().Single().DrawAuthUser(AuthStaticUser.AuthUser);
                Application.Current.Windows.OfType<MainWindow>().Single().ViewStory.Visibility = Visibility.Visible;
                Application.Current.Windows.OfType<MainWindow>().Single().MenuAdmin.Visibility = AuthStaticUser.AuthUser.positionId == 3 
                                ? Visibility.Visible 
                                : Visibility.Collapsed;
                Application.Current.Windows.OfType<MainWindow>().Single().MenuManager.Visibility = AuthStaticUser.AuthUser.positionId == 2 
                                ? Visibility.Visible 
                                : Visibility.Collapsed;
            }

            OrderBtn.IsEnabled = false;
            if(new ConfirmOrderWindow().ShowDialog() == true)
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

        private bool IsAuthNull()
        {
            // Open AuthWindow
            new AuthWindow().ShowDialog();

            // Check if user not log in
            return AuthStaticUser.AuthUser == null;

        }

        private Orders GetOrder()
        {
            return new Orders
            {
                idUser = AuthStaticUser.AuthUser.id,
                dateOrder = DateTime.Now,
                paid = true,
                performed = false,
                price = BasketOrder.BasketOrders.Sum(s => s.Book.retailPrice * s.Count)
            };
        }
    }
}
