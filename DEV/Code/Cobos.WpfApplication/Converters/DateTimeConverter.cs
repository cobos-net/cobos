using System;
using System.Windows;
using System.Windows.Data;

namespace Intergraph.AsiaPac.WpfApplication.Converters
{
    public class DateTimeConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DateTime dateTime = DateTime.Now;

            if (value != DependencyProperty.UnsetValue)
                dateTime = (DateTime) value;

            if (parameter == null)
                return dateTime.ToString();
            else
                return dateTime.ToString(parameter as string);

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
