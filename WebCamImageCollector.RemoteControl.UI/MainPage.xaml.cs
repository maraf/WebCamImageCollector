using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
        private const string baseUri = "http://m10pi2:8000";
        private const string authenticationToken = "abcdef";

        public MainPage()
        {
            InitializeComponent();
            btnStatus_Click(null, null);
        }

        private async void btnStart_Click(object sender, RoutedEventArgs e)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUri);
                client.DefaultRequestHeaders.Add("X-Authentication-Token", authenticationToken);
                HttpResponseMessage response = await client.PostAsync("/start", new StringContent(String.Empty));

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    btnStart.IsEnabled = false;
                    btnStop.IsEnabled = true;
                }
            }
        }

        private async void btnStop_Click(object sender, RoutedEventArgs e)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUri);
                client.DefaultRequestHeaders.Add("X-Authentication-Token", authenticationToken);
                HttpResponseMessage response = await client.PostAsync("/stop", new StringContent(String.Empty));

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    btnStart.IsEnabled = true;
                    btnStop.IsEnabled = false;
                }
            }
        }

        private async void btnStatus_Click(object sender, RoutedEventArgs e)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUri);
                client.DefaultRequestHeaders.Add("X-Authentication-Token", authenticationToken);
                HttpResponseMessage response = await client.PostAsync("/status", new StringContent(String.Empty));

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string responseText = await response.Content.ReadAsStringAsync();
                    if (responseText.Contains("true"))
                    {
                        btnStart.IsEnabled = false;
                        btnStop.IsEnabled = true;
                    }
                    else if (responseText.Contains("false"))
                    {
                        btnStart.IsEnabled = true;
                        btnStop.IsEnabled = false;
                    }
                }
            }
        }

        private async void btnDownload_Click(object sender, RoutedEventArgs e)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUri);
                client.DefaultRequestHeaders.Add("X-Authentication-Token", authenticationToken);
                HttpResponseMessage response = await client.PostAsync("/latest", new StringContent(String.Empty));

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream imageStream = await response.Content.ReadAsStreamAsync();
                    BitmapImage image = new BitmapImage();
                    await image.SetSourceAsync(imageStream.AsRandomAccessStream());
                    imgBackground.Source = image;
                }
            }
        }

        public void SetMessage(string message)
        {
            tblMessage.Text = message;
        }
    }
}
