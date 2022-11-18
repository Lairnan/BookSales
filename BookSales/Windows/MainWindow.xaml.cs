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
        public MainWindow((Size, Point) old)
        {
            InitializeComponent();
            _old = old;
            _windowMoves = new WindowMoves(this);
            this.MouseDown += (s, e) => Keyboard.ClearFocus();
            Loaded += OnLoaded;
            StateChanged += (s, e) => { if (this.WindowState == WindowState.Maximized) _windowMoves.SwitchState(); };
            SizeChanged += (s, e) => StateBtnText.Text = _windowMoves.isMax ? "2" : "1";
        }

        private (Size, Point) _old { get; set; }

        private void MainFrameOnNavigated(object sender, NavigationEventArgs e)
        {
            BackBtn.IsEnabled = MainFrame.CanGoBack;

            var frame = sender as Frame;
            ViewStory.IsEnabled = !(frame.Content is StoryViewPage);
            BasketBtn.Visibility = frame.Content is ClientViewPage ? Visibility.Visible : Visibility.Collapsed;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (AuthStaticUser.AuthUser != null) DrawAuthUser(AuthStaticUser.AuthUser);
            
			this.MouseMove += _windowMoves.DragMoveMouseMove;
			this.MouseLeftButtonDown += _windowMoves.DragMoveLeftBtnDown;
			this.MouseLeftButtonUp += _windowMoves.DragMoveLeftBtnUp;
            _windowMoves.SwitchState();

            _windowMoves.oldLoc = _old.Item2;
            _windowMoves.oldSize = _old.Item1;

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

        private readonly WindowMoves _windowMoves;

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

        private void MinBtn_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void StateBtn_Click(object sender, RoutedEventArgs e)
        {
            _windowMoves.SwitchState();
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
