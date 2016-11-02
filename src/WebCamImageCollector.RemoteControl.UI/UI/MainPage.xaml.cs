using Neptuo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using WebCamImageCollector.RemoteControl.Services;
using WebCamImageCollector.RemoteControl.UI.DesignData;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace WebCamImageCollector.RemoteControl.UI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            TryFindTask();
            UpdateLocalButtons();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            ClientRepository repository = ServiceProvider.Clients;

            MainViewModel viewModel = new MainViewModel(new MainViewModelService(repository), repository);
            foreach (RemoteClient remote in repository.EnumerateRemote())
            {
                viewModel.RemoteClients.Add(new ClientViewModel()
                {
                    Key = remote.Key,
                    Name = remote.Name,
                    Url = remote.Url
                });
            }

            LocalClient local = repository.FindLocal();
            if (local != null)
            {
                viewModel.LocalClient = new ClientViewModel()
                {
                    Key = local.Key,
                    Name = "Local",
                    Url = local.Url
                };
            }

            DataContext = viewModel;
        }

        private class MainViewModelService : MainViewModel.IService
        {
            private readonly ClientRepository repository;

            public MainViewModelService(ClientRepository repository)
            {
                Ensure.NotNull(repository, "repository");
                this.repository = repository;
            }

            public ClientViewModel CreateLocal(int port, string authenticationToken, int interval, int delay)
            {
                LocalClient client = repository.CreateOrReplaceLocal(port, authenticationToken, interval, delay);
                return new ClientViewModel()
                {
                    Key = client.Key,
                    Name = "Local",
                    Url = client.Url
                };
            }

            public ClientViewModel CreateRemote(string name, string url, string authenticationToken)
            {
                RemoteClient client = repository.CreateRemote(name, url, authenticationToken);
                return new ClientViewModel()
                {
                    Key = client.Key,
                    Name = client.Name,
                    Url = client.Url
                };
            }
        }

        private void btnControl_Click(object sender, RoutedEventArgs e)
        {
            Guid key = (Guid)((Button)sender).Tag;
            Frame.Navigate(typeof(ClientControlPage), key);
        }


        public const string TaskName = "LocalServer";
        public static readonly string TaskEntryPoint = typeof(Background.StartupTask).FullName;
        private IBackgroundTaskRegistration task;

        public bool IsLocalRunning
        {
            get { return task != null; }
        }

        private void TryFindTask()
        {
            foreach (KeyValuePair<Guid, IBackgroundTaskRegistration> item in BackgroundTaskRegistration.AllTasks)
            {
                if (item.Value.Name == TaskName)
                {
                    task = item.Value;
                    break;
                }
            }

            TryStartTask();
        }

        private async void TryStartTask()
        {
            if (task != null)
            {
                IBackgroundTaskRegistration2 task2 = task as IBackgroundTaskRegistration2;
                if (task2 != null)
                {
                    ApplicationTrigger trigger = task2.Trigger as ApplicationTrigger;
                    if (trigger != null)
                        await trigger.RequestAsync();
                }
            }
        }

        private void UpdateLocalButtons()
        {
            if (IsLocalRunning)
            {
                btnStartLocal.Visibility = Visibility.Collapsed;
                btnStopLocal.Visibility = Visibility.Visible;
            }
            else
            {
                btnStartLocal.Visibility = Visibility.Visible;
                btnStopLocal.Visibility = Visibility.Collapsed;
            }
        }

        private async void btnStartLocal_Click(object sender, RoutedEventArgs e)
        {
            LocalClient local = ServiceProvider.Clients.FindLocal();
            if (local == null || task != null)
                return;

            ApplicationTrigger trigger = new ApplicationTrigger();
            BackgroundTaskBuilder builder = new BackgroundTaskBuilder();
            builder.Name = TaskName;
            builder.TaskEntryPoint = TaskEntryPoint;
            builder.SetTrigger(trigger);
            task = builder.Register();

            ValueSet args = new ValueSet();
            args["Port"] = local.Port.ToString();
            args["Delay"] = local.Delay.ToString();
            args["Interval"] = local.Interval.ToString();

            if (!String.IsNullOrEmpty(local.AuthenticationToken))
                args["AuthenticationToken"] = local.AuthenticationToken;

            ApplicationTriggerResult result = await trigger.RequestAsync(args);
            if (result != ApplicationTriggerResult.Allowed)
                task = null;

            UpdateLocalButtons();
        }

        private void btnStopLocal_Click(object sender, RoutedEventArgs e)
        {
            if (task == null)
                return;

            task.Unregister(true);
            task = null;

            UpdateLocalButtons();
        }
    }
}
