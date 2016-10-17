using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using WebCamImageCollector.RemoteControl.Services;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace WebCamImageCollector.RemoteControl.UI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RemoteClientEditPage : Page
    {
        private RemoteClient client;

        public RemoteClientEditPage()
        {
            InitializeComponent();

            tbxUrl.Text = LocalSettings.Url;
            tbxAuthenticationToken.Text = LocalSettings.AuthenticationToken;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            client = (RemoteClient)e.Parameter;
            if (client != null)
            {
                tbxUrl.Text = client.Url;
                tbxAuthenticationToken.Text = client.AuthenticationToken;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (client == null)
            {
                Frame.Navigate(typeof(MainPage), new RemoteClient(tbxUrl.Text, tbxAuthenticationToken.Text));
            }
            else
            {
                client.UpdateEndpoint(tbxUrl.Text, tbxAuthenticationToken.Text);
                Frame.Navigate(typeof(MainPage));
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }
    }
}
