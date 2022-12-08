using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows;

namespace BookSales.BehaviorsFiles
{
    public class AnyItemsToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var collection = value as IEnumerable;
            if (collection == null)
                return Visibility.Collapsed;

            return collection.OfType<object>().Any() ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
