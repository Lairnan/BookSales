using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BookSales.Pages.AuthPages;
using BookSales.Pages.MainPages;

namespace BookSales
{
    public class WindowMoves
    {
        public WindowMoves(Window window)
        {
            this._window = window;
        }

        private readonly Window _window;

        private bool _mRestoreIfMove = false;

        public void DragMoveLeftBtnDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Source is Frame fr)
            {
                switch (fr.Content)
                {
                    case ClientViewPage vb when vb.BooksViewList.IsMouseOver:
                    case Authorization auth when auth.CaptchaRefresh.IsMouseOver:
                    case Registration reg when reg.ImageBox.IsMouseOver:
                        return;
                }
            }
            if (e.ClickCount == 2)
            {
                if (_window.ResizeMode == ResizeMode.CanResize || _window.ResizeMode == ResizeMode.CanResizeWithGrip)
                {
                    SwitchState();
                }

                return;
            }

            if (_window.WindowState == WindowState.Maximized)
            {
                _mRestoreIfMove = true;
                return;
            }

            _window.DragMove();
        }

        public void DragMoveLeftBtnUp(object sender, MouseButtonEventArgs e)
        {
            _mRestoreIfMove = false;
        }

        public void DragMoveMouseMove(object sender, MouseEventArgs e)
        {
            if (e.Source is Frame fr)
            {
                switch (fr.Content)
                {
                    case ClientViewPage vb when vb.BooksViewList.IsMouseOver:
                    case Authorization auth when auth.CaptchaRefresh.IsMouseOver:
                        return;
                }
            }
            if (!_mRestoreIfMove) return;
            _mRestoreIfMove = false;

            var percentHorizontal = e.GetPosition(_window).X / _window.ActualWidth;
            var targetHorizontal = _window.RestoreBounds.Width * percentHorizontal;

            var percentVertical = e.GetPosition(_window).Y / _window.ActualHeight;
            var targetVertical = _window.RestoreBounds.Height * percentVertical;


            var point = _window.PointToScreen(e.MouseDevice.GetPosition(_window));

            _window.Left = point.X - targetHorizontal;
            _window.Top = point.Y - targetVertical;

            _window.WindowState = WindowState.Normal;

            _window.DragMove();
        }

        private void SwitchState()
        {
            switch (_window.WindowState)
            {
                case WindowState.Normal:
                    _window.WindowState = WindowState.Maximized;
                    _window.MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;
                    break;
                case WindowState.Maximized:
                    _window.WindowState = WindowState.Normal;
                    _window.MaxWidth = double.PositiveInfinity;
                    break;
                default:
                    _window.WindowState = _window.WindowState;
                    break;
            }
        }
    }
}