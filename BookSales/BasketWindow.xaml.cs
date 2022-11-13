using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace BookSales
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
        }

        private void RemoveBook_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var basket = btn.DataContext as BasketOrder;
            var count = btn.CommandParameter as string;
            int countInt;
            if (string.IsNullOrWhiteSpace(count) || !int.TryParse(count, out countInt)) countInt = 1;
            BasketOrder.Remove(basket.Book, countInt);
            BasketViewList.Items.Refresh();
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
            BasketViewList.Items.Refresh();
        }
    }
}
