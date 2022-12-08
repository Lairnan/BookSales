using BookSales.Context;
using BookSales.Windows;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BookSales.Pages.Edits
{
    /// <summary>
    /// Логика взаимодействия для EditPublisherPage.xaml
    /// </summary>
    public partial class EditPublisherPage : Page
    {
        public EditPublisherPage(Publishers publisher)
        {
            InitializeComponent();

            NameBox.Text = publisher.name;
            Publisher = publisher;
        }

        private Publishers Publisher { get; set; }

        private async void SavePublisherBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                MessageBox.Show("Поле не может быть пустым");
                return;
            }

            try
            {
                SavePublisherBtn.IsEnabled = false;
                var name = NameBox.Text;

                using (var db = new BookSalesEntities())
                {
                    var publisher = db.Publishers.First(s => s.id == Publisher.id);
                    publisher.name = name;
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
            SavePublisherBtn.IsEnabled = true;
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
