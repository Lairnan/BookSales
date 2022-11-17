using BookSales.Context;
using System.Linq;
using System.Windows.Controls;
using System.Data.Entity;
using System.Windows.Input;
using System.Windows;
using BookSales.Windows;

namespace BookSales.Pages.MainPages
{
    /// <summary>
    /// Логика взаимодействия для StoryViewPage.xaml
    /// </summary>
    public partial class StoryViewPage : Page
    {
        public StoryViewPage()
        {
            InitializeComponent();
            using (var db = new BookSalesEntities())
            {
                var authUser = AuthStaticUser.AuthUser;
                StoryOrderList.ItemsSource = db.Orders.Include(s => s.Users).Where(s => s.idUser == authUser.id).Select(s => new BookingsConsist
                {
                    Order = s
                }).ToList();
            }
        }

        private void StoryView_Click(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;

            var stackPanel = sender as StackPanel;
            var itemsControl = stackPanel.Children[1];
            itemsControl.Visibility = itemsControl.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
