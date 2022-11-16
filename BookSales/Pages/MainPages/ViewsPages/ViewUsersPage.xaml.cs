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

        private async Task<List<UsersViewListClass>> GetUsersList(BookSalesEntities db)
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
    }

    public class UsersViewListClass : Users
    {
        public int Count { get; set; }
    }
}
