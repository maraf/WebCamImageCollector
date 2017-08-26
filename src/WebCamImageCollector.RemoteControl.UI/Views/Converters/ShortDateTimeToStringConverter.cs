using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace WebCamImageCollector.RemoteControl.Views.Converters
{
    public class ShortDateTimeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DateTime dateTime)
            {
                if (dateTime.Date == DateTime.Today)
                    return dateTime.ToString("HH:mm:ss");

                return dateTime.ToString("dd.MM.yyyy HH:mm:ss");
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
