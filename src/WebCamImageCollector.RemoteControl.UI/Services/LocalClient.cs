using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCamImageCollector.Capturing;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace WebCamImageCollector.RemoteControl.Services
{
    //public class LocalClient : ClientBase, IClient
    //{
    //    public Guid Key { get; private set; }
    //    public int Port { get; private set; }
    //    public int Interval { get; private set; }
    //    public int Delay { get; private set; }

    //    public string Name
    //    {
    //        get { return "Local"; }
    //    }

    //    public LocalClient(Guid key, int port, string authenticationToken, int interval, int delay)
    //        : base(String.Format("http://localhost:{0}/", port), authenticationToken)
    //    {
    //        Key = key;
    //        Port = port;
    //        Interval = interval;
    //        Delay = delay;
    //    }
    //}

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
            get { return "http://localhost"; }
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

        public async Task<ImageSource> DownloadLatest()
        {
            FileModel file = await service.FindLatestImageAsync();
            Stream imageStream = file.Content.AsStreamForRead();
            BitmapImage image = new BitmapImage();
            await image.SetSourceAsync(imageStream.AsRandomAccessStream());
            return image;
        }

        public Task<ClientRunningInfo> IsRunningAsync()
        {
            return Task.FromResult(new ClientRunningInfo()
            {
                Running = service.IsRunning
            });
        }

        public Task<bool> StartAsync()
        {
            service.Start();
            return Task.FromResult(service.IsRunning);
        }

        public Task<bool> StopAsync()
        {
            service.Stop();
            return Task.FromResult(!service.IsRunning);
        }
    }
}
