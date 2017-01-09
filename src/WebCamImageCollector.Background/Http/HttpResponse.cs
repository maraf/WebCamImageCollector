using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCamImageCollector.Http
{
    internal class HttpResponse
    {
        public int StatusCode { get; set; }
        public Dictionary<string, string> Headers { get; private set; }
        public StreamWriter Output { get; private set; }

        public HttpResponse(StreamWriter output)
        {
            Headers = new Dictionary<string, string>();
            Output = output;
        }
    }
}
