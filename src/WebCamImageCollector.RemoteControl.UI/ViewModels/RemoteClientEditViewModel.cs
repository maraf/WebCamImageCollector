using Neptuo;
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
    public class RemoteClientEditViewModel : ObservableObject
    {
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

        public ICommand Save { get; private set; }
        public ICommand Delete { get; private set; }

        public RemoteClientEditViewModel()
        {
            Name = "New";
            Url = "http://";
            Save = new SaveRemoteCommand(this, null);
        }

        public RemoteClientEditViewModel(Guid key)
        {
            ClientRepository repository = new ClientRepository();
            RemoteClient client = repository.FindRemote(key);
            if (client != null)
            {
                Name = client.Name;
                Url = client.Url;
                AuthenticationToken = client.AuthenticationToken;
            }

            Save = new SaveRemoteCommand(this, key);
            Delete = new DeleteRemoteCommand(key);
        }
    }
}
