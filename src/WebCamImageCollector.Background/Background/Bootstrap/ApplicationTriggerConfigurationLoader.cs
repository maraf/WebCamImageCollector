using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace WebCamImageCollector.Background.Bootstrap
{
    internal static class ApplicationTriggerConfigurationLoader
    {
        public static void Load(Configuration configuration, ApplicationTriggerDetails triggerDetails)
        {
            if (triggerDetails != null)
            {
                object portRaw = null;
                if (triggerDetails.Arguments.TryGetValue("Port", out portRaw) && portRaw != null)
                    configuration.Port = Int32.Parse(portRaw.ToString());

                object authTokenRaw = null;
                if (triggerDetails.Arguments.TryGetValue("AuthenticationToken", out authTokenRaw) && authTokenRaw != null)
                    configuration.AuthenticationToken = authTokenRaw.ToString();

                object delayRaw = null;
                if (triggerDetails.Arguments.TryGetValue("Delay", out delayRaw) && delayRaw != null)
                    configuration.Delay = TimeSpan.FromSeconds(Int32.Parse(delayRaw.ToString()));

                object intervalRaw = null;
                if (triggerDetails.Arguments.TryGetValue("Interval", out intervalRaw) && intervalRaw != null)
                {
                    TimeSpan interval = TimeSpan.FromSeconds(Int32.Parse(intervalRaw.ToString()));
                    if (interval > TimeSpan.FromSeconds(10) && interval < TimeSpan.FromMinutes(5))
                        configuration.Interval = interval;
                }
            }
        }
    }
}
