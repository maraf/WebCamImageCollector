using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace WebCamImageCollector.RemoteControl.Views.Converters
{
    public class BoolConverter : IValueConverter
    {
        [DefaultValue(true)]
        public bool Test { get; set; } = true;
        public object TrueValue { get; set; }
        public object FalseValue { get; set; }
        public object NullValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            object result = null;
            bool? boolValue = value as bool?;
            if (boolValue == null && NullValue != null)
            {
                result = NullValue;
            }
            else
            {
                if (boolValue == null)
                    boolValue = false;
                if (Test == boolValue.Value)
                    result = TrueValue;
                else
                    result = FalseValue;
            }

            if (targetType != null && result != null)
            {
                Type resultType = result.GetType();
                TypeConverter converter = TypeDescriptor.GetConverter(targetType);
                if (converter != null && converter.CanConvertFrom(resultType))
                    result = converter.ConvertFrom(null, CultureInfo.InvariantCulture, result);
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
