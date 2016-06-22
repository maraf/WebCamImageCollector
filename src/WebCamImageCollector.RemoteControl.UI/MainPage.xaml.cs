using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
        public MainPage()
        {
            InitializeComponent();
            DisableButtons();
            UpdateState();
        }

        private void OnNetworkError(HttpRequestException e)
        {
            DisableButtons();
            ShowMessage(e.Message);
        }

        private void DisableButtons()
        {
            btnDownload.IsEnabled = false;
            btnStart.IsEnabled = false;
            btnStop.IsEnabled = false;
        }

        private async Task UpdateState()
        {
            await SendRequest("/status", String.Empty, async response =>
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    btnDownload.IsEnabled = true;

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
            });
        }

        private async Task SendRequest(string url, string content, Action<HttpResponseMessage> onResponse)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(LocalSettings.Url);
                    client.DefaultRequestHeaders.Add("X-Authentication-Token", LocalSettings.AuthenticationToken);
                    onResponse(await client.PostAsync(url, new StringContent(content)));
                }
            }
            catch (HttpRequestException e)
            {
                OnNetworkError(e);
            }
        }

        private async void btnStart_Click(object sender, RoutedEventArgs e)
        {
            await SendRequest("/start", String.Empty, response =>
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    btnStart.IsEnabled = false;
                    btnStop.IsEnabled = true;
                }
            });
        }

        private async void btnStop_Click(object sender, RoutedEventArgs e)
        {
            await SendRequest("/stop", String.Empty, response =>
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    btnStart.IsEnabled = true;
                    btnStop.IsEnabled = false;
                }
            });
        }

        private async void btnStatus_Click(object sender, RoutedEventArgs e)
        {
            ClearMessage();
            await UpdateState();
        }

        private async void btnDownload_Click(object sender, RoutedEventArgs e)
        {
            ShowMessage("Downloading image...");
            await SendRequest("/latest", String.Empty, async response =>
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    ShowMessage("Reading content...");

                    Stream imageStream = await response.Content.ReadAsStreamAsync();
                    BitmapImage image = new BitmapImage();
                    await image.SetSourceAsync(imageStream.AsRandomAccessStream());
                    imgBackground.Source = image;

                    ClearMessage();
                }
            });
        }

        private void btnClearImage_Click(object sender, RoutedEventArgs e)
        {
            imgBackground.Source = null;
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SettingsPage));
        }

        public void ShowMessage(string message)
        {
            tblMessage.Text = message;
        }

        private void ClearMessage()
        {
            tblMessage.Text = String.Empty;
        }
    }
}
