using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace GItClient.Core.Convertors
{
    /// <summary>
    /// Using in WPF Binding
    /// If element property > 30 returns true
    /// Now it used only by CommandsBar
    /// </summary>
    [ValueConversion(typeof(double), typeof(bool))]
    public class DoubleToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value > 30;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
