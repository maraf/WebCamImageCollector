using Newtonsoft.Json;
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
    public abstract class ClientBase
    {
        public string Url { get; private set; }
        public string AuthenticationToken { get; private set; }

        protected ClientBase(string url, string authenticationToken)
        {
            Url = url;
            AuthenticationToken = authenticationToken;
        }

        private async Task<HttpResponseMessage> SendRequest(string url, string content, string etag = null)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Url);
                    client.DefaultRequestHeaders.Add("X-Authentication-Token", AuthenticationToken);

                    if (!String.IsNullOrEmpty(etag))
                        client.DefaultRequestHeaders.TryAddWithoutValidation("If-None-Match", etag);

                    HttpResponseMessage response = await client.PostAsync(url, new StringContent(content));
                    return response;
                }
            }
            catch (HttpRequestException e)
            {
                throw e;
            }
        }

        public async Task<ClientRunningInfo> IsRunningAsync()
        {
            HttpResponseMessage response = await SendRequest("/status", String.Empty);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                string responseText = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ClientRunningInfo>(responseText);
            }
            else
            {
                throw new ClientNotAvailableException();
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

        private string latestETag;
        private ClientImageModel latestImage;

        public async Task<ClientImageModel> DownloadLatest()
        {
            HttpResponseMessage response = await SendRequest("/latest", String.Empty, latestETag);
            if (response.StatusCode == HttpStatusCode.NotModified)
                return latestImage;

            Stream imageStream = await response.Content.ReadAsStreamAsync();
            BitmapImage image = new BitmapImage();
            await image.SetSourceAsync(imageStream.AsRandomAccessStream());

            latestETag = response.Headers.GetValues("ETag").FirstOrDefault();
            latestImage = new ClientImageModel()
            {
                Image = image,
                Stream = imageStream,
                Date = response.Headers.Date?.DateTime ?? DateTime.Now
            };
            return latestImage;
        }
    }
}
