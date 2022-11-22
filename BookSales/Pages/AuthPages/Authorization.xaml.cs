using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data.Entity;
using BookSales.Context;
using BookSales.Windows;

namespace BookSales.Pages.AuthPages
{
	public partial class Authorization : Page
	{
		public Authorization()
        {
            InitializeComponent();
            KeyDown += (s, e) =>
            {
                if(e.Key == Key.Enter & AuthBtn.IsEnabled)
                {
                    AuthBtn_Click(this, e);
                }
            };
            if (Application.Current.Windows.OfType<AuthWindow>().Single().IsDialog)
            {
                AuthGuest.IsEnabled = false;
            }
        }

        private async void AuthBtn_Click(object sender, RoutedEventArgs e)
        {
            AuthBtn.IsEnabled = false;
            var login = LogBox.Text;
            var pass = PassBox.Password;
            if (IsNullOrWhiteSpace(login, pass))
                MessageBox.Show("Поля не могут быть пустыми");
            else
            {
                if (CaptchaText.Visibility == Visibility.Visible && !IsAvailableCaptcha)
                    MessageBox.Show("Время капчи вышло");
                else if (CaptchaText.Visibility == Visibility.Visible && CaptchaText.Text != CaptchaBox.Text)
                    MessageBox.Show("Капча введена не верно");
                else
                {
                    var user = await Task.Run(() => GetUser(login, pass));
                    if (user != null)
                    {
                        AuthStaticUser.AuthUser = user;
                        (Size, Point) old;
                        old.Item1 = Application.Current.Windows.OfType<AuthWindow>().Single()._windowMoves.oldSize;
                        old.Item2 = Application.Current.Windows.OfType<AuthWindow>().Single()._windowMoves.oldLoc;
                        if (!Application.Current.Windows.OfType<AuthWindow>().Single().IsDialog)
                            new MainWindow(old).Show();
                        Application.Current.Windows.OfType<AuthWindow>().Single().Close();
                        return;
                    }
                    MessageBox.Show("Неверный логин или пароль");
                }
            }
            if(++Attempts > 2) 
            {
                CaptchaGenerate();
            }
            AuthBtn.IsEnabled = true;
        }

        private bool IsNullOrWhiteSpace(string login, string pass)
        {
            return string.IsNullOrWhiteSpace(login)
                || string.IsNullOrWhiteSpace(pass)
                || (CaptchaText.Visibility == Visibility.Visible
                && string.IsNullOrWhiteSpace(CaptchaText.Text));
        }

        private Users GetUser(string login, string pass)
        {
            using (var db = new BookSalesEntities())
            {
                var user = db.Users.Include(s => s.Positions).FirstOrDefault(s => s.login == login && s.password == pass);
                return user;
            }
        }

        private int Attempts = 0;

        private bool IsAvailableCaptcha { get; set; }

        private async void CaptchaGenerate() 
        {
            const string captchaOwnString =
                "abcdefghijklmnopqrstuvwxyz" +
                "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
                "1234567890" +
                "!@#$%";
            var text = "";
            CaptchaClear();
            var rnd = new Random();
            for(var i = 0; i < 6; i++)
            {
                text += captchaOwnString[rnd.Next(0, captchaOwnString.Length)];
            }

            CaptchaText.Text = text;

            await Task.Delay(15 * 1000); // Sleep method CaptchaGenerate 15 seconds
            if (CaptchaText.Text == text) // Check captcha this thread and user ui thread
            {
                IsAvailableCaptcha = false; // Set false if time out
            }
        }

        // Clear and set visibility visible for captcha
        private void CaptchaClear()
        {
            CaptchaRefresh.Visibility = Visibility.Visible;
            CaptchaText.Visibility = Visibility.Visible;
            CaptchaBox.Visibility = Visibility.Visible;
            CaptchaBox.Text = "";
            CaptchaText.Text = "";
        }

        private void RegBtn_Click(object sender, RoutedEventArgs e)
        {
            AuthWindow.AuthFrame.Navigate(new Registration());
        }

        private void CaptchaRefresh_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CaptchaGenerate();
        }

        private void AuthGuestBtn_Click(object sender, RoutedEventArgs e)
        {
            AuthStaticUser.AuthUser = null;
            (Size, Point) old;
            old.Item1 = Application.Current.Windows.OfType<AuthWindow>().Single()._windowMoves.oldSize;
            old.Item2 = Application.Current.Windows.OfType<AuthWindow>().Single()._windowMoves.oldLoc;
            new MainWindow(old).Show();
            Application.Current.Windows.OfType<AuthWindow>().Single().Close();
        }
    }
}