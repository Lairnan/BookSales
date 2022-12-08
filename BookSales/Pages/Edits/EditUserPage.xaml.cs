using BookSales.BehaviorsFiles;
using BookSales.Context;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BookSales.Pages.Edits
{
    /// <summary>
    /// Логика взаимодействия для EditUserPage.xaml
    /// </summary>
    public partial class EditUserPage : Page
    {
        public EditUserPage(Users user)
        {
            InitializeComponent();

            NameBox.Text = user.name;
            SurnameBox.Text = user.surname;
            PatronymicBox.Text = user.patronymic;
            DateOfBirthPicker.SelectedDate = user.dateOfBirth;
            LoginBox.Text = user.login;
            PasswordBox.Password = user.password;
            ImageBox.Source = user.image.ToImageSource();

            User = user;

            using(var db = new BookSalesEntities())
            {
                PositionBox.ItemsSource = db.Positions.OrderBy(s => s.id).ToList();
                PositionBox.SelectedItem = (PositionBox.ItemsSource as List<Positions>).First(s => s.id == user.positionId);
            }

            var dateNow = DateTime.Today;
            DateOfBirthPicker.DisplayDateEnd = DateTime.Parse($"{dateNow.Day}.{dateNow.Month}.{dateNow.Year - 5}");
            DateOfBirthPicker.DisplayDateStart = DateTime.Parse("01.01.1900");
        }

        private Users User { get; set; }

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

        private bool IsNullOrWhiteSpace()
        {
            if (string.IsNullOrWhiteSpace(SurnameBox.Text)
                || string.IsNullOrWhiteSpace(NameBox.Text)
                || string.IsNullOrWhiteSpace(LoginBox.Text)
                || string.IsNullOrWhiteSpace(PasswordBox.Password))
                return true;
            return false;
        }

        private async void SaveUserBtn_Click(object sender, RoutedEventArgs e)
        {
            if (IsNullOrWhiteSpace())
            {
                MessageBox.Show("Поля не могут быть пустыми");
                return;
            }

            SaveUserBtn.IsEnabled = false;
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
            else if (string.IsNullOrWhiteSpace(FileNamePath) && ImageBox.Source != null) image = User.image;
            try
            {
                using(var db = new BookSalesEntities())
                {
                    var user = db.Users.First(s => s.id == User.id);
                    user.surname = surname;
                    user.name = name;
                    user.patronymic = patronymic;
                    user.login = login;
                    user.password = password;
                    user.dateOfBirth = dateOfBirth;
                    user.image = image;
                    user.positionId = position.id;
                    await db.SaveChangesAsync();

                    MessageBox.Show("Изменения сохранены");
                }

                var wnd = Window.GetWindow(this);
                wnd.DialogResult = true;
                wnd.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            SaveUserBtn.IsEnabled = true;
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            var wnd = Window.GetWindow(this);
            wnd.DialogResult = false;
            wnd.Close();
        }
    }
}
