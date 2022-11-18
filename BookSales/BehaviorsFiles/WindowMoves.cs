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

            if(e.LeftButton == MouseButtonState.Pressed)
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
            var targetHorizontal = oldSize.Width * percentHorizontal;

            var percentVertical = e.GetPosition(_window).Y / _window.ActualHeight;
            var targetVertical = oldSize.Height * percentVertical;

            var point = _window.PointToScreen(e.MouseDevice.GetPosition(_window));

            _window.Left = point.X - targetHorizontal;
            _window.Top = point.Y - targetVertical;

            Normal(_window, false);

            if (e.LeftButton == MouseButtonState.Pressed)
                _window.DragMove();
        }

        public bool isMax { get; private set; }
        public Size oldSize { get; set; }
        public Point oldLoc { get; set; }

        public void SwitchState()
        {
            if (!isMax) Maximize(_window);
            else Normal(_window);
        }

        private void Maximize(Window wnd)
        {
            if (isMax) return;

            oldSize = new Size(wnd.Width, wnd.Height);
            oldLoc = new Point(wnd.Left, wnd.Top);

            isMax = true;
            wnd.WindowState = WindowState.Normal;
            wnd.Top = 0;
            wnd.Left = (oldLoc.X + (oldSize.Width / 2.5)) > SystemParameters.WorkArea.Width ? SystemParameters.WorkArea.Width : 0;
            wnd.Width = SystemParameters.WorkArea.Width;
            wnd.Height = SystemParameters.WorkArea.Height;
            wnd.ResizeMode = ResizeMode.NoResize;
        }

        private void Normal(Window wnd, bool loc = true)
        {
            if (!isMax) return;

            isMax = false;
            wnd.WindowState = WindowState.Normal;
            if (loc)
            {
                wnd.Top = oldLoc.Y;
                wnd.Left = oldLoc.X;
            }
            wnd.Width = oldSize.Width;
            wnd.Height = oldSize.Height;
            wnd.ResizeMode = ResizeMode.CanResize;
        }
    }
}