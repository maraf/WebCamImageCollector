using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Windows.ApplicationModel.Background;

namespace WebCamImageCollector.Background
{
    public sealed class StartupTask : IBackgroundTask//, IHttpHandler
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            Application application = new Application();
            await application.RunAsync(taskInstance);
        }
    }
}
