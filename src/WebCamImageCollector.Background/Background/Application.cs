using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using WebCamImageCollector.Background.Bootstrap;
using WebCamImageCollector.Capturing;
using WebCamImageCollector.Http;
using Windows.ApplicationModel.Background;
using Windows.Devices.Enumeration;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;

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

            IHttpHandler httpHandler = new AuthenticationHttpHandler(configuration.AuthenticationToken, this);
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
                        ImageQuality quality = GetImageQuality(request);
                        string fileEtag = file.CreatedAt.ToString("yyyyMMddHHmmss") + quality;
                        string etag = request.Headers["If-None-Match"];
                        if (fileEtag == etag)
                        {
                            response.StatusCode = 304;
                            return true;
                        }

                        response.StatusCode = 200;
                        response.Headers["Content-Type"] = "image/jpeg";
                        response.Headers["Date"] = file.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss");
                        response.Headers["ETag"] = fileEtag;

                        Stream content = file.Content.AsStreamForRead();

                        switch (quality)
                        {
                            case ImageQuality.Full:
                                break;
                            case ImageQuality.Medium:
                                int width = 600;
                                content = await ResizeImage(content, width, 0);
                                break;
                            case ImageQuality.Thumbnail:
                                width = 200;
                                content = await ResizeImage(content, width, 0);
                                break;
                            default:
                                throw new NotSupportedException(quality.ToString());
                        }

                        await content.CopyToAsync(response.Output.BaseStream);
                        file.Content.Dispose();
                    }
                }
            }

            return true;
        }

        private ImageQuality GetImageQuality(HttpRequest request)
        {
            string rawValue = request.QueryString["quality"];
            if (String.IsNullOrEmpty(rawValue))
                return ImageQuality.Full;

            ImageQuality value;
            if (Enum.TryParse(rawValue, true, out value))
                return value;

            return ImageQuality.Full;
        }

        private async Task<Stream> ResizeImage(Stream imageData, int desiredWidth, int desiredHeight)
        {
            IRandomAccessStream imageStream = imageData.AsRandomAccessStream();
            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(imageStream);

            if (decoder.PixelWidth > desiredWidth || decoder.PixelHeight > desiredHeight)
            {
                using (imageData)
                using (imageStream)
                {
                    InMemoryRandomAccessStream resizedStream = new InMemoryRandomAccessStream();

                    BitmapEncoder encoder = await BitmapEncoder.CreateForTranscodingAsync(resizedStream, decoder);
                    double widthRatio = (double)desiredWidth / decoder.PixelWidth;
                    double heightRatio = (double)desiredHeight / decoder.PixelHeight;

                    double scaleRatio = Math.Min(widthRatio, heightRatio);

                    if (desiredWidth == 0)
                        scaleRatio = heightRatio;

                    if (desiredHeight == 0)
                        scaleRatio = widthRatio;

                    uint aspectHeight = (uint)Math.Floor(decoder.PixelHeight * scaleRatio);
                    uint aspectWidth = (uint)Math.Floor(decoder.PixelWidth * scaleRatio);

                    encoder.BitmapTransform.InterpolationMode = BitmapInterpolationMode.Linear;

                    encoder.BitmapTransform.ScaledHeight = aspectHeight;
                    encoder.BitmapTransform.ScaledWidth = aspectWidth;

                    await encoder.FlushAsync();
                    resizedStream.Seek(0);
                    return resizedStream.AsStreamForRead();
                }
            }

            return imageData;
        }

        private class StatusResponse
        {
            public bool Running { get; set; }
            public string LastError { get; set; }
        }
    }
}
