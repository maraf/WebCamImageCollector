using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace WebCamImageCollector.Background
{
    public sealed class WebServer : IDisposable
    {
        private const uint BufferSize = 8192;
        private readonly ICaptureService service;
        private readonly string authenticationToken;
        private readonly StreamSocketListener listener;

        public WebServer(ICaptureService service, string authenticationToken)
        {
            this.service = service;
            this.authenticationToken = authenticationToken;

            listener = new StreamSocketListener();
            listener.ConnectionReceived += OnConnectionReceived;
        }

        public IAsyncAction StartAsync(int port)
        {
            return listener.BindServiceNameAsync(port.ToString());
        }

        private async void OnConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs e)
        {
            StringBuilder request = new StringBuilder();
            using (IInputStream input = e.Socket.InputStream)
            {
                byte[] data = new byte[BufferSize];
                IBuffer buffer = data.AsBuffer();
                uint dataRead = BufferSize;
                while (dataRead == BufferSize)
                {
                    await input.ReadAsync(buffer, BufferSize, InputStreamOptions.Partial);
                    request.Append(Encoding.UTF8.GetString(data, 0, data.Length));
                    dataRead = buffer.Length;
                }
            }

            string host = null;
            string path = null;
            string method = null;
            NameValueCollection queryString = new NameValueCollection();
            NameValueCollection headers = new NameValueCollection();

            string[] lines = request.ToString().Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length > 0)
            {
                string[] parts = lines[0].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 3)
                {
                    method = parts[0];
                    path = parts[1];

                    int indexOfQuery = path.IndexOf('?');
                    if (indexOfQuery > 0)
                    {
                        string query = path.Substring(indexOfQuery + 1);
                        path = path.Substring(0, indexOfQuery);

                        string[] parameters = query.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string parameter in parameters)
                        {
                            string[] keyValue = parameter.Split(new[] { '=' });
                            if (keyValue.Length == 2)
                                queryString[keyValue[0].ToLowerInvariant()] = keyValue[1];
                        }
                    }
                }
                else
                {
                    // TODO: Bad request.
                    return;
                }

                for (int i = 1; i < lines.Length; i++)
                {
                    string line = lines[i];
                    int separatorIndex = line.IndexOf(':');
                    if(separatorIndex >= 0)
                    {
                        string name = line.Substring(0, separatorIndex).Trim();
                        string value = line.Substring(separatorIndex + 1).Trim();
                        headers[name] = value;

                        if (name.ToLowerInvariant() == "host")
                            host = headers[name];
                    }
                }
            }
            else
            {
                // TODO: Bad request.
                return;
            }
            
            using (IOutputStream output = e.Socket.OutputStream)
            {
                using (Stream response = output.AsStreamForWrite())
                {
                    using (var bodyStream = new MemoryStream())
                    using (var streamWriter = new StreamWriter(bodyStream))
                    {
                        NameValueCollection responseHeaders = new NameValueCollection();
                        int statusCode = await HandleRequest(method, host, path, headers, responseHeaders, streamWriter);
                        string statusText = GetStatusText(statusCode);
                        await streamWriter.FlushAsync();
                        bodyStream.Seek(0, SeekOrigin.Begin);

                        StringBuilder responseHttpHeader = new StringBuilder();
                        responseHttpHeader.AppendLine($"HTTP/1.1 {statusCode} {statusText}");
                        responseHttpHeader.AppendLine($"Content-Length: {bodyStream.Length}");

                        foreach (string key in responseHeaders.Keys)
                            responseHttpHeader.AppendLine(key + ": " + responseHeaders[key]);

                        responseHttpHeader.AppendLine("Connection: close");
                        responseHttpHeader.AppendLine();

                        byte[] outputBytes = Encoding.UTF8.GetBytes(responseHttpHeader.ToString());
                        await response.WriteAsync(outputBytes, 0, outputBytes.Length);
                        await bodyStream.CopyToAsync(response);
                        await response.FlushAsync();
                    }

                }
            }
        }

        private string GetStatusText(int statusCode)
        {
            switch (statusCode)
            {
                case 200:
                    return "OK";
                case 404:
                    return "Not Found";
                case 401:
                    return "Unauthorized";
                default:
                    return String.Empty;
            }
        }

        private async Task<int> HandleRequest(string method, string host, string path, NameValueCollection requestHeaders, NameValueCollection responseHeaders, StreamWriter response)
        {
            if (method == "POST")
            {
                string authenticationToken = requestHeaders["X-Authentication-Token"];
                if (String.IsNullOrEmpty(authenticationToken) || this.authenticationToken != authenticationToken)
                {
                    return 401;
                }

                if (path == "/start")
                {
                    service.Start();
                    return 200;
                }
                else if (path == "/stop")
                {
                    service.Stop();
                    return 200;
                }
                else if (path == "/status")
                {
                    response.WriteLine("{ running: " + (service.IsRunning ? "true" : "false") + " }");
                    return 200;
                }
                else if(path == "/latest")
                {
                    FileModel file = await service.FindLatestImageAsync();
                    if (file == null)
                        return 404;

                    responseHeaders["Content-Type"] = "image/jpeg";
                    file.Content.AsStreamForRead().CopyTo(response.BaseStream);
                    file.Content.Dispose();
                    return 200;
                }
            }

            return 404;
        }

        public void Dispose()
        {
            if (listener != null)
            {
                listener.CancelIOAsync().AsTask().Wait();
                listener.Dispose();
            }
        }
    }
}
