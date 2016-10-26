using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCamImageCollector.Background.Bootstrap;
using WebCamImageCollector.Capturing;
using WebCamImageCollector.Http;
using Windows.ApplicationModel.Background;
using Windows.Devices.Enumeration;

namespace WebCamImageCollector.Background
{
    internal class Application : IHttpHandler
    {
        private BackgroundTaskDeferral deferral;
        private HttpServer server;
        private CaptureService captureService;

        private readonly object lastCaptureErrorLock = new object();
        private string lastCaptureError;

        public async Task RunAsync(IBackgroundTaskInstance taskInstance)
        {
            deferral = taskInstance.GetDeferral();

            Configuration configuration = new Configuration();
            DefaultConfigurationLoader.Load(configuration);
            ApplicationTriggerConfigurationLoader.Load(configuration, taskInstance.TriggerDetails as ApplicationTriggerDetails);
            ApplicationTriggerDetails triggerDetails = taskInstance.TriggerDetails as ApplicationTriggerDetails;

            DeviceInformationCollection devices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
            DeviceInformation device = devices.Where(d => d.EnclosureLocation != null).FirstOrDefault(d => d.EnclosureLocation.Panel == Panel.Back);
            if (device == null)
                device = devices.FirstOrDefault();

            captureService = new CaptureService(configuration.Interval, configuration.Delay, device);
            captureService.ExceptionHandler += OnCaptureException;
            captureService.TryStartIfNotStopped();

            IHttpHandler httpHandler = new AuthenticationHttpHandler(configuration.AuthenticationToken, null);
            server = new HttpServer(httpHandler);
            await server.StartAsync(configuration.Port);
        }

        private void OnCaptureException(Exception e)
        {
            lock (lastCaptureErrorLock)
                lastCaptureError = e.Message;
        }

        public async Task<bool> TryHandleAsync(HttpRequest request, HttpResponse response)
        {
            if (request.Method == "POST")
            {
                if (request.Path == "/start")
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
                    StatusResponse output = new StatusResponse();
                    lock (lastCaptureErrorLock)
                    {

                        output.Running = captureService.IsRunning;
                        output.LastError = lastCaptureError;
                        lastCaptureError = null;
                    }

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
                        response.StatusCode = 200;
                        response.Headers["Content-Type"] = "image/jpeg";
                        response.Headers["Date"] = file.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss");

                        await file.Content.AsStreamForRead().CopyToAsync(response.Output.BaseStream);
                        file.Content.Dispose();
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
