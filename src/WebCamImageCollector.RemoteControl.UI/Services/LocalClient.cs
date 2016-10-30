using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCamImageCollector.RemoteControl.Services
{
    public class LocalClient : ClientBase, IClient
    {
        public Guid Key { get; private set; }
        public int Port { get; private set; }
        public int Interval { get; private set; }
        public int Delay { get; private set; }

        public string Name
        {
            get { return "Local"; }
        }

        public LocalClient(Guid key, int port, string authenticationToken, int interval, int delay)
            : base(String.Format("http://localhost:{0}/", port), authenticationToken)
        {
            Key = key;
            Port = port;
            Interval = interval;
            Delay = delay;
        }
    }
}
