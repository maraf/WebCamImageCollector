using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
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

        private async Task<HttpResponseMessage> SendRequest(string url, string content, string etag = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Url);
                    client.DefaultRequestHeaders.Add("X-Authentication-Token", AuthenticationToken);

                    if (!String.IsNullOrEmpty(etag))
                        client.DefaultRequestHeaders.TryAddWithoutValidation("If-None-Match", etag);

                    HttpResponseMessage response = await client.PostAsync(url, new StringContent(content), cancellationToken);
                    return response;
                }
            }
            catch (Exception e)
            {
                throw new ClientException(e);
            }
        }

        public async Task<ClientRunningInfo> IsRunningAsync(CancellationToken cancellationToken)
        {
            HttpResponseMessage response = await SendRequest("/status", String.Empty, null, cancellationToken);
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

        public async Task<bool> StartAsync(CancellationToken cancellationToken)
        {
            HttpResponseMessage response = await SendRequest("/start", String.Empty, null, cancellationToken);
            return response.StatusCode == HttpStatusCode.OK;
        }

        public async Task<bool> StopAsync(CancellationToken cancellationToken)
        {
            HttpResponseMessage response = await SendRequest("/stop", String.Empty, null, cancellationToken);
            return response.StatusCode == HttpStatusCode.OK;
        }

        private string latestETag;
        private ClientImageModel latestImage;

        public async Task<ClientImageModel> DownloadLatestAsync(ImageQuality quality, CancellationToken cancellationToken)
        {
            string url = "/latest";
            switch (quality)
            {
                case ImageQuality.Medium:
                    url += "?quality=medium";
                    break;
                case ImageQuality.Thumbnail:
                    url += "?quality=thumbnail";
                    break;
            }

            HttpResponseMessage response = await SendRequest(url, String.Empty, latestETag, cancellationToken);
            if (response.StatusCode == HttpStatusCode.NotModified)
                return latestImage;
            else if (response.StatusCode == HttpStatusCode.InternalServerError)
                throw new ClientNotAvailableException();

            Stream imageStream = await response.Content.ReadAsStreamAsync();
            BitmapImage image = new BitmapImage();
            await image.SetSourceAsync(imageStream.AsRandomAccessStream());

            latestETag = response.Headers.GetValues("ETag").FirstOrDefault();
            latestImage = new ClientImageModel(image, imageStream, response.Headers.Date?.DateTime ?? DateTime.Now);

            return latestImage;
        }
    }
}
