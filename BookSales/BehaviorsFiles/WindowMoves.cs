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
                SwitchState();

                return;
            }

            if (isMax)
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
                    case Registration reg when reg.ImageBox.IsMouseOver:
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

        private bool isMax { get; set; }
        private Size oldSize { get; set; }
        private Point oldPoint { get; set; }

        public void SwitchState()
        {
            if (!isMax) Maximize(_window);
            else Normal(_window);
        }

        private object _lock = new object();

        public void Maximize(Window wnd)
        {
            lock (_lock)
            {
                if (isMax) return;

                oldSize = new Size(_window.Width, _window.Height);
                oldPoint = new Point(_window.Top, _window.Left);

                var x = SystemParameters.WorkArea.Width;
                var y = SystemParameters.WorkArea.Height;
                wnd.WindowState = WindowState.Normal;
                wnd.Top = 0;
                wnd.Left = 0;
                wnd.Width = x;
                wnd.Height = y;
                wnd.ResizeMode = ResizeMode.NoResize;

                isMax = true;
            }
        }

        public void Normal(Window wnd)
        {
            lock (_lock)
            {
                if (!isMax) return;

                wnd.WindowState = WindowState.Normal;
                wnd.Top = oldPoint.Y;
                wnd.Left = oldPoint.X;
                wnd.Width = oldSize.Width;
                wnd.Height = oldSize.Height;
                wnd.ResizeMode = ResizeMode.CanResize;

                isMax = false;
            }
        }
    }
}