using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebCamImageCollector.Capturing;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace WebCamImageCollector.RemoteControl.Services
{
    public class LocalClient : IClient
    {
        private readonly CaptureService service;

        public Guid Key { get; private set; }
        public int Port { get; private set; }
        public int Interval { get; private set; }
        public int Delay { get; private set; }
        public string AuthenticationToken { get; private set; }

        public string Name
        {
            get { return "Local"; }
        }

        public string Url
        {
            get { return $"http://localhost:{Port}"; }
        }

        public LocalClient(Guid key, int port, string authenticationToken, int interval, int delay)
        {
            Key = key;
            Port = port;
            Interval = interval;
            Delay = delay;
            AuthenticationToken = authenticationToken;
            service = new CaptureService(TimeSpan.FromSeconds(interval), TimeSpan.FromSeconds(delay));
        }

        public async Task<ClientImageModel> DownloadLatestAsync(ImageQuality quality, CancellationToken cancellationToken)
        {
            FileModel file = await service.FindLatestImageAsync();
            Stream content = file.Content.AsStreamForRead();

            switch (quality)
            {
                case ImageQuality.Full:
                    break;
                case ImageQuality.Medium:
                    int width = 600;
                    content = await ResizeImage(content, width, 0);
                    break;
                case ImageQuality.Thumbnail:
                    width = 200;
                    content = await ResizeImage(content, width, 0);
                    break;
                default:
                    throw new NotSupportedException(quality.ToString());
            }

            BitmapImage image = new BitmapImage();
            await image.SetSourceAsync(content.AsRandomAccessStream());
            return new ClientImageModel(image, content, file.CreatedAt);
        }

        private async Task<Stream> ResizeImage(Stream imageData, int desiredWidth, int desiredHeight)
        {
            IRandomAccessStream imageStream = imageData.AsRandomAccessStream();
            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(imageStream);

            if (decoder.PixelWidth > desiredWidth || decoder.PixelHeight > desiredHeight)
            {
                using (imageData)
                using (imageStream)
                {
                    InMemoryRandomAccessStream resizedStream = new InMemoryRandomAccessStream();

                    BitmapEncoder encoder = await BitmapEncoder.CreateForTranscodingAsync(resizedStream, decoder);
                    double widthRatio = (double)desiredWidth / decoder.PixelWidth;
                    double heightRatio = (double)desiredHeight / decoder.PixelHeight;

                    double scaleRatio = Math.Min(widthRatio, heightRatio);

                    if (desiredWidth == 0)
                        scaleRatio = heightRatio;

                    if (desiredHeight == 0)
                        scaleRatio = widthRatio;

                    uint aspectHeight = (uint)Math.Floor(decoder.PixelHeight * scaleRatio);
                    uint aspectWidth = (uint)Math.Floor(decoder.PixelWidth * scaleRatio);

                    encoder.BitmapTransform.InterpolationMode = BitmapInterpolationMode.Linear;

                    encoder.BitmapTransform.ScaledHeight = aspectHeight;
                    encoder.BitmapTransform.ScaledWidth = aspectWidth;

                    await encoder.FlushAsync();
                    resizedStream.Seek(0);
                    return resizedStream.AsStreamForRead();
                }
            }
            return imageData;
        }

        public Task<ClientRunningInfo> IsRunningAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(new ClientRunningInfo()
            {
                Running = service.IsRunning
            });
        }

        public Task<bool> StartAsync(CancellationToken cancellationToken)
        {
            service.Start();
            return Task.FromResult(service.IsRunning);
        }

        public Task<bool> StopAsync(CancellationToken cancellationToken)
        {
            service.Stop();
            return Task.FromResult(!service.IsRunning);
        }
    }
}
