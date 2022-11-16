using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BookSales.Context;
using BookSales.Pages.AuthPages;

namespace BookSales
{
	public partial class AuthWindow : Window
	{
		public AuthWindow()
        {
            InitializeComponent();
			this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
			this.MouseDown += (s, e) => Keyboard.ClearFocus();
			Loaded += OnLoaded;
		}

		public bool IsDialog { get; set; } = false;

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			AuthFrame = AFrame;
			AuthFrame.Content = new Authorization();
			var windowMoves = new WindowMoves(this);
			this.MouseMove += windowMoves.DragMoveMouseMove;
			this.MouseLeftButtonDown += windowMoves.DragMoveLeftBtnDown;
			this.MouseLeftButtonUp += windowMoves.DragMoveLeftBtnUp;
        }

		internal static Frame AuthFrame { get; set; }

        private void MinBtn_Click(object sender, RoutedEventArgs e)
        {
			this.WindowState= WindowState.Minimized;
        }

        private void StateBtn_Click(object sender, RoutedEventArgs e)
        {
			switch(this.WindowState)
			{
				case WindowState.Normal:
					this.WindowState= WindowState.Maximized;
                    this.MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;
                    break;
				case WindowState.Maximized:
                    this.WindowState = WindowState.Normal;
                    this.MaxWidth = double.PositiveInfinity;
                    break;
			}
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
			if (IsDialog) this.Close();
			else Application.Current.Shutdown();
        }
    }
}