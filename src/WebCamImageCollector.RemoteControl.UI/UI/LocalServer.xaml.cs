using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using WebCamImageCollector.RemoteControl.Services;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace WebCamImageCollector.RemoteControl.UI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LocalServer : Page
    {
        public const string TaskName = "LocalServer";
        //public static readonly string TaskEntryPoint = typeof(LocalServerStartupTask).FullName;
        public static readonly string TaskEntryPoint = typeof(WebCamImageCollector.Background.StartupTask).FullName;

        private IBackgroundTaskRegistration task;
        private LocalClient client;

        public LocalServer()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            client = ServiceProvider.RemoteClients.LocalClient;
            if (client != null)
            {
                tbxPort.Text = client.Port.ToString();
                tbxAuthenticationToken.Text = client.AuthenticationToken;
            }

            foreach (KeyValuePair<Guid, IBackgroundTaskRegistration> item in BackgroundTaskRegistration.AllTasks)
            {
                if (item.Value.Name == TaskName)
                {
                    task = item.Value;
                    break;
                }
            }

            UpdateButtons();
        }

        private void UpdateButtons()
        {
            btnStart.IsEnabled = !(btnStop.IsEnabled = task != null);
        }

        private async void btnStart_Click(object sender, RoutedEventArgs e)
        {
            int port = -1;
            if (!String.IsNullOrEmpty(tbxPort.Text) && Int32.TryParse(tbxPort.Text, out port))
            {
                if (client == null)
                    client = new LocalClient(port, tbxAuthenticationToken.Text);
                else
                    client.Update(port, tbxAuthenticationToken.Text);

                ServiceProvider.RemoteClients.SetLocal(client);
                ServiceProvider.RemoteClients.Save();
            }

            if (task == null)
            {
                ApplicationTrigger trigger = new ApplicationTrigger();

                BackgroundTaskBuilder builder = new BackgroundTaskBuilder();
                builder.Name = TaskName;
                builder.TaskEntryPoint = TaskEntryPoint;
                builder.SetTrigger(trigger);
                task = builder.Register();

                ValueSet args = new ValueSet();
                
                if (!String.IsNullOrEmpty(tbxPort.Text))
                    args["Port"] = client.Port;

                if (!String.IsNullOrEmpty(client.AuthenticationToken))
                    args["AuthenticationToken"] = client.AuthenticationToken;

                ApplicationTriggerResult result = await trigger.RequestAsync(args);
                if (result != ApplicationTriggerResult.Allowed)
                    task = null;

                UpdateButtons();
            }
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            if (task != null)
            {
                task.Unregister(true);
                task = null;
                UpdateButtons();
            }
        }
    }
}
