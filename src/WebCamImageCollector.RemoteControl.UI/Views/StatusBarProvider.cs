using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Windows.UI.ViewManagement;

namespace WebCamImageCollector.RemoteControl.Views
{
    public class StatusBarProvider
    {
        public static bool TryExecute(Action<StatusBar> handler)
        {
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                StatusBar statusBar = StatusBar.GetForCurrentView();
                handler(statusBar);
                return true;
            }

            return false;
        }

        public static async Task<bool> TryExecuteAsync(Func<StatusBar, Task> handler)
        {
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                StatusBar statusBar = StatusBar.GetForCurrentView();
                await handler(statusBar);
                return true;
            }

            return false;
        }
    }
}
