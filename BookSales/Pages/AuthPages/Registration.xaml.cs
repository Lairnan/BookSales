using BookSales.Context;
using Microsoft.Win32;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BookSales.Pages.AuthPages
{
    /// <summary>
    /// Логика взаимодействия для Registration.xaml
    /// </summary>
    public partial class Registration : Page
    {
        public Registration()
        {
            InitializeComponent();
            KeyDown += (s, e) =>
            {
                if (e.Key == Key.Enter & RegBtn.IsEnabled)
                {
                    RegBtn_Click(this, e);
                }
            };
        }

        private string FileNamePath { get; set; }

        private void AddImageBtn_Click(object sender, RoutedEventArgs e)
        {
            var openFile = new OpenFileDialog()
            {
                Filter = "Image (png, jpg, jpeg, bmp) | *.png; *.jpg; *.jpeg; *.bmp"
            };
            if (openFile.ShowDialog() == false) return;
            var image = openFile.FileName;
            if (image == null) return;

            ImageBox.Source = File.ReadAllBytes(image).ToImageSource();
            FileNamePath = image;
            ImageBox.Visibility = Visibility.Visible;
            BtnClear.IsEnabled = true;
        }

        private void ClearImageBtn_Click(object sender, RoutedEventArgs e)
        {
            ImageBox.Visibility = Visibility.Collapsed;
            ImageBox.Source = null;
            BtnClear.IsEnabled = false;
        }

        private async void RegBtn_Click(object sender, RoutedEventArgs e)
        {
            if (IsNullOrWhiteSpace())
            {
                MessageBox.Show("Поля не могут быть пустыми");
                return;
            }

            RegBtn.IsEnabled = false;
            var surname = SurnameBox.Text;
            var name = NameBox.Text;
            var patronymic = PatronymicBox.Text;
            var dateOfBirth = (DateTime)DateOfBirthBox.SelectedDate;
            if (dateOfBirth == null)
            {
                MessageBox.Show("Выберите дату!");
                return;
            }
            var login = LoginBox.Text;
            var password = PasswordBox.Password;
            byte[] image = null;
            if (!string.IsNullOrWhiteSpace(FileNamePath))
                image = File.ReadAllBytes(FileNamePath);

            var user = new Users
            {
                surname = surname,
                name = name,
                patronymic = patronymic,
                login = login,
                positionId = 1,
                dateOfBirth = dateOfBirth,
                password = password,
                image = image,
            };
            if(await TryAddUser(user))
                MessageBox.Show("Вы успешно зарегистрировались!");
            RegBtn.IsEnabled = true;
        }

        private async Task<bool> TryAddUser(Users user)
        {
            using (var db = await Task.Run(() => new BookSalesEntities()))
            {
                try
                {
                    db.Users.Add(user);
                    await db.SaveChangesAsync();
                    AuthWindow.AuthFrame.Navigate(new Authorization());
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                return false;
            }
        }

        private bool IsNullOrWhiteSpace()
        {
            if (string.IsNullOrWhiteSpace(SurnameBox.Text)
                || string.IsNullOrWhiteSpace(NameBox.Text)
                || string.IsNullOrWhiteSpace(LoginBox.Text)
                || string.IsNullOrWhiteSpace(PasswordBox.Password))
                return true;
            return false;
        }

        private void AuthBtn_Click(object sender, RoutedEventArgs e)
        {
            AuthWindow.AuthFrame.Navigate(new Authorization());
        }
    }
}
