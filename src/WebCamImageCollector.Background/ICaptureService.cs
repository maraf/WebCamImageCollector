using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage.Streams;

namespace WebCamImageCollector.Background
{
    public interface ICaptureService
    {
        bool IsRunning { get; }

        void Start();
        void Stop();

        IAsyncOperation<FileModel> FindLatestImageAsync();
    }
}
