using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCamImageCollector.Background;
using Windows.ApplicationModel.Background;

namespace WebCamImageCollector.RemoteControl
{
    public class LocalServerStartupTask : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            StartupTask inner = new StartupTask();
            inner.Run(taskInstance);
        }
    }
}
