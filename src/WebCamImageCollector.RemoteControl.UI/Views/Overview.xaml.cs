using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WebCamImageCollector.RemoteControl.Services;
using WebCamImageCollector.RemoteControl.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace WebCamImageCollector.RemoteControl.Views
{
    public sealed partial class Overview : Page
    {
        public Overview()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            OverviewViewModel viewModel = new OverviewViewModel();

            ClientRepository repository = new ClientRepository();
            foreach (RemoteClient remote in repository.EnumerateRemote())
                viewModel.Clients.Add(new ClientOverviewViewModel(remote));

            LocalClient local = repository.FindLocal();
            if (local != null)
                viewModel.Clients.Add(new ClientOverviewViewModel(local));

            DataContext = viewModel;
        }

        private void Remotes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ClientOverviewViewModel viewModel = e.AddedItems.FirstOrDefault() as ClientOverviewViewModel;
            if (viewModel != null)
            {
                //Frame.Navigate(typeof(UI.ClientControlPage), viewModel.Key);
                Frame.Navigate(typeof(Image), viewModel.Key);
            }
        }

        private void AboutIcon_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(About));
        }
    }
}
