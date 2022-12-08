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
    /// Логика взаимодействия для ViewPublishersPage.xaml
    /// </summary>
    public partial class ViewPublishersPage : Page
    {
        public ViewPublishersPage()
        {
            InitializeComponent();
            Loaded += (s, e) => ApplyFilter();
        }

        public async void ApplyFilter()
        {
            var filter = FilterText.Text.Trim().ToLower();
            using (var db = new BookSalesEntities())
            {
                IEnumerable<Publishers> listBooks = await Task.Run(() => db.Publishers.ToList());

                listBooks = await Task.Run(() => listBooks.Where(s => s.name.ToLower().Trim().Contains(filter)));

                PublishersViewList.ItemsSource = await Task.Run(() => listBooks.ToList());
            }
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            FilterText.Text = string.Empty;
        }

        private void FilterText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (PublishersViewList?.ItemsSource != null) ApplyFilter();
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            ApplyFilter();
        }

        private void EditPublisherMenu_Click(object sender, RoutedEventArgs e)
        {
            var publisher = PublishersViewList.SelectedItem as Publishers;
            var addWindow = new AdditionalWindow();
            AdditionalWindow.AddFrame.Navigate(new EditPublisherPage(publisher));
            if (addWindow.ShowDialog() == true) ApplyFilter();
        }

        private void RemovePublisherMenu_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите удалить запись?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Information)
                != MessageBoxResult.Yes) return;
            try
            {
                var publisher = PublishersViewList.SelectedItem as Publishers;
                using (var db = new BookSalesEntities())
                {
                    var publisherDb = db.Publishers.First(s => s.id == publisher.id);
                    db.Publishers.Remove(publisherDb);
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
