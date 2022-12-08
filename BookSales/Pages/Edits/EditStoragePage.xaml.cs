using BookSales.Context;
using BookSales.Windows;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BookSales.Pages.Edits
{
    /// <summary>
    /// Логика взаимодействия для EditStoragePage.xaml
    /// </summary>
    public partial class EditStoragePage : Page
    {
        public EditStoragePage(Storage storage)
        {
            InitializeComponent();

            NameBox.Text = storage.name;
            AddressBox.Text = storage.address;
            Storage = storage;
        }

        private Storage Storage { get; set; }

        private async void SaveStorageBtn_Click(object sender, RoutedEventArgs e)
        {
            if (IsNullOrWhiteSpace())
            {
                MessageBox.Show("Поля не могут быть пустыми");
                return;
            }

            try
            {
                SaveStorageBtn.IsEnabled = false;
                var name = NameBox.Text;
                var address = AddressBox.Text;

                using (var db = new BookSalesEntities())
                {
                    var storage = db.Storage.First(s => s.id == Storage.id);
                    storage.name = name;
                    storage.address = address;
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
            SaveStorageBtn.IsEnabled = true;
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
