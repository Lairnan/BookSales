using BookSales.BehaviorsFiles;
using BookSales.Context;
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
using System.Windows.Shapes;

namespace BookSales.Windows
{
    /// <summary>
    /// Логика взаимодействия для ViewOrderConsistWindow.xaml
    /// </summary>
    public partial class ViewOrderConsistWindow : Window
    {
        public ViewOrderConsistWindow(BookingsConsist bookingsConsist)
        {
            InitializeComponent();

            OrderConsistView.ItemsSource = bookingsConsist.OrderConsist;
        }
    }
}
