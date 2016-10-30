using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace WebCamImageCollector.RemoteControl.Services
{
    public class RemoteClient : ClientBase, IClient
    {
        public Guid Key { get; private set; }
        public string Name { get; private set; }

        public RemoteClient(Guid key, string name, string url, string authenticationToken)
            : base(url, authenticationToken)
        {
            Key = key;
            Name = name;
        }
    }
}
