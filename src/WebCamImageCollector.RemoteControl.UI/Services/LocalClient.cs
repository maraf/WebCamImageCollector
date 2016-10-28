using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCamImageCollector.RemoteControl.Services
{
    public class LocalClient
    {
        public Guid Key { get; private set; }
        public int Port { get; private set; }
        public string AuthenticationToken { get; private set; }
        public int Interval { get; private set; }
        public int Delay { get; private set; }

        public LocalClient(Guid key, int port, string authenticationToken, int interval, int delay)
        {
            Key = key;
            Port = port;
            AuthenticationToken = authenticationToken;
            Interval = interval;
            Delay = delay;
        }
    }
}
