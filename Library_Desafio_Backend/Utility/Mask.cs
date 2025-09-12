using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library_Desafio_Backend.Utility
{
    public class Mask
    {
        public static string MaskMoney(decimal value, char decimalSeparator = '.', char thousandseparator = ',')
        {
            if (value < 0)
            {
                return "0" + decimalSeparator + "00";
            }
            CultureInfo culture = new System.Globalization.CultureInfo("de-DE");
            culture.NumberFormat.NumberDecimalDigits = 2;
            string finalValue = value.ToString("N", culture);
            char thousand = '#';
            char decimals = '$';
            finalValue = finalValue.Replace('.', thousand).Replace(',', decimals);
            finalValue = finalValue.Replace(thousand, thousandseparator).Replace(decimals, decimalSeparator);
            return value.ToString("N", culture);
        }
    }
}
