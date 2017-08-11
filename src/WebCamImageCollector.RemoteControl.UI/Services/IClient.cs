using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace WebCamImageCollector.RemoteControl.Services
{
    public interface IClient
    {
        string Name { get; }
        string Url { get; }

        Task<ClientRunningInfo> IsRunningAsync(CancellationToken cancellationToken = default(CancellationToken));
        Task<bool> StartAsync(CancellationToken cancellationToken = default(CancellationToken));
        Task<bool> StopAsync(CancellationToken cancellationToken = default(CancellationToken));
        Task<ClientImageModel> DownloadLatest(ImageQuality quality = ImageQuality.Full, CancellationToken cancellationToken = default(CancellationToken));
    }
}
