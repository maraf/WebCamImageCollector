using Neptuo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace WebCamImageCollector.RemoteControl.Services
{
    public class ClientImageModel
    {
        public ImageSource Image { get; private set; }
        public Stream Stream { get; private set; }
        public DateTime Date { get; private set; }

        public int Width { get; private set; }
        public int Height { get; private set; }

        public ClientImageModel(BitmapImage image, Stream stream, DateTime date)
        {
            Ensure.NotNull(image, "image");
            Ensure.NotNull(stream, "stream");
            Image = image;
            Stream = stream;
            Date = date;
            Width = image.PixelWidth;
            Height = image.PixelHeight;
        }
    }
}
