using Neptuo;
using Neptuo.Observables.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCamImageCollector.RemoteControl.Services;
using System.ComponentModel;
using WebCamImageCollector.RemoteControl.Views;

namespace WebCamImageCollector.RemoteControl.ViewModels.Commands
{
    public class SaveRemoteCommand : NavigateCommand
    {
        private readonly RemoteClientEditViewModel viewModel;
        private readonly Guid? key;

        public SaveRemoteCommand(RemoteClientEditViewModel viewModel, Guid? key)
            : base(typeof(Overview))
        {
            Ensure.NotNull(viewModel, "viewModel");
            this.viewModel = viewModel;
            this.key = key;

            viewModel.PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(viewModel.Name) || e.PropertyName == nameof(viewModel.Url) || e.PropertyName == nameof(viewModel.AuthenticationToken))
                RaiseCanExecuteChanged();
        }

        public override bool CanExecute()
        {
            return !string.IsNullOrEmpty(viewModel.Name) && !string.IsNullOrEmpty(viewModel.Url);
        }

        public override void Execute()
        {
            ClientRepository repository = new ClientRepository();

            if (key == null)
                repository.CreateRemote(viewModel.Name, viewModel.Url, viewModel.AuthenticationToken);
            else
                repository.TryUpdateRemote(key.Value, viewModel.Name, viewModel.Url, viewModel.AuthenticationToken);

            base.Execute();
        }
    }
}
