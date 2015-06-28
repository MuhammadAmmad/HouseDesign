using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace HouseDesign.Classes
{
    public static class FieldValidation
    {
        public static bool IsValidLongNumericField(String text)
        {
            long number;
            if(long.TryParse(text, out number))
            {
                return true;
            }
            return false;
        }

        public static bool IsValidDecimalNumericField(String text)
        {
            decimal number;
            if (decimal.TryParse(text, out number))
            {
                return true;
            }
            return false;
        }

        public static bool IsValidFloatNumericField(String text)
        {
            float number;
            if (float.TryParse(text, out number))
            {
                return true;
            }
            return false;
        }
        public static bool IsValidDoubleNumericField(String text)
        {
            double number;
            if (double.TryParse(text, out number))
            {
                return true;
            }
            return false;
        }

        public static bool IsValidEmail(string inputEmail)
        {
            string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                  @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                  @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            Regex re = new Regex(strRegex);
            if (re.IsMatch(inputEmail))
                return (true);
            else
                return (false);
        }
    }
}
