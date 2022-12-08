using BookSales.Context;
using BookSales.Windows;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BookSales.Pages.Edits
{
    /// <summary>
    /// Логика взаимодействия для EditAuthorPage.xaml
    /// </summary>
    public partial class EditAuthorPage : Page
    {
        public EditAuthorPage(Authors author)
        {
            InitializeComponent();

            SurnameBox.Text = author.surname;
            NameBox.Text = author.name;
            PatronymicBox.Text = author.patronymic;

            Author = author;
        }

        private Authors Author { get; set; }

        private async void SaveAuthorBtn_Click(object sender, RoutedEventArgs e)
        {
            if (IsNullOrWhiteSpace())
            {
                MessageBox.Show("Поля не могут быть пустыми");
                return;
            }

            try
            {
                SaveAuthorBtn.IsEnabled = false;
                var surname = SurnameBox.Text;
                var name = NameBox.Text;
                var patronymic = string.IsNullOrWhiteSpace(PatronymicBox.Text) ? null : PatronymicBox.Text;

                using (var db = new BookSalesEntities())
                {
                    var author = db.Authors.First(s => s.id == Author.id);
                    author.surname = surname;
                    author.name = name;
                    author.patronymic = patronymic;
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
            SaveAuthorBtn.IsEnabled = true;
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

        private bool IsNullOrWhiteSpace()
        {
            if (string.IsNullOrWhiteSpace(SurnameBox.Text)
                || string.IsNullOrWhiteSpace(NameBox.Text))
                return true;
            return false;
        }
    }
}
