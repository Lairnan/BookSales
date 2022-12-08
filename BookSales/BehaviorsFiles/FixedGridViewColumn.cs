using System.Windows;
using System.Windows.Controls;

namespace BookSales.BehaviorsFiles
{
    public class FixedGridViewColumn : GridViewColumn
    {
        static FixedGridViewColumn()
        {
            WidthProperty.OverrideMetadata(typeof(FixedGridViewColumn), new FrameworkPropertyMetadata(null, new CoerceValueCallback(OnCourceWidth)));
        }

        private static object OnCourceWidth(DependencyObject sender, object baseValue)
        {
            var fgvc = sender as FixedGridViewColumn;
            if (fgvc != null) return fgvc.FixedWidth;

            return 0.0;
        }

        public double FixedWidth
        {
            get { return (double)GetValue(FixedWidthProperty); }
            set { SetValue(FixedWidthProperty, value); }
        }



        public static readonly DependencyProperty FixedWidthProperty =
            DependencyProperty.Register("FixedWidth", typeof(double), typeof(FixedGridViewColumn),
            new FrameworkPropertyMetadata(double.NaN, new PropertyChangedCallback(OnFixedWidthChanged)));



        private static void OnFixedWidthChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var fgvc = sender as FixedGridViewColumn;

            if (fgvc != null) fgvc.CoerceValue(WidthProperty);

        }
    }
}
