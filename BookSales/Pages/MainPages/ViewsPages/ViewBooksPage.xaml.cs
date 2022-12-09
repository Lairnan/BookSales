using BookSales.Context;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Data.Entity;
using BookSales.Pages.Edits;
using BookSales.Windows;
using System;

namespace BookSales.Pages.MainPages.ViewsPages
{
    /// <summary>
    /// Логика взаимодействия для ViewBooksPage.xaml
    /// </summary>
    public partial class ViewBooksPage : Page
    {
        public ViewBooksPage()
        {
            InitializeComponent();
            Loaded += (s, e) => RefreshItems();
        }

        public void RefreshItems()
        {
            var index = (GenreBox?.SelectedIndex ?? 0) < 1 ? 0 : GenreBox.SelectedIndex;

            using (var db = new BookSalesEntities())
            {
                var list = new List<Genres> { new Genres { name = "Очистить" } };

                list.AddRange(db.Genres.ToList());

                GenreBox.ItemsSource = list;
                GenreBox.SelectedIndex = index;
            }

            ApplyFilter();
        }

        private async void ApplyFilter()
        {
            using (var db = new BookSalesEntities())
            {
                IEnumerable<Books> newList = await Task.Run(() => db.Books
                                                .Include(s => s.Genres)
                                                .Include(s => s.Authors)
                                                .Include(s => s.Publishers)
                                                .Include(s => s.PlaceHolder)
                                                .Include(s => s.PlaceHolder.Storage));

                var genre = GenreBox.SelectedItem as Genres;
                var filter = FilterText.Text.ToLower().Trim();
                newList = await Task.Run(() =>
                {
                    if (!filter.StartsWith("@")) return newList.Where(s => s.name.ToLower().Trim().Contains(filter));

                    filter = filter.Remove(0, 1);
                    return newList.Where(s => s.Authors.surname.ToLower().Trim().Contains(filter)
                                           || s.Authors.name.ToLower().Trim().Contains(filter)
                                           || (s.Authors.patronymic != null &&
                                               s.Authors.patronymic.ToLower().Trim().Contains(filter))).ToList();
                });

                if (GenreBox?.SelectedIndex > 0) newList = await Task.Run(() => newList.Where(s => s.genreId == genre?.id));

                switch (OrderBox?.SelectedIndex)
                {
                    case 0:
                        newList = newList.OrderBy(s => s.retailPrice);
                        break;
                    case 1:
                        newList = newList.OrderByDescending(s => s.retailPrice);
                        break;
                }

                BooksViewList.ItemsSource = newList.ToList();
            }
        }

        private void FilterText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (BooksViewList?.ItemsSource != null) ApplyFilter();
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            FilterText.Text = string.Empty;
            RefreshItems();
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            FilterText.Text = string.Empty;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BooksViewList?.ItemsSource != null) ApplyFilter();
        }

        private void EditBookMenu_Click(object sender, RoutedEventArgs e)
        {
            var book = BooksViewList.SelectedItem as Books;
            var addWindow = new AdditionalWindow();
            AdditionalWindow.AddFrame.Navigate(new EditBookPage(book));
            if (addWindow.ShowDialog() == true) RefreshItems();
        }

        private void RemoveBookMenu_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите удалить запись?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Information)
                != MessageBoxResult.Yes) return;
            try
            {
                var book = BooksViewList.SelectedItem as Books;
                using (var db = new BookSalesEntities())
                {
                    var bookDb = db.Books.First(s => s.id == book.id);
                    db.Books.Remove(bookDb);
                    db.SaveChanges();
                    RefreshItems();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
