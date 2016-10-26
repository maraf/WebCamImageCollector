using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCamImageCollector.Background.Http
{
    internal class HttpRequest
    {
        public string Method { get; set; }
        public string Host { get; set; }
        public string Path { get; set; }

        public NameValueCollection Headers { get; set; }
        public NameValueCollection QueryString { get; set; }
    }

}
