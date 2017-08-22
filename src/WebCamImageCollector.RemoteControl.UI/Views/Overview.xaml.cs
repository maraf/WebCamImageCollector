using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using WebCamImageCollector.RemoteControl.Services;
using WebCamImageCollector.RemoteControl.ViewModels;
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

namespace WebCamImageCollector.RemoteControl.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Overview : Page
    {
        public Overview()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            OverviewViewModel viewModel = new OverviewViewModel();

            ClientRepository repository = new ClientRepository();
            foreach (RemoteClient remote in repository.EnumerateRemote())
                viewModel.Remotes.Add(new ClientOverviewViewModel(remote));

            LocalClient local = repository.FindLocal();
            if (local != null)
                viewModel.Local = new ClientOverviewViewModel(local);

            DataContext = viewModel;
        }

        private void Remotes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ClientOverviewViewModel viewModel = e.AddedItems.FirstOrDefault() as ClientOverviewViewModel;
            if (viewModel != null)
            {
                ((Frame)Window.Current.Content).Navigate(typeof(UI.ClientControlPage), viewModel.Key);
            }
        }
    }
}
