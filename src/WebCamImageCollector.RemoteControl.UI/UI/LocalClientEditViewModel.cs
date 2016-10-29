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

        public ICommand Save { get; private set; }
        public ICommand Delete { get; private set; }
        public ICommand Close { get; private set; }

        public LocalClientEditViewModel(IService service)
        {
            Save = new SaveCommand(service, this);
            Delete = new DeleteCommand(service);
            Close = new CloseCommand(service.Close);
        }

        public interface IService
        {
            void Save(int port, string authenticationToken, int interval, int delay);
            void Delete();
            void Close();
        }

        private class SaveCommand : ICommand
        {
            private readonly IService service;
            private readonly LocalClientEditViewModel viewModel;

            public event EventHandler CanExecuteChanged;

            public SaveCommand(IService service, LocalClientEditViewModel viewModel)
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
                service.Save(viewModel.Port, viewModel.AuthenticationToken, viewModel.IntervalSeconds, viewModel.DelaySeconds);
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
