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
    public class ClientOverviewViewModel : ObservableObject
    {
        public Guid Key { get; private set; }

        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string url;
        public string Url
        {
            get { return url; }
            set
            {
                if (url != value)
                {
                    url = value;
                    RaisePropertyChanged();
                }
            }
        }

        private bool? isRunning;
        public bool? IsRunning
        {
            get { return isRunning; }
            set
            {
                if (isRunning != value)
                {
                    isRunning = value;
                    RaisePropertyChanged();
                }
            }
        }

        private bool isStatusLoading;
        public bool IsStatusLoading
        {
            get { return isStatusLoading; }
            set
            {
                if (isStatusLoading != value)
                {
                    isStatusLoading = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool IsRemote { get; private set; }

        public ICommand Start { get; private set; }
        public ICommand Stop { get; private set; }
        public ICommand CheckStatus { get; private set; }
        public ICommand Edit { get; private set; }

        public ClientOverviewViewModel(IClient client)
        {
            Name = client.Name;
            Url = client.Url;

            Start = new StartCommand(client);
            Stop = new StopCommand(client);
            CheckStatus = new CheckStatusCommand(client, this);

            if (client is RemoteClient remote)
            {
                Key = remote.Key;
                Edit = new NavigateCommand(typeof(RemoteClientEdit), remote.Key);
                IsRemote = true;
            }
            else if(client is LocalClient local)
            {
                Key = local.Key;
                Edit = new NavigateCommand(typeof(LocalClientEdit));
            }
        }
    }
}
