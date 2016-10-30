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
                    Url = remote.Name
                });
            }

            LocalClient local = repository.FindLocal();
            if (local != null)
            {
                viewModel.LocalClient = new ClientViewModel()
                {
                    Key = local.Key,
                    Name = "Local",
                    Url = String.Format("http://localhost{0}/", local.Port) // TODO: Use IClient unified API with Url+AuthenticationToken etc.
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
                    Url = String.Format("http://localhost{0}/", client.Port) // TODO: Use IClient unified API with Url+AuthenticationToken etc.
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
    }
}
