using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace WebCamImageCollector.Background.Capturing
{
    public sealed class FileModel
    {
        public IInputStream Content { get; private set; }
        public long Size { get; private set; }

        public FileModel(IInputStream content, long size)
        {
            Content = content;
            Size = size;
        }
    }
}
