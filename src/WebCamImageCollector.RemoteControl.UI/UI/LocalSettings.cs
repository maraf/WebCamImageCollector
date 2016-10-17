using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace WebCamImageCollector.RemoteControl.UI
{
    public static class LocalSettings
    {
        private static ApplicationDataContainer Container
        {
            get { return ApplicationData.Current.LocalSettings; }
        }

        public static Action UrlChanged;
        public static Action AuthenticationTokenChanged;

        public static string Url
        {
            get { return (string)Container.Values["Url"] ?? "http://m10pi2:8000"; }
            set
            {
                if (Url != value)
                {
                    Container.Values["Url"] = value;

                    if (UrlChanged != null)
                        UrlChanged();
                }
            }
        }

        public static string AuthenticationToken
        {
            get { return (string)Container.Values["AuthenticationToken"] ?? "{3FFF8234-F0B4-4DEB-AB91-75C98ECE550D}"; }
            set
            {
                if (AuthenticationToken != value)
                {
                    Container.Values["AuthenticationToken"] = value;

                    if (AuthenticationTokenChanged != null)
                        AuthenticationTokenChanged();
                }
            }
        }
    }
}
