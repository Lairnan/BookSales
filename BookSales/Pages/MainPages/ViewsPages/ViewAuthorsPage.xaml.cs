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
    /// Логика взаимодействия для ViewAuthorsPage.xaml
    /// </summary>
    public partial class ViewAuthorsPage : Page
    {
        public ViewAuthorsPage()
        {
            InitializeComponent();
            Loaded += (s, e) => ApplyFilter();
        }

        public async void ApplyFilter()
        {
            var filter = FilterText.Text.Trim().ToLower();
            using (var db = new BookSalesEntities())
            {
                IEnumerable<Authors> listBooks = await Task.Run(() => db.Authors.ToList());

                listBooks = await Task.Run(() => listBooks.Where(s => s.name.ToLower().Trim().Contains(filter)));

                AuthorsViewList.ItemsSource = await Task.Run(() => listBooks.ToList());
            }
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            FilterText.Text = string.Empty;
        }

        private void FilterText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (AuthorsViewList?.ItemsSource != null) ApplyFilter();
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            ApplyFilter();
        }

        private void EditAuthorMenu_Click(object sender, RoutedEventArgs e)
        {
            var author = AuthorsViewList.SelectedItem as Authors;
            var addWindow = new AdditionalWindow();
            AdditionalWindow.AddFrame.Navigate(new EditAuthorPage(author));
            if (addWindow.ShowDialog() == true) ApplyFilter();
        }

        private void RemoveAuthorMenu_Click(object sender, RoutedEventArgs e)
        {
            if(MessageBox.Show("Вы действительно хотите удалить запись?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Information) 
                != MessageBoxResult.Yes) return;
            try
            {
                var author = AuthorsViewList.SelectedItem as Authors;
                using (var db = new BookSalesEntities())
                {
                    var authorDb = db.Authors.First(s => s.id == author.id);
                    db.Authors.Remove(authorDb);
                    db.SaveChanges();
                    ApplyFilter();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
