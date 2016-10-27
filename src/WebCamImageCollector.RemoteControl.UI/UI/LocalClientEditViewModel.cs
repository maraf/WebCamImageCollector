using Neptuo.Observables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCamImageCollector.RemoteControl.UI
{
    public class LocalClientEditViewModel : ObservableObject
    {
        private int port;
        public int Port
        {
            get { return port; }
            set
            {
                if (port != value)
                {
                    port = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string authenticationToken;
        public string AuthenticationToken
        {
            get { return authenticationToken; }
            set
            {
                if (authenticationToken != value)
                {
                    authenticationToken = value;
                    RaisePropertyChanged();
                }
            }
        }

        private int intervalSeconds;
        public int IntervalSeconds
        {
            get { return intervalSeconds; }
            set
            {
                if (intervalSeconds != value)
                {
                    intervalSeconds = value;
                    RaisePropertyChanged();
                }
            }
        }

        private int delaySeconds;
        public int DelaySeconds
        {
            get { return delaySeconds; }
            set
            {
                if (delaySeconds != value)
                {
                    delaySeconds = value;
                    RaisePropertyChanged();
                }
            }
        }
    }
}
