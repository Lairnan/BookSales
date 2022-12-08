using BookSales.BehaviorsFiles;
using BookSales.Context;
using BookSales.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BookSales.Pages.MainPages
{
    /// <summary>
    /// Логика взаимодействия для ViewBooksPage.xaml
    /// </summary>
    public partial class ClientViewPage : Page
    {
        public ClientViewPage()
        {
            InitializeComponent();
            GetItemsAsync();
        }

        private async void GetItemsAsync()
        {
            using (var db = new BookSalesEntities())
            {
                var listBooks = await Task.Run(() => db.Books
                                        .Include(s => s.Genres)
                                        .Include(s => s.Authors)
                                        .Include(s => s.Publishers)
                                        .Include(s => s.PlaceHolder)
                                        .ToList());
                BooksViewList.ItemsSource = listBooks;
                var listGenres = new List<Genres>();
                listGenres.Add(new Genres { name = "Очистить" });
                await Task.Run(() => listGenres.AddRange(db.Genres.ToList()));
                GenreBox.ItemsSource = listGenres;
            }
        }

        private void AddInBasketBtn_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var book = btn.DataContext as Books;
            var count = btn.CommandParameter as string;
            int countInt;
            if (string.IsNullOrWhiteSpace(count) || !int.TryParse(count, out countInt)) countInt = 1;
            if (book.PlaceHolder.stock < 1)
            {
                MessageBox.Show("Данной книги нет в наличии");
                return;
            }
            BasketOrder.Add(book, countInt, book.PlaceHolder.stock);
        }

        private void FilterText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (BooksViewList == null) return;
            ApplyFilter();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BooksViewList == null) return;
            ApplyFilter();
        }

        public async void ApplyFilter()
        {
            using (var db = new BookSalesEntities())
            {
                IEnumerable<Books> newList = await Task.Run(() => db.Books
                                                .Include(s => s.Genres)
                                                .Include(s => s.Authors)
                                                .Include(s => s.Publishers)
                                                .Include(s => s.PlaceHolder));

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

                if (GenreBox.SelectedIndex != 0) newList = await Task.Run(() => newList.Where(s => s.genreId == genre?.id));

                switch (OrderBox.SelectedIndex)
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
        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            FilterText.Text = string.Empty;
        }
    }
}
