using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace TaskDataGrid.Converters
{
    public class DateTimeToDateYearsConverter : IMultiValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var h = parameter;

            return ((DateTime)value).ToString("yyyy");
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var g = values;
            if ((int)values[0]==1)
            {
                return ((DateTime)values[1]).ToString("yyyy");
            } 
            else if ((int)values[0]==2)
            {
                return ((DateTime)values[1]).ToString("MMMM", CultureInfo.GetCultureInfo("ru-Ru"));
            } if ((int)values[0]==3)
            {
                return ((DateTime)values[1]).ToString("dd");
            }
            return ((DateTime)values[1]);


        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
        //public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        //{
        //    var h = parameter;

        //    return ((DateTime)value).ToString("yyyy");
        //}

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class DateTimeToDateMonthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string mymonth = ((DateTime)value).ToString("MMMM", CultureInfo.GetCultureInfo("ru-Ru"));
            return mymonth;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
    public class DateTimeToDateDayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
          
            return ((DateTime)value).ToString("dd");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
