using BookSales.Pages.MainPages;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace BookSales
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            this.MouseDown += (s, e) => Keyboard.ClearFocus();
            Loaded += OnLoaded;
        }

        private void MainFrameOnNavigated(object sender, NavigationEventArgs e)
        {
            BackBtn.IsEnabled = MainFrame.CanGoBack;
            var windowMoves = new WindowMoves(this);
            this.MouseMove += windowMoves.DragMoveMouseMove;
            this.MouseLeftButtonDown += windowMoves.DragMoveLeftBtnDown;
            this.MouseLeftButtonUp += windowMoves.DragMoveLeftBtnUp;
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
        }

        public void DrawAuthUser(Users authUser)
        {
            AuthUser.Text = $"{authUser.surname} " +
                $"{authUser.name} " +
                $"{authUser.patronymic}, " +
                $"{authUser.Positions.name}";
        }

        internal static Frame MainFrame { get; private set; }

        private void MinBtn_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void StateBtn_Click(object sender, RoutedEventArgs e)
        {
            switch (this.WindowState)
            {
                case WindowState.Normal:
                    this.WindowState = WindowState.Maximized;
                    this.MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;
                    break;
                case WindowState.Maximized:
                    this.WindowState = WindowState.Normal;
                    this.MaxWidth = double.PositiveInfinity;
                    break;
            }
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            new AuthWindow().Show();
            AuthStaticUser.AuthUser = null;
            this.Close();
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.GoBack();
        }
    }
}
