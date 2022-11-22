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

        private ObservableCollection<Books> BooksList { get; set; }

        private async void GetItemsAsync()
        {
            using (var db = new BookSalesEntities())
            {
                var listBooks = db.Books.Include(s => s.Genres).Include(s => s.Authors).Include(s => s.Publishers).ToList();
                BooksList = new ObservableCollection<Books>(listBooks);
                BooksViewList.ItemsSource = listBooks;
                PlaceHolders = db.PlaceHolder.ToList();
                var listGenres = new List<Genres>();
                listGenres.Add(new Genres { name = "Очистить" });
                listGenres.AddRange(await Task.Run(() => db.Genres.ToList()));
                GenreBox.ItemsSource = listGenres;
            }
        }

        private List<PlaceHolder> PlaceHolders { get; set; }

        private void AddInBasketBtn_Click(object sender, RoutedEventArgs e)
        {
            // Check if user auth how guest
            if(AuthStaticUser.AuthUser == null)
            {
                // Say user that he need auth for add item in basket
                if (MessageBox.Show("Для добавления товара в корзину, вам необходимо пройти авторизация",
                    "Не удалось добавить товар в корзину",
                    MessageBoxButton.OKCancel, MessageBoxImage.Warning) != MessageBoxResult.OK) return;

                if (IsAuthNull()) return;
                
                // Draw auth user in top panel
                Application.Current.Windows.OfType<MainWindow>().Single().DrawAuthUser(AuthStaticUser.AuthUser);
            }

            var btn = sender as Button;
            var book = btn.DataContext as Books;
            var count = btn.CommandParameter as string;
            int countInt;
            if (string.IsNullOrWhiteSpace(count) || !int.TryParse(count, out countInt)) countInt = 1;
            var placeHolder = PlaceHolders.SingleOrDefault(s => s.idBook == book.id);
            if (placeHolder == null)
            {
                MessageBox.Show("Данной книги нет в наличии");
                return;
            }
            BasketOrder.Add(book, countInt, placeHolder.stock);
        }

        private bool IsAuthNull()
        {
            // Open AuthWindow
            var authWindow = new AuthWindow();
            authWindow.IsDialog = true;
            authWindow.ShowDialog();

            // Check if user not log in
            return AuthStaticUser.AuthUser == null;

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

        private async void ApplyFilter()
        {
            var list = BooksList;
            var genre = GenreBox.SelectedItem as Genres;

            var newList = await Task.Run(() => list.Where(s => s.name.ToLower().Trim().Contains(FilterText.Text.ToLower().Trim())));
            if (GenreBox.SelectedIndex != 0) newList = await Task.Run(() => newList.Where(s => s.genreId == genre.id));

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
}
