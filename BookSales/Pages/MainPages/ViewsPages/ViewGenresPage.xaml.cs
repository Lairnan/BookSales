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
    /// Логика взаимодействия для ViewGenresPage.xaml
    /// </summary>
    public partial class ViewGenresPage : Page
    {
        public ViewGenresPage()
        {
            InitializeComponent();
            Loaded += (s, e) => ApplyFilter();
        }

        public async void ApplyFilter()
        {
            var filter = FilterText.Text.Trim().ToLower();
            using (var db = new BookSalesEntities())
            {
                IEnumerable<Genres> listBooks = await Task.Run(() => db.Genres.ToList());

                listBooks = await Task.Run(() => listBooks.Where(s => s.name.ToLower().Trim().Contains(filter)));

                GenresViewList.ItemsSource = await Task.Run(() => listBooks.ToList());
            }
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            FilterText.Text = string.Empty;
        }

        private void FilterText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (GenresViewList?.ItemsSource != null) ApplyFilter();
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            ApplyFilter();
        }

        private void EditGenreMenu_Click(object sender, RoutedEventArgs e)
        {
            var genre = GenresViewList.SelectedItem as Genres;
            var addWindow = new AdditionalWindow();
            AdditionalWindow.AddFrame.Navigate(new EditGenrePage(genre));
            if (addWindow.ShowDialog() == true) ApplyFilter();
        }

        private void RemoveGenreMenu_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите удалить запись?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Information)
                != MessageBoxResult.Yes) return;
            try
            {
                var genre = GenresViewList.SelectedItem as Genres;
                using (var db = new BookSalesEntities())
                {
                    var genreDb = db.Genres.First(s => s.id == genre.id);
                    db.Genres.Remove(genreDb);
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
