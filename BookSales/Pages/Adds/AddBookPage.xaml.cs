using BookSales.BehaviorsFiles;
using BookSales.Context;
using BookSales.Windows;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BookSales.Pages.Adds
{
    /// <summary>
    /// Логика взаимодействия для AddBookPage.xaml
    /// </summary>
    public partial class AddBookPage : Page
    {
        public AddBookPage()
        {
            InitializeComponent();

            ReleaseDateBox.DisplayDateStart = DateTime.Parse("01.01.1900");
            ReleaseDateBox.DisplayDateEnd = DateTime.Today;

            InitializeValues();
        }

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

        private async void AddBookBtn_Click(object sender, RoutedEventArgs e)
        {
            if (IsNullOrWhiteSpace())
            {
                MessageBox.Show("Поля не могут быть пустыми");
                return;
            }
            try
            {
                AddBookBtn.IsEnabled = false;
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
                if (releaseDate > ReleaseDateBox.DisplayDateEnd || releaseDate < ReleaseDateBox.DisplayDateStart)
                {
                    MessageBox.Show("Неверно выбранная дата");
                    return;
                }
                var retailPrice = decimal.Parse(RetailPriceBox.Text);
                var amount = int.Parse(AmountBox.Text);
                var storage = StorageBox.SelectedItem as Storage;
                byte[] image = null;
                if (!string.IsNullOrWhiteSpace(FileNamePath))
                    image = File.ReadAllBytes(FileNamePath);

                using(var db = new BookSalesEntities())
                {
                    var book = new Books
                    {
                        name = name,
                        authorId = author.id,
                        genreId = genre.id,
                        publisherId = publisher.id,
                        pages = pageCount,
                        releaseDate = releaseDate,
                        retailPrice = retailPrice,
                        image = image,
                    };

                    db.Books.Add(book);
                    await db.SaveChangesAsync();

                    var placeHolder = new PlaceHolder
                    {
                        idBook = book.id,
                        idStorage = storage.id,
                        stock = amount
                    };
                    
                    db.PlaceHolder.Add(placeHolder);
                    await db.SaveChangesAsync();
                    MessageBox.Show("Успешно добавлено");
                }

                var wnd = Window.GetWindow(this);
                wnd.DialogResult = true;
                wnd.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            AddBookBtn.IsEnabled = true;
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
