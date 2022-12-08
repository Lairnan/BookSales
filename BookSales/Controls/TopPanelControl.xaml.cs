using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

namespace BookSales.Controls
{

	public partial class TopPanelControl : UserControl
	{
		public TopPanelControl()
		{
			InitializeComponent();
		}

		[DllImport("user32.dll")]
		private static extern IntPtr SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);

		private void PnlControlBar_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ClickCount == 2)
			{
				SwitchState();
				return;
			}

			var window = (this.Parent as Grid).Parent as Window;
			var helper = new WindowInteropHelper(window);
			SendMessage(helper.Handle, 161, 2, 0);
			WindowCurrentState.Text = window.WindowState == WindowState.Maximized ? "2" : "1";
		}

		private void PnlControlBar_OnMouseEnter(object sender, MouseEventArgs e)
		{
			var window = (this.Parent as Grid).Parent as Window;
			window.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
		}

		private void CloseBtn_Click(object sender, RoutedEventArgs e)
		{
			var window = (this.Parent as Grid).Parent as Window;
			window.Close();
		}

		private void MinimizeBtn_Click(object sender, RoutedEventArgs e)
		{
			var window = (this.Parent as Grid).Parent as Window;
			window.WindowState = WindowState.Minimized;
		}

		private void SwitchStateBtn_Click(object sender, RoutedEventArgs e)
		{
			SwitchState();
		}

		private void SwitchState()
		{
			var window = (this.Parent as Grid).Parent as Window;
			window.WindowState =
				window.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
			WindowCurrentState.Text = window.WindowState == WindowState.Maximized ? "2" : "1";
		}
	}
}