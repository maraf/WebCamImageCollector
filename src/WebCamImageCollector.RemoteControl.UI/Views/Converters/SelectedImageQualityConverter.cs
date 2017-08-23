using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCamImageCollector.RemoteControl.Services;
using Windows.UI.Xaml.Data;

namespace WebCamImageCollector.RemoteControl.Views.Converters
{
    public class SelectedImageQualityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            ImageQuality source = (ImageQuality)value;
            if (Enum.TryParse((string)parameter, out ImageQuality target))
                return source == target;

            throw Ensure.Exception.NotSupported($"Not support converter parameter value '{parameter}'.");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if ((bool)value)
            {
                if (Enum.TryParse((string)parameter, out ImageQuality target))
                    return target;
            }

            return null;
        }
    }
}
