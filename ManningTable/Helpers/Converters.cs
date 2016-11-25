using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Helpers
{
    class Converters
    {
    }

    public class ArithmConverters:IMultiValueConverter
    {
        public string Operator
        {
            get;
            set;
        }

        /// <summary>
        /// Конвертер чисел в их произведение.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType == typeof(Thickness))
            {
                if (values.All(r => r != null && r != System.Windows.DependencyProperty.UnsetValue))
                {
                    Decimal? v = 1;
                    if (parameter != null)
                        v = decimal.Parse(parameter.ToString());
                    for (int i = 0; i < values.Length; ++i)
                    {
                        v *= decimal.Parse(values[i].ToString());
                    }
                    return new Thickness(System.Convert.ToDouble(v), 0, 0, 0);
                }
                else
                    return new Thickness(0, 0, 0, 0);
            }
            else
                if (NumericHelper.IsNumericType(targetType))
                {
                    if (values.All(r => r != null && r != System.Windows.DependencyProperty.UnsetValue))
                    {
                        Decimal? v = 1;
                        if (parameter != null)
                            v = decimal.Parse(parameter.ToString());
                        for (int i = 0; i < values.Length; ++i)
                        {
                            v *= decimal.Parse(values[i].ToString());
                        }
                        return v;
                    }
                    else
                        return 0m;
                }
                else
                    return 0m;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Вспомогательный класс проверки на число
    /// </summary>
    public static class NumericHelper
    {
        public static bool IsNumeric(this object t)
        {
            switch (Type.GetTypeCode(t.GetType()))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsNumericType(Type t)
        {
            switch (Type.GetTypeCode(t))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }
    }
}
