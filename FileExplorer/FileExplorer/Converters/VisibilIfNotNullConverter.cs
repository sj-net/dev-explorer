using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace FileExplorer.Converters
{
    internal class VisibilIfNotNullConverter : MarkupExtension, IValueConverter
    {
        private VisibilIfNotNullConverter? conveter;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return conveter ?? new VisibilIfNotNullConverter();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value == null || value == string.Empty) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
