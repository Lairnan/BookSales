using BookSales.BehaviorsFiles;
using BookSales.Context;
using BookSales.Windows;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Data.Entity;
using BookSales.Pages.Adds;

namespace BookSales.Pages.Edits
{
    /// <summary>
    /// Логика взаимодействия для EditBookPage.xaml
    /// </summary>
    public partial class EditBookPage : Page
    {
        public EditBookPage(Books book)
        {
            InitializeComponent();

            ReleaseDateBox.DisplayDateEnd = DateTime.Today;

            Book = book;

            InitializeValues();

            NameBox.Text = book.name;
            ImageBox.Source = book.image?.ToImageSource();
            PageCountBox.Text = book.pages.ToString();
            RetailPriceBox.Text = book.retailPrice.ToString("0.00");
            ReleaseDateBox.SelectedDate = book.releaseDate;
            AmountBox.Text = book.PlaceHolder.stock.ToString();

        }

        private Books Book { get; set; }

        public async void InitializeValues()
        {
            using(var db = new BookSalesEntities())
            {
                var listAuthors = await Task.Run(() => db.Authors.ToList());
                var listGenres = await Task.Run(() => db.Genres.ToList());
                var listPublishers = await Task.Run(() => db.Publishers.ToList());
                var listStorages = await Task.Run(() => db.Storage.ToList());

                AuthorBox.ItemsSource = listAuthors;
                GenreBox.ItemsSource = listGenres;
                PublisherBox.ItemsSource = listPublishers;
                StorageBox.ItemsSource = listStorages;

                AuthorBox.SelectedItem = (AuthorBox.ItemsSource as List<Authors>).First(s => s.id == Book.authorId);
                GenreBox.SelectedItem = (GenreBox.ItemsSource as List<Genres>).First(s => s.id == Book.genreId);
                PublisherBox.SelectedItem = (PublisherBox.ItemsSource as List<Publishers>).First(s => s.id == Book.publisherId);
                StorageBox.SelectedItem = (StorageBox.ItemsSource as List<Storage>).First(s => s.id == Book.PlaceHolder.idStorage);
            }
        }

        private string FileNamePath { get; set; }

        private void AddImageBtn_Click(object sender, RoutedEventArgs e)
        {
            var openFile = new OpenFileDialog()
            {
                Filter = "Image (png, jpg, jpeg, bmp) | *.png; *.jpg; *.jpeg; *.bmp"
            };
            if (openFile.ShowDialog() == false) return;
            var image = openFile.FileName;
            if (image == null) return;

            ImageBox.Source = File.ReadAllBytes(image).ToImageSource();
            FileNamePath = image;
            ImageBox.Visibility = Visibility.Visible;
            BtnClear.IsEnabled = true;
        }

        private void ClearImageBtn_Click(object sender, RoutedEventArgs e)
        {
            ImageBox.Visibility = Visibility.Collapsed;
            ImageBox.Source = null;
            BtnClear.IsEnabled = false;
        }

        private async void SaveBookBtn_Click(object sender, RoutedEventArgs e)
        {
            if (IsNullOrWhiteSpace())
            {
                MessageBox.Show("Поля не могут быть пустыми");
                return;
            }
            try
            {
                SaveBookBtn.IsEnabled = false;
                var name = NameBox.Text;
                var author = AuthorBox.SelectedItem as Authors;
                var genre = GenreBox.SelectedItem as Genres;
                var publisher = PublisherBox.SelectedItem as Publishers;
                var pageCount = int.Parse(PageCountBox.Text);
                var releaseDate = ReleaseDateBox.SelectedDate.Value;
                if (releaseDate == null)
                {
                    MessageBox.Show("Выберите дату!");
                    return;
                }
                if (releaseDate > ReleaseDateBox.DisplayDateEnd)
                {
                    MessageBox.Show("Неверно выбранная дата");
                    return;
                }
                var retailPrice = decimal.Parse(RetailPriceBox.Text.Replace('.', ','));
                var amount = Math.Abs(int.Parse(AmountBox.Text));
                var storage = StorageBox.SelectedItem as Storage;
                byte[] image = null;
                if (!string.IsNullOrWhiteSpace(FileNamePath))
                    image = File.ReadAllBytes(FileNamePath);
                else if (string.IsNullOrWhiteSpace(FileNamePath) && ImageBox.Source != null) image = Book.image;

                using(var db = new BookSalesEntities())
                {
                    var book = db.Books.Include(s => s.PlaceHolder).First(s => s.id == Book.id);
                    book.name = name;
                    book.authorId = author.id;
                    book.genreId = genre.id;
                    book.publisherId = publisher.id;
                    book.pages = pageCount;
                    book.releaseDate = releaseDate;
                    book.retailPrice = retailPrice;
                    book.image = image;
                    book.PlaceHolder.stock = amount;
                    book.PlaceHolder.idStorage = storage.id;
                    await db.SaveChangesAsync();
                    MessageBox.Show("Изменения сохранены");
                }

                var wnd = Window.GetWindow(this);
                wnd.DialogResult = true;
                wnd.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            SaveBookBtn.IsEnabled = true;
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            var wnd = Window.GetWindow(this);
            wnd.DialogResult = false;
            wnd.Close();
        }

        private bool IsNullOrWhiteSpace()
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text)
                || string.IsNullOrWhiteSpace(PageCountBox.Text)
                || string.IsNullOrWhiteSpace(RetailPriceBox.Text)
                || string.IsNullOrWhiteSpace(AmountBox.Text))
                return true;
            return false;
        }

        private void AddAuthorBtn_Click(object sender, RoutedEventArgs e)
        {
            AdditionalWindow.AddFrame.Navigate(new AddAuthorPage());
        }

        private void AddGenreBtn_Click(object sender, RoutedEventArgs e)
        {
            AdditionalWindow.AddFrame.Navigate(new AddGenrePage());
        }

        private void AddPublisherBtn_Click(object sender, RoutedEventArgs e)
        {
            AdditionalWindow.AddFrame.Navigate(new AddPublisherPage());
        }

        private void AddStorageBtn_Click(object sender, RoutedEventArgs e)
        {
            AdditionalWindow.AddFrame.Navigate(new AddStoragePage());
        }
    }
}
