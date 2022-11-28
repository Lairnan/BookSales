using BookSales.Context;
using System.Windows.Controls;
using System.Linq;
using System.Data.Entity;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Windows;

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
            GetItemsAsync();
        }

        private async void GetItemsAsync()
        {
            OrdersViewList.ItemsSource = OrdersList;
            using (var db = new BookSalesEntities())
            {
                var listBooks = await Task.Run(() => db.Orders.Include(s => s.Users).Select(s => new BookingsConsist {Order = s}).ToList());
                listBooks.ForEach(s => OrdersList.Add(s));
            }
        }

        private ObservableCollection<BookingsConsist> OrdersList { get; set; } = new ObservableCollection<BookingsConsist>();

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OrdersViewList == null) return;
            ApplyFilter();
        }

        private async void ApplyFilter()
        {
            var list = OrdersList;
            IEnumerable<BookingsConsist> newList;
            if (DateTime.TryParse(FilterText.Text, out var date))
                newList = await Task.Run(() => list.Where(s => s.Order.dateOrder == date));
            else newList = list;
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

            switch (OrderBox.SelectedIndex)
            {
                case 0:
                    newList = newList.OrderBy(s => s.Price);
                    break;
                case 1:
                    newList = newList.OrderByDescending(s => s.Price);
                    break;
            }

            OrdersViewList.ItemsSource = newList.ToList();
        }

        private void FilterText_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OrdersViewList == null) return;
            ApplyFilter();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FilterText.Text = string.Empty;
        }
    }
}
