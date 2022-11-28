using BookSales.Context;
using BookSales.Pages.MainPages;
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
            ViewStory.IsEnabled = !(frame.Content is StoryViewPage) && AuthStaticUser.AuthUser != null;
            BasketBtn.Visibility = frame.Content is ClientViewPage ? Visibility.Visible : Visibility.Collapsed;
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
                    MainFrame.Navigate(new ManagerViewPage());
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
            new BasketWindow().ShowDialog();
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

        private void ViewBookBtn_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ClientViewPage());
            ViewBooks.Visibility = Visibility.Collapsed;
        }

        private void ViewStoryBtn_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new StoryViewPage());
        }
    }
}
