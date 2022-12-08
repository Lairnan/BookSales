using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using BookSales.BehaviorsFiles;
using BookSales.Context;
using BookSales.Pages.AuthPages;
using BookSales.Pages.MainPages;

namespace BookSales.Windows
{
	public partial class AuthWindow : Window
	{

		public AuthWindow()
        {
			InitializeComponent();
			this.MouseDown += (s, e) => Keyboard.ClearFocus();
			Loaded += OnLoaded;
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			AuthFrame = AFrame;
			AuthFrame.Content = new Authorization();
            AuthFrame.Navigated += MainFrameOnNavigated;
        }

        private void MainFrameOnNavigated(object sender, NavigationEventArgs e)
        {
            var frame = sender as Frame;
            this.Title = (frame.Content as Page).Title;
        }

        internal static Frame AuthFrame { get; set; }
    }
}