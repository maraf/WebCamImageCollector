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

        public void SetMessage(string message)
        {
            tblMessage.Text = message;
        }


        private void imgBackground_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            Transform.TranslateX += e.Delta.Translation.X;
            Transform.TranslateY += e.Delta.Translation.Y;

            BitmapImage image = imgBackground.Source as BitmapImage;
            if (image != null)
            {
                double width = image.PixelWidth - Scrollster.ActualWidth;
                if (Transform.TranslateX > 0)
                    Transform.TranslateX = 0;
                else if (Transform.TranslateX < -width)
                    Transform.TranslateX = -width;
            }
        }

        //void Img_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        //{
        //    // dim the image while panning
        //    imgBackground.Opacity = 0.4;
        //}

        //void Img_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        //{
        //    // reset the Opacity
        //    imgBackground.Opacity = 1;
        //}
    }
}
