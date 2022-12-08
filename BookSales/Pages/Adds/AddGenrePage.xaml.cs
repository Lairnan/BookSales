using BookSales.Context;
using BookSales.Windows;
using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для AddGenrePage.xaml
    /// </summary>
    public partial class AddGenrePage : Page
    {
        public AddGenrePage()
        {
            InitializeComponent();
        }

        private async void AddGenreBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                MessageBox.Show("Поле не может быть пустым");
                return;
            }

            try
            {
                AddGenreBtn.IsEnabled = false;
                var name = NameBox.Text;

                using (var db = new BookSalesEntities())
                {
                    var genre = new Genres
                    {
                        name = name,
                    };

                    db.Genres.Add(genre);
                    await db.SaveChangesAsync();
                    MessageBox.Show("Успешно добавлено");
                }

                if (AdditionalWindow.AddFrame.CanGoBack)
                {
                    AdditionalWindow.AddFrame.GoBack();
                    (AdditionalWindow.AddFrame.Content as AddBookPage)?.InitializeValues();
                }
                else
                {
                    var wnd = Window.GetWindow(this);
                    wnd.DialogResult = true;
                    wnd.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            AddGenreBtn.IsEnabled = true;
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
