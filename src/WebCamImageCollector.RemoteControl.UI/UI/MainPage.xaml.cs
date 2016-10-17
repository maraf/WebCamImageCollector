using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using WebCamImageCollector.RemoteControl.Services;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

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
            base.OnNavigatedFrom(e);
            frmRoot.Navigate(typeof(RemoteClientListPage), e.Parameter);
        }

        private void NavigateRootTo(Type page)
        {
            frmRoot.Navigate(page);
            btnMenu.IsChecked = false;
        }

        private void btnRemoteClient_Click(object sender, RoutedEventArgs e)
        {
            NavigateRootTo(typeof(RemoteClientListPage));
        }

        private void btnLocalServer_Click(object sender, RoutedEventArgs e)
        {
            NavigateRootTo(typeof(LocalServer));
        }
    }
}
