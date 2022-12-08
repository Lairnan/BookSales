using BookSales.Context;
using BookSales.Pages.Edits;
using BookSales.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BookSales.Pages.MainPages.ViewsPages
{
    /// <summary>
    /// Логика взаимодействия для ViewStoragesPage.xaml
    /// </summary>
    public partial class ViewStoragesPage : Page
    {
        public ViewStoragesPage()
        {
            InitializeComponent();
            Loaded += (s, e) => ApplyFilter();
        }

        public async void ApplyFilter()
        {
            var filter = FilterText.Text.Trim().ToLower();
            using (var db = new BookSalesEntities())
            {
                IEnumerable<Storage> listBooks = await Task.Run(() => db.Storage.ToList());

                listBooks = await Task.Run(() =>
                {
                    if (!filter.StartsWith("@")) return listBooks.Where(s => s.name.ToLower().Trim().Contains(filter));

                    filter = filter.Remove(0, 1);
                    return listBooks.Where(s => s.address.ToLower().Trim().Contains(filter));
                });
                listBooks = await Task.Run(() => listBooks.Where(s => s.address.ToLower().Trim().Contains(filter)));

                StoragesViewList.ItemsSource = await Task.Run(() => listBooks.ToList());
            }
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            FilterText.Text = string.Empty;
        }

        private void FilterText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (StoragesViewList?.ItemsSource != null) ApplyFilter();
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            ApplyFilter();
        }

        private void EditStorageMenu_Click(object sender, RoutedEventArgs e)
        {
            var storage = StoragesViewList.SelectedItem as Storage;
            var addWindow = new AdditionalWindow();
            AdditionalWindow.AddFrame.Navigate(new EditStoragePage(storage));
            if (addWindow.ShowDialog() == true) ApplyFilter();
        }

        private void RemoveStorageMenu_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите удалить запись?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Information)
                != MessageBoxResult.Yes) return;
            try
            {
                var storage = StoragesViewList.SelectedItem as Storage;
                using (var db = new BookSalesEntities())
                {
                    var storageDb = db.Storage.First(s => s.id == storage.id);
                    db.Storage.Remove(storageDb);
                    db.SaveChanges();
                    ApplyFilter();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
