using BookSales.Context;
using BookSales.Windows;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BookSales.Pages.Edits
{
    /// <summary>
    /// Логика взаимодействия для EditGenrePage.xaml
    /// </summary>
    public partial class EditGenrePage : Page
    {
        public EditGenrePage(Genres genre)
        {
            InitializeComponent();

            NameBox.Text = genre.name;
            Genre = genre;
        }

        private Genres Genre { get; set; }

        private async void SaveGenreBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                MessageBox.Show("Поле не может быть пустым");
                return;
            }

            try
            {
                SaveGenreBtn.IsEnabled = false;
                var name = NameBox.Text;

                using (var db = new BookSalesEntities())
                {
                    var genre = db.Genres.First(s => s.id == Genre.id);
                    genre.name = name;
                    await db.SaveChangesAsync();
                    MessageBox.Show("Изменения сохранены");
                }

                var wnd = Window.GetWindow(this);
                wnd.DialogResult = true;
                wnd.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            SaveGenreBtn.IsEnabled = true;
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            if (AdditionalWindow.AddFrame.CanGoBack) AdditionalWindow.AddFrame.GoBack();
            else
            {
                var wnd = Window.GetWindow(this);
                wnd.DialogResult = false;
                wnd.Close();
            }
        }
    }
}
