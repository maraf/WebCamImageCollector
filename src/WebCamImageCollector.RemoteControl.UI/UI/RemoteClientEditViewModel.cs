using Neptuo;
using Neptuo.Observables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WebCamImageCollector.RemoteControl.UI
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
        public ICommand Close { get; private set; }

        public RemoteClientEditViewModel(IService service)
        {
            Save = new SaveCommand(service, this);
            Delete = new DeleteCommand(service);
            Close = new CloseCommand(service.Close);
        }

        public interface IService
        {
            void Save(string name, string url, string authenticationToken);
            void Delete();
            void Close();
        }

        private class SaveCommand : ICommand
        {
            private readonly IService service;
            private readonly RemoteClientEditViewModel viewModel;

            public event EventHandler CanExecuteChanged;

            public SaveCommand(IService service, RemoteClientEditViewModel viewModel)
            {
                Ensure.NotNull(service, "service");
                Ensure.NotNull(viewModel, "viewModel");
                this.service = service;
                this.viewModel = viewModel;
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                service.Save(viewModel.Name, viewModel.Url, viewModel.AuthenticationToken);
            }
        }

        private class DeleteCommand : ICommand
        {
            private readonly IService service;

            public event EventHandler CanExecuteChanged;

            public DeleteCommand(IService service)
            {
                Ensure.NotNull(service, "service");
                this.service = service;
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                service.Delete();
            }
        }

        private class CloseCommand : ICommand
        {
            private readonly Action close;

            public event EventHandler CanExecuteChanged;

            public CloseCommand(Action close)
            {
                this.close = close;
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                close();
            }
        }
    }
}
