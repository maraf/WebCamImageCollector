using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace WebCamImageCollector.RemoteControl.Services
{
    public interface IClient
    {
        string Name { get; }
        string Url { get; }

        Task<ClientRunningInfo> IsRunningAsync();
        Task<bool> StartAsync();
        Task<bool> StopAsync();
        Task<ClientImageModel> DownloadLatest();
    }
}
