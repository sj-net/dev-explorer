namespace FileExplorer.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Markup;

    public class InverseBoolToVisisbilityConverter : MarkupExtension, IValueConverter
    {
        private InverseBoolToVisisbilityConverter? conveter;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return conveter ?? new InverseBoolToVisisbilityConverter();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return Visibility.Collapsed;
            }
            bool result = (bool)value;
            return result ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
