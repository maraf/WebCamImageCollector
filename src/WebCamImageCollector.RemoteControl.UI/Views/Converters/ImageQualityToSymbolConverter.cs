using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCamImageCollector.RemoteControl.Services;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace WebCamImageCollector.RemoteControl.Views.Converters
{
    public class ImageQualityToSymbolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            switch ((ImageQuality)value)
            {
                case ImageQuality.Full:
                    return Symbol.FourBars;
                case ImageQuality.Medium:
                    return Symbol.TwoBars;
                case ImageQuality.Thumbnail:
                    return Symbol.OneBar;
                default:
                    throw Ensure.Exception.NotSupported(value.ToString());
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
