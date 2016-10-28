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

        public ICommand EditRemote { get; private set; }
        public ICommand EditLocal { get; private set; }

        public MainViewModel()
        {
            RemoteClients = new ObservableCollection<ClientViewModel>();

            EditRemote = new EditRemoteCommand(this);
            EditLocal = new EditLocalCommand(this);
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
                viewModel.RemoteClientEdit = new RemoteClientEditViewModel()
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
