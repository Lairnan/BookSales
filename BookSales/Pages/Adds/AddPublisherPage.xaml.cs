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
    /// Логика взаимодействия для AddPublisherPage.xaml
    /// </summary>
    public partial class AddPublisherPage : Page
    {
        public AddPublisherPage()
        {
            InitializeComponent();
        }

        private async void AddPublisherBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                MessageBox.Show("Поле не может быть пустым");
                return;
            }

            try
            {
                AddPublisherBtn.IsEnabled = false;
                var name = NameBox.Text;

                using (var db = new BookSalesEntities())
                {
                    var publisher = new Publishers
                    {
                        name = name,
                    };

                    db.Publishers.Add(publisher);
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
            AddPublisherBtn.IsEnabled = true;
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
