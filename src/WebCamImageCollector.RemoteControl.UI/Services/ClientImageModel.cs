using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace WebCamImageCollector.RemoteControl.Services
{
    public class ClientImageModel
    {
        public ImageSource Image { get; set; }
        public Stream Stream { get; set; }
        public DateTime Date { get; set; }
    }
}
