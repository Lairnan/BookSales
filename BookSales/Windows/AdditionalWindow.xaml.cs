using BookSales.Pages.Adds;
using BookSales.Pages.Edits;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace BookSales.Windows
{
    /// <summary>
    /// Логика взаимодействия для AdditionalWindow.xaml
    /// </summary>
    public partial class AdditionalWindow : Window
    {
        public AdditionalWindow()
        {
            InitializeComponent();
            this.MouseDown += (s, e) => Keyboard.ClearFocus();
            AddFrame = AFrame;
            AddFrame.Navigated += MainFrameOnNavigated;
        }

        private void MainFrameOnNavigated(object sender, NavigationEventArgs e)
        {
            var frame = sender as Frame;
            this.Title = (frame.Content as Page).Title;
            if(frame.Content is AddBookPage addBookPage) addBookPage.InitializeValues();
            else if(frame.Content is EditBookPage editBookPage) editBookPage.InitializeValues();
            this.MinWidth = (frame.Content as Page).Width + 50;
            this.MinHeight = (frame.Content as Page).Height + 100;
            this.Width = (frame.Content as Page).Width + 100;
            this.Height = (frame.Content as Page).Height + 150;
        }

        internal static Frame AddFrame { get; set; }
    }
}
