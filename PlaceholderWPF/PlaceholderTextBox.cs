using System.Windows.Controls;

namespace PlaceholderWPF
{
    public class PlaceholderTextBox : TextBox
    {
        public bool isPlaceholder;
        public string placeholderText;

        public string Placeholder
        {
            get => placeholderText;
            set
            {
                placeholderText = value;
                SetPlaceholder();
            }
        }

        public void SetPlaceholder()
        {
            if (string.IsNullOrWhiteSpace(base.Text))
            {
                base.Text = placeholderText;
                isPlaceholder = true;
            }
        }

        public void RemovePlaceholder()
        {
            if (isPlaceholder)
            {
                base.Text = "";
                isPlaceholder = false;
            }
        }

        public PlaceholderTextBox()
        {
            GotFocus += PlaceholderTextBox_GotFocus;
            LostFocus += PlaceholderTextBox_LostFocus; ;
        }

        private void PlaceholderTextBox_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            SetPlaceholder();
        }

        private void PlaceholderTextBox_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            RemovePlaceholder();
        }
    }
}
