using BookSales.BehaviorsFiles;
using BookSales.Context;
using BookSales.Pages.Adds;
using BookSales.Pages.MainPages;
using BookSales.Pages.MainPages.ViewsPages;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace BookSales.Windows
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.MouseDown += (s, e) => Keyboard.ClearFocus();
            Loaded += OnLoaded;
        }

        private void MainFrameOnNavigated(object sender, NavigationEventArgs e)
        {
            BackBtn.IsEnabled = MainFrame.CanGoBack;

            var frame = sender as Frame;
            ViewStory.Visibility = !(frame.Content is StoryViewPage) && AuthStaticUser.AuthUser != null ? Visibility.Visible : Visibility.Collapsed;
            BasketBtn.Visibility = frame.Content is ClientViewPage ? Visibility.Visible : Visibility.Collapsed;

            this.Title = (frame.Content as Page).Title;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (AuthStaticUser.AuthUser != null) DrawAuthUser(AuthStaticUser.AuthUser);

            MainFrame = this.CurrentFrame;
            MainFrame.Navigated += MainFrameOnNavigated;

            if (AuthStaticUser.AuthUser == null)
            {
                MainFrame.Navigate(new ClientViewPage());
                return;
            }

            switch (AuthStaticUser.AuthUser.Positions.id)
            {
                case 1:
                    MainFrame.Navigate(new ClientViewPage());
                    break;
                case 2:
                case 3:
                    MainFrame.Navigate(new ViewOrdersPage());
                    break;
                default:
                    MessageBox.Show("Данное окно не отработано");
                    this.Close(); 
                    break;
            }

            if(AuthStaticUser.AuthUser != null && AuthStaticUser.AuthUser.Positions.id > 1) 
            {
                ViewBooks.Visibility = Visibility.Visible;
            }
        }

        private void OpenBasketWindow_Click(object sender, RoutedEventArgs e)
        {
            if (new BasketWindow().ShowDialog() == true)
            {
                if (MainFrame.Content is ClientViewPage clientViewPage)
                {
                    clientViewPage.GetItemsAsync();
                    clientViewPage.ApplyFilter();
                }
            }
        }

        public void DrawAuthUser(Users authUser)
        {
            AuthUser.Text = $"{authUser.surname} " +
                $"{authUser.name} " +
                $"{authUser.patronymic}, " +
                $"{authUser.Positions.name}";
        }

        internal static Frame MainFrame { get; private set; }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            new AuthWindow().Show();
            AuthStaticUser.AuthUser = null;
            this.Close();
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.Content is ClientViewPage &&
                AuthStaticUser.AuthUser != null && AuthStaticUser.AuthUser.Positions.id > 1)
                ViewBooks.Visibility = Visibility.Visible;
            MainFrame.GoBack();
        }

        private void ViewClientPageBtn_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ClientViewPage());
            ViewBooks.Visibility = Visibility.Collapsed;
        }

        private void ViewStoryBtn_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new StoryViewPage());
        }

        private void ViewBooksMenu_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.Content is ViewBooksPage) return;
            MainFrame.Navigate(new ViewBooksPage());
        }

        private void ViewAuthorsMenu_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.Content is ViewAuthorsPage) return;
            MainFrame.Navigate(new ViewAuthorsPage());
        }

        private void ViewGenresMenu_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.Content is ViewGenresPage) return;
            MainFrame.Navigate(new ViewGenresPage());
        }

        private void ViewPublishersMenu_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.Content is ViewPublishersPage) return;
            MainFrame.Navigate(new ViewPublishersPage());
        }

        private void ViewUsersMenu_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.Content is ViewUsersPage) return;
            MainFrame.Navigate(new ViewUsersPage());
        }

        private void ViewOrdersMenu_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.Content is ViewOrdersPage) return;
            MainFrame.Navigate(new ViewOrdersPage());
        }

        private void ViewStoragesMenu_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.Content is ViewStoragesPage) return;
            MainFrame.Navigate(new ViewStoragesPage());
        }

        private void AddBookMenu_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AdditionalWindow();
            AdditionalWindow.AddFrame.Navigate(new AddBookPage());
            if (addWindow.ShowDialog() == true) RefreshItems();
        }

        private void AddAuthorMenu_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AdditionalWindow();
            AdditionalWindow.AddFrame.Navigate(new AddAuthorPage());
            if (addWindow.ShowDialog() == true) RefreshItems();
        }

        private void AddGenreMenu_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AdditionalWindow();
            AdditionalWindow.AddFrame.Navigate(new AddGenrePage());
            if (addWindow.ShowDialog() == true) RefreshItems();
        }

        private void AddPublisherMenu_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AdditionalWindow();
            AdditionalWindow.AddFrame.Navigate(new AddPublisherPage());
            if (addWindow.ShowDialog() == true) RefreshItems();
        }

        private void AddStorageMenu_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AdditionalWindow();
            AdditionalWindow.AddFrame.Navigate(new AddStoragePage());
            if (addWindow.ShowDialog() == true) RefreshItems();
        }

        private void AddUserMenu_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AdditionalWindow();
            AdditionalWindow.AddFrame.Navigate(new AddUserPage());
            if (addWindow.ShowDialog() == true) RefreshItems();
        }

        private void RefreshItems()
        {
            if (MainFrame.Content is ClientViewPage clientPage) { clientPage.GetItemsAsync(); clientPage.ApplyFilter(); }
            else if (MainFrame.Content is ViewBooksPage viewBooksPage) viewBooksPage.RefreshItems();
            else if (MainFrame.Content is ViewAuthorsPage viewAuthorsPage) viewAuthorsPage.ApplyFilter();
            else if (MainFrame.Content is ViewGenresPage viewGenresPage) viewGenresPage.ApplyFilter();
            else if (MainFrame.Content is ViewPublishersPage viewPublishersPage) viewPublishersPage.ApplyFilter();
            else if (MainFrame.Content is ViewUsersPage viewUsersPage) viewUsersPage.ApplyFilter();
            else if (MainFrame.Content is ViewStoragesPage viewStoragesPage) viewStoragesPage.ApplyFilter();
        }
    }
}
