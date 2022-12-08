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
    /// Логика взаимодействия для AddAuthorPage.xaml
    /// </summary>
    public partial class AddAuthorPage : Page
    {
        public AddAuthorPage()
        {
            InitializeComponent();
        }

        private async void AddAuthorBtn_Click(object sender, RoutedEventArgs e)
        {
            if (IsNullOrWhiteSpace())
            {
                MessageBox.Show("Поля не могут быть пустыми");
                return;
            }

            try
            {
                AddAuthorBtn.IsEnabled = false;
                var surname = SurnameBox.Text;
                var name = NameBox.Text;
                var patronymic = string.IsNullOrWhiteSpace(PatronymicBox.Text) ? null : PatronymicBox.Text;

                using (var db = new BookSalesEntities())
                {
                    var author = new Authors
                    {
                        surname = surname,
                        name = name,
                        patronymic = patronymic
                    };

                    db.Authors.Add(author);
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
            AddAuthorBtn.IsEnabled = true;
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
