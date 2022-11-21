using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace BookSales.Controls
{
    public class DatePickerEx : DatePicker
    {
        static DatePickerEx()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DatePickerEx), new FrameworkPropertyMetadata(typeof(DatePickerEx)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var popup = GetTemplateChild("PART_Popup") as Popup;
            if (popup != null)
                ApplyCustomTemplate(popup);
        }

        void ApplyCustomTemplate(Popup popup)
        {
            var calendar = popup.Child as Calendar;
            if (calendar == null) return;
            calendar.SetResourceReference(Calendar.StyleProperty, "DatePickerEx_CustomPopup");
        }
    }
}
