using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace WebCamImageCollector.RemoteControl.Services
{
    public class RemoteClient
    {
        public string Url { get; private set; }
        public string AuthenticationToken { get; private set; }

        public RemoteClient(string url, string authenticationToken)
        {
            Url = url;
            AuthenticationToken = authenticationToken;
        }

        public void UpdateEndpoint(string url, string authenticationToken)
        {
            if (String.IsNullOrEmpty(url))
                throw new ArgumentException("url");

            Url = url;
            AuthenticationToken = authenticationToken;
        }

        private async Task<HttpResponseMessage> SendRequest(string url, string content)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Url);
                    client.DefaultRequestHeaders.Add("X-Authentication-Token", AuthenticationToken);

                    HttpResponseMessage response = await client.PostAsync(url, new StringContent(content));
                    return response;
                }
            }
            catch (HttpRequestException e)
            {
                throw e;
            }
        }

        public async Task<bool> IsRunningAsync()
        {
            HttpResponseMessage response = await SendRequest("/status", String.Empty);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                string responseText = await response.Content.ReadAsStringAsync();
                if (responseText.Contains("true"))
                    return true;

                return false;
            }
            else
            {
                throw new RemoteClientNotAvailableException();
            }
        }

        public async Task<bool> StartAsync()
        {
            HttpResponseMessage response = await SendRequest("/start", String.Empty);
            return response.StatusCode == HttpStatusCode.OK;
        }

        public async Task<bool> StopAsync()
        {
            HttpResponseMessage response = await SendRequest("/stop", String.Empty);
            return response.StatusCode == HttpStatusCode.OK;
        }

        public async Task<ImageSource> DownloadLatest()
        {
            HttpResponseMessage response = await SendRequest("/latest", String.Empty);
            Stream imageStream = await response.Content.ReadAsStreamAsync();
            BitmapImage image = new BitmapImage();
            await image.SetSourceAsync(imageStream.AsRandomAccessStream());
            return image;
        }
    }
}
