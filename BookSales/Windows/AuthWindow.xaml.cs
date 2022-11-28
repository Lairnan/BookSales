using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BookSales.Context;
using BookSales.Pages.AuthPages;

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

        public bool IsDialog { get; set; } = false;

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			AuthFrame = AFrame;
			AuthFrame.Content = new Authorization();
        }

        internal static Frame AuthFrame { get; set; }
    }
}