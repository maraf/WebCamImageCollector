using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;

namespace WebCamImageCollector.Background.Bootstrap
{
    internal static class ApplicationTriggerConfigurationLoader
    {
        public static void Load(Configuration configuration, ApplicationTriggerDetails triggerDetails)
        {
            if (triggerDetails != null)
            {
                ProcessValueInt(triggerDetails.Arguments, "Port", port => configuration.Port = port);
                ProcessValueString(triggerDetails.Arguments, "AuthenticationToken", authToken => configuration.AuthenticationToken = authToken);
                ProcessValueTimeSpan(triggerDetails.Arguments, "Delay", delay => configuration.Delay = delay);
                ProcessValueTimeSpan(triggerDetails.Arguments, "Interval", interval =>
                {
                    if (interval > TimeSpan.FromSeconds(10) && interval < TimeSpan.FromMinutes(5))
                        configuration.Interval = interval;
                });
            }
        }

        private static void ProcessValueString(ValueSet values, string key, Action<string> handler)
        {
            object raw = null;
            if (values.TryGetValue(key, out raw) && raw != null)
                handler(raw.ToString());
        }

        private static void ProcessValueInt(ValueSet values, string key, Action<int> handler)
        {
            ProcessValueString(values, key, raw =>
            {
                int value;
                if (Int32.TryParse(raw, out value))
                    handler(value);
            });
        }

        private static void ProcessValueTimeSpan(ValueSet values, string key, Action<TimeSpan> handler)
        {
            ProcessValueInt(values, key, raw =>
            {
                TimeSpan value = TimeSpan.FromSeconds(raw);
                handler(value);
            });
        }
    }
}
