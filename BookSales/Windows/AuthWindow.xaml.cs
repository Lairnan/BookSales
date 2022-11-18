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
            _windowMoves = new WindowMoves(this);
            this.MaxHeight = SystemParameters.WorkArea.Height;
			this.MouseDown += (s, e) => Keyboard.ClearFocus();
			Loaded += OnLoaded;
		}

		public bool IsDialog { get; set; } = false;

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			AuthFrame = AFrame;
			AuthFrame.Content = new Authorization();
			this.MouseMove += _windowMoves.DragMoveMouseMove;
			this.MouseLeftButtonDown += _windowMoves.DragMoveLeftBtnDown;
			this.MouseLeftButtonUp += _windowMoves.DragMoveLeftBtnUp;
        }

		private readonly WindowMoves _windowMoves;

		internal static Frame AuthFrame { get; set; }

        private void MinBtn_Click(object sender, RoutedEventArgs e)
        {
			this.WindowState= WindowState.Minimized;
        }

        private void StateBtn_Click(object sender, RoutedEventArgs e)
        {
			_windowMoves.SwitchState();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
			if (IsDialog) this.Close();
			else Application.Current.Shutdown();
        }
    }
}