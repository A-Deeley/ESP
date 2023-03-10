using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SGI.ValueConverters
{
    public class DisplayDollarSignConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!double.TryParse(value.ToString(), out double number))
            {
                return $"${value}";
            }

            number = Math.Round(number, 2, MidpointRounding.AwayFromZero);
            string text = number.ToString("N2");
            

            if (text[0] == '-')
            {
                return $"-${text[1..]}";
            }
            else
                return $"${text}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new NotImplementedException();
        }
    }
}
