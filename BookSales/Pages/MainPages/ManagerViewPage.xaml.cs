using BookSales.Pages.MainPages.ViewsPages;
using BookSales.Windows;
using System.Windows;
using System.Windows.Controls;

namespace BookSales.Pages.MainPages
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class ManagerViewPage : Page
    {
        public ManagerViewPage()
        {
            InitializeComponent();
        }

        private void ViewUsersBtn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.MainFrame.Navigate(new ViewUsersPage());
        }

        private void ViewOredersBtn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.MainFrame.Navigate(new ViewOrdersPage());
        }
    }
}
