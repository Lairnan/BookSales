using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Data.Entity;
using BookSales.Context;
using BookSales.Pages.Edits;
using BookSales.Windows;
using System;

namespace BookSales.Pages.MainPages.ViewsPages
{
    /// <summary>
    /// Логика взаимодействия для ViewUsersPage.xaml
    /// </summary>
    public partial class ViewUsersPage : Page
    {
        public ViewUsersPage()
        {
            InitializeComponent();
            Loaded += (s, e) => ApplyFilter();
        }

        public async void ApplyFilter()
        {
            var filter = FilterText.Text.Trim().ToLower();
            using (var db = new BookSalesEntities())
            {
                var listUsers = await Task.Run(() => db.Users.Include(s => s.Positions));

                listUsers = await Task.Run(() => listUsers.Where(s => s.surname.ToLower().Trim().Contains(filter)
                                    || s.name.ToLower().Trim().Contains(filter)
                                    || (s.patronymic != null && s.patronymic.ToLower().Trim().Contains(filter))));

                UsersViewList.ItemsSource = await Task.Run(() => listUsers.ToList());
            }
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            FilterText.Text = string.Empty;
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            var user = (sender as Button).DataContext as Users;
        }

        private void RemoveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите удалить?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) 
                != MessageBoxResult.Yes) return;
            var user = (sender as Button).DataContext as Users;
            using(var db = new BookSalesEntities())
            {
                var dbUser = db.Users.First(s => s.id == user.id);
                db.Users.Remove(dbUser);
                db.SaveChanges();
            }
            ApplyFilter();
        }

        private void FilterText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (UsersViewList?.ItemsSource != null) ApplyFilter();
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            ApplyFilter();
        }

        private void EditUserMenu_Click(object sender, RoutedEventArgs e)
        {
            var user = UsersViewList.SelectedItem as Users;
            var addWindow = new AdditionalWindow();
            AdditionalWindow.AddFrame.Navigate(new EditUserPage(user));
            if (addWindow.ShowDialog() == true) ApplyFilter();
        }

        private void RemoveUserMenu_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите удалить запись?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Information)
                != MessageBoxResult.Yes) return;
            try
            {
                var user = UsersViewList.SelectedItem as Users;
                using (var db = new BookSalesEntities())
                {
                    var userDb = db.Users.First(s => s.id == user.id);
                    db.Users.Remove(userDb);
                    db.SaveChanges();
                    ApplyFilter();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
