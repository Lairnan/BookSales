using BookSales.Context;
using BookSales.Windows;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
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
            using(var db = new BookSalesEntities())
            {
                BooksViewList.ItemsSource = db.Books.Include(s => s.Genres).Include(s => s.Authors).Include(s => s.Publishers).ToList();
                PlaceHolders = db.PlaceHolder.ToList();
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
    }
}
