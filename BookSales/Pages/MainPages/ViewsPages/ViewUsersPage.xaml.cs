using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Data.Entity;
using System.Collections.Generic;
using BookSales.Context;

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
            Loaded += ViewUsersPage_Loaded;
        }

        private async void ViewUsersPage_Loaded(object sender, RoutedEventArgs e)
        {
            using(var db = new BookSalesEntities())
            {
                var listUsers = await GetUsersList(db);
                UsersViewList.ItemsSource = listUsers;
                var style = UsersViewList.Style;
                UsersViewList.Style = null;
                UsersViewList.Style = style;
            }
        }

        private async void ApplyFilter()
        {
            string filter;
            await Task.Delay(45);
            if (FilterText.isPlaceholder) filter = string.Empty;
            else filter = FilterText.Text.Trim().ToLower();
            using (var db = new BookSalesEntities())
            {
                var listUsers = await GetUsersList(db);

                listUsers = await Task.Run(() => listUsers.Where(s => s.surname.ToLower().Trim().Contains(filter)
                                    || s.name.ToLower().Trim().Contains(filter)
                                    || s.patronymic.ToLower().Trim().Contains(filter)));

                UsersViewList.ItemsSource = await Task.Run(() => listUsers.ToList());

                var style = UsersViewList.Style;
                UsersViewList.Style = null;
                UsersViewList.Style = style;
            }
        }

        private async Task<IEnumerable<UsersViewListClass>> GetUsersList(BookSalesEntities db)
        {
            var list = await Task.Run(() => db.Users.Include(s => s.Positions).Select(s => new UsersViewListClass
            {
                id = s.id,
                name = s.name,
                surname = s.surname,
                patronymic = s.patronymic,
                login = s.login,
                password = s.password,
                dateOfBirth = s.dateOfBirth,
                image = s.image,
                positionId = s.positionId,
                Positions = s.Positions,
            }));
            return list.ToList();
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
    }

    public class UsersViewListClass : Users
    {
        public int Count { get; set; }
    }
}
