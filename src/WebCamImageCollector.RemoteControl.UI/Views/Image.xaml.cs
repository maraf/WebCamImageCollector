using Neptuo;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WebCamImageCollector.RemoteControl.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Image : NavigationPage, ImageViewModel.IMessageService, IMessagePage
    {
        public Image()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            Guid key = (Guid)e.Parameter;

            ClientRepository repository = new ClientRepository();
            IClient client = repository.FindRemote(key);
            if (client == null)
                client = repository.FindLocal();

            if (client == null)
                throw Ensure.Exception.ArgumentOutOfRange("parameter", "Unnable to find a client with key '{0}'.", key);

            DataContext = new ImageViewModel(client, this);
        }

        private void ContentPanel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            MessagePanel.Visibility = MessagePanel.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        public void ShowDate(DateTime dateTime)
        {
            throw new NotImplementedException();
        }

        public void ShowError(string text)
        {
            InfoMessage.Visibility = Visibility.Collapsed;
            ErrorMessage.Visibility = Visibility.Visible;
            ErrorMessage.Text = text;
        }

        public void ShowInfo(string text)
        {
            ErrorMessage.Visibility = Visibility.Collapsed;
            InfoMessage.Visibility = Visibility.Visible;
            InfoMessage.Text = text;
        }
    }
}
