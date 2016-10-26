using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.Media.MediaProperties;
using Windows.Media.Capture;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using System.IO;
using Windows.Storage.Search;
using System.IO.IsolatedStorage;
using Windows.Devices.Enumeration;
using WebCamImageCollector.Background.Http;
using WebCamImageCollector.Background.Capturing;
using Newtonsoft.Json;

namespace WebCamImageCollector.Background
{
    internal sealed class StartupTask : IBackgroundTask, IHttpHandler
    {
        private string authenticationToken = "{3FFF8234-F0B4-4DEB-AB91-75C98ECE550D}";
        private BackgroundTaskDeferral deferral;
        private HttpServer server;
        private CaptureService captureService;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            deferral = taskInstance.GetDeferral();

            int port = 8000;
            TimeSpan interval = TimeSpan.FromMinutes(1);
            TimeSpan delay = TimeSpan.Zero;

            ApplicationTriggerDetails triggerDetails = taskInstance.TriggerDetails as ApplicationTriggerDetails;
            if (triggerDetails != null)
            {
                object portRaw = null;
                if (triggerDetails.Arguments.TryGetValue("Port", out portRaw) && portRaw != null)
                    port = Int32.Parse(portRaw.ToString());

                object authTokenRaw = null;
                if (triggerDetails.Arguments.TryGetValue("AuthenticationToken", out authTokenRaw) && authTokenRaw != null)
                    authenticationToken = authTokenRaw.ToString();

                object delayRaw = null;
                if (triggerDetails.Arguments.TryGetValue("Delay", out delayRaw) && delayRaw != null)
                    delay = TimeSpan.FromSeconds(Int32.Parse(delayRaw.ToString()));

                object intervalRaw = null;
                if (triggerDetails.Arguments.TryGetValue("Interval", out intervalRaw) && intervalRaw != null)
                {
                    TimeSpan value = TimeSpan.FromSeconds(Int32.Parse(intervalRaw.ToString()));
                    if (interval > TimeSpan.FromSeconds(10) && interval < TimeSpan.FromMinutes(5))
                        interval = value;
                }
            }

            server = new HttpServer(this);
            await server.StartAsync(port);

            DeviceInformationCollection devices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
            DeviceInformation device = devices.Where(d => d.EnclosureLocation != null).FirstOrDefault(d => d.EnclosureLocation.Panel == Panel.Back);
            if (device == null)
                device = devices.FirstOrDefault();

            captureService = new CaptureService(interval, delay, device);
            captureService.ExceptionHandler += OnCaptureException;
            captureService.TryStartIfNotStopped();
        }

        private void OnCaptureException(Exception e)
        {
            lastCaptureError = e.Message;
        }

        private string lastCaptureError;

        public async Task<bool> TryHandleAsync(HttpRequest request, HttpResponse response)
        {
            if (request.Method == "POST")
            {
                string authenticationToken = request.Headers["X-Authentication-Token"];
                if (String.IsNullOrEmpty(authenticationToken) || this.authenticationToken != authenticationToken)
                {
                    response.StatusCode = 401;
                }
                else if (request.Path == "/start")
                {
                    captureService.Start();
                    response.StatusCode = 200;
                }
                else if (request.Path == "/stop")
                {
                    captureService.Stop();
                    response.StatusCode = 200;
                }
                else if (request.Path == "/status")
                {
                    StatusResponse output = new StatusResponse()
                    {
                        Running = captureService.IsRunning,
                        LastError = lastCaptureError
                    };
                    lastCaptureError = null;

                    response.Output.WriteLine(JsonConvert.SerializeObject(output));
                    response.StatusCode = 200;
                }
                else if (request.Path == "/latest")
                {
                    FileModel file = await captureService.FindLatestImageAsync();
                    if (file == null)
                    {
                        response.StatusCode = 404;
                    }
                    else
                    {
                        response.Headers["Content-Type"] = "image/jpeg";
                        file.Content.AsStreamForRead().CopyTo(response.Output.BaseStream);
                        file.Content.Dispose();
                        response.StatusCode = 200;
                    }
                }
            }

            return true;
        }

        private class StatusResponse
        {
            public bool Running { get; set; }
            public string LastError { get; set; }
        }
    }
}
