using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace WebCamImageCollector.Http
{
    public sealed class HttpServer : IDisposable
    {
        private const uint BufferSize = 8192;
        private readonly IHttpHandler handler;
        private readonly StreamSocketListener listener;

        internal HttpServer(IHttpHandler handler)
        {
            this.handler = handler;

            listener = new StreamSocketListener();
            listener.ConnectionReceived += OnConnectionReceived;
        }

        public IAsyncAction StartAsync(int port)
        {
            return listener.BindServiceNameAsync(port.ToString());
        }

        private async void OnConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs e)
        {
            HttpRequest request = await TryParseRequest(e.Socket);
            if (request == null)
                return;

            try
            {
                using (IOutputStream output = e.Socket.OutputStream)
                {
                    using (Stream responseOutput = output.AsStreamForWrite())
                    {
                        using (var bodyStream = new MemoryStream())
                        using (var streamWriter = new StreamWriter(bodyStream))
                        {
                            HttpResponse response = new HttpResponse(streamWriter);

                            if (await handler.TryHandleAsync(request, response))
                            {

                                string statusText = GetStatusText(response.StatusCode);
                                await streamWriter.FlushAsync();
                                bodyStream.Seek(0, SeekOrigin.Begin);

                                StringBuilder responseHttpHeader = new StringBuilder();
                                responseHttpHeader.AppendLine($"HTTP/1.1 {response.StatusCode} {statusText}");
                                responseHttpHeader.AppendLine($"Content-Length: {bodyStream.Length}");

                                foreach (string key in response.Headers.Keys)
                                    responseHttpHeader.AppendLine(key + ": " + response.Headers[key]);

                                responseHttpHeader.AppendLine("Connection: close");
                                responseHttpHeader.AppendLine();

                                byte[] outputBytes = Encoding.UTF8.GetBytes(responseHttpHeader.ToString());
                                await responseOutput.WriteAsync(outputBytes, 0, outputBytes.Length);
                                await bodyStream.CopyToAsync(responseOutput);
                                await responseOutput.FlushAsync();
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private async Task<HttpRequest> TryParseRequest(StreamSocket socket)
        {
            HttpRequest result = new HttpRequest()
            {
                Headers = new NameValueCollection(),
                QueryString = new NameValueCollection()
            };

            StringBuilder request = new StringBuilder();
            using (IInputStream input = socket.InputStream)
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

            string[] lines = request.ToString().Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length > 0)
            {
                string[] parts = lines[0].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 3)
                {
                    result.Method = parts[0];
                    result.Path = parts[1];

                    int indexOfQuery = result.Path.IndexOf('?');
                    if (indexOfQuery > 0)
                    {
                        string query = result.Path.Substring(indexOfQuery + 1);
                        result.Path = result.Path.Substring(0, indexOfQuery);

                        string[] parameters = query.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string parameter in parameters)
                        {
                            string[] keyValue = parameter.Split(new[] { '=' });
                            if (keyValue.Length == 2)
                                result.QueryString[keyValue[0].ToLowerInvariant()] = keyValue[1];
                        }
                    }
                }
                else
                {
                    // TODO: Bad request.
                    return null;
                }

                for (int i = 1; i < lines.Length; i++)
                {
                    string line = lines[i];
                    int separatorIndex = line.IndexOf(':');
                    if (separatorIndex >= 0)
                    {
                        string name = line.Substring(0, separatorIndex).Trim();
                        string value = line.Substring(separatorIndex + 1).Trim();
                        result.Headers[name] = value;

                        if (name.ToLowerInvariant() == "host")
                            result.Host = result.Headers[name];
                    }
                }
            }
            else
            {
                // TODO: Bad request.
                return null;
            }

            return result;
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
