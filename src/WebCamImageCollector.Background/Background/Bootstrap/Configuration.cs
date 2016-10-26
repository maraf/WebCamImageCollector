using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCamImageCollector.Background.Bootstrap
{
    internal class Configuration
    {
        public int Port { get; set; }
        public string AuthenticationToken { get; set; }
        public TimeSpan Interval { get; set; }
        public TimeSpan Delay { get; set; }
    }
}
