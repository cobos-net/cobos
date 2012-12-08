using System;
using System.Windows;
using System.Windows.Data;
using System.Globalization;

namespace Cobos.WpfApplication.Converters
{
	public class DBNullConverter : IValueConverter
	{
		public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
		{
			if ( value == DBNull.Value )
			{
				return DependencyProperty.UnsetValue;
			}
			else
			{
				// return base.Convert(value, targetType, parameter, culture);
				return value;
			}
		}

		public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
		{
			if ( value == null )
			{
				return DBNull.Value;
			}
			else
			{
				// return base.ConvertBack(value, targetType, parameter, culture);
				return value;
			}
		}
	}
}
