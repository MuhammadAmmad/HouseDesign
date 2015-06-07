using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace HouseDesign.Converters
{
    public class AddMessageConverter:IValueConverter
    {
        public String Message { get; set; }
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Decimal decimalValue = System.Convert.ToDecimal(value);
            if(decimalValue!=0)
            {
                return String.Format(Message, decimalValue);
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
 	        throw new NotImplementedException();
        }
}
}
