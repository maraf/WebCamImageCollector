using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCamImageCollector.RemoteControl.Services;

namespace WebCamImageCollector.RemoteControl.UI
{
    public static class ServiceProvider
    {
        public static RemoteClientRepository RemoteClients { get; set; }
    }
}
