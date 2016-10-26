using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCamImageCollector.Background.Http
{
    internal class HttpResponse
    {
        public int StatusCode { get; set; }
        public NameValueCollection Headers { get; private set; }
        public StreamWriter Output { get; private set; }

        public HttpResponse(StreamWriter output)
        {
            Headers = new NameValueCollection();
            Output = output;
        }
    }
}
