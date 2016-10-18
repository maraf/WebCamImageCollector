using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCamImageCollector.RemoteControl.Services
{
    public class LocalClient
    {
        public int Port { get; private set; }
        public string AuthenticationToken { get; private set; }

        public LocalClient(int port, string authenticationToken)
        {
            Port = port;
            AuthenticationToken = authenticationToken;
        }

        public void Update(int port, string authenticationToken)
        {
            Port = port;
            AuthenticationToken = authenticationToken;
        }
    }
}
