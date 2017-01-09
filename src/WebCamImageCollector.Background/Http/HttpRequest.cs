using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCamImageCollector.Http
{
    internal class HttpRequest
    {
        public string Method { get; set; }
        public string Host { get; set; }
        public string Path { get; set; }

        public Dictionary<string, string> Headers { get; set; }
        public Dictionary<string, string> QueryString { get; set; }
    }
}
