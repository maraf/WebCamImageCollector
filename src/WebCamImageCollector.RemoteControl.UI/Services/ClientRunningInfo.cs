using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCamImageCollector.RemoteControl.Services
{
    public class ClientRunningInfo
    {
        public bool IsRunning { get; private set; }
        public string Message { get; private set; }

        public ClientRunningInfo(bool isRunning, string message)
        {
            IsRunning = isRunning;
            Message = message;
        }
    }
}
