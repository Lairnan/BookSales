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
    /// Логика взаимодействия для AddStoragePage.xaml
    /// </summary>
    public partial class AddStoragePage : Page
    {
        public AddStoragePage()
        {
            InitializeComponent();
        }

        private async void AddStorageBtn_Click(object sender, RoutedEventArgs e)
        {
            if (IsNullOrWhiteSpace())
            {
                MessageBox.Show("Поля не могут быть пустыми");
                return;
            }

            try
            {
                AddStorageBtn.IsEnabled = false;
                var name = NameBox.Text;
                var address = AddressBox.Text;

                using (var db = new BookSalesEntities())
                {
                    var storage = new Storage
                    {
                        name = name,
                        address = address
                    };

                    db.Storage.Add(storage);
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
            AddStorageBtn.IsEnabled = true;
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
            if (string.IsNullOrWhiteSpace(NameBox.Text)
                || string.IsNullOrWhiteSpace(AddressBox.Text))
                return true;
            return false;
        }
    }
}
