using Neptuo.Observables;
using Neptuo.Observables.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.ComponentModel;

namespace WebCamImageCollector.RemoteControl.UI
{
    public class MainViewModel : ObservableObject
    {
        private readonly IService service;

        public ClientViewModel LocalClient { get; set; }
        public ObservableCollection<ClientViewModel> RemoteClients { get; private set; }

        private RemoteClientEditViewModel remoteClientEdit;
        public RemoteClientEditViewModel RemoteClientEdit
        {
            get { return remoteClientEdit; }
            set
            {
                if (remoteClientEdit != value)
                {
                    remoteClientEdit = value;
                    RaisePropertyChanged();
                }
            }
        }

        private LocalClientEditViewModel localClientEdit;
        public LocalClientEditViewModel LocalClientEdit
        {
            get { return localClientEdit; }
            set
            {
                if (localClientEdit != value)
                {
                    localClientEdit = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ICommand CreateRemote { get; private set; }
        public ICommand EditRemote { get; private set; }
        public ICommand EditLocal { get; private set; }

        public MainViewModel(IService service)
        {
            this.service = service;

            RemoteClients = new ObservableCollection<ClientViewModel>();

            CreateRemote = new CreateRemoteCommand(this);
            EditRemote = new EditRemoteCommand(this);
            EditLocal = new EditLocalCommand(this);
        }

        private RemoteClientEditViewModel.IService CreateRemoteEditService(ClientViewModel client)
        {
            return new RemoteClientEditService(this, client);
        }

        public interface IService
        {
            ClientViewModel CreateRemote(string name, string url, string authenticationToken);
        }

        private class RemoteClientEditService : RemoteClientEditViewModel.IService
        {
            private readonly MainViewModel main;
            private ClientViewModel client;

            public RemoteClientEditService(MainViewModel main, ClientViewModel client)
            {
                this.main = main;
                this.client = client;
            }

            public void Close()
            {
                main.RemoteClientEdit = null;
            }

            public void Delete()
            {
                throw new NotImplementedException();
            }

            public void Save(string name, string url, string authenticationToken)
            {
                if (client == null)
                {
                    client = main.service.CreateRemote(name, url, authenticationToken);
                    main.RemoteClients.Add(client);
                    Close();
                }
            }
        }

        private class CreateRemoteCommand : ICommand
        {
            private readonly MainViewModel viewModel;

            public CreateRemoteCommand(MainViewModel viewModel)
            {
                this.viewModel = viewModel;
            }

            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                viewModel.RemoteClientEdit = new RemoteClientEditViewModel(viewModel.CreateRemoteEditService(null));
            }
        }

        private class EditRemoteCommand : ICommand
        {
            private readonly MainViewModel viewModel;

            public EditRemoteCommand(MainViewModel viewModel)
            {
                this.viewModel = viewModel;
            }

            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                ClientViewModel client = (ClientViewModel)parameter;
                viewModel.RemoteClientEdit = new RemoteClientEditViewModel(viewModel.CreateRemoteEditService(client))
                {
                    Name = client.Name,
                    Url = client.Url,
                    AuthenticationToken = null
                };
            }
        }

        private class EditLocalCommand : ICommand
        {
            private readonly MainViewModel viewModel;

            public EditLocalCommand(MainViewModel viewModel)
            {
                this.viewModel = viewModel;
                this.viewModel.PropertyChanged += OnPropertyChanged;
            }

            private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == nameof(MainViewModel.LocalClient) && CanExecuteChanged != null)
                    CanExecuteChanged(this, EventArgs.Empty);
            }

            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter)
            {
                return viewModel.LocalClient != null;
            }

            public void Execute(object parameter)
            {
                viewModel.LocalClientEdit = new LocalClientEditViewModel()
                {
                    
                };
            }
        }
    }
}
