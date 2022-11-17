using BookSales.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BookSales.Windows
{
    /// <summary>
    /// Логика взаимодействия для ConfirmOrderWindow.xaml
    /// </summary>
    public partial class ConfirmOrderWindow : Window
    {
        public ConfirmOrderWindow()
        {
            InitializeComponent();
            BasketViewList.ItemsSource = BasketOrder.BasketOrders;
            if (BasketOrder.BasketOrders.Any()) OrderPrice.Text = $"Итоговая цена является: {GetPrice():0.00} руб.";
            else OrderPrice.Text = string.Empty;
        }

        private void ConfirmBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }

        private decimal GetPrice()
        {
            decimal price = 0;
            foreach (var item in BasketOrder.BasketOrders)
            {
                price += item.Book.retailPrice * item.Count;
            }

            return price;
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }
    }
}
