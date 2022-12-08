using BookSales.BehaviorsFiles;
using BookSales.Context;
using BookSales.Windows;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BookSales.Pages.Adds
{
    /// <summary>
    /// Логика взаимодействия для AddUserPage.xaml
    /// </summary>
    public partial class AddUserPage : Page
    {
        public AddUserPage()
        {
            InitializeComponent();

            using(var db = new BookSalesEntities())
            {
                PositionBox.ItemsSource = db.Positions.OrderBy(s => s.id).ToList();
            }

            var dateNow = DateTime.Today;
            DateOfBirthPicker.DisplayDateEnd = DateTime.Parse($"{dateNow.Day}.{dateNow.Month}.{dateNow.Year - 5}");
            DateOfBirthPicker.DisplayDateStart = DateTime.Parse("01.01.1900");
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

        private async Task<bool> TryAddUser(Users user)
        {
            using (var db = await Task.Run(() => new BookSalesEntities()))
            {
                try
                {
                    db.Users.Add(user);
                    await db.SaveChangesAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
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

        private async void AddUserBtn_Click(object sender, RoutedEventArgs e)
        {
            if (IsNullOrWhiteSpace())
            {
                MessageBox.Show("Поля не могут быть пустыми");
                return;
            }

            AddUserBtn.IsEnabled = false;
            var surname = SurnameBox.Text;
            var name = NameBox.Text;
            var patronymic = string.IsNullOrWhiteSpace(PatronymicBox.Text) ? null : PatronymicBox.Text;
            var dateOfBirth = (DateTime)DateOfBirthPicker.SelectedDate;
            if (dateOfBirth == null)
            {
                MessageBox.Show("Выберите дату!");
                return;
            }
            if (dateOfBirth > DateOfBirthPicker.DisplayDateEnd || dateOfBirth < DateOfBirthPicker.DisplayDateStart)
            {
                MessageBox.Show("Неверно выбранная дата");
                return;
            }
            var position = PositionBox.SelectedItem as Positions;
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
                positionId = position.id,
                dateOfBirth = dateOfBirth,
                password = password,
                image = image,
                dateOfStart = DateTime.Now
            };
            if (await TryAddUser(user))
            {
                MessageBox.Show("Успешно добавлена!");

                var wnd = Window.GetWindow(this);
                wnd.DialogResult = true;
                wnd.Close();
            }
            AddUserBtn.IsEnabled = true;
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            var wnd = Window.GetWindow(this);
            wnd.DialogResult = false;
            wnd.Close();
        }
    }
}
