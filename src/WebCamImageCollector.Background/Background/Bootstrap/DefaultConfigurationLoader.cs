using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCamImageCollector.Background.Bootstrap
{
    internal static class DefaultConfigurationLoader
    {
        public static void Load(Configuration configuration)
        {
            configuration.Port = 8000;
            configuration.AuthenticationToken = "{3FFF8234-F0B4-4DEB-AB91-75C98ECE550D}";
            configuration.Interval = TimeSpan.FromMinutes(1);
            configuration.Delay = TimeSpan.Zero;
        }
    }
}
