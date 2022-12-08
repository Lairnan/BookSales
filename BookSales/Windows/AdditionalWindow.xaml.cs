using System;
using System.Collections.Generic;
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
        }

        internal static Frame AddFrame { get; set; }
    }
}
