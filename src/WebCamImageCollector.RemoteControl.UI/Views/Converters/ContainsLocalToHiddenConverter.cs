using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCamImageCollector.RemoteControl.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace WebCamImageCollector.RemoteControl.Views.Converters
{
    public class ContainsLocalToHiddenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            IEnumerable<ClientOverviewViewModel> models = (IEnumerable<ClientOverviewViewModel>)value;
            return models.Any(m => !m.IsRemote) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
