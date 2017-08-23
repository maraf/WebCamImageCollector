using Neptuo.Observables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WebCamImageCollector.RemoteControl.Services;
using WebCamImageCollector.RemoteControl.ViewModels.Commands;
using WebCamImageCollector.RemoteControl.Views;

namespace WebCamImageCollector.RemoteControl.ViewModels
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

        private int delay;
        public int Delay
        {
            get { return delay; }
            set
            {
                if (delay != value)
                {
                    delay = value;
                    RaisePropertyChanged();
                }
            }
        }

        private int interval;
        public int Interval
        {
            get { return interval; }
            set
            {
                if (interval != value)
                {
                    interval = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ICommand Save { get; private set; }
        public ICommand Delete { get; private set; }
        public ICommand Back { get; private set; }

        public LocalClientEditViewModel(LocalClient client)
        {
            if (client != null)
            {
                Port = client.Port;
                AuthenticationToken = client.AuthenticationToken;
                Delay = client.Delay;
                Interval = client.Interval;

                Delete = new DeleteLocalCommand();
            }

            Save = new SaveLocalCommand(this);
            Back = new NavigateCommand(typeof(Overview));
        }
    }
}
